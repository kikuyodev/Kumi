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
using osu.Framework.Bindables;
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

            gameplayClockContainer?.Seek(note.Note.StartTime);
            return true;
        });

        AddAssert("first note closer in time", () =>
        {
            var note = Playfield!.ChildrenOfType<DrawableNote>().FirstOrDefault();
            return note!.Time.Current == note.Note.StartTime;
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
            StartTime = startTime,
            Type = new NoteProperty<NoteType>(),
            Flags = new NoteProperty<NoteFlags>(),
            NoteColor = new NoteProperty<Color4>(Color4.White),
            Windows = new NoteWindows()
        };
    }

    protected abstract Playfield CreatePlayfield(WorkingChart workingChart);

    protected partial class TestNote : INote
    {
        public double StartTime
        {
            get => StartTimeBindable.Value;
            set => StartTimeBindable.Value = value;
        }
        
        public NoteProperty<NoteType> Type { get; init; } = null!;
        public NoteProperty<NoteFlags> Flags { get; init; } = null!;
        public NoteProperty<Color4> NoteColor { get; init; } = null!;
        public NoteWindows Windows { get; set; } = null!;

        public Bindable<double> StartTimeBindable { get; } = new Bindable<double>();
    }
}
