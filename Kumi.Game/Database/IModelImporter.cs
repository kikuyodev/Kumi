namespace Kumi.Game.Database;

public interface IModelImporter<TModel> : ICanAcceptFiles
    where TModel : class, IHasGuidPrimaryKey
{
    /// <summary>
    /// Import multiple files from the filesystem.
    /// </summary>
    /// <param name="tasks">The import tasks</param>
    Task<IEnumerable<TModel>> ImportModels(ImportTask[] tasks);
}
