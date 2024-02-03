// using Android.Content;
// using Android.Graphics;
// using Android.Graphics.Drawables;
// using Android.Util;
// using Android.Views;
// using Android.Widget;
// using System.ComponentModel;
//
// namespace Vapolia.SegmentedViews;
//
//
// public class SegmentedControlRenderer : ViewRenderer<SegmentedControl, RadioGroup>
// {
//   private RadioGroup _nativeControl;
//   private RadioButton _nativeRadioButtonControl;
//   private Android.Graphics.Color _unselectedItemBackgroundColor = Android.Graphics.Color.Transparent;
//   private readonly Context _context;
//
//   public SegmentedControlRenderer(Context context) : base(context)
//   {
//     _context = context;
//   }
//
//   protected virtual void OnElementChanged(ElementChangedEventArgs<SegmentedControl> e)
//   {
//     base.OnElementChanged(e);
//     
//     RadioGroup control = this.Control;
//     if (e.OldElement != null)
//     {
//       if (this._nativeControl != null)
//         this._nativeControl.CheckedChange -= new EventHandler<RadioGroup.CheckedChangeEventArgs>(this.NativeControl_ValueChanged);
//       this.RemoveElementHandlers();
//     }
//     if (e.NewElement == null)
//       return;
//     this.AddElementHandlers();
//   }
//
//   private void AddElementHandlers(bool addChildrenHandlersOnly = false)
//   {
//     if (((VisualElementRenderer<SegmentedControl>) this).Element == null)
//       return;
//     if (!addChildrenHandlersOnly)
//     {
//       ((VisualElement) ((VisualElementRenderer<SegmentedControl>) this).Element).SizeChanged += new EventHandler(this.Element_SizeChanged);
//       ((VisualElementRenderer<SegmentedControl>) this).Element.OnElementChildrenChanging += new EventHandler<ElementChildrenChanging>(this.OnElementChildrenChanging);
//     }
//     if (((VisualElementRenderer<SegmentedControl>) this).Element.Children == null)
//       return;
//     foreach (BindableObject child in (IEnumerable<SegmentedControlOption>) ((VisualElementRenderer<SegmentedControl>) this).Element.Children)
//       child.PropertyChanged += new PropertyChangedEventHandler(this.Segment_PropertyChanged);
//   }
//
//   private void RemoveElementHandlers(bool removeChildrenHandlersOnly = false)
//   {
//     if (((VisualElementRenderer<SegmentedControl>) this).Element == null)
//       return;
//     if (!removeChildrenHandlersOnly)
//     {
//       ((VisualElement) ((VisualElementRenderer<SegmentedControl>) this).Element).SizeChanged -= new EventHandler(this.Element_SizeChanged);
//       ((VisualElementRenderer<SegmentedControl>) this).Element.OnElementChildrenChanging -= new EventHandler<ElementChildrenChanging>(this.OnElementChildrenChanging);
//     }
//     if (((VisualElementRenderer<SegmentedControl>) this).Element.Children == null)
//       return;
//     foreach (BindableObject child in (IEnumerable<SegmentedControlOption>) ((VisualElementRenderer<SegmentedControl>) this).Element.Children)
//       child.PropertyChanged -= new PropertyChangedEventHandler(this.Segment_PropertyChanged);
//   }
//
//   private void Element_SizeChanged(object sender, EventArgs e)
//   {
//     if (this.Control != null || ((VisualElementRenderer<SegmentedControl>) this).Element == null)
//       return;
//     LayoutInflater layoutInflater = LayoutInflater.From(this._context);
//     this._nativeControl = (RadioGroup) layoutInflater.Inflate(Resource.Layout.RadioGroup, (ViewGroup) null);
//     this.SetNativeControlSegments(layoutInflater);
//     RadioButton childAt = (RadioButton) this._nativeControl.GetChildAt(((VisualElementRenderer<SegmentedControl>) this).Element.SelectedSegment);
//     if (childAt != null)
//       childAt.Checked = true;
//     this._nativeControl.CheckedChange += new EventHandler<RadioGroup.CheckedChangeEventArgs>(this.NativeControl_ValueChanged);
//     this.SetNativeControl(this._nativeControl);
//   }
//
//   private void Segment_PropertyChanged(object sender, PropertyChangedEventArgs e)
//   {
//     if (this._nativeControl == null || ((VisualElementRenderer<SegmentedControl>) this).Element == null || !(sender is SegmentedControlOption segmentedControlOption) || !(this._nativeControl.GetChildAt(((VisualElementRenderer<SegmentedControl>) this).Element.Children.IndexOf(segmentedControlOption)) is RadioButton childAt))
//       return;
//     switch (e.PropertyName)
//     {
//       case "Text":
//         childAt.Text = segmentedControlOption.Text;
//         break;
//       case "IsEnabled":
//         childAt.Enabled = ((VisualElement) segmentedControlOption).IsEnabled;
//         break;
//     }
//   }
//
//   protected virtual void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
//   {
//     base.OnElementPropertyChanged(sender, e);
//     string propertyName = e.PropertyName;
//     // ISSUE: reference to a compiler-generated method
//     switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(propertyName))
//     {
//       case 179272953:
//         if (!(propertyName == "BorderWidth"))
//           return;
//         break;
//       case 188567026:
//         if (!(propertyName == "Children"))
//           return;
//         this.SetNativeControlSegments(LayoutInflater.FromContext(this._context));
//         this.AddElementHandlers(true);
//         return;
//       case 1350693843:
//         if (!(propertyName == "TintColor"))
//           return;
//         break;
//       case 1498036510:
//         if (!(propertyName == "Renderer"))
//           return;
//         this.Element_SizeChanged((object) null, (EventArgs) null);
//         ((VisualElementRenderer<SegmentedControl>) this).Element?.RaiseSelectionChanged();
//         return;
//       case 2212655714:
//         if (!(propertyName == "SelectedTextColor") || this._nativeControl == null || ((VisualElementRenderer<SegmentedControl>) this).Element == null)
//           return;
//         ((TextView) this._nativeControl.GetChildAt(((VisualElementRenderer<SegmentedControl>) this).Element.SelectedSegment)).SetTextColor(ColorExtensions.ToAndroid(((VisualElementRenderer<SegmentedControl>) this).Element.SelectedTextColor));
//         return;
//       case 2470332614:
//         if (!(propertyName == "BorderColor"))
//           return;
//         break;
//       case 2527699425:
//         if (!(propertyName == "TextColor"))
//           return;
//         break;
//       case 2724873441:
//         if (!(propertyName == "FontSize"))
//           return;
//         break;
//       case 3033280525:
//         if (!(propertyName == "SelectedSegment") || this._nativeControl == null || ((VisualElementRenderer<SegmentedControl>) this).Element == null)
//           return;
//         if (((VisualElementRenderer<SegmentedControl>) this).Element.SelectedSegment < 0)
//         {
//           LayoutInflater layoutInflater = LayoutInflater.From(this._context);
//           this._nativeControl = (RadioGroup) layoutInflater.Inflate(Resource.Layout.RadioGroup, (ViewGroup) null);
//           this.SetNativeControlSegments(layoutInflater);
//           this._nativeControl.CheckedChange += new EventHandler<RadioGroup.CheckedChangeEventArgs>(this.NativeControl_ValueChanged);
//           this.SetNativeControl(this._nativeControl);
//         }
//         this.SetSelectedRadioButton(((VisualElementRenderer<SegmentedControl>) this).Element.SelectedSegment);
//         ((VisualElementRenderer<SegmentedControl>) this).Element.RaiseSelectionChanged();
//         return;
//       case 3541024718:
//         if (!(propertyName == "IsEnabled"))
//           return;
//         break;
//       case 4130445440:
//         if (!(propertyName == "FontFamily"))
//           return;
//         break;
//       default:
//         return;
//     }
//     this.OnPropertyChanged();
//   }
//
//   private void SetNativeControlSegments(LayoutInflater layoutInflater)
//   {
//     if (this._nativeControl == null || ((VisualElementRenderer<SegmentedControl>) this).Element?.Children == null)
//       return;
//     if (this._nativeControl.ChildCount > 0)
//       this._nativeControl.RemoveAllViews();
//     for (int index = 0; index < ((VisualElementRenderer<SegmentedControl>) this).Element.Children.Count; ++index)
//     {
//       SegmentedControlOption child = ((VisualElementRenderer<SegmentedControl>) this).Element.Children[index];
//       RadioButton radioButton = (RadioButton) layoutInflater.Inflate(Resource.Layout.RadioButton, (ViewGroup) null);
//       if (radioButton == null)
//         return;
//       if (((VisualElement) child).WidthRequest > 0.0)
//         radioButton.LayoutParameters = (ViewGroup.LayoutParams) new RadioGroup.LayoutParams((int) Math.Round(((VisualElement) child).WidthRequest), -2, 0.0f);
//       else
//         radioButton.LayoutParameters = (ViewGroup.LayoutParams) new RadioGroup.LayoutParams(0, -2, 1f);
//       radioButton.Text = child.Text;
//       if (index == 0)
//         radioButton.SetBackgroundResource(Resource.Drawable.segmented_control_first_background);
//       else if (index == ((VisualElementRenderer<SegmentedControl>) this).Element.Children.Count - 1)
//         radioButton.SetBackgroundResource(Resource.Drawable.segmented_control_last_background);
//       this.ConfigureRadioButton(index, radioButton);
//       this._nativeControl.AddView((View) radioButton);
//     }
//     this.SetSelectedRadioButton(((VisualElementRenderer<SegmentedControl>) this).Element.SelectedSegment);
//   }
//
//   private void SetSelectedRadioButton(int index)
//   {
//     if (!(this._nativeControl.GetChildAt(index) is RadioButton childAt))
//       return;
//     childAt.Checked = true;
//   }
//
//   private void OnPropertyChanged()
//   {
//     if (this._nativeControl == null || ((VisualElementRenderer<SegmentedControl>) this).Element == null)
//       return;
//     for (int index = 0; index < ((VisualElementRenderer<SegmentedControl>) this).Element.Children.Count; ++index)
//     {
//       RadioButton childAt = (RadioButton) this._nativeControl.GetChildAt(index);
//       this.ConfigureRadioButton(index, childAt);
//     }
//   }
//
//   private void ConfigureRadioButton(int index, RadioButton radioButton)
//   {
//     if (index == ((VisualElementRenderer<SegmentedControl>) this).Element.SelectedSegment)
//     {
//       radioButton.SetTextColor(ColorExtensions.ToAndroid(((VisualElementRenderer<SegmentedControl>) this).Element.SelectedTextColor));
//       this._nativeRadioButtonControl = radioButton;
//     }
//     else
//     {
//       Android.Graphics.Color color = ((VisualElement) ((VisualElementRenderer<SegmentedControl>) this).Element).IsEnabled ? ColorExtensions.ToAndroid(((VisualElementRenderer<SegmentedControl>) this).Element.TextColor) : ColorExtensions.ToAndroid(((VisualElementRenderer<SegmentedControl>) this).Element.DisabledColor);
//       radioButton.SetTextColor(color);
//     }
//     radioButton.TextSize = Convert.ToSingle(((VisualElementRenderer<SegmentedControl>) this).Element.FontSize);
//     Typeface typeface = FontExtensions.ToTypeface(Font.OfSize(((VisualElementRenderer<SegmentedControl>) this).Element.FontFamily, ((VisualElementRenderer<SegmentedControl>) this).Element.FontSize));
//     radioButton.SetTypeface(typeface, TypefaceStyle.Normal);
//     Android.Graphics.Drawables.Drawable[] children = ((DrawableContainer.DrawableContainerState) radioButton.Background?.GetConstantState())?.GetChildren();
//     if (children != null)
//     {
//       GradientDrawable gradientDrawable1 = children[0] is GradientDrawable gradientDrawable2 ? gradientDrawable2 : (GradientDrawable) ((DrawableWrapper) children[0]).Drawable;
//       GradientDrawable gradientDrawable3 = children[1] is GradientDrawable gradientDrawable4 ? gradientDrawable4 : (GradientDrawable) ((DrawableWrapper) children[1]).Drawable;
//       Android.Graphics.Color argb = ((VisualElement) ((VisualElementRenderer<SegmentedControl>) this).Element).IsEnabled ? ColorExtensions.ToAndroid(((VisualElementRenderer<SegmentedControl>) this).Element.TintColor) : ColorExtensions.ToAndroid(((VisualElementRenderer<SegmentedControl>) this).Element.DisabledColor);
//       Android.Graphics.Color color = ((VisualElement) ((VisualElementRenderer<SegmentedControl>) this).Element).IsEnabled ? ColorExtensions.ToAndroid(((VisualElementRenderer<SegmentedControl>) this).Element.BorderColor) : ColorExtensions.ToAndroid(((VisualElementRenderer<SegmentedControl>) this).Element.DisabledColor);
//       int pixel = this.ConvertDipToPixel(((VisualElementRenderer<SegmentedControl>) this).Element.BorderWidth);
//       if (gradientDrawable1 != null)
//       {
//         gradientDrawable1.SetStroke(pixel, color);
//         gradientDrawable1.SetColor((int) argb);
//       }
//       if (gradientDrawable3 != null)
//       {
//         gradientDrawable3.SetStroke(pixel, color);
//         gradientDrawable3.SetColor((int) this._unselectedItemBackgroundColor);
//       }
//     }
//     radioButton.Enabled = ((VisualElement) ((VisualElementRenderer<SegmentedControl>) this).Element.Children[index]).IsEnabled;
//   }
//
//   private void NativeControl_ValueChanged(object sender, RadioGroup.CheckedChangeEventArgs e)
//   {
//     RadioGroup radioGroup = (RadioGroup) sender;
//     if (radioGroup.CheckedRadioButtonId == -1)
//       return;
//     int checkedRadioButtonId = radioGroup.CheckedRadioButtonId;
//     View viewById = radioGroup.FindViewById(checkedRadioButtonId);
//     int index = radioGroup.IndexOfChild(viewById);
//     RadioButton childAt = (RadioButton) radioGroup.GetChildAt(index);
//     this._nativeRadioButtonControl?.SetTextColor(((VisualElement) ((VisualElementRenderer<SegmentedControl>) this).Element).IsEnabled ? ColorExtensions.ToAndroid(((VisualElementRenderer<SegmentedControl>) this).Element.TextColor) : ColorExtensions.ToAndroid(((VisualElementRenderer<SegmentedControl>) this).Element.DisabledColor));
//     childAt.SetTextColor(ColorExtensions.ToAndroid(((VisualElementRenderer<SegmentedControl>) this).Element.SelectedTextColor));
//     this._nativeRadioButtonControl = childAt;
//     ((VisualElementRenderer<SegmentedControl>) this).Element.SelectedSegment = index;
//   }
//
//   private void OnElementChildrenChanging(object sender, EventArgs e)
//   {
//     this.RemoveElementHandlers(true);
//   }
//
//   private int ConvertDipToPixel(double dip)
//   {
//     return (int) TypedValue.ApplyDimension(ComplexUnitType.Dip, (float) dip, this._context.Resources.DisplayMetrics);
//   }
//
//   public virtual void SetBackgroundColor(Android.Graphics.Color color)
//   {
//     this._unselectedItemBackgroundColor = color;
//     this.OnPropertyChanged();
//     // ISSUE: explicit non-virtual call
//     __nonvirtual (((View) this).SetBackgroundColor(Android.Graphics.Color.Transparent));
//   }
//
//   protected virtual void Dispose(bool disposing)
//   {
//     if (this._nativeControl != null)
//       this._nativeControl.CheckedChange -= new EventHandler<RadioGroup.CheckedChangeEventArgs>(this.NativeControl_ValueChanged);
//     if (this._nativeRadioButtonControl != null)
//     {
//       this._nativeRadioButtonControl.Dispose();
//       this._nativeRadioButtonControl = (RadioButton) null;
//     }
//     this.RemoveElementHandlers();
//     try
//     {
//       base.Dispose(disposing);
//       this._nativeControl = (RadioGroup) null;
//     }
//     catch (Exception ex)
//     {
//     }
//   }
//
//   public static void Init()
//   {
//     DateTime now = DateTime.Now;
//   }
// }