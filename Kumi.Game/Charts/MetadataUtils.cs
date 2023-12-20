using System.Text;

namespace Kumi.Game.Charts;

public static class MetadataUtils
{
    public static bool IsRomanised(char c)
        => c <= 0xFF;

    public static bool IsRomanised(string? str)
        => string.IsNullOrEmpty(str) || str.All(IsRomanised);

    public static string StripNonRomanisedCharacters(string? str)
    {
        if (string.IsNullOrEmpty(str))
            return string.Empty;

        var sb = new StringBuilder(str.Length);

        foreach (var c in str)
        {
            if (IsRomanised(c))
                sb.Append(c);
        }
        
        return sb.ToString().Trim();
    }
}
