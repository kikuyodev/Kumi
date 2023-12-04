using Kumi.Game.Charts;
using Kumi.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osuTK;

namespace Kumi.Game.Screens.Select;

public partial class ChartSetInfoWedgeContent : Container
{
    private readonly IWorkingChart chart;

    private TextFlowContainer charterFlow = null!;

    public ChartSetInfoWedgeContent(IWorkingChart chart)
    {
        this.chart = chart;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Children = new Drawable[]
        {
            new ChartSetInfoWedgeBackground(chart)
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding
                {
                    Vertical = 20,
                    Horizontal = 40
                },
                Children = new Drawable[]
                {
                    new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Vertical,
                        Anchor = Anchor.BottomLeft,
                        Origin = Anchor.BottomLeft,
                        Spacing = new Vector2(0, 8),
                        Children = new Drawable[]
                        {
                            new FillFlowContainer
                            {
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y,
                                Direction = FillDirection.Vertical,
                                Spacing = new Vector2(0, 4),
                                Children = new Drawable[]
                                {
                                    new SpriteText
                                    {
                                        Text = new RomanisableString(chart.ChartInfo.Metadata.Title, chart.ChartInfo.Metadata.TitleRomanised),
                                        Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.SemiBold, 20f),
                                        UseFullGlyphHeight = false
                                    },
                                    new SpriteText
                                    {
                                        Text = new RomanisableString(chart.ChartInfo.Metadata.Artist, chart.ChartInfo.Metadata.ArtistRomanised),
                                        Font = KumiFonts.GetFont(FontFamily.Montserrat),
                                        Alpha = 0.5f,
                                        UseFullGlyphHeight = false
                                    }
                                }
                            },
                            charterFlow = new TextFlowContainer(s =>
                            {
                                s.Alpha = 0.5f;
                                s.Font = KumiFonts.GetFont(size: 12f);
                                s.UseFullGlyphHeight = false;
                            })
                            {
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y
                            }
                        }
                    },
                    new SpriteText
                    {
                        Anchor = Anchor.BottomRight,
                        Origin = Anchor.BottomRight,
                        Text = chart.ChartInfo.DifficultyName,
                        Font = KumiFonts.GetFont(FontFamily.Montserrat)
                    }
                }
            }
        };

        charterFlow.AddText("Charted by ");
        charterFlow.AddText(chart.ChartInfo.ChartSet!.Creator.Username, s =>
        {
            s.Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.SemiBold, 12f);
            s.Alpha = 1f;
            s.Colour = Color4Extensions.FromHex("80DFFF");
        });
    }
}
