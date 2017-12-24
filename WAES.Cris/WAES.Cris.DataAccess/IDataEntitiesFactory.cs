namespace WAES.Cris.DataAccess
{
  /// <summary>
  /// Provides mechanisms to create <see cref="IDataEntities"/> instances.
  /// </summary>
  public interface IDataEntitiesFactory
  {
    /// <summary>
    /// Creates a concrete implementation of <see cref="IDataEntities"/>
    /// </summary>
    /// <returns></returns>
    IDataEntities Create();
  }
}