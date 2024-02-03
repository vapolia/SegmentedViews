#if ANDROID
using Vapolia.SegmentedViews.Platforms.Android;
#endif
#if IOS
using Vapolia.SegmentedViews.Platforms.Ios;
#endif

[assembly: Microsoft.Maui.Controls.XmlnsDefinition("https://vapolia.eu/Vapolia.SegmentedViews", "Vapolia.SegmentedViews")]
[assembly: Microsoft.Maui.Controls.XmlnsPrefix("https://vapolia.eu/Vapolia.SegmentedViews", "segmented")]
[assembly: Microsoft.Maui.Controls.XmlnsPrefix("clr-namespace:Vapolia.SegmentedViews;assembly=Vapolia.SegmentedViews", "segmented")]

namespace Vapolia.SegmentedViews;

public static class MauiAppBuilderExtensions
{
    /// <summary>
    /// Add Maui handlers for this control
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static MauiAppBuilder UseSegmentedControl(this MauiAppBuilder builder)
    {
        builder.ConfigureMauiHandlers(handlers =>
        {
#if ANDROID
            handlers.TryAddHandler<SegmentedView, SegmentedViewHandler>();
#elif IOS || MACCATALYST
            handlers.TryAddHandler<SegmentedView, SegmentedViewHandler>();
// #elif WINDOWS
//            handlers.TryAddHandler<SegmentedControl, SegmentedControlHandler>();
#endif
        });

        return builder;
    }
}
