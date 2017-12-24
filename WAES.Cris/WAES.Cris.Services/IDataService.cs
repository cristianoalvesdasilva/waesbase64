using System;
using System.Linq;

namespace WAES.Cris.Services
{
  /// <summary>
  /// Provides a set of methods to support a service that interacts with a data layer.
  /// </summary>
  internal interface IDataService : IDisposable
  {
    /// <summary>
    /// Queries out an entity set without keeping tracking of it against the context.
    /// This method should be used in read-only scenarios where changes are not intended to be refelected in the context.
    /// </summary>
    /// <typeparam name="T">Type to be queried out.</typeparam>
    /// <returns><see cref="IQueryable{T}"/> containing non-tracked entities.</returns>
    IQueryable<T> Query<T>() where T : class;
  }
}