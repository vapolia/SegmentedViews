using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

[assembly: XmlnsDefinition("https://vapolia.eu/Vapolia.SegmentedViews", "Vapolia.SegmentedViews")]
[assembly: Microsoft.Maui.Controls.XmlnsPrefix("https://vapolia.eu/Vapolia.SegmentedViews", "segmented")]
[assembly: Microsoft.Maui.Controls.XmlnsPrefix("clr-namespace:Vapolia.SegmentedViews;assembly=Vapolia.SegmentedViews", "segmented")]

namespace Vapolia.SegmentedViews;

[SuppressMessage("Usage", "CA2255: ’ModuleInitializer’ warning")]
public static class MauiAppBuilderExtensions
{
    //Force load the assembly for the XAML compiler, to fix:
    // Error XC0000 : Cannot resolve type "https://vapolia.eu/Vapolia.SegmentedViews:segmented:Segment".
    [ModuleInitializer]
    internal static void LoadAssembly()
    {
        _ = typeof(Segment);
        _ = typeof(SegmentedView);
    }

    
    /// <summary>
    /// Add Maui handlers for this control
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static MauiAppBuilder UseSegmentedView(this MauiAppBuilder builder)
    {
        builder.ConfigureMauiHandlers(handlers =>
        {
#if ANDROID
            handlers.TryAddHandler<SegmentedView, SegmentedViewHandler>();
#elif IOS || MACCATALYST
            handlers.TryAddHandler<SegmentedView, SegmentedViewHandler>();
#elif WINDOWS
            handlers.TryAddHandler<SegmentedView, SegmentedViewHandler>();
#endif
        });

        return builder;
    }
}
