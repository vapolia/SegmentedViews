using CommunityToolkit.WinUI.Controls;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml.Media;
using ListViewSelectionMode = Microsoft.UI.Xaml.Controls.ListViewSelectionMode;
using SelectionChangedEventArgs = Microsoft.UI.Xaml.Controls.SelectionChangedEventArgs;

namespace Vapolia.SegmentedViews;

internal class SegmentedViewHandler : ViewHandler<ISegmentedView, Segmented>
{
    private IFontManager? fontManager;
    
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

        [nameof(ISegmentedView.SelectedBackgroundColor)] = MapTintColor,
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

    public SegmentedViewHandler(IPropertyMapper? mapper) : base(mapper ?? Mapper)
    {
    }

    protected override Segmented CreatePlatformView()
    {
        var segmented = new Segmented
        {
            SelectionMode = ListViewSelectionMode.Single,
            SelectedIndex = -1
        };

        return segmented;
    }

    public override bool NeedsContainer => false;

    protected override void ConnectHandler(Segmented platformView)
    {
        base.ConnectHandler(platformView);
        fontManager = Services?.GetService<IFontManager>();
        platformView.SelectionChanged += PlatformView_OnSelectionChanged;
    }

    protected override void DisconnectHandler(Segmented platformView)
    {
        platformView.SelectionChanged -= PlatformView_OnSelectionChanged;
        base.DisconnectHandler(platformView);
    }

    private void PlatformView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var i = PlatformView.SelectedIndex;
        if (VirtualView.SelectedIndex != i)
            VirtualView.SetSelectedIndex(i);
    }

    static void MapChildren(SegmentedViewHandler handler, ISegmentedView virtualView)
    {
        var segmentControl = handler.PlatformView;
        segmentControl.Items.Clear();

        var widths = virtualView.GetWidths();
        
        var wrapContent = widths.All(o => o.IsAuto);
        //segmentControl.ApportionsSegmentWidthsByContent = wrapContent;

        var fontFamily = handler.fontManager != null ? handler.fontManager.GetFontFamily(virtualView.Font) : FontFamily.XamlAutoFontFamily;
        var fontSize = handler.fontManager?.GetFontSize(virtualView.Font) ?? 0;
        var spacing = (int)Math.Round(virtualView.CharacterSpacing);
        var padding = virtualView.ItemPadding.ToPlatform();
        var textColor = virtualView.TextColor.ToPlatform();
        //var borderColor = virtualView.BorderColor.ToPlatform();
        

        for (var i = 0; i < virtualView.Children.Count; i++)
        {
            var segment = virtualView.Children[i];
            var width = widths[i];
            
            var item = new SegmentedItem
            {
                Content = segment.GetText(virtualView),
                IsSelected = false,
                IsEnabled = true,
                Padding = padding,
                CharacterSpacing = spacing,
                Foreground = textColor,
                //BorderBrush = borderColor, //Separator color
            };

            if (handler.fontManager != null)
            {
                item.FontFamily = fontFamily;
                item.FontSize = fontSize;
            }

            if (width.IsStar || width.IsAuto)
                item.Width = width.IsAuto || width.IsStar ? double.NaN : width.Value; //autosize
            else
            {
                if (width.Value == 0)
                {
                    item.Width = 0;
                    item.IsEnabled = false;
                }
                else
                    item.Width = width.Value;
            }

            segmentControl.Items.Add(item);
        }
        
        MapSelectedIndex(handler, virtualView);
    }

    static void MapTintColor(SegmentedViewHandler handler, ISegmentedView control)
    {
        //TintColor
        //handler.PlatformView.Background = (control.IsEnabled ? control.SelectedBackgroundColor : control.DisabledColor).ToPlatform();
        //handler.PlatformView.SelectedSegmentTintColor = (control.IsEnabled ? control.SelectedBackgroundColor : control.DisabledColor).ToPlatform();
    }

    static void MapSelectedIndex(SegmentedViewHandler handler, ISegmentedView control) 
        => handler.PlatformView.SelectedIndex = control.SelectedIndex;

    static void MapItemPadding(SegmentedViewHandler handler, ISegmentedView control)
    {
        foreach (SegmentedItem item in handler.PlatformView.Items)
            item.Padding = control.ItemPadding.ToPlatform();
    }
    
    static void MapIsEnabled2(SegmentedViewHandler handler, ISegmentedView control)
    {
        handler.PlatformView.IsEnabled = control.IsEnabled;
        MapTintColor(handler, control);
    }

    static void MapBorderColor(SegmentedViewHandler handler, ISegmentedView control)
    {
        var borderColor = control.BorderColor.ToPlatform();
        handler.PlatformView.BorderBrush = borderColor;
        
        //Separator color
        //foreach (SegmentedItem item in handler.PlatformView.Items)
        //  item.BorderBrush = borderColor;
    }

    static void MapBackgroundColor(SegmentedViewHandler handler, ISegmentedView control) 
        => handler.PlatformView.Background = control.BackgroundColor.ToPlatform();

    static void MapDisabledColor(SegmentedViewHandler handler, ISegmentedView control)
    {
        // ???
    }

    static void MapTextColor(SegmentedViewHandler handler, ISegmentedView control)
    {
        foreach (SegmentedItem item in handler.PlatformView.Items)
            item.Foreground = control.TextColor.ToPlatform();
    }

    static void MapSelectedTextColor(SegmentedViewHandler handler, ISegmentedView control)
    {
        // ???
    }

    static void MapCharacterSpacing(SegmentedViewHandler handler, ITextStyle control)
    {
        var spacing = (int)Math.Round(control.CharacterSpacing);
        foreach (SegmentedItem item in handler.PlatformView.Items)
            item.CharacterSpacing = spacing;
    }

    static void MapFont(SegmentedViewHandler handler, ITextStyle control)
    {
        var fontManager = handler.fontManager;
        if (fontManager == null)
            return;
        
        var fontFamily = fontManager.GetFontFamily(control.Font);
        var fontSize = fontManager.GetFontSize(control.Font);

        foreach (SegmentedItem item in handler.PlatformView.Items)
        {
            item.FontFamily = fontFamily;
            item.FontSize = fontSize;
        }
    }
    
        
    private static void MapIsSelectionRequired(SegmentedViewHandler handler, ISegmentedView control)
    {
        if (control.IsSelectionRequired)
        {
            if(handler.PlatformView.SelectedIndex < 0 && handler.PlatformView.Items.Count > 0)
                handler.PlatformView.SelectedIndex = 0;
        }
    }
}