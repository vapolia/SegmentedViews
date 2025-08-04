using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;

[assembly: XmlnsDefinition("https://vapolia.eu/Vapolia.SegmentedViews", "Vapolia.SegmentedViews", AssemblyName = "Vapolia.SegmentedViews")]
[assembly: Microsoft.Maui.Controls.XmlnsPrefix("https://vapolia.eu/Vapolia.SegmentedViews", "segmented")]
[assembly: Microsoft.Maui.Controls.XmlnsPrefix("clr-namespace:Vapolia.SegmentedViews;assembly=Vapolia.SegmentedViews", "segmented")]

namespace Vapolia.SegmentedViews;

[SuppressMessage("Usage", "CA2255: ’ModuleInitializer’ warning")]
[SupportedOSPlatform("iOS15.0")]
[SupportedOSPlatform("MacCatalyst14.0")]
[SupportedOSPlatform("Android27.0")]
[SupportedOSPlatform("Windows10.0.19041")]
//[SupportedOSPlatform("Tizen6.5")]
public static class MauiAppBuilderExtensions
{
    /// <summary>
    /// Try to fix ns not found for Segment (but not SegmentedView) ?!
    /// When using https://vapolia.eu/Vapolia.SegmentedViews instead of clr-namespace:Vapolia.SegmentedViews;assembly=Vapolia.SegmentedViews
    /// </summary>
    public static Vapolia.SegmentedViews.Segment InternalSegment;
    
    /// <summary>
    /// Add Maui handlers for this control
    /// </summary>
    public static MauiAppBuilder UseSegmentedView(this MauiAppBuilder builder)
    {
        //?! Try to fix ns not found for Segment (but not SegmentedView)
        InternalSegment = new ();
        
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
