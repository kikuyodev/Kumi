using Kumi.Game.Charts;
using Kumi.Game.Graphics.UserInterface;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.UserInterface;

namespace Kumi.Game.Screens.Edit.Setup.Sections;

public partial class MetadataSection : SetupSection
{
    private KumiTextBox titleTextBox = null!;
    private KumiTextBox titleRomanisedTextBox = null!;
    private Drawable titleRomanisedLabel = null!;
    
    private KumiTextBox artistTextBox = null!;
    private KumiTextBox artistRomanisedTextBox = null!;
    private Drawable artistRomanisedLabel = null!;

    private KumiTextBox sourceTextBox = null!;
    private KumiTextBox genreTextBox = null!;
    private KumiTextBox tagsTextBox = null!;
    
    [BackgroundDependencyLoader]
    private void load()
    {
        var metadata = Chart.Metadata;

        AddRange(new[]
        {
            CreateLabelledComponent(titleTextBox = CreateTextBox<KumiTextBox>(
                                        !string.IsNullOrEmpty(metadata.Title) ? metadata.Title : metadata.TitleRomanised), "Title"),
            titleRomanisedLabel = CreateLabelledComponent(titleRomanisedTextBox = CreateTextBox<RomanisedTextBox>(
                                                              !string.IsNullOrEmpty(metadata.TitleRomanised)
                                                                  ? metadata.TitleRomanised
                                                                  : MetadataUtils.StripNonRomanisedCharacters(metadata.Title)), "Title Romanised"),

            CreateLabelledComponent(artistTextBox = CreateTextBox<KumiTextBox>(
                                        !string.IsNullOrEmpty(metadata.Artist) ? metadata.Artist : metadata.ArtistRomanised), "Artist"),
            artistRomanisedLabel = CreateLabelledComponent(artistRomanisedTextBox = CreateTextBox<RomanisedTextBox>(
                                                              !string.IsNullOrEmpty(metadata.ArtistRomanised)
                                                                  ? metadata.ArtistRomanised
                                                                  : MetadataUtils.StripNonRomanisedCharacters(metadata.Artist)), "Artist Romanised"),
            
            CreateLabelledComponent(sourceTextBox = CreateTextBox<KumiTextBox>(metadata.Source), "Source"),
            CreateLabelledComponent(genreTextBox = CreateTextBox<RomanisedTextBox>(metadata.Genre), "Genre"),
            CreateLabelledComponent(tagsTextBox = CreateTextBox<KumiTextBox>(metadata.Tags), "Tags"),
        });
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        foreach (var textBox in new[] { titleTextBox, titleRomanisedTextBox, artistTextBox, artistRomanisedTextBox, sourceTextBox, genreTextBox, tagsTextBox })
            textBox.OnCommit += onCommit;
        
        titleTextBox.Current.BindValueChanged(e => transferIfRomanised(e.NewValue, titleRomanisedTextBox));
        artistTextBox.Current.BindValueChanged(e => transferIfRomanised(e.NewValue, artistRomanisedTextBox));
        
        updateVisibleState();
        FinishTransforms();
    }

    private void transferIfRomanised(string value, KumiTextBox textBox)
    {
        if (MetadataUtils.IsRomanised(value))
            textBox.Current.Value = value;
        
        updateVisibleState();
        Scheduler.AddOnce(updateMetadata);
    }

    private void updateVisibleState()
    {
        titleRomanisedLabel.FadeTo(titleRomanisedTextBox.Text == titleTextBox.Text ? 0 : 1, 200, Easing.OutQuint);
        artistRomanisedLabel.FadeTo(artistRomanisedTextBox.Text == artistTextBox.Text ? 0 : 1, 200, Easing.OutQuint);
    }

    private void onCommit(TextBox sender, bool newText)
    {
        if (!newText)
            return;

        Scheduler.AddOnce(updateMetadata);
    }

    private void updateMetadata()
    {
        Chart.Metadata.Title = titleTextBox.Text;
        Chart.Metadata.TitleRomanised = titleRomanisedTextBox.Text;
        
        Chart.Metadata.Artist = artistTextBox.Text;
        Chart.Metadata.ArtistRomanised = artistRomanisedTextBox.Text;
        
        Chart.Metadata.Source = sourceTextBox.Text;
        Chart.Metadata.Genre = genreTextBox.Text;
        Chart.Metadata.Tags = tagsTextBox.Text;
        
        Chart.SaveState();
    }
}
