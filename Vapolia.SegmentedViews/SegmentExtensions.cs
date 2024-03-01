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
    
    public static List<GridLength> GetWidths(this ISegmentedView segmentedView)
    {
        return segmentedView.Children.Select((segment,i) =>
        {
            if (segment.Width != null)
                return segment.Width.Value;
            if(segmentedView.WidthDefinitions?.Count > i)
                return segmentedView.WidthDefinitions[i];
            return segmentedView.ItemsDefaultWidth;
        }).ToList();
    }
}