using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using UIKit;

namespace Vapolia.SegmentedViews.Platforms.Ios;

internal class SegmentedViewHandler : ViewHandler<ISegmentedView, UISegmentedControl>
{
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

        [nameof(ISegmentedView.TintColor)] = MapTintColor,
        [nameof(ISegmentedView.SelectedTextColor)] = MapSelectedTextColor,
        [nameof(ITextStyle.TextColor)] = MapTextColor,
        [nameof(ISegmentedView.DisabledColor)] = MapDisabledColor,
        [nameof(ISegmentedView.BackgroundColor)] = MapBackgroundColor,
        [nameof(ISegmentedView.BorderColor)] = MapBorderColor,
        // [nameof(ISegmentedControl.BorderWidth)] = MapBorderWidth,
    };    

    public SegmentedViewHandler() : base(Mapper)
    {
    }

    public SegmentedViewHandler(IPropertyMapper mapper) : base(mapper ?? Mapper)
    {
    }

    protected override UISegmentedControl CreatePlatformView() => new();

    public override bool NeedsContainer => false;

    protected override void ConnectHandler(UISegmentedControl platformView)
    {
        base.ConnectHandler(platformView);
        platformView.ValueChanged += PlatformView_ValueChanged;
    }

    protected override void DisconnectHandler(UISegmentedControl platformView)
    {
        platformView.ValueChanged -= PlatformView_ValueChanged;
        base.DisconnectHandler(platformView);
    }

    void PlatformView_ValueChanged(object? sender, EventArgs e)
    {
        var i = (int)PlatformView.SelectedSegment;
        if (VirtualView.SelectedIndex != i)
            VirtualView.SetSelectedIndex(i);
    }

    static void MapChildren(SegmentedViewHandler handler, ISegmentedView virtualView)
    {
        var segmentControl = handler.PlatformView;
        segmentControl.RemoveAllSegments();

        var widths = virtualView.GetWidths();
        
        var wrapContent = widths.All(o => o.IsAuto);
        segmentControl.ApportionsSegmentWidthsByContent = wrapContent;

        for (var i = 0; i < virtualView.Children.Count; i++)
        {
            var segment = virtualView.Children[i];
            var width = widths[i];
            segmentControl.InsertSegment(segment.GetText(virtualView), i, false);

            segmentControl.SetEnabled(true, i);
            
            if (width.IsStar || width.IsAuto)
                segmentControl.SetWidth(0, i); //autosize
            else
            {
                if (width.Value == 0)
                {
                    segmentControl.SetWidth(1, i);
                    segmentControl.SetEnabled(false, i);
                }
                else
                    segmentControl.SetWidth((nfloat)width.Value, i);
            }
        }
        
        MapSelectedIndex(handler, virtualView);
    }

    static void MapTintColor(SegmentedViewHandler handler, ISegmentedView control)
    {
        var color = (control.IsEnabled ? control.TintColor : control.DisabledColor).ToPlatform();
        handler.PlatformView.TintColor = color;
        handler.PlatformView.SelectedSegmentTintColor = color;
    }

    static void MapSelectedIndex(SegmentedViewHandler handler, ISegmentedView control) 
        => handler.PlatformView.SelectedSegment = control.SelectedIndex;

    static void MapItemPadding(SegmentedViewHandler handler, ISegmentedView control)
    {
        var padding = control.ItemPadding;
        // handler.PlatformView.SetContentPositionAdjustment(new ((nfloat)padding.Left, (nfloat)padding.Top), UISegmentedControlSegment.Any, UIBarMetrics.Default);
        //TODO
        //See https://gist.github.com/nubbel/f675113429b5c7429252
    }
    
    static void MapIsEnabled2(SegmentedViewHandler handler, ISegmentedView control)
    {
        handler.PlatformView.Enabled = control.IsEnabled;
        MapTintColor(handler, control);
    }

    static void MapBorderColor(SegmentedViewHandler handler, ISegmentedView control) 
    {
        handler.PlatformView.Layer.BorderColor = control.BorderColor.ToPlatform().CGColor;
    }

    static void MapBackgroundColor(SegmentedViewHandler handler, ISegmentedView control) 
        => handler.PlatformView.BackgroundColor = control.BackgroundColor.ToPlatform();

    static void MapDisabledColor(SegmentedViewHandler handler, ISegmentedView control) 
        => SetTextColor(handler.PlatformView, control.TextColor, UIControlState.Disabled);

    static void MapTextColor(SegmentedViewHandler handler, ISegmentedView control) 
        => SetTextColor(handler.PlatformView, control.TextColor, UIControlState.Normal);

    static void MapSelectedTextColor(SegmentedViewHandler handler, ISegmentedView control) 
        => SetTextColor(handler.PlatformView, control.TextColor, UIControlState.Selected);

    static void SetTextColor(UISegmentedControl control, Color color, UIControlState state)
    {
        var titleTextAttributes = new UIStringAttributes { ForegroundColor = color.ToPlatform() };
        control.SetTitleTextAttributes(titleTextAttributes, state);
    }
    
    static void MapCharacterSpacing(SegmentedViewHandler handler, ITextStyle control)
    {
        var kerningAdjustment = control.CharacterSpacing == 0 ? null : (float?)control.CharacterSpacing;
        var titleTextAttributes = new UIStringAttributes { KerningAdjustment = kerningAdjustment };
        handler.PlatformView.SetTitleTextAttributes(titleTextAttributes, UIControlState.Normal);
        titleTextAttributes = new UIStringAttributes { KerningAdjustment = kerningAdjustment };
        handler.PlatformView.SetTitleTextAttributes(titleTextAttributes, UIControlState.Disabled);
        titleTextAttributes = new UIStringAttributes { KerningAdjustment = kerningAdjustment };
        handler.PlatformView.SetTitleTextAttributes(titleTextAttributes, UIControlState.Selected);
    }

    static void MapFont(SegmentedViewHandler handler, ITextStyle control)
    {
        var fontManager = handler.Services?.GetRequiredService<IFontManager>();
        if (fontManager == null)
            return;
        
        var uiFont = fontManager.GetFont(control.Font, UIFont.ButtonFontSize);

        var titleTextAttributes = new UIStringAttributes { Font = uiFont };
        handler.PlatformView.SetTitleTextAttributes(titleTextAttributes, UIControlState.Normal);
    }
    
        
    private static void MapIsSelectionRequired(SegmentedViewHandler handler, ISegmentedView control)
    {
        // handler.PlatformView.SelectionRequired = control.IsSelectionRequired;
        // if (control.IsSelectionRequired && control.SelectedIndex < 0 && control.Children.Count > 0)
        // {
        //     control.SetSelectedIndex(0);
        //     ((MaterialButton)handler.PlatformView.GetChildAt(0)!).Checked = true;
        // }
    }
}