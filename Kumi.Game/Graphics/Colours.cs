using osu.Framework.Extensions.Color4Extensions;
using osuTK.Graphics;

namespace Kumi.Game.Graphics;

/// <summary>
/// Defines the colors used in the game.
/// In the future, we might want to make this configurable from the settings,
/// or potentially make UI elements skinnable by the user. Would be cool.
/// </summary>
public class Colours
{
    #region Static Colors

    public static readonly Color4 KAT_COLOR = new Color4(104, 192, 193, 255);
    public static readonly Color4 DON_COLOR = new Color4(249, 72, 38, 255);

    #region Seafoam

    public static readonly Color4 SEAFOAM_ACCENT = Color4Extensions.FromHex("00FFAA");
    public static readonly Color4 SEAFOAM_ACCENT_LIGHT = Color4Extensions.FromHex("33FFBB");
    public static readonly Color4 SEAFOAM_ACCENT_LIGHTER = Color4Extensions.FromHex("66FFCC");

    public static readonly Color4 SEAFOAM = Color4Extensions.FromHex("39AC86");
    public static readonly Color4 SEAFOAM_LIGHT = Color4Extensions.FromHex("53C69F");
    public static readonly Color4 SEAFOAM_LIGHTER = Color4Extensions.FromHex("8CD9BF");

    #endregion
    #region Cyan

    public static readonly Color4 CYAN_ACCENT = Color4Extensions.FromHex("00AAFF");
    public static readonly Color4 CYAN_ACCENT_LIGHT = Color4Extensions.FromHex("33BBFF");
    public static readonly Color4 CYAN_ACCENT_LIGHTER = Color4Extensions.FromHex("66CCFF");

    public static readonly Color4 CYAN = Color4Extensions.FromHex("3986AC");
    public static readonly Color4 CYAN_LIGHT = Color4Extensions.FromHex("539FC6");
    public static readonly Color4 CYAN_LIGHTER = Color4Extensions.FromHex("8CBFD9");

    #endregion
    #region Blue

    public static readonly Color4 BLUE_ACCENT = Color4Extensions.FromHex("0055FF");
    public static readonly Color4 BLUE_ACCENT_LIGHT = Color4Extensions.FromHex("3377FF");
    public static readonly Color4 BLUE_ACCENT_LIGHTER = Color4Extensions.FromHex("6699FF");

    public static readonly Color4 BLUE = Color4Extensions.FromHex("3960AC");
    public static readonly Color4 BLUE_LIGHT = Color4Extensions.FromHex("5379C6");
    public static readonly Color4 BLUE_LIGHTER = Color4Extensions.FromHex("8CA6D9");

    #endregion
    #region Red

    public static readonly Color4 RED_ACCENT = Color4Extensions.FromHex("FF0040");
    public static readonly Color4 RED_ACCENT_LIGHT = Color4Extensions.FromHex("FF3366");
    public static readonly Color4 RED_ACCENT_LIGHTER = Color4Extensions.FromHex("FF668C");

    public static readonly Color4 RED = Color4Extensions.FromHex("AC3956");
    public static readonly Color4 RED_LIGHT = Color4Extensions.FromHex("C65370");
    public static readonly Color4 RED_LIGHTER = Color4Extensions.FromHex("D98C9F");

    #endregion
    #region Orange
    
    public static readonly Color4 ORANGE_ACCENT = Color4Extensions.FromHex("FF2B00");
    public static readonly Color4 ORANGE_ACCENT_LIGHT = Color4Extensions.FromHex("FF5533");
    public static readonly Color4 ORANGE_ACCENT_LIGHTER = Color4Extensions.FromHex("FF8066");

    public static readonly Color4 ORANGE = Color4Extensions.FromHex("AC4D39");
    public static readonly Color4 ORANGE_LIGHT = Color4Extensions.FromHex("C66653");
    public static readonly Color4 ORANGE_LIGHTER = Color4Extensions.FromHex("D9998C");

    #endregion
    #region Yellow

    public static readonly Color4 YELLOW_ACCENT = Color4Extensions.FromHex("FFC800");
    public static readonly Color4 YELLOW_ACCENT_LIGHT = Color4Extensions.FromHex("FFD333");
    public static readonly Color4 YELLOW_ACCENT_LIGHTER = Color4Extensions.FromHex("FFDE66");

    public static readonly Color4 YELLOW = Color4Extensions.FromHex("AC9339");
    public static readonly Color4 YELLOW_LIGHT = Color4Extensions.FromHex("C6AD53");
    public static readonly Color4 YELLOW_LIGHTER = Color4Extensions.FromHex("D9C88C");

    #endregion
    #region Purple

    public static readonly Color4 PURPLE_ACCENT = Color4Extensions.FromHex("4000FF");
    public static readonly Color4 PURPLE_ACCENT_LIGHT = Color4Extensions.FromHex("6633FF");
    public static readonly Color4 PURPLE_ACCENT_LIGHTER = Color4Extensions.FromHex("8C66FF");

    public static readonly Color4 PURPLE = Color4Extensions.FromHex("5639AC");
    public static readonly Color4 PURPLE_LIGHT = Color4Extensions.FromHex("7053C6");
    public static readonly Color4 PURPLE_LIGHTER = Color4Extensions.FromHex("9F8CD9");

    #endregion
    #region Grays

    public static readonly Color4 GRAY_F = Color4Extensions.FromHex("FFF");
    public static readonly Color4 GRAY_E = Color4Extensions.FromHex("EEE");
    public static readonly Color4 GRAY_D = Color4Extensions.FromHex("DDD");
    public static readonly Color4 GRAY_C = Color4Extensions.FromHex("CCC");
    public static readonly Color4 GRAY_B = Color4Extensions.FromHex("BBB");
    public static readonly Color4 GRAY_A = Color4Extensions.FromHex("AAA");
    public static readonly Color4 GRAY_9 = Color4Extensions.FromHex("999");
    public static readonly Color4 GRAY_8 = Color4Extensions.FromHex("888");
    public static readonly Color4 GRAY_7 = Color4Extensions.FromHex("777");
    public static readonly Color4 GRAY_6 = Color4Extensions.FromHex("666");
    public static readonly Color4 GRAY_5 = Color4Extensions.FromHex("555");
    public static readonly Color4 GRAY_4 = Color4Extensions.FromHex("444");
    public static readonly Color4 GRAY_3 = Color4Extensions.FromHex("333");
    public static readonly Color4 GRAY_2 = Color4Extensions.FromHex("222");
    public static readonly Color4 GRAY_1 = Color4Extensions.FromHex("111");
    public static readonly Color4 GRAY_0 = Color4Extensions.FromHex("000");

    public static Color4 Gray(byte amt) => new Color4(amt, amt, amt, 255);
    public static Color4 Gray(float amt) => new Color4(amt, amt, amt, 1);

    #endregion

    #endregion
}
