using Kumi.Game.Graphics;
using Kumi.Game.Scoring;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;

namespace Kumi.Game.Screens.Play;

public partial class ResultsScreen : ScreenWithChartBackground
{
    public override float ParallaxAmount => 0.0025f;
    public override float DimAmount => 0.75f;

    private readonly ScoreInfo score;

    public ResultsScreen(ScoreInfo score)
    {
        this.score = score;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        AddInternal(new FillFlowContainer
        {
            Direction = FillDirection.Vertical,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Children = new Drawable[]
            {
                new SpriteText
                {
                    Font = KumiFonts.GetFont(),
                    Text = score.TotalScore.ToString(),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                },
                new SpriteText
                {
                    Font = KumiFonts.GetFont(),
                    Text = score.MaxCombo.ToString(),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                },
            }
        });
    }
}
