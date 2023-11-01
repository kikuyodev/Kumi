namespace Kumi.Game.Database;

public interface IModelFileManager<in TModel, in TFileModel>
    where TModel : class
    where TFileModel : class
{
    /// <summary>
    /// Adds a new file. If the file already exists, it will be overwritten.
    /// </summary>
    /// <param name="model">The item to add the file to.</param>
    /// <param name="contents">The file contents.</param>
    /// <param name="fileName">The name of the file.</param>
    void AddFile(TModel model, Stream contents, string fileName);

    /// <summary>
    /// Deletes an existing file.
    /// </summary>
    /// <param name="model">The item to delete the file from.</param>
    /// <param name="file">The existing file to delete.</param>
    void DeleteFile(TModel model, TFileModel file);

    /// <summary>
    /// Replace an existing file with new contents.
    /// </summary>
    /// <param name="model">The item to replace the file with.</param>
    /// <param name="file">The existing file to be replaced.</param>
    /// <param name="contents">The new file contents.</param>
    void ReplaceFile(TModel model, TFileModel file, Stream contents);
}
