using osu.Framework.Graphics.Sprites;

namespace Kumi.Game.Graphics;

public static class KumiFonts
{
    // This value is used to scale the font size to match figma's font size.
    // This is due to how osu!framework renders fonts.
    private const float font_scalar = 1.2f;

    public static FontUsage GetFont(FontFamily family = FontFamily.Inter, FontWeight weight = FontWeight.Regular, float size = 16f, bool italics = false)
        => new FontUsage(getFontName(family, weight, italics), size * font_scalar);

    private static string getFontFamily(FontFamily family)
    {
        return family switch
        {
            FontFamily.Montserrat => "Montserrat",
            FontFamily.Inter => "Inter",
            _ => throw new ArgumentOutOfRangeException(nameof(family), family, null)
        };
    }

    private static string getFontWeight(FontWeight weight)
    {
        return weight switch
        {
            FontWeight.Thin => "Thin",
            FontWeight.ExtraLight => "ExtraLight",
            FontWeight.Light => "Light",
            FontWeight.Regular => "",
            FontWeight.Medium => "Medium",
            FontWeight.SemiBold => "SemiBold",
            FontWeight.Bold => "Bold",
            FontWeight.ExtraBold => "ExtraBold",
            FontWeight.Black => "Black",
            _ => throw new ArgumentOutOfRangeException(nameof(weight), weight, null)
        };
    }

    private static string getFontName(FontFamily family, FontWeight weight, bool italics)
        => weight == FontWeight.Regular
               ? $"{getFontFamily(family)}{(italics ? "-Italic" : "")}"
               : $"{getFontFamily(family)}-{getFontWeight(weight)}{(italics ? "-Italic" : "")}";
}

public enum FontFamily
{
    Montserrat,
    Inter
}

public enum FontWeight
{
    Thin,
    ExtraLight,
    Light,
    Regular,
    Medium,
    SemiBold,
    Bold,
    ExtraBold,
    Black
}
