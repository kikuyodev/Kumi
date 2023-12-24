using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace Kumi.Game.Gameplay.Mods;

public class ModDoubleTime : Mod
{
    public override LocalisableString Name => "Double Time";
    public override string Acronym => "DT";
    public override IconUsage Icon => FontAwesome.Solid.AngleDoubleRight;
    public override Type[] IncompatibleMods => new[] { typeof(ModHalfTime) };

    public override void ApplyToTrack(Track track)
    {
        track.AddAdjustment(AdjustableProperty.Tempo, new BindableDouble(1.5f));
    }
}
