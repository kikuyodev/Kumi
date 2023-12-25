namespace Kumi.Game.Database;

public interface IModelExporter<TModel>
    where TModel : class, IHasGuidPrimaryKey
{
    /// <summary>
    /// Export a model to a designated export path.
    /// </summary>
    /// <param name="model">The model to export.</param>
    Task<string> Export(TModel model);

    /// <summary>
    /// Exports a model to a stream.
    /// </summary>
    /// <param name="model">The model to export.</param>
    /// <param name="output">The output stream to write to.</param>
    Task ExportModelToStream(TModel model, Stream output);
}
