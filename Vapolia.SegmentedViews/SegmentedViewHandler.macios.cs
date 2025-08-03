using CoreGraphics;
using Foundation;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using ObjCRuntime;
using UIKit;

namespace Vapolia.SegmentedViews;

// ReSharper disable once InconsistentNaming
internal class UISegmentedControlEx : UISegmentedControl
{
    private Thickness? paddingSize;

    [Export("init")]
    public UISegmentedControlEx() {}

    [Export("initWithCoder:")]
    public UISegmentedControlEx(NSCoder coder) : base(coder) {}
    protected UISegmentedControlEx(NSObjectFlag t) : base(t) {}
    protected internal UISegmentedControlEx(NativeHandle handle) : base(handle) {}

    public Thickness? Padding
    {
        get => paddingSize;
        set
        {
            paddingSize = value;
            InvalidateIntrinsicContentSize();
        }
    }

    /// <summary>
    /// Maui does not use constraint positioning (ie: intrinsicContentSize)
    /// </summary>
    public override CGSize SizeThatFits(CGSize sizeToFit)
    {
        var size = base.SizeThatFits(sizeToFit);
        
        if (Padding is { IsEmpty: false } padding)
        {
            //On iOS the default minimum padding is 10.
            //To match Android, let's subtract 10.
            var width = padding.HorizontalThickness - 10;
            
            size.Width += (nfloat)(2 * width * (NumberOfSegments + 1));
            size.Height += (nfloat)(padding.VerticalThickness * 2);

            if (size.Width > sizeToFit.Width)
                size.Width = sizeToFit.Width;
            if (size.Height > sizeToFit.Height)
                size.Height = sizeToFit.Height;
        }

        return size;
    }
}

internal class SegmentedViewHandler : ViewHandler<ISegmentedView, UISegmentedControlEx>
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

    protected override UISegmentedControlEx CreatePlatformView() => new();

    public override bool NeedsContainer => false;

    protected override void ConnectHandler(UISegmentedControlEx platformView)
    {
        base.ConnectHandler(platformView);
        platformView.ValueChanged += PlatformView_ValueChanged;
    }

    protected override void DisconnectHandler(UISegmentedControlEx platformView)
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
        handler.PlatformView.TintColor = (control.IsEnabled ? control.SelectedBackgroundColor : control.DisabledColor).ToPlatform();
        handler.PlatformView.SelectedSegmentTintColor = (control.IsEnabled ? control.SelectedBackgroundColor : control.DisabledColor).ToPlatform();
    }

    static void MapSelectedIndex(SegmentedViewHandler handler, ISegmentedView control) 
        => handler.PlatformView.SelectedSegment = control.SelectedIndex;

    static void MapItemPadding(SegmentedViewHandler handler, ISegmentedView control)
    {
        handler.PlatformView.Padding = control.ItemPadding;
        // handler.PlatformView.SetContentPositionAdjustment(new ((nfloat)padding.Left, (nfloat)padding.Top), UISegmentedControlSegment.Any, UIBarMetrics.Default);
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
        => SetTextColor(handler.PlatformView, control.TextColor, UIControlState.Disabled); //Don't use disabled color otherwise the text is invisible

    static void MapTextColor(SegmentedViewHandler handler, ISegmentedView control) 
        => SetTextColor(handler.PlatformView, control.TextColor, UIControlState.Normal);

    static void MapSelectedTextColor(SegmentedViewHandler handler, ISegmentedView control) 
        => SetTextColor(handler.PlatformView, control.SelectedTextColor, UIControlState.Selected);

    static void SetTextColor(UISegmentedControlEx control, Color color, UIControlState state)
    {
        var titleTextAttributes = new UIStringAttributes { ForegroundColor = color.ToPlatform() };
        control.SetTitleTextAttributes(titleTextAttributes, state);
    }
    
    static void MapCharacterSpacing(SegmentedViewHandler handler, ITextStyle control)
    {
        var kerningAdjustment = control.CharacterSpacing == 0 ? null : (float?)control.CharacterSpacing;
        var titleTextAttributes = new UIStringAttributes { KerningAdjustment = kerningAdjustment };
        handler.PlatformView.SetTitleTextAttributes(titleTextAttributes, UIControlState.Normal);
        titleTextAttributes = new() { KerningAdjustment = kerningAdjustment };
        handler.PlatformView.SetTitleTextAttributes(titleTextAttributes, UIControlState.Disabled);
        titleTextAttributes = new() { KerningAdjustment = kerningAdjustment };
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