using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;

namespace Vapolia.SegmentedViews;

[ContentProperty(nameof(Children))]
public class SegmentedView : View, ISegmentedView
{
  public const int NoSelection = -1;
  
  private bool disableNotifications;

  #region Bindable Definitions
  public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(SegmentedView), propertyChanged: (bindable, value, newValue) => ((SegmentedView)bindable).OnItemsSourceChanged((IEnumerable?)value, (IEnumerable?)newValue));
  public static readonly BindableProperty TextPropertyNameProperty = BindableProperty.Create(nameof(TextPropertyName), typeof(string), typeof(SegmentedView));
  public static readonly BindableProperty TextConverterProperty = BindableProperty.Create(nameof(TextConverter), typeof(IValueConverter), typeof(SegmentedView));
  public static readonly BindableProperty TintColorProperty = BindableProperty.Create(nameof(TintColor), typeof(Color), typeof(SegmentedView), Colors.Blue);
  public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(SegmentedView), Colors.Black);
  public static readonly BindableProperty SelectedTextColorProperty = BindableProperty.Create(nameof(SelectedTextColor), typeof(Color), typeof(SegmentedView), Colors.White);
  public static readonly BindableProperty DisabledColorProperty = BindableProperty.Create(nameof(DisabledColor), typeof(Color), typeof(SegmentedView), Colors.LightGray);
  public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(nameof(FontSize), typeof(double), typeof(SegmentedView), 12.0);
  public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(nameof(FontFamily), typeof(string), typeof(SegmentedView));
  public static readonly BindableProperty SelectedIndexProperty = BindableProperty.Create(nameof(SelectedIndex), typeof(int), typeof(SegmentedView), NoSelection, BindingMode.TwoWay);
  public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(SegmentedView), null, BindingMode.TwoWay);
  public static readonly BindableProperty SelectionChangedCommandProperty = BindableProperty.Create(nameof(SelectionChangedCommand), typeof(ICommand), typeof(SegmentedView));
  public static readonly BindableProperty SelectionChangedCommandParameterProperty = BindableProperty.Create(nameof(SelectionChangedCommandParameter), typeof(object), typeof(SegmentedView));

  // /// <summary>
  // /// Container'border color
  // /// </summary>
  // public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(SegmentedControl), defaultValueCreator: bindable => ((SegmentedControl)bindable).TintColor);
  // /// <summary>
  // /// Container'border width
  // /// </summary>
  // public static readonly BindableProperty BorderWidthProperty = BindableProperty.Create(nameof(BorderWidth), typeof(double), typeof(SegmentedControl), defaultValueCreator: _ => DeviceInfo.Platform == DevicePlatform.Android ? 1.0 : 0.0);
  #endregion

  #region Bindables Properties

  //if (DesignMode.IsDesignModeEnabled)
  [EditorBrowsable(EditorBrowsableState.Never)]
  public ObservableCollection<Segment> Children { get; } = new ();

  /// <summary>
  /// Optional. If set, replaces the Children property. Each item in this collection is converted into Children. The collection is observed and the view updates dynamically.
  /// The item is converted into a string using the optional TextPropertyName (ie: what property to used for display on each object, or null for the object itself),
  /// and the optional TextConverter (ie: how to convert each object or each property - depending if TextPropertyName is set - into a string)
  /// </summary>
  public IEnumerable? ItemsSource { get => (IEnumerable?)GetValue(ItemsSourceProperty); set => SetValue(ItemsSourceProperty, value); }

  public string? TextPropertyName { get => (string?)GetValue(TextPropertyNameProperty); set => SetValue(TextPropertyNameProperty, value); }
  public IValueConverter? TextConverter { get => (IValueConverter?)GetValue(TextConverterProperty); set => SetValue(TextConverterProperty, value); }
  
  /// <summary>
  /// Color of both the border of the container, and the background of selected segments
  /// </summary>
  public Color TintColor { get => (Color)GetValue(TintColorProperty); set => SetValue(TintColorProperty, value); }
  /// <summary>
  /// Color of the text of unselected segments 
  /// </summary>
  public Color TextColor { get => (Color)GetValue(TextColorProperty); set => SetValue(TextColorProperty, value); }
  /// <summary>
  /// Color of the text of the selected segment 
  /// </summary>
  public Color SelectedTextColor { get => (Color)GetValue(SelectedTextColorProperty); set => SetValue(SelectedTextColorProperty, value); }
  /// <summary>
  /// Color of everything when the control is disabled, except the text of the selected segment
  /// </summary>
  public Color DisabledColor { get => (Color)GetValue(DisabledColorProperty); set => SetValue(DisabledColorProperty, value); }
  // public Color BorderColor { get => (Color)GetValue(BorderColorProperty); set => SetValue(BorderColorProperty, value); }
  // public double BorderWidth { get => (double)GetValue(BorderWidthProperty); set => SetValue(BorderWidthProperty, value); }

  [TypeConverter(typeof(FontSizeConverter))]
  public double FontSize { get => (double)GetValue(FontSizeProperty); set => SetValue(FontSizeProperty, value); }
  public string? FontFamily { get => (string?)GetValue(FontFamilyProperty); set => SetValue(FontFamilyProperty, value); }

  /// <summary>
  /// Can be -1 for no selection (default)
  /// </summary>
  public int SelectedIndex { get => (int)GetValue(SelectedIndexProperty); set => SetValue(SelectedIndexProperty, value); }

  /// <summary>
  /// When used with ItemsSource. Converted into SelectedIndex.
  /// </summary>
  public object? SelectedItem { get => GetValue(SelectedItemProperty); set => SetValue(SelectedItemProperty, value); }
  
  /// <summary>
  /// Triggered after either SelectedIndex or SelectedItem have changed 
  /// </summary>
  public ICommand? SelectionChangedCommand { get => (ICommand?)GetValue(SelectionChangedCommandProperty); set => SetValue(SelectionChangedCommandProperty, value); }
  public object? SelectionChangedCommandParameter { get => GetValue(SelectionChangedCommandParameterProperty); set => SetValue(SelectionChangedCommandParameterProperty, value); }
  #endregion


  public SegmentedView()
  {
    Children.CollectionChanged += (sender, args) =>
    {
      if (args.Action is NotifyCollectionChangedAction.Add)
      {
        foreach (Segment segment in args.NewItems!)
          segment.PropertyChanged += SegmentPropertyChanged;
        Handler?.UpdateValue(nameof(ISegmentedView.Children));
      }
      else if (args.Action is NotifyCollectionChangedAction.Remove)
      {
        if (args.OldItems != null)
        {
          foreach (Segment segment in args.OldItems)
            segment.PropertyChanged -= SegmentPropertyChanged;
          Handler?.UpdateValue(nameof(ISegmentedView.Children));
        }
      }
      else if (args.Action is NotifyCollectionChangedAction.Reset)
      {
        // foreach (var segment in Children)
        //   segment.PropertyChanged -= SegmentPropertyChanged;
        Handler?.UpdateValue(nameof(ISegmentedView.Children));
      }
      else
        throw new NotSupportedException();
    };

    void SegmentPropertyChanged(object? sender, PropertyChangedEventArgs e) 
      => Handler?.UpdateValue(nameof(ISegmentedView.Children));
  }

  protected override void OnHandlerChanged()
  {
    base.OnHandlerChanged();
    if (Handler == null)
    {
      //Unsubscribe
      OnItemsSourceChanged(ItemsSource, null);
    }
  }

  private void OnItemsSourceChanged(IEnumerable? oldValue, IEnumerable? itemsSource)
  {
    if (oldValue is INotifyCollectionChanged nccOld)
      nccOld.CollectionChanged -= ItemsSourceCollectionChanged;
    
    Children.Clear();
    
    if (itemsSource == null)
      return;

    foreach (var item in itemsSource)
      Children.Add(new () { Item = item });

    if (itemsSource is INotifyCollectionChanged ncc)
      ncc.CollectionChanged += ItemsSourceCollectionChanged;

    void ItemsSourceCollectionChanged(object? sender, NotifyCollectionChangedEventArgs args)
    {
      switch (args.Action)
      {
        case NotifyCollectionChangedAction.Add:
        {
          var i = args.NewStartingIndex;
          foreach (var item in args.NewItems!)
            Children.Insert(i++, new () { Item = item });
          break;
        }
        
        case NotifyCollectionChangedAction.Remove:
        {
          if (args.OldItems != null)
          {
            foreach (var item in args.OldItems)
              Children.RemoveAt(args.OldStartingIndex);
          }

          break;
        }

        case NotifyCollectionChangedAction.Reset:
          Children.Clear();
          break;
      
        // default:
        //   throw new NotSupportedException();
      }
    }
  }

  protected override void OnPropertyChanged(string? propertyName = null)
  {
    base.OnPropertyChanged(propertyName);

    switch (propertyName)
    {
      case nameof(SelectedIndex):
        if (!disableNotifications)
        {
          disableNotifications = true;
          if (SelectedIndex >= 0 && SelectedIndex < Children.Count)
            SelectedItem = Children[SelectedIndex].Item;
          else
            SelectedItem = null;
          disableNotifications = false;
        }
        break;

      case nameof(SelectedItem):
        if (!disableNotifications)
        {
          disableNotifications = true;
          SelectedIndex = Children.Select(o => o.Item).ToList().IndexOf(SelectedItem);
          disableNotifications = false;
        }
        break;
    }
  }

  protected override void OnBindingContextChanged()
  {
    base.OnBindingContextChanged();
    foreach (var child in Children)
      child.BindingContext = BindingContext;
  }

  void ISegmentedView.SetSelectedIndex(int i)
  {
    disableNotifications = true;
    
    SelectedIndex = i;
    
    if (i >= 0 && i < Children.Count)
      SelectedItem = Children[i].Item;
    else
      SelectedItem = null;
    
    disableNotifications = false;
    
    if(SelectionChangedCommand?.CanExecute(SelectionChangedCommandParameter) == true)
      SelectionChangedCommand.Execute(SelectionChangedCommandParameter);
  }
}
