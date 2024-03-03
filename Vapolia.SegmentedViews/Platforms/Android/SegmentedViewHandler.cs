using Android.Views;
using Android.Widget;
using Google.Android.Material.Button;
using Google.Android.Material.Internal;
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
        [nameof(ISegmentedView.WidthDefinitions)] = MapChildren,
        [nameof(ISegmentedView.IsEnabled)] = MapIsEnabled2,
        [nameof(ISegmentedView.SelectedIndex)] = MapSelectedIndex,
        [nameof(ISegmentedView.IsSelectionRequired)] = MapIsSelectionRequired,

        [nameof(ISegmentedView.ItemPadding)] = MapItemPadding,
        [nameof(ITextStyle.CharacterSpacing)] = MapCharacterSpacing,
        [nameof(ITextStyle.Font)] = MapFont,
        
        [nameof(ISegmentedView.TintColor)] = ReconfigureRadioButtons,
        [nameof(ISegmentedView.SelectedTextColor)] = MapTextColor,
        [nameof(ITextStyle.TextColor)] = MapTextColor,
        [nameof(ISegmentedView.DisabledColor)] = ReconfigureRadioButtons,
        [nameof(ISegmentedView.BackgroundColor)] = ReconfigureRadioButtons,
        
        // [nameof(ISegmentedControl.BorderColor)] = MapBorderColor,
        // [nameof(ISegmentedControl.BorderWidth)] = MapBorderWidth,
    };


    public SegmentedViewHandler() : base(Mapper)
    {
    }

    public SegmentedViewHandler(IPropertyMapper mapper) : base(mapper ?? Mapper)
    {
    }

    protected override MaterialButtonToggleGroup CreatePlatformView()
    {
        //Xamarin.AndroidX.Compose.Material3
        //Material3.
        
        var rg = new MaterialButtonToggleGroup(Context, null, Resource.Attribute.materialButtonToggleGroupStyle);
        rg.Orientation = Orientation.Horizontal;
        rg.LayoutParameters = new (ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
        rg.SingleSelection = true;

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

        var buttonIndex = -1;
        if (rg.CheckedButtonId >= 0)
        {
            var button = rg.FindViewById(rg.CheckedButtonId);
            buttonIndex = rg.IndexOfChild(button);
        }

        //Will trigger MapSelectedSegment and switch the selected button
        if (VirtualView.SelectedIndex != buttonIndex)
            VirtualView.SetSelectedIndex(buttonIndex);
    }

    static void MapChildren(SegmentedViewHandler handler, ISegmentedView virtualView)
    {
        var rg = handler.PlatformView;
        rg.RemoveAllViews();
        handler.selectedButton = null;

        var widths = virtualView.GetWidths();
        
        var i = 0;
        foreach (var segment in virtualView.Children)
        {
            var rb = new MaterialButton(handler.Context, null, Resource.Attribute.materialButtonOutlinedStyle);
            
            SetTextColor(rb, virtualView);
            rb.Text = segment.GetText(virtualView);

            var width = widths[i];
            
            if(width.IsStar)
                rb.LayoutParameters = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.MatchParent, (float)width.Value);
            else if(width.IsAuto)
                rb.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.MatchParent);
            else if(width.IsAbsolute)
                rb.LayoutParameters = new LinearLayout.LayoutParams((int)handler.Context.ToPixels(width.Value), ViewGroup.LayoutParams.MatchParent);
            else //relative, what does that mean ?
                rb.LayoutParameters = new LinearLayout.LayoutParams((int)handler.Context.ToPixels(width.Value), ViewGroup.LayoutParams.MatchParent);

            ConfigureRadioButton(handler, virtualView, rb);
            rg.AddView(rb);
            i++;
        }

        MapSelectedIndex(handler, virtualView);
        
        //rg.ForceLayout();
        //rg.RequestLayout();
        //handler.VirtualView.InvalidateMeasure();
    }

    static void ConfigureRadioButton(SegmentedViewHandler handler, ISegmentedView control, MaterialButton rb)
    {
        var virtualView = handler.VirtualView;

        rb.BackgroundTintList = new (
        [
            [-global::Android.Resource.Attribute.StateEnabled], //disabled
            [global::Android.Resource.Attribute.StateChecked],  // checked            
            [global::Android.Resource.Attribute.StateEnabled],  //enabled
            //[-global::Android.Resource.Attribute.StateChecked],    // unchecked
        ], 
        [
            virtualView.DisabledColor.ToPlatform(),
            virtualView.TintColor.ToPlatform(),
            virtualView.BackgroundColor.ToPlatform(),
        ]);

        rb.Enabled = virtualView.IsEnabled;
        
        var fontManager = handler.Services.GetRequiredService<IFontManager>()!;
        //rb.UpdateFont(control, fontManager);
        
        var padding = handler.Context.ToPixels(control.ItemPadding);
        rb.SetPadding((int)padding.Left, (int)padding.Top, (int)padding.Right, (int)padding.Bottom);
    }
    
    static void MapSelectedIndex(SegmentedViewHandler handler, ISegmentedView control)
    {
        if (control.IsSelectionRequired && control.SelectedIndex < 0 || control.SelectedIndex >= control.Children.Count)
        {
            if (control.Children.Count > 0)
            {
                control.SetSelectedIndex(0);
                ((MaterialButton)handler.PlatformView.GetChildAt(0)!).Checked = true;
            }
        }
        
        var button = (MaterialButton?)handler.PlatformView.GetChildAt(control.SelectedIndex);
        
        handler.disableButtonNotifications = true;
        try
        {
            if (handler.selectedButton != null && handler.selectedButton != button)
            {
                if (handler.selectedButton.Checked)
                    handler.selectedButton.Checked = false;
            }

            if (button?.Checked == false)
                button.Checked = true;
        }
        finally
        {
            handler.disableButtonNotifications = false;
        }

        handler.selectedButton = button;
    }



    static void MapTextColor(SegmentedViewHandler handler, ISegmentedView control) 
        => DoForAllChildren(handler, button => SetTextColor(button, control));

    static void SetTextColor(MaterialButton button, ISegmentedView virtualView)
    {
        button.ForegroundTintList = new (
            [
                [-global::Android.Resource.Attribute.StateEnabled],
                [global::Android.Resource.Attribute.StateChecked],            
                [global::Android.Resource.Attribute.StateEnabled],
            ], 
            [
                virtualView.TextColor.ToPlatform(), //not DisabledColor otherwise text color = background color and text is not visible
                virtualView.SelectedTextColor.ToPlatform(),
                virtualView.TextColor.ToPlatform(),
            ]);
    }
    
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
        var fontManager = handler.Services.GetRequiredService<IFontManager>()!;
        //DoForAllChildren(handler, v => v.UpdateFont(control, fontManager));
    }
    
    private static void MapIsSelectionRequired(SegmentedViewHandler handler, ISegmentedView control)
    {
        handler.PlatformView.SelectionRequired = control.IsSelectionRequired;
        MapSelectedIndex(handler, control);
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
}