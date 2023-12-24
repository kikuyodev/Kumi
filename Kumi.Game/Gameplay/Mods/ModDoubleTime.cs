using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;

namespace Kumi.Game.Gameplay.Mods;

public class ModDoubleTime : Mod
{
    public override string Name => "Double Time";
    public override string Acronym => "DT";

    public override void ApplyToTrack(Track track)
    {
        track.AddAdjustment(AdjustableProperty.Tempo, new BindableDouble(1.5f));
    }
}
