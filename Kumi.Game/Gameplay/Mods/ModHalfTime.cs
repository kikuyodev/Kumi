using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace Kumi.Game.Gameplay.Mods;

public class ModHalfTime : Mod
{
    public override LocalisableString Name => "Half time";
    public override string Acronym => "HT";
    public override IconUsage Icon => FontAwesome.Solid.AngleDoubleLeft;
    public override Type[] IncompatibleMods => new[] { typeof(ModDoubleTime) };

    public override void ApplyToTrack(Track track)
    {
        track.AddAdjustment(AdjustableProperty.Tempo, new BindableDouble(0.75f));
    }
}
