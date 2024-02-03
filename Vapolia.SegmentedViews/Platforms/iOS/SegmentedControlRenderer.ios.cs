// using System.ComponentModel;
// using Microsoft.Maui.Controls.Platform;
// using Microsoft.Maui.Platform;
// using UIKit;
//
// namespace Vapolia.SegmentedViews;
//
//
// public class SegmentedControlRenderer : ViewRenderer<SegmentedControl, UISegmentedControl>
// {
//   private UISegmentedControl _nativeControl;
//
//   protected virtual void OnElementChanged(ElementChangedEventArgs<SegmentedControl> e)
//   {
//     base.OnElementChanged(e);
//     if (this.Control == null && ((VisualElementRenderer<SegmentedControl>) this).Element != null)
//     {
//       this._nativeControl = new UISegmentedControl();
//       this.SetNativeControlSegments(((VisualElementRenderer<SegmentedControl>) this).Element.Children);
//       this._nativeControl.Enabled = ((VisualElement) ((VisualElementRenderer<SegmentedControl>) this).Element).IsEnabled;
//       this.SetNativeControl(this._nativeControl);
//       this.SetEnabledStateColor();
//       this.SetFont();
//       this.SetSelectedTextColor();
//       this.SetTextColor();
//       this.SetBorder();
//     }
//     if (e.OldElement != null)
//     {
//       if (this._nativeControl != null)
//         this._nativeControl.ValueChanged -= new EventHandler(this.NativeControl_SelectionChanged);
//       this.RemoveElementHandlers();
//     }
//     if (e.NewElement == null)
//       return;
//     if (this._nativeControl != null)
//       this._nativeControl.ValueChanged += new EventHandler(this.NativeControl_SelectionChanged);
//     this.AddElementHandlers(e.NewElement);
//   }
//
//   private void SetNativeControlSegments(IList<SegmentedControlOption> children)
//   {
//     if (this._nativeControl == null)
//       return;
//     if (this._nativeControl.NumberOfSegments > (nint) 0)
//       this._nativeControl.RemoveAllSegments();
//     for (int index = 0; index < children.Count; ++index)
//     {
//       this._nativeControl.InsertSegment(children[index].Text, (nint) index, false);
//       this._nativeControl.SetEnabled(((VisualElement) children[index]).IsEnabled, (nint) index);
//       if (((VisualElement) children[index]).WidthRequest > 0.0)
//         this._nativeControl.SetWidth((nfloat) ((VisualElement) children[index]).WidthRequest, (nint) index);
//     }
//     if (((VisualElementRenderer<SegmentedControl>) this).Element == null)
//       return;
//     this._nativeControl.SelectedSegment = (nint) ((VisualElementRenderer<SegmentedControl>) this).Element.SelectedSegment;
//   }
//
//   private void AddElementHandlers(SegmentedControl element, bool addChildHandlersOnly = false)
//   {
//     if (element == null)
//       return;
//     if (!addChildHandlersOnly)
//       element.OnElementChildrenChanging += new EventHandler<ElementChildrenChanging>(this.OnElementChildrenChanging);
//     if (element.Children == null)
//       return;
//     foreach (BindableObject child in (IEnumerable<SegmentedControlOption>) element.Children)
//       child.PropertyChanged += new PropertyChangedEventHandler(this.SegmentPropertyChanged);
//   }
//
//   private void RemoveElementHandlers(bool removeChildrenHandlersOnly = false)
//   {
//     if (((VisualElementRenderer<SegmentedControl>) this).Element == null)
//       return;
//     if (!removeChildrenHandlersOnly)
//       ((VisualElementRenderer<SegmentedControl>) this).Element.OnElementChildrenChanging -= new EventHandler<ElementChildrenChanging>(this.OnElementChildrenChanging);
//     if (((VisualElementRenderer<SegmentedControl>) this).Element.Children == null)
//       return;
//     foreach (BindableObject child in (IEnumerable<SegmentedControlOption>) ((VisualElementRenderer<SegmentedControl>) this).Element.Children)
//       child.PropertyChanged -= new PropertyChangedEventHandler(this.SegmentPropertyChanged);
//   }
//
//   private void OnElementChildrenChanging(object sender, EventArgs e)
//   {
//     this.RemoveElementHandlers(true);
//   }
//
//   private void SegmentPropertyChanged(object sender, PropertyChangedEventArgs e)
//   {
//     if (this._nativeControl == null || ((VisualElementRenderer<SegmentedControl>) this).Element == null || !(sender is SegmentedControlOption segmentedControlOption))
//       return;
//     int segment = ((VisualElementRenderer<SegmentedControl>) this).Element.Children.IndexOf(segmentedControlOption);
//     switch (e.PropertyName)
//     {
//       case "Text":
//         this._nativeControl.SetTitle(segmentedControlOption.Text, (nint) segment);
//         break;
//       case "IsEnabled":
//         this._nativeControl.SetEnabled(((VisualElement) segmentedControlOption).IsEnabled, (nint) segment);
//         break;
//     }
//   }
//
//   protected virtual void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
//   {
//     base.OnElementPropertyChanged(sender, e);
//     if (e.PropertyName == "Renderer")
//     {
//       ((VisualElementRenderer<SegmentedControl>) this).Element?.RaiseSelectionChanged();
//     }
//     else
//     {
//       if (this._nativeControl == null || ((VisualElementRenderer<SegmentedControl>) this).Element == null)
//         return;
//       string propertyName = e.PropertyName;
//       // ISSUE: reference to a compiler-generated method
//       switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(propertyName))
//       {
//         case 179272953:
//           if (!(propertyName == "BorderWidth"))
//             return;
//           goto label_34;
//         case 188567026:
//           if (!(propertyName == "Children") || ((VisualElementRenderer<SegmentedControl>) this).Element.Children == null)
//             return;
//           this.SetNativeControlSegments(((VisualElementRenderer<SegmentedControl>) this).Element.Children);
//           this.AddElementHandlers(((VisualElementRenderer<SegmentedControl>) this).Element, true);
//           return;
//         case 1350693843:
//           if (!(propertyName == "TintColor"))
//             return;
//           this.SetEnabledStateColor();
//           return;
//         case 2212655714:
//           if (!(propertyName == "SelectedTextColor"))
//             return;
//           this.SetSelectedTextColor();
//           return;
//         case 2470332614:
//           if (!(propertyName == "BorderColor"))
//             return;
//           goto label_34;
//         case 2527699425:
//           if (!(propertyName == "TextColor"))
//             return;
//           this.SetTextColor();
//           return;
//         case 2724873441:
//           if (!(propertyName == "FontSize"))
//             return;
//           break;
//         case 3033280525:
//           if (!(propertyName == "SelectedSegment"))
//             return;
//           this._nativeControl.SelectedSegment = (nint) ((VisualElementRenderer<SegmentedControl>) this).Element.SelectedSegment;
//           ((VisualElementRenderer<SegmentedControl>) this).Element.RaiseSelectionChanged();
//           return;
//         case 3541024718:
//           if (!(propertyName == "IsEnabled"))
//             return;
//           this._nativeControl.Enabled = ((VisualElement) ((VisualElementRenderer<SegmentedControl>) this).Element).IsEnabled;
//           this.SetEnabledStateColor();
//           return;
//         case 4130445440:
//           if (!(propertyName == "FontFamily"))
//             return;
//           break;
//         default:
//           return;
//       }
//       this.SetFont();
//       return;
//       label_34:
//       this.SetBorder();
//     }
//   }
//
//   private void SetEnabledStateColor()
//   {
//     if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
//       this._nativeControl.SelectedSegmentTintColor = ((VisualElement) ((VisualElementRenderer<SegmentedControl>) this).Element).IsEnabled ? ColorExtensions.ToUIColor(((VisualElementRenderer<SegmentedControl>) this).Element.TintColor) : ColorExtensions.ToUIColor(((VisualElementRenderer<SegmentedControl>) this).Element.DisabledColor);
//     else
//       this._nativeControl.TintColor = ((VisualElement) ((VisualElementRenderer<SegmentedControl>) this).Element).IsEnabled ? ColorExtensions.ToUIColor(((VisualElementRenderer<SegmentedControl>) this).Element.TintColor) : ColorExtensions.ToUIColor(((VisualElementRenderer<SegmentedControl>) this).Element.DisabledColor);
//   }
//
//   private void SetFont()
//   {
//     UITextAttributes titleTextAttributes = this._nativeControl.GetTitleTextAttributes(UIControlState.Normal);
//     UIFont uiFont = string.IsNullOrEmpty(((VisualElementRenderer<SegmentedControl>) this).Element.FontFamily) ? UIFont.SystemFontOfSize((nfloat) ((VisualElementRenderer<SegmentedControl>) this).Element.FontSize) : UIFont.FromName(((VisualElementRenderer<SegmentedControl>) this).Element.FontFamily, (nfloat) ((VisualElementRenderer<SegmentedControl>) this).Element.FontSize);
//     titleTextAttributes.Font = uiFont;
//     this._nativeControl.SetTitleTextAttributes(titleTextAttributes, UIControlState.Normal);
//   }
//
//   private void SetTextColor()
//   {
//     UITextAttributes titleTextAttributes = this._nativeControl.GetTitleTextAttributes(UIControlState.Normal);
//     titleTextAttributes.TextColor = ColorExtensions.ToUIColor(((VisualElementRenderer<SegmentedControl>) this).Element.TextColor);
//     this._nativeControl.SetTitleTextAttributes(titleTextAttributes, UIControlState.Normal);
//   }
//
//   private void SetSelectedTextColor()
//   {
//     UITextAttributes titleTextAttributes = this._nativeControl.GetTitleTextAttributes(UIControlState.Normal);
//     titleTextAttributes.TextColor = ColorExtensions.ToUIColor(((VisualElementRenderer<SegmentedControl>) this).Element.SelectedTextColor);
//     this._nativeControl.SetTitleTextAttributes(titleTextAttributes, UIControlState.Selected);
//   }
//
//   private void SetBorder()
//   {
//     this._nativeControl.Layer.BorderWidth = (nfloat) ((VisualElementRenderer<SegmentedControl>) this).Element.BorderWidth;
//     this._nativeControl.Layer.BorderColor = ((VisualElement) ((VisualElementRenderer<SegmentedControl>) this).Element).IsEnabled ? ColorExtensions.ToCGColor(((VisualElementRenderer<SegmentedControl>) this).Element.BorderColor) : ColorExtensions.ToCGColor(((VisualElementRenderer<SegmentedControl>) this).Element.DisabledColor);
//   }
//
//   private void NativeControl_SelectionChanged(object sender, EventArgs e)
//   {
//     ((VisualElementRenderer<SegmentedControl>) this).Element.SelectedSegment = (int) this._nativeControl.SelectedSegment;
//   }
//
//   protected virtual void Dispose(bool disposing)
//   {
//     if (this._nativeControl != null)
//     {
//       this._nativeControl.ValueChanged -= new EventHandler(this.NativeControl_SelectionChanged);
//       this._nativeControl?.Dispose();
//       this._nativeControl = (UISegmentedControl) null;
//     }
//     this.RemoveElementHandlers();
//     base.Dispose(disposing);
//   }
//
//   public static void Initialize()
//   {
//   }
// }