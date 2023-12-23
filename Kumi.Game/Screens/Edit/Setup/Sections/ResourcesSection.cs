using Kumi.Game.Charts;
using Kumi.Game.Extensions;
using Kumi.Game.Overlays;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;

namespace Kumi.Game.Screens.Edit.Setup.Sections;

public partial class ResourcesSection : SetupSection
{
    private FileChooser backgroundFileSelector = null!;
    private FileChooser audioFileSelector = null!;

    [Resolved]
    private Editor editor { get; set; } = null!;

    [Resolved]
    private MusicController music { get; set; } = null!;

    [Resolved]
    private ChartManager chartManager { get; set; } = null!;

    [Resolved]
    private IBindable<WorkingChart> workingChart { get; set; } = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new[]
        {
            CreateLabelledComponent(backgroundFileSelector = new FileChooser(".jpg", ".jpeg", ".png")
            {
                TabbableContentContainer = this
            }, "Background", f =>
            {
                f.AddText("Backgrounds should follow common usage guidelines, and should be appropriate for all ages. More information can be found in our ");
                f.AddText("Content usage guidelines ", s => s.Colour = Color4Extensions.FromHex("69C"));
                f.AddText("and our ");
                f.AddText("Visual content guidelines ", s => s.Colour = Color4Extensions.FromHex("69C"));
                f.AddText(".");
            }),
            CreateLabelledComponent(audioFileSelector = new FileChooser(".mp3", ".ogg")
            {
                TabbableContentContainer = this
            }, "Audio", f =>
            {
                f.AddText("Audio should be appropriate for all ages. For more information, please read our ");
                f.AddText("Content usage guidelines", s => s.Colour = Color4Extensions.FromHex("69C"));
                f.AddText(".");
            })
        });

        if (!string.IsNullOrEmpty(workingChart.Value.Metadata.BackgroundFile))
            backgroundFileSelector.Current.Value = new FileInfo(workingChart.Value.Metadata.BackgroundFile);

        if (!string.IsNullOrEmpty(workingChart.Value.Metadata.AudioFile))
            audioFileSelector.Current.Value = new FileInfo(workingChart.Value.Metadata.AudioFile);

        backgroundFileSelector.Current.BindValueChanged(onBackgroundChanged);
        audioFileSelector.Current.BindValueChanged(onAudioChanged);

        updateText();
    }

    private bool changeBackgroundImage(FileInfo newFile)
    {
        if (!newFile.Exists)
            return false;

        var set = workingChart.Value.ChartSetInfo;
        var destination = new FileInfo(newFile.Name);
        var oldFile = set.GetFile(workingChart.Value.Metadata.BackgroundFile);

        using (var stream = newFile.OpenRead())
        {
            if (oldFile != null)
                chartManager.DeleteFile(set, oldFile);

            chartManager.AddFile(set, stream, destination.Name);
        }

        workingChart.Value.Metadata.BackgroundFile = destination.Name;

        Chart.SaveState();
        editor.RefreshBackground();

        return true;
    }

    private bool changeAudioTrack(FileInfo newFile)
    {
        if (!newFile.Exists)
            return false;

        var set = workingChart.Value.ChartSetInfo;
        var destination = new FileInfo(newFile.Name);
        var oldFile = set.GetFile(workingChart.Value.Metadata.AudioFile);

        using (var stream = newFile.OpenRead())
        {
            if (oldFile != null)
                chartManager.DeleteFile(set, oldFile);

            chartManager.AddFile(set, stream, destination.Name);
        }

        workingChart.Value.Metadata.AudioFile = destination.Name;

        Chart.SaveState();
        music.ReloadCurrentTrack();

        return true;
    }

    private void onBackgroundChanged(ValueChangedEvent<FileInfo?> file)
    {
        if (file.NewValue == null || !changeBackgroundImage(file.NewValue))
            backgroundFileSelector.Current.Value = file.OldValue;

        updateText();
    }

    private void onAudioChanged(ValueChangedEvent<FileInfo?> file)
    {
        if (file.NewValue == null || !changeAudioTrack(file.NewValue))
            audioFileSelector.Current.Value = file.OldValue;

        updateText();
    }

    private void updateText()
    {
        backgroundFileSelector.Text = backgroundFileSelector.Current.Value?.Name ?? "No file chosen";
        audioFileSelector.Text = audioFileSelector.Current.Value?.Name ?? "No file chosen";
    }
}
