using Kumi.Game.Overlays.Settings.Components;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Graphics;
using osu.Framework.Localisation;

namespace Kumi.Game.Overlays.Settings.Audio;

public partial class VolumeSection : SettingSection
{
    protected override LocalisableString Header => "Volume";

    [BackgroundDependencyLoader]
    private void load(AudioManager audio)
    {
        Children = new Drawable[]
        {
            new SliderBarSettingItem<double>
            {
                Label = "Master Volume",
                Current = audio.Volume
            },
            new SliderBarSettingItem<double>
            {
                Label = "Effect Volume",
                Current = audio.VolumeSample
            },
            new SliderBarSettingItem<double>
            {
                Label = "Music Volume",
                Current = audio.VolumeTrack
            }
        };
    }
}
