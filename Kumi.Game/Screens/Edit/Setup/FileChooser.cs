using Kumi.Game.Database;
using Kumi.Game.Graphics.UserInterface;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace Kumi.Game.Screens.Edit.Setup;

public partial class FileChooser : CompositeDrawable, IHasCurrentValue<FileInfo?>, ICanAcceptFiles, IHasPopover
{
    private readonly string[] handledFileExtensions;
    
    public IEnumerable<string> HandledFileExtensions => handledFileExtensions;

    [Resolved]
    private KumiGameBase? game { get; set; }
    
    private string? initialChosenPath;

    private readonly BindableWithCurrent<FileInfo?> current = new BindableWithCurrent<FileInfo?>();
    
    public Bindable<FileInfo?> Current
    {
        get => current.Current;
        set => current.Current = value;
    }
    
    public string Text
    {
        get => component.Text;
        set => component.Text = value;
    }

    public CompositeDrawable TabbableContentContainer
    {
        set => component.TabbableContentContainer = value;
    }

    private readonly TextBoxWithPopover.PopoverTextBox component;
    
    public FileChooser(params string[] handledFileExtensions)
    {
        this.handledFileExtensions = handledFileExtensions;

        RelativeSizeAxes = Axes.X;
        Height = 30;
        
        InternalChild = component = new TextBoxWithPopover.PopoverTextBox
        {
            RelativeSizeAxes = Axes.X,
            Height = 30,
            OnFocused = this.ShowPopover
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        
        game?.RegisterFileAcceptor(this);
        Current.BindValueChanged(onFileSelected);
    }

    private void onFileSelected(ValueChangedEvent<FileInfo?> file)
    {
        if (file.NewValue != null)
            this.HidePopover();
        
        initialChosenPath = file.NewValue?.DirectoryName;
    }

    #region ICanAcceptFiles

    Task ICanAcceptFiles.Import(params string[] paths)
    {
        Schedule(() => Current.Value = new FileInfo(paths.First()));
        return Task.CompletedTask;
    }

    Task ICanAcceptFiles.Import(ImportTask[] tasks)
        => throw new NotImplementedException();

    #endregion

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);
        game?.UnregisterFileAcceptor(this);
    }

    public Popover? GetPopover() => new FileChooserPopover(initialChosenPath, handledFileExtensions, Current);
    
    private partial class FileChooserPopover : KumiPopover
    {
        public FileChooserPopover(string? chosenPath, string[] handledExtensions, IBindable<FileInfo?>? currentFile)
        {
            Child = new Container
            {
                Size = new Vector2(600, 400),
                Child = new KumiFileSelector(chosenPath, handledExtensions)
                {
                    RelativeSizeAxes = Axes.Both,
                    CurrentFile = { BindTarget = currentFile }
                }
            };
        }
    }
}
