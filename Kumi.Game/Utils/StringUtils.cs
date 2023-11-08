using System.Globalization;

namespace Kumi.Game.Utils;

public static class StringUtils
{
    public static T AssertAndFetch<T>(string value)
        where T : unmanaged
    {
        if (!AssertProperty<T>(value))
            throw new InvalidDataException($"Invalid property value, expected {typeof(T).Name} but got {value}");
        
        return (T) Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
    }

    public static bool AssertProperty<T>(string value)
        where T : unmanaged
    {
        try
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
            return true;
        } catch
        {
            return false;
        }
    }
    
    /// <summary>
    /// A more complex version of the standard <see cref="string.Split(char,int,System.StringSplitOptions)"/> method that splits the input string by the <paramref name="delimiter"/>.
    /// This function can handle strings that contain the delimiter as part of the value.
    /// </summary>
    public static IEnumerable<string> SplitComplex(this string input, char delimiter)
    {
        string? currentValue = null;
        
        for (var i = 0; i < input.Length; i++)
        {
            var character = input[i];

            if (character == delimiter)
            {
                if (currentValue != null)
                {
                    yield return currentValue;
                    currentValue = null;
                }

                continue;
            }

            if (character == '"')
            {
                var nextQuoteIndex = input.IndexOf('"', i + 1);
                
                if (nextQuoteIndex == -1)
                    throw new ArgumentException($"Could not find the closing quote for the value starting at index {i}.");
                
                currentValue = input[(i + 1)..nextQuoteIndex];
                i = nextQuoteIndex;
            }
            else
            {
                currentValue ??= string.Empty;
                currentValue += character;
            }
        }
        
        // add the last value if it exists
        if (currentValue != null)
            yield return currentValue;
    }
}
