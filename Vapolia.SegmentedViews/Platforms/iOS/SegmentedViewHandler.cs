using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using UIKit;

namespace Vapolia.SegmentedViews.Platforms.Ios;

internal class SegmentedViewHandler : ViewHandler<ISegmentedView, UISegmentedControl>
{
    bool disableRgNotifications;

    public static IPropertyMapper<ISegmentedView, SegmentedViewHandler> Mapper = new PropertyMapper<ISegmentedView, SegmentedViewHandler>(ViewMapper)
    {
        [nameof(ISegmentedView.Children)] = MapChildren,
        [nameof(ISegmentedView.IsEnabled)] = MapIsEnabled,
        [nameof(ISegmentedView.SelectedIndex)] = MapSelectedIndex,
        [nameof(ISegmentedView.TintColor)] = MapTintColor,
        [nameof(ISegmentedView.SelectedTextColor)] = MapSelectedTextColor,
        [nameof(ISegmentedView.TextColor)] = MapTextColor,
        // [nameof(ISegmentedControl.BorderColor)] = MapBorderColor,
        // [nameof(ISegmentedControl.BorderWidth)] = MapBorderWidth,
        [nameof(ISegmentedView.FontSize)] = MapFontSizeFamily,
        [nameof(ISegmentedView.FontFamily)] = MapFontSizeFamily,
    };    

    public SegmentedViewHandler() : base(Mapper)
    {
    }

    public SegmentedViewHandler(IPropertyMapper mapper) : base(mapper ?? Mapper)
    {
    }

    protected override UISegmentedControl CreatePlatformView() => new();

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
        if(disableRgNotifications)
            return;

        var i = (int)PlatformView.SelectedSegment;
        if (VirtualView.SelectedIndex != i)
            VirtualView.SetSelectedIndex(i);
    }

    static void MapChildren(SegmentedViewHandler handler, ISegmentedView control)
    {
        var virtualView = handler.VirtualView;
        var segmentControl = handler.PlatformView;
        segmentControl.RemoveAllSegments();

        for (var i = 0; i < virtualView.Children.Count; i++)
            segmentControl.InsertSegment(virtualView.Children[i].GetText(virtualView), i, true);

        MapSelectedIndex(handler, control);
    }

    static void MapTintColor(SegmentedViewHandler handler, ISegmentedView control)
    {
        var color = (control.IsEnabled ? control.TintColor : control.DisabledColor).ToPlatform();
        handler.PlatformView.TintColor = color;
        handler.PlatformView.SelectedSegmentTintColor = color;
    }

    static void MapSelectedIndex(SegmentedViewHandler handler, ISegmentedView control) 
        => handler.PlatformView.SelectedSegment = control.SelectedIndex;

    static void MapIsEnabled(SegmentedViewHandler handler, ISegmentedView control)
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

    static void MapTextColor(SegmentedViewHandler handler, ISegmentedView control)
    {
        var titleTextAttributes = handler.PlatformView.GetTitleTextAttributes(UIControlState.Normal);
        titleTextAttributes.ForegroundColor = control.TextColor.ToPlatform();
        handler.PlatformView.SetTitleTextAttributes(titleTextAttributes, UIControlState.Normal);
    }

    static void MapFontSizeFamily(SegmentedViewHandler handler, ISegmentedView control)
    {
        var titleTextAttributes = handler.PlatformView.GetTitleTextAttributes(UIControlState.Normal);
        titleTextAttributes.Font = string.IsNullOrEmpty(control.FontFamily) ? UIFont.SystemFontOfSize((nfloat)control.FontSize) : UIFont.FromName(control.FontFamily, (nfloat) control.FontSize);
        handler.PlatformView.SetTitleTextAttributes(titleTextAttributes, UIControlState.Normal);
    }
}