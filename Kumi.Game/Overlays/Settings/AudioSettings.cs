using Kumi.Game.Overlays.Settings.Audio;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace Kumi.Game.Overlays.Settings;

public partial class AudioSettings : SettingScreen
{
    [BackgroundDependencyLoader]
    private void load()
    {
        Children = new Drawable[]
        {
            new VolumeSection()
        };
    }
}
