using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Vapolia.SegmentedViews;

/// <summary>
/// Manages weak event subscriptions to prevent memory leaks
/// </summary>
internal static class WeakEventManager
{
    private static readonly ConditionalWeakTable<object, List<WeakEventSubscription>> Subscriptions = new();
    private static readonly Lock Lock = new();

    /// <summary>
    /// Subscribe to PropertyChanged events using weak references
    /// </summary>
    /// <param name="source">The object that raises PropertyChanged events</param>
    /// <param name="target">The object that handles the events</param>
    /// <param name="handler">The event handler method</param>
    public static void Subscribe(INotifyPropertyChanged source, object target, PropertyChangedEventHandler handler)
    {
        if (source == null || target == null || handler == null)
            return;

        lock (Lock)
        {
            var subscriptions = Subscriptions.GetOrCreateValue(source);
            
            // Clean up dead references first
            CleanupDeadReferences(subscriptions);
            
            // Add new subscription
            var subscription = new WeakEventSubscription(target, handler);
            subscriptions.Add(subscription);
            
            // Subscribe to the actual event if this is the first subscription for this source
            if (subscriptions.Count == 1)
            {
                source.PropertyChanged += OnSourcePropertyChanged;
            }
        }
    }

    /// <summary>
    /// Unsubscribe from PropertyChanged events
    /// </summary>
    /// <param name="source">The object that raises PropertyChanged events</param>
    /// <param name="target">The object that handles the events</param>
    /// <param name="handler">The event handler method</param>
    public static void Unsubscribe(INotifyPropertyChanged source, object target, PropertyChangedEventHandler handler)
    {
        if (source == null || target == null || handler == null)
            return;

        lock (Lock)
        {
            if (!Subscriptions.TryGetValue(source, out var subscriptions))
                return;

            // Remove matching subscriptions
            for (int i = subscriptions.Count - 1; i >= 0; i--)
            {
                var subscription = subscriptions[i];
                if (!subscription.TargetReference.TryGetTarget(out var targetObj) || 
                    targetObj == target && subscription.Handler.Method == handler.Method)
                {
                    subscriptions.RemoveAt(i);
                }
            }

            // If no more subscriptions, unsubscribe from the actual event
            if (subscriptions.Count == 0)
            {
                source.PropertyChanged -= OnSourcePropertyChanged;
                Subscriptions.Remove(source);
            }
        }
    }

    /// <summary>
    /// Unsubscribe all events for a specific target object
    /// </summary>
    /// <param name="target">The target object to unsubscribe</param>
    public static void UnsubscribeAll(object target)
    {
        if (target == null)
            return;

        lock (Lock)
        {
            var sourcesToRemove = new List<object>();
            
            foreach (var kvp in Subscriptions)
            {
                var source = kvp.Key;
                var subscriptions = kvp.Value;
                
                // Remove subscriptions for this target
                for (int i = subscriptions.Count - 1; i >= 0; i--)
                {
                    var subscription = subscriptions[i];
                    if (!subscription.TargetReference.TryGetTarget(out var targetObj) || targetObj == target)
                    {
                        subscriptions.RemoveAt(i);
                    }
                }

                // If no more subscriptions, mark source for removal
                if (subscriptions.Count == 0)
                {
                    sourcesToRemove.Add(source);
                }
            }

            // Clean up sources with no subscriptions
            foreach (var source in sourcesToRemove)
            {
                if (source is INotifyPropertyChanged notifySource)
                {
                    notifySource.PropertyChanged -= OnSourcePropertyChanged;
                }
                Subscriptions.Remove(source);
            }
        }
    }

    private static void OnSourcePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender == null)
            return;

        List<WeakEventSubscription>? subscriptions;
        lock (Lock)
        {
            if (!Subscriptions.TryGetValue(sender, out subscriptions))
                return;

            // Create a copy to avoid issues with concurrent modification
            subscriptions = new List<WeakEventSubscription>(subscriptions);
        }

        // Invoke handlers outside the lock
        var deadSubscriptions = new List<WeakEventSubscription>();
        
        foreach (var subscription in subscriptions)
        {
            if (subscription.TargetReference.TryGetTarget(out var target))
            {
                try
                {
                    subscription.Handler.Invoke(sender, e);
                }
                catch
                {
                    // Ignore exceptions in event handlers
                }
            }
            else
            {
                deadSubscriptions.Add(subscription);
            }
        }

        // Clean up dead subscriptions
        if (deadSubscriptions.Count > 0)
        {
            lock (Lock)
            {
                if (Subscriptions.TryGetValue(sender, out var currentSubscriptions))
                {
                    foreach (var deadSubscription in deadSubscriptions)
                    {
                        currentSubscriptions.Remove(deadSubscription);
                    }

                    if (currentSubscriptions.Count == 0 && sender is INotifyPropertyChanged notifySource)
                    {
                        notifySource.PropertyChanged -= OnSourcePropertyChanged;
                        Subscriptions.Remove(sender);
                    }
                }
            }
        }
    }

    private static void CleanupDeadReferences(List<WeakEventSubscription> subscriptions)
    {
        for (int i = subscriptions.Count - 1; i >= 0; i--)
        {
            if (!subscriptions[i].TargetReference.TryGetTarget(out _))
            {
                subscriptions.RemoveAt(i);
            }
        }
    }

    private class WeakEventSubscription(object target, PropertyChangedEventHandler handler)
    {
        public WeakReference<object> TargetReference { get; } = new(target);
        public PropertyChangedEventHandler Handler { get; } = handler;
    }
}
