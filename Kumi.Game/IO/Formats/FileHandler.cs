namespace Kumi.Game.IO.Formats;

public abstract class FileHandler<T, TSection> : FileHandler<T>, IFileHandler<T, TSection>
    where TSection : struct
{
    public TSection CurrentSection { get; set; }
    protected abstract IFileHandler<T, TSection>.SectionHeaderValues SectionHeader { get; }

    public bool CloseStreamUponProcessed { get; set; }

    protected FileHandler(int version, bool closeStreamUponProcessed = true)
        : base(version)
    {
        CloseStreamUponProcessed = closeStreamUponProcessed;
    }
    
    #region IFileHandler implementation

    IFileHandler<T, TSection>.SectionHeaderValues IFileHandler<T, TSection>.SectionHeader => SectionHeader;

    #endregion
}

public abstract class FileHandler<T> : IFileHandler<T>
{
    /// <summary>
    /// The characters used to denote a comment.
    /// </summary>
    protected string CommentCharacter { get; } = "#";

    public int Version { get; }
    public T Current { get; set; } = default!;

    protected FileHandler(int version)
    {
        Version = version;
    }

    protected abstract void Process(T input);
    
    /// <summary>
    /// Runs further processing on the output, once parsing is complete.
    /// </summary>
    protected virtual void PostProcess()
    {
        return;
    }

    public virtual void Dispose()
    {
    }

    #region IFileHandler implementation
    
    void IFileHandler<T>.PostProcess() => PostProcess();

    #endregion
}
