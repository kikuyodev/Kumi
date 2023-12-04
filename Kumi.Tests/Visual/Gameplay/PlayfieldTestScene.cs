using System.Diagnostics;
using Kumi.Game.Charts;
using Kumi.Game.Charts.Objects;
using Kumi.Game.Charts.Objects.Windows;
using Kumi.Game.Gameplay;
using Kumi.Game.Gameplay.Clocks;
using Kumi.Game.Gameplay.Drawables;
using Kumi.Game.Graphics;
using Kumi.Game.Tests;
using NUnit.Framework;
using osu.Framework.Development;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Testing;
using osuTK.Graphics;

namespace Kumi.Tests.Visual.Gameplay;

public abstract partial class PlayfieldTestScene : KumiTestScene
{
    protected Playfield? Playfield;
    private GameplayClockContainer? gameplayClockContainer;

    private TestWorkingChart? workingChart;

    private SpriteText timeText = null!;

    [SetUp]
    public void Setup()
    {
        Schedule(() =>
        {
            Debug.Assert(ThreadSafety.IsUpdateThread);
            Clear();

            Playfield?.Dispose();

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
        AddStep("load playfield", () => { LoadComponent(Playfield = CreatePlayfield(workingChart!)); });
        AddStep("add playfield", () =>
        {
            Add(gameplayClockContainer = new GameplayClockContainer(workingChart!.Track)
            {
                RelativeSizeAxes = Axes.Both,
                Child = Playfield
            });

            gameplayClockContainer.StartTime = -1000;
        });
        AddAssert("notes loaded", () => Playfield!.ChildrenOfType<DrawableNote>().Any());

        AddAssert("seek clock", () =>
        {
            var note = Playfield!.ChildrenOfType<DrawableNote>().FirstOrDefault();
            if (note == null) return false;

            gameplayClockContainer?.Seek(note.Note.Time);
            return true;
        });

        AddAssert("first note closer in time", () =>
        {
            var note = Playfield!.ChildrenOfType<DrawableNote>().FirstOrDefault();
            return note!.Time.Current == note.Note.Time;
        });

        CreateAsserts();

        AddStep("reset clock", () => { gameplayClockContainer?.Reset(startClock: true); });
    }

    protected override void Update()
    {
        base.Update();

        if (Playfield == null)
            return;

        timeText.Text = $"Time: {gameplayClockContainer?.Time.Current:N2}";
    }

    protected virtual void CreateAsserts()
    {
    }

    protected virtual void CreateChartData(Chart chart)
    {
        chart.Notes.AddRange(new[]
        {
            CreateNote(500),
            CreateNote(1000),
            CreateNote(2000)
        });
    }

    protected virtual INote CreateNote(float startTime)
    {
        return new TestNote
        {
            Time = startTime,
            Type = NoteType.Don,
            Flags = NoteFlags.None,
            NoteColor = Color4.White,
            Windows = new NoteWindows()
        };
    }

    protected abstract Playfield CreatePlayfield(WorkingChart workingChart);

    protected partial class TestNote : INote
    {
        public double Time { get; set; }
        public NoteType Type { get; set; }
        public NoteFlags Flags { get; set; }
        public Color4 NoteColor { get; set; }
        public NoteWindows Windows { get; set; } = null!;
    }
}
