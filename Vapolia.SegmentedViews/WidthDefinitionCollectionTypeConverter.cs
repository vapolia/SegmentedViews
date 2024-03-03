using System.ComponentModel;
using System.Globalization;

namespace Vapolia.SegmentedViews;

public class WidthDefinitionCollectionTypeConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        => sourceType == typeof(string);
    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
        => destinationType == typeof(string);

    public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object? value)
    {
        var strValue = value?.ToString();

        if (strValue == null) 
            throw new InvalidOperationException($"Cannot convert \"{strValue}\" into {typeof(WidthDefinitionCollection)}");
    
        var converter = new GridLengthTypeConverter();
        var definitions = strValue.Split(',').Select(length => (GridLength?)converter.ConvertFromInvariantString(length)).ToList();
        if(definitions.Any(d => d == null))
            throw new InvalidOperationException($"Cannot convert \"{strValue}\" into {typeof(WidthDefinitionCollection)}");

        return new WidthDefinitionCollection(definitions.Cast<GridLength>());
    }


    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        if (value is not WidthDefinitionCollection cdc)
            throw new NotSupportedException();
        var converter = new GridLengthTypeConverter();
        return string.Join(", ", cdc.Select(cd => converter.ConvertToInvariantString(cd)));
    }
}