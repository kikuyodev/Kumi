namespace Kumi.Game.Database;

public interface IModelExporter<TModel>
    where TModel : class, IHasGuidPrimaryKey
{
    /// <summary>
    /// Export a model to a designated export path.
    /// </summary>
    /// <param name="model">The model to export.</param>
    Task Export(TModel model);
}
