using Kumi.Game.Screens.Backgrounds;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace Kumi.Game.Graphics.Backgrounds;

/// <summary>
/// A representation of a background, which can be used in a <see cref="BackgroundScreen" />.
/// </summary>
public partial class Background : CompositeDrawable, IEquatable<Background>
{
    /// <summary>
    /// The sprite that represents this background.
    /// </summary>
    public readonly Sprite Sprite;

    public Background()
    {
        RelativeSizeAxes = Axes.Both;
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;

        AddInternal(Sprite = new Sprite
        {
            RelativeSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            FillMode = FillMode.Fill
        });
    }

    /// <summary>
    /// Sets the background to a texture.
    /// </summary>
    /// <param name="texture">The texture to set.</param>
    public void SetBackground(Texture texture) => Sprite.Texture = texture;

    public bool Equals(Background? other)
    {
        if (ReferenceEquals(null, other))
            return false;
        if (ReferenceEquals(this, other))
            return true;

        return Sprite.Equals(other.Sprite);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj.GetType() != GetType())
            return false;

        return Equals((Background) obj);
    }

    public override int GetHashCode() => Sprite.GetHashCode();
}
