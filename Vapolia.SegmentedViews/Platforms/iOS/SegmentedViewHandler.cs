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
        
        [nameof(ISegmentedView.ItemPadding)] = MapItemPadding,
        [nameof(ISegmentedView.TintColor)] = MapTintColor,
        
        [nameof(ISegmentedView.SelectedTextColor)] = MapSelectedTextColor,
        [nameof(ITextStyle.TextColor)] = MapTextColor,
        [nameof(ISegmentedView.DisabledColor)] = MapDisabledColor,
        
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
            segmentControl.InsertSegment(segment.GetText(virtualView), i, true);

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
        handler.PlatformView.SetContentPositionAdjustment(new ((nfloat)padding.Left, (nfloat)padding.Top), UISegmentedControlSegment.Any, UIBarMetrics.Default);
    }
    
    static void MapIsEnabled2(SegmentedViewHandler handler, ISegmentedView control)
    {
        handler.PlatformView.Enabled = control.IsEnabled;
        MapTintColor(handler, control);
    }

    static void MapSelectedTextColor(SegmentedViewHandler handler, ISegmentedView control) 
    {
        var titleTextAttributes = handler.PlatformView.GetTitleTextAttributes(UIControlState.Selected);
        titleTextAttributes.ForegroundColor = control.SelectedTextColor.ToPlatform();
        handler.PlatformView.SetTitleTextAttributes(titleTextAttributes, UIControlState.Selected);
    }

    static void MapDisabledColor(SegmentedViewHandler handler, ISegmentedView control) 
    {
        var titleTextAttributes = handler.PlatformView.GetTitleTextAttributes(UIControlState.Disabled);
        titleTextAttributes.ForegroundColor = control.DisabledColor.ToPlatform();
        handler.PlatformView.SetTitleTextAttributes(titleTextAttributes, UIControlState.Disabled);
    }

    static void MapTextColor(SegmentedViewHandler handler, ISegmentedView control)
    {
        var titleTextAttributes = handler.PlatformView.GetTitleTextAttributes(UIControlState.Normal);
        titleTextAttributes.ForegroundColor = control.TextColor.ToPlatform();
        handler.PlatformView.SetTitleTextAttributes(titleTextAttributes, UIControlState.Normal);
    }
    
    static void MapCharacterSpacing(SegmentedViewHandler handler, ITextStyle control)
    {
        var kerningAdjustment = control.CharacterSpacing == 0 ? null : (float?)control.CharacterSpacing;
        
        var titleTextAttributes = handler.PlatformView.GetTitleTextAttributes(UIControlState.Normal);
        titleTextAttributes.KerningAdjustment = kerningAdjustment;
        handler.PlatformView.SetTitleTextAttributes(titleTextAttributes, UIControlState.Normal);

        titleTextAttributes = handler.PlatformView.GetTitleTextAttributes(UIControlState.Disabled);
        titleTextAttributes.KerningAdjustment = kerningAdjustment;
        handler.PlatformView.SetTitleTextAttributes(titleTextAttributes, UIControlState.Disabled);

        titleTextAttributes = handler.PlatformView.GetTitleTextAttributes(UIControlState.Selected);
        titleTextAttributes.KerningAdjustment = kerningAdjustment;
        handler.PlatformView.SetTitleTextAttributes(titleTextAttributes, UIControlState.Selected);
    }

    static void MapFont(SegmentedViewHandler handler, ITextStyle control)
    {
        var fontManager = handler.Services.GetRequiredService<IFontManager>();
        var uiFont = fontManager.GetFont(control.Font, UIFont.ButtonFontSize);
        
        var titleTextAttributes = handler.PlatformView.GetTitleTextAttributes(UIControlState.Normal);
        titleTextAttributes.Font = uiFont;
        handler.PlatformView.SetTitleTextAttributes(titleTextAttributes, UIControlState.Normal);
    }
}