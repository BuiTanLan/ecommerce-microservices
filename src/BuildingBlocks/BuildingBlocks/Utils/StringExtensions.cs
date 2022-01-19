using System.ComponentModel;

namespace BuildingBlocks.Utils;

public static class StringExtensions
{
    public static T ConvertTo<T>(this object input)
    {
        return ConvertTo<T>(input.ToString());
    }

    public static T ConvertTo<T>(this string input)
    {
        try
        {
            var converter = TypeDescriptor.GetConverter(typeof(T));
            return (T)converter.ConvertFromString(input);
        }
        catch (NotSupportedException)
        {
            return default;
        }
    }
}
