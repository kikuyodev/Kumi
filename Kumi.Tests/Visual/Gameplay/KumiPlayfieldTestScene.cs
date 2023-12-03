using System.Diagnostics;
using Kumi.Game.Charts;
using Kumi.Game.Charts.Objects;
using Kumi.Game.Charts.Objects.Windows;
using Kumi.Game.Gameplay;
using Kumi.Game.Gameplay.Clocks;
using Kumi.Game.Gameplay.Drawables;
using Kumi.Game.Graphics;
using Kumi.Game.Input;
using Kumi.Game.Tests;
using NUnit.Framework;
using osu.Framework.Development;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Testing;
using osuTK.Graphics;

namespace Kumi.Tests.Visual.Gameplay;

public partial class KumiPlayfieldTestScene : KumiTestScene
{
    private Playfield? playfield;
    private GameplayClockContainer gameplayClockContainer = null!;
    private TestWorkingChart? workingChart;

    private SpriteText timeText = null!;

    [SetUp]
    public void Setup()
    {
        Schedule(() =>
        {
            Debug.Assert(ThreadSafety.IsUpdateThread);
            Clear();

            playfield?.Dispose();

            Add(timeText = new SpriteText
            {
                Margin = new MarginPadding(12),
                Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.Medium, size: 14)
            });

            workingChart = TestResources.CreateWorkingChart(AudioManager, LargeTextureStore, CreateChartData);
        });
    }

    [Test]
    public void PlayfieldTests()
    {
        AddStep("load playfield", () => LoadComponent(playfield = new KumiPlayfield(workingChart!)));
        AddStep("add playfield", () =>
        {
            Add(new GameplayKeybindContainer
            {
                RelativeSizeAxes = Axes.Both,
                Child = gameplayClockContainer = new GameplayClockContainer(workingChart!.Track)
                {
                    RelativeSizeAxes = Axes.Both,
                    Child = playfield
                }
            });

            gameplayClockContainer.StartTime = -1000;
        });

        AddAssert("notes loaded", () => playfield!.ChildrenOfType<DrawableNote>().Any());

        AddAssert("seek clock", () =>
        {
            var note = playfield!.ChildrenOfType<DrawableNote>().FirstOrDefault();
            if (note == null) return false;

            gameplayClockContainer.Seek(note.Note.Time);
            return true;
        });

        AddAssert("first note closer in time", () =>
        {
            var note = playfield!.ChildrenOfType<DrawableNote>().FirstOrDefault();
            return note!.Time.Current == note.Note.Time;
        });

        AddStep("reset clock", () => { gameplayClockContainer.Reset(startClock: true); });
    }

    protected override void Update()
    {
        base.Update();

        if (playfield == null)
            return;

        timeText.Text = $"Time: {gameplayClockContainer.Time.Current:N2}";
    }

    protected virtual void CreateChartData(Chart chart)
    {
        chart.Notes.AddRange(new[]
        {
            CreateNote(500),
            CreateNote(1000),
            CreateNote(2000, NoteType.Kat),
            CreateNote(3500, NoteType.Kat),
            CreateNote(3750, NoteType.Don, true),
            CreateNote(4000, NoteType.Kat, true),
        });
    }

    protected virtual INote CreateNote(float startTime, NoteType type = NoteType.Don, bool big = false)
    {
        return new DrumHit(startTime)
        {
            Type = type,
            Flags = big ? NoteFlags.Big : NoteFlags.None,
            NoteColor = Color4.White,
            Windows = new NoteWindows()
        };
    }
}
