using System;
using System.Data.Entity;
using System.Threading.Tasks;
using WAES.Cris.Model;

namespace WAES.Cris.DataAccess
{
  /// <summary>
  /// Provides mechanisms to abstract the interaction with the data layer.
  /// </summary>
  public interface IDataEntities : IDisposable
  {
    /// <summary>
    ///  Provides access to the BinData dataset.
    /// </summary>
    IDbSet<BinData> BinDataSet { get; }

    /// <summary>
    /// Returns a System.Data.Entity.DbSet`1 instance for access to entities of the given type in the context and the underlying store.
    /// </summary>
    /// <typeparam name="TEntity">The type entity for which a set should be returned.</typeparam>
    /// <returns>A set for the given entity type.</returns>
    IDbSet<TEntity> Set<TEntity>() where TEntity : class;

    /// <summary>
    /// Asynchronously saves all changes made in this context to the underlying database.
    /// </summary>
    /// <returns>>A task that represents the asynchronous operation.
    /// Task result contains the number of state entries written to the underlying database
    /// </returns>
    Task<int> SaveChangesAsync();
  }
}