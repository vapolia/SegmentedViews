using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Font = Microsoft.Maui.Font;
using RadioButton = Android.Widget.RadioButton;

namespace Vapolia.SegmentedViews.Platforms.Android;

internal class SegmentedViewHandler : ViewHandler<ISegmentedView, RadioGroup>
{
    RadioButton? selectedButton;
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

    protected override RadioGroup CreatePlatformView()
    {
        using var layoutInflater = LayoutInflater.From(Context)!;
        return (RadioGroup)layoutInflater.Inflate(Resource.Layout.RadioGroup, null);
    }

    protected override void ConnectHandler(RadioGroup platformView)
    {
        base.ConnectHandler(platformView);

        platformView.EnsureId();
        platformView.CheckedChange += PlatformView_CheckedChange;
    }

    protected override void DisconnectHandler(RadioGroup platformView)
    {
        platformView.CheckedChange -= PlatformView_CheckedChange;
        selectedButton = null;

        base.DisconnectHandler(platformView);
    }

    void PlatformView_CheckedChange(object? sender, RadioGroup.CheckedChangeEventArgs e)
    {
        if(disableRgNotifications)
            return;
        
        var rg = (RadioGroup)sender!;
        if (rg.CheckedRadioButtonId != -1)
        {
            var id = rg.CheckedRadioButtonId;
            var radioButton = rg.FindViewById(id);
            var i = rg.IndexOfChild(radioButton);

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

        using var layoutInflater = LayoutInflater.From(handler.Context)!;       
        
        for (var i = 0; i < virtualView.Children.Count; i++)
        {
            var o = virtualView.Children[i];
            var rb = (RadioButton)layoutInflater.Inflate(Resource.Layout.RadioButton, null)!;

            rb.LayoutParameters = new RadioGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent, 1f);
            rb.Text = o.GetText(control);
            SetTextColor(rb, i == control.SelectedIndex, control);

            if (i == 0)
                rb.SetBackgroundResource(Resource.Drawable.segmented_control_first_background);
            else if (i == virtualView.Children.Count - 1)
                rb.SetBackgroundResource(Resource.Drawable.segmented_control_last_background);

            ConfigureRadioButton(virtualView, i, rb);
            rg.AddView(rb);
        }

        MapSelectedIndex(handler, control);
    }

    static void ConfigureRadioButton(ISegmentedView virtualView, int i, RadioButton rb)
    {
        var gradientDrawable = (StateListDrawable)rb.Background!;
        var drawableContainerState = (DrawableContainer.DrawableContainerState)gradientDrawable.GetConstantState()!;
        var children = drawableContainerState.GetChildren();

        var selectedShape = children[0] as GradientDrawable ?? (GradientDrawable)((InsetDrawable)children[0]).Drawable;
        var unselectedShape = children[1] as GradientDrawable ?? (GradientDrawable)((InsetDrawable)children[1]).Drawable;

        var color = virtualView.IsEnabled ? virtualView.TintColor.ToPlatform() : virtualView.DisabledColor.ToPlatform();

        selectedShape.SetStroke(3, color);
        selectedShape.SetColor(color);
        unselectedShape.SetStroke(3, color);

        rb.Enabled = virtualView.IsEnabled;
    }
    
    static void MapTintColor(SegmentedViewHandler handler, ISegmentedView control) 
        => ReconfigureRadioButtons(handler, control);

    static void MapIsEnabled(SegmentedViewHandler handler, ISegmentedView control) 
        => ReconfigureRadioButtons(handler, control);

    static void MapSelectedIndex(SegmentedViewHandler handler, ISegmentedView control)
    {
        var virtualView = handler.VirtualView;

        var option = (RadioButton?)handler.PlatformView.GetChildAt(control.SelectedIndex);
        
        handler.disableRgNotifications = true;
        try
        {

            if (handler.selectedButton != null && handler.selectedButton != option)
            {
                if (handler.selectedButton.Checked)
                    handler.selectedButton.Checked = false;
                SetTextColor(handler.selectedButton, false, virtualView);
            }

            if (option?.Checked == false)
                option.Checked = true;
        }
        finally
        {
            handler.disableRgNotifications = false;
        }

        if (option != null)
            SetTextColor(option, true, virtualView);
        
        handler.selectedButton = option;
    }

    static void SetTextColor(RadioButton rb, bool isSelected, ISegmentedView virtualView)
    {
        var textColor =
            isSelected ? virtualView.SelectedTextColor.ToPlatform() 
            : virtualView.IsEnabled ? virtualView.TextColor.ToPlatform() 
            : virtualView.DisabledColor.ToPlatform();

        rb.SetTextColor(textColor);
    }

    static void MapSelectedTextColor(SegmentedViewHandler handler, ISegmentedView control)
    {
        var rb = (RadioButton?)handler.PlatformView.GetChildAt(control.SelectedIndex);
        if(rb != null)
            SetTextColor(rb, true, control);
    }

    static void MapTextColor(SegmentedViewHandler handler, ISegmentedView control)
    {
        for (var i = 0; i < handler.PlatformView.ChildCount; i++)
        {
            var rb = (RadioButton)handler.PlatformView.GetChildAt(i)!;
            SetTextColor(rb, i == control.SelectedIndex, control);
        }
    }

    // static void MapBorderColor(SegmentedControlHandler handler, ISegmentedControl control)
    // {
    //     handler.PlatformView.bot
    // }
    //
    // static void MapBorderWidth(SegmentedControlHandler handler, ISegmentedControl control)
    // {
    // }

    // static void MapFontSize(SegmentedControlHandler handler, ISegmentedControl control)
    // {
    //     for (var i = 0; i < handler.PlatformView.ChildCount; i++)
    //     {
    //         var v = (RadioButton)handler.PlatformView.GetChildAt(i);
    //         v.SetTextSize(ComplexUnitType.Dip, control.FontSize.ToEm());
    //     }
    // }

    static void MapFontSizeFamily(SegmentedViewHandler handler, ISegmentedView control)
    {
        var fontManager = handler.Services.GetRequiredService<IFontManager>();
        var typeface = Font.OfSize(control.FontFamily, control.FontSize).ToTypeface(fontManager);

        for (var i = 0; i < handler.PlatformView.ChildCount; i++)
        {
            var v = (RadioButton)handler.PlatformView.GetChildAt(i)!;
            v.SetTypeface(typeface, TypefaceStyle.Normal);
        }
    }
        
    static void ReconfigureRadioButtons(SegmentedViewHandler handler, ISegmentedView control)
    {
        for (var i = 0; i < control.Children.Count; i++)
        {
            var rb = (RadioButton)handler.PlatformView.GetChildAt(i)!;
            ConfigureRadioButton(handler.VirtualView, i, rb);
        }
    }
}