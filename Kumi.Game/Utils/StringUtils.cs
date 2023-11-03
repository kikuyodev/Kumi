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
}
