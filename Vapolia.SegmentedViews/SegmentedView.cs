using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using Microsoft.Maui.Controls.Internals;

namespace Vapolia.SegmentedViews;

[ContentProperty(nameof(Children))]
public class SegmentedView : View, ISegmentedView, IFontElement
{
  public const int NoSelection = -1;

  private bool disableNotifications;

  #region Bindable Definitions

  public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(SegmentedView), propertyChanged: (bindable, value, newValue) => ((SegmentedView)bindable).OnItemsSourceChanged((IEnumerable?)value, (IEnumerable?)newValue));
  //public static readonly BindableProperty TextPropertyNameProperty = BindableProperty.Create(nameof(TextPropertyName), typeof(string), typeof(SegmentedView));
  public static readonly BindableProperty TextConverterProperty = BindableProperty.Create(nameof(TextConverter), typeof(IValueConverter), typeof(SegmentedView));
  public static readonly BindableProperty SelectedBackgroundColorProperty = BindableProperty.Create(nameof(SelectedBackgroundColor), typeof(Color), typeof(SegmentedView), Colors.Blue);
  public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(SegmentedView), Colors.Black);
  public static readonly BindableProperty SelectedTextColorProperty = BindableProperty.Create(nameof(SelectedTextColor), typeof(Color), typeof(SegmentedView), Colors.White);
  public static readonly BindableProperty DisabledColorProperty = BindableProperty.Create(nameof(DisabledColor), typeof(Color), typeof(SegmentedView), Colors.LightGray);
  public new static readonly BindableProperty BackgroundColorProperty = BindableProperty.Create(nameof(BackgroundColor), typeof(Color), typeof(SegmentedView), Colors.LightGray);
  public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(SegmentedView), Colors.LightGray);

  #region Font

  public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(nameof(FontFamily), typeof(string), typeof(IFontElement), propertyChanged: OnFontFamilyChanged);
  public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(nameof(FontSize), typeof(double), typeof(IFontElement), 0d, propertyChanged: OnFontSizeChanged, defaultValueCreator: FontSizeDefaultValueCreator);
  public static readonly BindableProperty FontAttributesProperty = BindableProperty.Create(nameof(FontAttributes), typeof(FontAttributes), typeof(IFontElement), FontAttributes.None, propertyChanged: OnFontAttributesChanged);
  public static readonly BindableProperty FontAutoScalingEnabledProperty = BindableProperty.Create(nameof(FontAutoScalingEnabled), typeof(bool), typeof(IFontElement), true, propertyChanged: OnFontAutoScalingEnabledChanged);
  public static readonly BindableProperty CharacterSpacingProperty = BindableProperty.Create(nameof(CharacterSpacing), typeof(double), typeof(SegmentedView), 0.0d, propertyChanged: OnCharacterSpacingPropertyChanged);

  #endregion

  //public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(nameof(FontFamily), typeof(string), typeof(SegmentedView));
  public static readonly BindableProperty ItemPaddingProperty = BindableProperty.Create(nameof(ItemPadding), typeof(Thickness), typeof(SegmentedView), new Thickness(10, 3));
  public static readonly BindableProperty SelectedIndexProperty = BindableProperty.Create(nameof(SelectedIndex), typeof(int), typeof(SegmentedView), NoSelection, BindingMode.TwoWay);
  public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(SegmentedView), null, BindingMode.TwoWay);
  public static readonly BindableProperty SelectionChangedCommandProperty = BindableProperty.Create(nameof(SelectionChangedCommand), typeof(ICommand), typeof(SegmentedView));
  public static readonly BindableProperty SelectionChangedCommandParameterProperty = BindableProperty.Create(nameof(SelectionChangedCommandParameter), typeof(object), typeof(SegmentedView));
  public static readonly BindableProperty IsSelectionRequiredProperty = BindableProperty.Create(nameof(IsSelectionRequired), typeof(bool), typeof(SegmentedView), true);

  public static readonly BindableProperty WidthDefinitionsProperty = BindableProperty.Create(nameof(WidthDefinitions), typeof(WidthDefinitionCollection), typeof(SegmentedView));
  public static readonly BindableProperty ItemsDefaultWidthProperty = BindableProperty.Create(nameof(ItemsDefaultWidth), typeof(GridLength), typeof(SegmentedView), GridLength.Auto);


  // /// <summary>
  // /// Container'border color
  // /// </summary>
  // public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(SegmentedControl), defaultValueCreator: bindable => ((SegmentedControl)bindable).TintColor);
  // /// <summary>
  // /// Container'border width
  // /// </summary>
  // public static readonly BindableProperty BorderWidthProperty = BindableProperty.Create(nameof(BorderWidth), typeof(double), typeof(SegmentedControl), defaultValueCreator: _ => DeviceInfo.Platform == DevicePlatform.Android ? 1.0 : 0.0);

  #endregion

  //if (DesignMode.IsDesignModeEnabled)
  [EditorBrowsable(EditorBrowsableState.Never)]
  public ObservableCollection<Segment> Children { get; } = [];

  #region Bindables Properties

  /// <summary>
  /// Optional. If set, replaces the Children property. Each item in this collection is converted into Children. The collection is observed and the view updates dynamically.
  /// The item is converted into a string using the optional TextPropertyName (ie: what property to used for display on each object, or null for the object itself),
  /// and the optional TextConverter (ie: how to convert each object or each property - depending if TextPropertyName is set - into a string)
  /// </summary>
  public IEnumerable? ItemsSource { get => (IEnumerable?)GetValue(ItemsSourceProperty); set => SetValue(ItemsSourceProperty, value); }

  //public string? TextPropertyName { get => (string?)GetValue(TextPropertyNameProperty); set => SetValue(TextPropertyNameProperty, value); }
  public IValueConverter? TextConverter { get => (IValueConverter?)GetValue(TextConverterProperty); set => SetValue(TextConverterProperty, value); }

  /// <summary>
  /// Color of the background of selected segments
  /// </summary>
  public Color SelectedBackgroundColor { get => (Color?)GetValue(SelectedBackgroundColorProperty) ?? (Color)SelectedBackgroundColorProperty.DefaultValue; set => SetValue(SelectedBackgroundColorProperty, value); }

  /// <summary>
  /// Color of the text of unselected segments 
  /// </summary>
  public Color TextColor { get => (Color?)GetValue(TextColorProperty) ?? (Color)TextColorProperty.DefaultValue; set => SetValue(TextColorProperty, value); }

  /// <summary>
  /// Color of the text of the selected segment 
  /// </summary>
  public Color SelectedTextColor { get => (Color?)GetValue(SelectedTextColorProperty) ?? (Color)SelectedTextColorProperty.DefaultValue; set => SetValue(SelectedTextColorProperty, value); }

  /// <summary>
  /// Color of everything when the control is disabled, except the text of the selected segment
  /// </summary>
  public Color DisabledColor { get => (Color?)GetValue(DisabledColorProperty) ?? (Color)DisabledColorProperty.DefaultValue; set => SetValue(DisabledColorProperty, value); }
  // public Color BorderColor { get => (Color)GetValue(BorderColorProperty); set => SetValue(BorderColorProperty, value); }
  // public double BorderWidth { get => (double)GetValue(BorderWidthProperty); set => SetValue(BorderWidthProperty, value); }

  /// <summary>
  /// Color of the background of unselected segments
  /// </summary>
  public new Color BackgroundColor { get => (Color?)GetValue(BackgroundColorProperty) ?? (Color)BackgroundColorProperty.DefaultValue; set => SetValue(BackgroundColorProperty, value); }

  /// <summary>
  /// Color of the border of the container
  /// </summary>
  public Color BorderColor { get => (Color?)GetValue(BorderColorProperty) ?? (Color)BorderColorProperty.DefaultValue; set => SetValue(BorderColorProperty, value); }

  #region Font

  Microsoft.Maui.Font ITextStyle.Font => this.ToFont();

  static void OnFontFamilyChanged(BindableObject bindable, object oldValue, object newValue)
    => ((IFontElement)bindable).OnFontFamilyChanged((string)oldValue, (string)newValue);

  static void OnFontSizeChanged(BindableObject bindable, object oldValue, object newValue)
    => ((IFontElement)bindable).OnFontSizeChanged((double)oldValue, (double)newValue);

  static void OnFontAutoScalingEnabledChanged(BindableObject bindable, object oldValue, object newValue)
    => ((IFontElement)bindable).OnFontAutoScalingEnabledChanged((bool)oldValue, (bool)newValue);

  static object FontSizeDefaultValueCreator(BindableObject bindable)
    => ((IFontElement)bindable).FontSizeDefaultValueCreator();

  static void OnFontAttributesChanged(BindableObject bindable, object oldValue, object newValue)
    => ((IFontElement)bindable).OnFontAttributesChanged((FontAttributes)oldValue, (FontAttributes)newValue);

  static void OnCharacterSpacingPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    => ((SegmentedView)bindable).OnCharacterSpacingPropertyChanged((double)oldValue, (double)newValue);

  void IFontElement.OnFontFamilyChanged(string oldValue, string newValue) => HandleFontChanged();
  void IFontElement.OnFontSizeChanged(double oldValue, double newValue) => HandleFontChanged();
  double IFontElement.FontSizeDefaultValueCreator() => Handler?.MauiContext?.Services.GetService<IFontManager>()?.DefaultFontSize ?? 0d;
  void IFontElement.OnFontAttributesChanged(FontAttributes oldValue, FontAttributes newValue) => HandleFontChanged();
  void IFontElement.OnFontAutoScalingEnabledChanged(bool oldValue, bool newValue) => HandleFontChanged();

  void HandleFontChanged()
  {
    Handler?.UpdateValue(nameof(ITextStyle.Font));
    InvalidateMeasure();
  }

  void OnCharacterSpacingPropertyChanged(double oldValue, double newValue) => InvalidateMeasure();

  public FontAttributes FontAttributes { get => (FontAttributes)GetValue(FontAttributesProperty); set => SetValue(FontAttributesProperty, value); }
  public string? FontFamily { get => (string?)GetValue(FontFamilyProperty); set => SetValue(FontFamilyProperty, value); }

  [TypeConverter(typeof(FontSizeConverter))]
  public double FontSize { get => (double)GetValue(FontSizeProperty); set => SetValue(FontSizeProperty, value); }

  public bool FontAutoScalingEnabled { get => (bool)GetValue(FontAutoScalingEnabledProperty); set => SetValue(FontAutoScalingEnabledProperty, value); }
  public double CharacterSpacing { get => (double)GetValue(CharacterSpacingProperty); set => SetValue(CharacterSpacingProperty, value); }

  #endregion

  public Thickness ItemPadding { get => (Thickness)GetValue(ItemPaddingProperty); set => SetValue(ItemPaddingProperty, value); }

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

  /// <summary>
  /// If false, tapping a selected item unselects it
  /// </summary>
  public bool IsSelectionRequired { get => (bool)GetValue(IsSelectionRequiredProperty); set => SetValue(IsSelectionRequiredProperty, value); }

  /// <summary>
  /// Used only with ItemsSource to set the Width of each item
  /// </summary>
  [TypeConverter(typeof(WidthDefinitionCollectionTypeConverter))]
  public WidthDefinitionCollection? WidthDefinitions { get => (WidthDefinitionCollection?)GetValue(WidthDefinitionsProperty); set => SetValue(WidthDefinitionsProperty, value); }

  [TypeConverter(typeof(GridLengthTypeConverter))]
  public GridLength ItemsDefaultWidth { get => (GridLength)GetValue(ItemsDefaultWidthProperty); set => SetValue(ItemsDefaultWidthProperty, value); }

  #endregion

  public SegmentedView()
  {
    Children.CollectionChanged += (sender, args) =>
    {
      if (args.Action is NotifyCollectionChangedAction.Add)
      {
        foreach (Segment segment in args.NewItems!)
          WeakEventManager.Subscribe(segment, this, SegmentPropertyChanged);
        Handler?.UpdateValue(nameof(ISegmentedView.Children));
      }
      else if (args.Action is NotifyCollectionChangedAction.Remove)
      {
        if (args.OldItems != null)
        {
          foreach (Segment segment in args.OldItems)
            WeakEventManager.Unsubscribe(segment, this, SegmentPropertyChanged);
          Handler?.UpdateValue(nameof(ISegmentedView.Children));
        }
      }
      else if (args.Action is NotifyCollectionChangedAction.Reset)
      {
        // Unsubscribe from all segments when collection is reset
        foreach (var segment in Children)
          WeakEventManager.Unsubscribe(segment, this, SegmentPropertyChanged);
        Handler?.UpdateValue(nameof(ISegmentedView.Children));
      }
      else
        throw new NotSupportedException();

      OnChildrenChanged();
    };
  }

  private void SegmentPropertyChanged(object? sender, PropertyChangedEventArgs e)
    => Handler?.UpdateValue(nameof(ISegmentedView.Children));

  protected override void OnHandlerChanged()
  {
    base.OnHandlerChanged();
    if (Handler == null)
    {
      //Unsubscribe from all weak events
      WeakEventManager.UnsubscribeAll(this);
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
      Children.Add(new() { Item = item });

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
            Children.Insert(i++, new() { Item = item });
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
          var prevDisableNotifications = disableNotifications;
          disableNotifications = true;
          lastSelectedItem = null;
          lastSelectedIndex = SelectedIndex;
          SetTheSelectedIndex(SelectedIndex);
          disableNotifications = prevDisableNotifications;
        }

        break;

      case nameof(SelectedItem):
        if (!disableNotifications)
        {
          var prevDisableNotifications = disableNotifications;
          disableNotifications = true;
          lastSelectedItem = SelectedItem;
          lastSelectedIndex = null;
          SetTheSelectedItem(lastSelectedItem);
          disableNotifications = prevDisableNotifications;
        }

        break;
    }
  }

  #region Used to set the selected item again if SelectedItem or SelectedIndex is set before ItemsSource is set

  private object? lastSelectedItem;
  private int? lastSelectedIndex;

  private void OnChildrenChanged()
  {
    if (lastSelectedItem == null && lastSelectedIndex == null)
      return;

    if (!disableNotifications)
    {
      var prevDisableNotifications = disableNotifications;
      disableNotifications = true;
      if (lastSelectedIndex != null)
        SetTheSelectedIndex(lastSelectedIndex.Value);
      else
        SetTheSelectedItem(lastSelectedItem);
      disableNotifications = prevDisableNotifications;
    }
  }

  void SetTheSelectedIndex(int index)
  {
    if (index >= 0 && index < Children.Count)
      SelectedItem = Children[index].Item;
    else
      SelectedItem = null;
  }

  void SetTheSelectedItem(object? item)
  {
    SelectedIndex = Children.Select(o => o.Item).ToList().IndexOf(item);
  }

  #endregion

  protected override void OnBindingContextChanged()
  {
    base.OnBindingContextChanged();
    foreach (var child in Children)
      child.BindingContext = BindingContext;
  }

  void ISegmentedView.SetSelectedIndex(int i)
  {
    if(i == SelectedIndex)
      return;
    
    var prevDisableNotifications = disableNotifications;
    disableNotifications = true;

    SelectedIndex = i;

    if (i >= 0 && i < Children.Count)
      SelectedItem = Children[i].Item;
    else
      SelectedItem = null;

    disableNotifications = prevDisableNotifications;

    if (SelectionChangedCommand?.CanExecute(SelectionChangedCommandParameter) == true)
      SelectionChangedCommand.Execute(SelectionChangedCommandParameter);
  }
}