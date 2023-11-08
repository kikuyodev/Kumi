namespace Kumi.Game.IO.Formats;

public abstract class FileHandler<T, TSection> : FileHandler<T>, IFileHandler<T, TSection>
    where T : class
    where TSection : struct
{
    public TSection CurrentSection { get; set; }
    protected virtual IFileHandler<T, TSection>.SectionHeaderValues SectionHeader => IFileHandler<T, TSection>.DefaultHeaders;

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
    where T : class
{
    /// <summary>
    /// The characters used to denote a comment.
    /// </summary>
    protected virtual string CommentCharacter { get; } = "#";

    public int Version { get; }
    public T Current { get; set; } = default!;

    protected FileHandler(int version)
    {
        Version = version;
    }

    protected abstract void PreProcess(T input);

    /// <summary>
    /// Runs further processing on the output, once parsing is complete.
    /// </summary>
    protected virtual void PostProcess()
    {
    }

    public virtual void Dispose()
    {
    }

    #region IFileHandler implementation

    void IFileHandler<T>.PostProcess() => PostProcess();

    #endregion

}
