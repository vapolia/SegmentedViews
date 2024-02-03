using System.Globalization;

namespace Vapolia.SegmentedViews;

internal static class SegmentExtensions
{
    public static string GetText(this Segment segment, ISegmentedView segmentedControl)
    {
        if (segment.Item == null)
            return string.Empty;

        var obj = segmentedControl.TextPropertyName != null ? segment.Item.GetType().GetProperty(segmentedControl.TextPropertyName)?.GetValue(segment.Item) : segment.Item;

        if (segmentedControl.TextConverter != null)
            obj = segmentedControl.TextConverter.Convert(obj, typeof(string), null, CultureInfo.CurrentCulture);

        return obj?.ToString() ?? string.Empty;
    }
}