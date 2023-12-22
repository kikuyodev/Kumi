using Kumi.Game.Charts;
using Kumi.Game.Graphics.UserInterface;

namespace Kumi.Game.Screens.Edit.Setup;

public partial class RomanisedTextBox : KumiTextBox
{
    protected override bool CanAddCharacter(char character)
        => MetadataUtils.IsRomanised(character);
}
