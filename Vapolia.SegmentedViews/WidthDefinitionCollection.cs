namespace Vapolia.SegmentedViews;

public class WidthDefinitionCollection : List<GridLength>
{
    public WidthDefinitionCollection() {}
  
    public WidthDefinitionCollection(IEnumerable<GridLength> definitions) : base(definitions)
    {
    }
}