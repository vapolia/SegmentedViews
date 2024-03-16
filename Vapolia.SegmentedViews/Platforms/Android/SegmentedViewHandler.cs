using Android.Content.Res;
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
    ColorStateList? BorderColor;

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
        
        [nameof(ISegmentedView.SelectedBackgroundColor)] = ReconfigureRadioButtons,
        [nameof(ISegmentedView.SelectedTextColor)] = MapTextColor,
        [nameof(ITextStyle.TextColor)] = MapTextColor,
        [nameof(ISegmentedView.DisabledColor)] = ReconfigureRadioButtons,
        [nameof(ISegmentedView.BackgroundColor)] = ReconfigureRadioButtons,
        [nameof(ISegmentedView.BorderColor)] = MapBorderColor,
        // [nameof(ISegmentedView.BorderWidth)] = MapBorderWidth,
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

    /// <summary>
    /// Fix tabs won't use the full container's width
    /// </summary>
    /// <remarks>
    /// Credits: yurkinh
    /// </remarks>
    public override void PlatformArrange(Rect frame)
    {
        var platformView = PlatformView;
        var virtualView = VirtualView;

        if (frame.Width <= 0 || frame.Height <= 0 || platformView == null! || virtualView == null!)
            return;

        // If the Width and Height are both explicit, then it has already done MeasureSpecMode.Exactly in both dimensions; no need to do it again
        // Otherwise it needs a second measurement pass so TextView can properly handle alignments
        var needsExactMeasure = (virtualView.VerticalLayoutAlignment == Microsoft.Maui.Primitives.LayoutAlignment.Fill
                                 || virtualView.HorizontalLayoutAlignment == Microsoft.Maui.Primitives.LayoutAlignment.Fill)
                                && virtualView is not { Width: >= 0, Height: >= 0 };
        
        // Depending on our layout situation, the TextView may need an additional measurement pass at the final size
        // in order to properly handle any TextAlignment properties and some internal bookkeeping
        if (needsExactMeasure)
            platformView.Measure(MakeExact(frame.Width), MakeExact(frame.Height));
        
        base.PlatformArrange(frame);
        
        int MakeExact(double size) => MeasureSpecMode.Exactly.MakeMeasureSpec((int)platformView.Context.ToPixels(size));
    }

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
        var segmentContainer = handler.PlatformView;
        segmentContainer.RemoveAllViews();
        handler.selectedButton = null;

        var widths = virtualView.GetWidths();
        
        var i = 0;
        foreach (var segment in virtualView.Children)
        {
            var segmentButton = new MaterialButton(handler.Context, null, Resource.Attribute.materialButtonOutlinedStyle);
            
            SetTextColor(segmentButton, virtualView);
            segmentButton.Text = segment.GetText(virtualView);

            var width = widths[i];
            
            if(width.IsStar)
                segmentButton.LayoutParameters = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.MatchParent, (float)width.Value);
            else if(width.IsAuto)
                segmentButton.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.MatchParent);
            else if(width.IsAbsolute)
                segmentButton.LayoutParameters = new LinearLayout.LayoutParams((int)handler.Context.ToPixels(width.Value), ViewGroup.LayoutParams.MatchParent);
            else //relative, what does that mean ?
                segmentButton.LayoutParameters = new LinearLayout.LayoutParams((int)handler.Context.ToPixels(width.Value), ViewGroup.LayoutParams.MatchParent);

            ConfigureButton(segmentButton, handler, virtualView);
            segmentContainer.AddView(segmentButton);
            i++;
        }
        
        MapSelectedIndex(handler, virtualView);
    }

    static void ConfigureButton(MaterialButton button, SegmentedViewHandler handler, ISegmentedView control)
    {
        button.BackgroundTintList = new (
        [
            [-global::Android.Resource.Attribute.StateEnabled], //disabled
            [global::Android.Resource.Attribute.StateChecked],  // checked            
            [],  //default
        ], 
        [
            control.DisabledColor.ToPlatform(),
            control.SelectedBackgroundColor.ToPlatform(),
            control.BackgroundColor.ToPlatform(),
        ]);

        button.StrokeColor = handler.BorderColor;
        button.Enabled = control.IsEnabled;
        
        UpdateFont(button, handler, control);
        button.UpdateCharacterSpacing(control);
        
        var padding = handler.Context.ToPixels(control.ItemPadding);
        button.SetPadding((int)padding.Left, (int)padding.Top, (int)padding.Right, (int)padding.Bottom);
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
        var colors = new ColorStateList(
            [
                [-global::Android.Resource.Attribute.StateEnabled],
                [global::Android.Resource.Attribute.StateChecked],
                [global::Android.Resource.Attribute.Checked],
                [global::Android.Resource.Attribute.StateSelected],
                [global::Android.Resource.Attribute.StatePressed],
                [],
            ],
            [
                virtualView.TextColor.ToPlatform(), //do not use DisabledColor otherwise text color = background color and text is not visible. Create DisabledTextColor instead ?
                virtualView.SelectedTextColor.ToPlatform(),
                virtualView.SelectedTextColor.ToPlatform(),
                virtualView.SelectedTextColor.ToPlatform(),
                virtualView.SelectedTextColor.ToPlatform(),
                virtualView.TextColor.ToPlatform(),
            ]);
        
        button.SetTextColor(colors);
    }
    
    static void MapBorderColor(SegmentedViewHandler handler, ISegmentedView virtualView)
    {
        handler.BorderColor = new (
            [
                [-global::Android.Resource.Attribute.StateEnabled],
                [],
            ],
            [
                virtualView.DisabledColor.ToPlatform(),
                virtualView.BorderColor.ToPlatform(),
            ]);
        
        DoForAllChildren(handler, v => v.StrokeColor = handler.BorderColor);
    }

    // static void MapBorderWidth(SegmentedControlHandler handler, ISegmentedControl control)
    // {
    // }

    static void MapCharacterSpacing(SegmentedViewHandler handler, ITextStyle control) 
        => DoForAllChildren(handler, v => v.UpdateCharacterSpacing(control));

    static void MapFont(SegmentedViewHandler handler, ITextStyle control)
    {
        var fontManager = handler.Services?.GetRequiredService<IFontManager>()!;
        DoForAllChildren(handler, v => UpdateFont(v, handler, control, fontManager));
    }

    static void UpdateFont(MaterialButton button, SegmentedViewHandler handler, ITextStyle control, IFontManager? fontManager = null)
    {
        fontManager ??= handler.Services?.GetRequiredService<IFontManager>()!;
        button.UpdateFont(control, fontManager);
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
        DoForAllChildren(handler, v => ConfigureButton(v, handler, control));
    }

    static void ReconfigureRadioButtons(SegmentedViewHandler handler, ISegmentedView control)
        => DoForAllChildren(handler, v => ConfigureButton(v, handler, control));

    static void DoForAllChildren(SegmentedViewHandler handler, Action<MaterialButton> action)
    {
        for (var i = 0; i < handler.PlatformView.ChildCount; i++)
        {
            var v = (MaterialButton)handler.PlatformView.GetChildAt(i)!;
            action(v);
        }
    }
}