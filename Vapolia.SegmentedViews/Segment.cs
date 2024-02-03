using System.ComponentModel;

namespace Vapolia.SegmentedViews;

public class Segment : BindableObject
{
  public static readonly BindableProperty ItemProperty = BindableProperty.Create(nameof (Item), typeof (object), typeof (Segment), propertyChanged: (bindable, value, newValue) => ((Segment)bindable).OnItemChanged(value, newValue));

  public object? Item
  {
    get => GetValue(ItemProperty);
    set => SetValue(ItemProperty, value);
  }

  private void OnItemChanged(object value, object newValue)
  {
    if (value is INotifyPropertyChanged notifyPropertyChanged1)
      notifyPropertyChanged1.PropertyChanged -= OnItemPropertyChanged;
    
    if (newValue is INotifyPropertyChanged notifyPropertyChanged2)
      notifyPropertyChanged2.PropertyChanged += OnItemPropertyChanged;

    //Simulate the change of the whole item when an item's property has changed
    void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
      if(e.PropertyName != nameof(Item))
        OnPropertyChanged(nameof(Item));
    }
  }
}
