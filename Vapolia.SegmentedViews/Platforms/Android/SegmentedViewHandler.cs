using Android.Views;
using Android.Widget;
using Google.Android.Material.Button;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Orientation = Android.Widget.Orientation;

namespace Vapolia.SegmentedViews.Platforms.Android;

internal class SegmentedViewHandler : ViewHandler<ISegmentedView, MaterialButtonToggleGroup>
{
    MaterialButton? selectedButton;
    bool disableButtonNotifications;

    public static IPropertyMapper<ISegmentedView, SegmentedViewHandler> Mapper = new PropertyMapper<ISegmentedView, SegmentedViewHandler>(ViewMapper)
    {
        [nameof(ISegmentedView.Children)] = MapChildren,
        [nameof(ISegmentedView.IsEnabled)] = MapIsEnabled2,
        [nameof(ISegmentedView.SelectedIndex)] = MapSelectedIndex,
        
        [nameof(ISegmentedView.ItemPadding)] = MapItemPadding,
        [nameof(ISegmentedView.TintColor)] = ReconfigureRadioButtons,
        
        [nameof(ISegmentedView.SelectedTextColor)] = MapTextColor,
        [nameof(ITextStyle.TextColor)] = MapTextColor,
        [nameof(ISegmentedView.DisabledColor)] = ReconfigureRadioButtons,
        
        // [nameof(ISegmentedControl.BorderColor)] = MapBorderColor,
        // [nameof(ISegmentedControl.BorderWidth)] = MapBorderWidth,
        [nameof(ITextStyle.CharacterSpacing)] = MapCharacterSpacing,
        [nameof(ITextStyle.Font)] = MapFont,
    };

    public SegmentedViewHandler() : base(Mapper)
    {
    }

    public SegmentedViewHandler(IPropertyMapper mapper) : base(mapper ?? Mapper)
    {
    }

    protected override MaterialButtonToggleGroup CreatePlatformView()
    {
        //using var layoutInflater = LayoutInflater.From(Context)!;
        //var rg = (RadioGroup)layoutInflater.Inflate(Resource.Layout.RadioGroup, null)!;

        //Xamarin.AndroidX.Compose.Material3
        //Material3.
        
        var rg = new MaterialButtonToggleGroup(Context, null, Resource.Attribute.materialButtonToggleGroupStyle);
        rg.Orientation = Orientation.Horizontal;
        rg.LayoutParameters = new (ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
        rg.SingleSelection = true;
        rg.SelectionRequired = false;
        
        //result in too large items
        //rg.MeasureWithLargestChildEnabled = true;

        //OK. It takes whole space
        //rg.BackgroundTintList = ColorStateList.ValueOf(Color.Red);
        
        //Does nothing
        //rg.SetForegroundGravity(GravityFlags.FillHorizontal | GravityFlags.CenterVertical);
        //rg.SetGravity(GravityFlags.FillHorizontal | GravityFlags.CenterVertical);

        return rg;
    }

    public override bool NeedsContainer => false;

    protected override void ConnectHandler(MaterialButtonToggleGroup platformView)
    {
        base.ConnectHandler(platformView);

        //platformView.EnsureId();
        platformView.ButtonChecked += PlatformView_CheckedChange;
    }

    protected override void DisconnectHandler(MaterialButtonToggleGroup platformView)
    {
        platformView.ButtonChecked -= PlatformView_CheckedChange;
        selectedButton = null;

        base.DisconnectHandler(platformView);
    }

    // /// <summary>
    // /// Invokes "IView.Frame" after calling PlatformView.Layout
    // /// </summary>
    // public override void PlatformArrange(Rect frame)
    // {
    //     base.PlatformArrange(frame);
    // }

    void PlatformView_CheckedChange(object? sender, MaterialButtonToggleGroup.ButtonCheckedEventArgs e)
    {
        if(disableButtonNotifications)
            return;
        
        var rg = (MaterialButtonToggleGroup)sender!;

        if (rg.CheckedButtonId != -1)
        {
            var id = rg.CheckedButtonId;
            var button = rg.FindViewById(id);
            var i = rg.IndexOfChild(button);

            //Should trigger MapSelectedSegment and switch the selected rg
            if (VirtualView.SelectedIndex != i)
                VirtualView.SetSelectedIndex(i);
        }
    }

    static void MapChildren(SegmentedViewHandler handler, ISegmentedView control)
    {
        var virtualView = handler.VirtualView;
        var rg = handler.PlatformView;
        rg.RemoveAllViews();
        handler.selectedButton = null;

        //using var layoutInflater = LayoutInflater.From(handler.Context)!;       
        
        for (var i = 0; i < virtualView.Children.Count; i++)
        {
            var o = virtualView.Children[i];
            
            //var rb = (RadioButton)layoutInflater.Inflate(Resource.Layout.RadioButton, null)!;
            var rb = new MaterialButton(handler.Context, null, Resource.Attribute.materialButtonOutlinedStyle);
            //rb.SetButtonDrawable(null);
            //rb.SetForegroundGravity(GravityFlags.Center);
            
            //rb.SetSingleLine(true);
            //rb.Ellipsize = TextUtils.TruncateAt.End;
            
            rb.Text = o.GetText(control);
            SetTextColor(rb, control);

            // rb.SetBackgroundResource(
            //     i == 0 ? Resource.Drawable.segmented_control_first_background :
            //     i == virtualView.Children.Count - 1 ? Resource.Drawable.segmented_control_last_background :
            //     Resource.Drawable.segmented_control_background);

            if(control.DisplayMode == SegmentDisplayMode.EqualWidth)
                rb.LayoutParameters = new MaterialButtonToggleGroup.LayoutParams(0, ViewGroup.LayoutParams.WrapContent, 1f);
            else
                rb.LayoutParameters = new RadioGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.MatchParent);

            ConfigureRadioButton(handler, control, rb);
            rg.AddView(rb);
        }

        MapSelectedIndex(handler, control);
        
        //rg.ForceLayout();
        //rg.RequestLayout();
        //handler.VirtualView.InvalidateMeasure();
    }

    static void ConfigureRadioButton(SegmentedViewHandler handler, ISegmentedView control, MaterialButton rb)
    {
        var virtualView = handler.VirtualView;

#region change colors
        // var drawableContainerState = (DrawableContainer.DrawableContainerState)rb.Background!.GetConstantState()!;
        // var drawables = drawableContainerState.GetChildren();
        //
        // var selectedShape = drawables[0] as GradientDrawable ?? (GradientDrawable)((InsetDrawable)drawables[0]).Drawable;
        // var unselectedShape = drawables[1] as GradientDrawable ?? (GradientDrawable)((InsetDrawable)drawables[1]).Drawable;
        //
        // var color = virtualView.IsEnabled ? virtualView.TintColor.ToPlatform() : virtualView.DisabledColor.ToPlatform();
        //
        // selectedShape.SetStroke(3, color);
        // selectedShape.SetColor(color);
        // unselectedShape.SetStroke(3, color);

        rb.BackgroundTintList = new (
        [
            [-global::Android.Resource.Attribute.StateEnabled], //disabled
            [global::Android.Resource.Attribute.StateEnabled],  //enabled
            //[-global::Android.Resource.Attribute.Checked],    // unchecked
            //[global::Android.Resource.Attribute.Checked],     // checked            
        ], 
        [
            virtualView.DisabledColor.ToPlatform(),
            virtualView.TintColor.ToPlatform(),
        ]);
#endregion

        rb.Enabled = virtualView.IsEnabled;
        
        var fontManager = handler.Services.GetRequiredService<IFontManager>();
        rb.UpdateFont(control, fontManager);
        
        var padding = handler.Context.ToPixels(control.ItemPadding);
        rb.SetPadding((int)padding.Left, (int)padding.Top, (int)padding.Right, (int)padding.Bottom);
    }
    
    static void MapSelectedIndex(SegmentedViewHandler handler, ISegmentedView control)
    {
        //var virtualView = handler.VirtualView;

        var button = (MaterialButton?)handler.PlatformView.GetChildAt(control.SelectedIndex);
        
        handler.disableButtonNotifications = true;
        try
        {
            if (handler.selectedButton != null && handler.selectedButton != button)
            {
                if (handler.selectedButton.Checked)
                    handler.selectedButton.Checked = false;
                //SetTextColor(handler.selectedButton, virtualView);
            }

            if (button?.Checked == false)
                button.Checked = true;
        }
        finally
        {
            handler.disableButtonNotifications = false;
        }

        // if (button != null)
        //     SetTextColor(button, virtualView);
        
        handler.selectedButton = button;
    }

    static void SetTextColor(MaterialButton rb, ISegmentedView virtualView)
    {
        rb.ForegroundTintList = new (
            [
                [-global::Android.Resource.Attribute.StateEnabled],
                [global::Android.Resource.Attribute.StateChecked],            
                [global::Android.Resource.Attribute.StateEnabled],
            ], 
            [
                virtualView.DisabledColor.ToPlatform(),
                virtualView.SelectedTextColor.ToPlatform(),
                virtualView.TextColor.ToPlatform(),
            ]);
        
        // var textColor =
        //     isSelected ? virtualView.SelectedTextColor.ToPlatform() 
        //     : virtualView.IsEnabled ? virtualView.TextColor.ToPlatform() 
        //     : virtualView.DisabledColor.ToPlatform();
        //
        // rb.ForegroundTintList = ColorStateList.ValueOf(textColor);
    }

    static void MapTextColor(SegmentedViewHandler handler, ISegmentedView control) 
        => DoForAllChildren(handler, v => SetTextColor(v, control));

    // static void MapBorderColor(SegmentedControlHandler handler, ISegmentedControl control)
    // {
    //     handler.PlatformView.bot
    // }
    //
    // static void MapBorderWidth(SegmentedControlHandler handler, ISegmentedControl control)
    // {
    // }

    static void MapCharacterSpacing(SegmentedViewHandler handler, ITextStyle control) 
        => DoForAllChildren(handler, v => v.UpdateCharacterSpacing(control));

    static void MapFont(SegmentedViewHandler handler, ITextStyle control)
    {
        var fontManager = handler.Services.GetRequiredService<IFontManager>();
        DoForAllChildren(handler, v => v.UpdateFont(control, fontManager));
    }
    
    static void MapItemPadding(SegmentedViewHandler handler, ISegmentedView control)
    {
        var padding = handler.Context.ToPixels(control.ItemPadding);

        DoForAllChildren(handler, v =>
        {
            v.SetPadding((int)padding.Left, (int)padding.Top, (int)padding.Right, (int)padding.Bottom);
        });
    }

    static void MapIsEnabled2(SegmentedViewHandler handler, ISegmentedView control)
    {
        MapIsEnabled(handler, control);
        DoForAllChildren(handler, v => ConfigureRadioButton(handler, control, v));
    }

    static void ReconfigureRadioButtons(SegmentedViewHandler handler, ISegmentedView control) 
        => DoForAllChildren(handler, v => ConfigureRadioButton(handler, control, v));

    static void DoForAllChildren(SegmentedViewHandler handler, Action<MaterialButton> action)
    {
        for (var i = 0; i < handler.PlatformView.ChildCount; i++)
        {
            var v = (MaterialButton)handler.PlatformView.GetChildAt(i)!;
            action(v);
        }
    }

    static void DoForAllChildren(SegmentedViewHandler handler, Action<MaterialButton, int> action)
    {
        for (var i = 0; i < handler.PlatformView.ChildCount; i++)
        {
            var v = (MaterialButton)handler.PlatformView.GetChildAt(i)!;
            action(v, i);
        }
    }
}