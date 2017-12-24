using System;
using System.Data.Entity;
using System.Linq;
using WAES.Cris.DataAccess;

namespace WAES.Cris.Services
{
  /// <summary>
  /// Responsible for supportting a service that interacts with a data layer.
  /// </summary>
  public class DataService : IDataService
  {
    private readonly IDataEntitiesFactory dbContextFactory;

    /// <summary>
    /// Current open context instance.
    /// </summary>
    protected IDataEntities DbContext { get; private set; }

    /// <summary>
    /// Initializes a <see cref="DataService"/> instance with an injected <see cref="IDataEntitiesFactory"/> instance.
    /// </summary>
    /// <param name="dataEntitiesFactory"></param>
    public DataService(IDataEntitiesFactory dataEntitiesFactory)
    {
      this.dbContextFactory = dataEntitiesFactory;
      this.DbContext = this.dbContextFactory.Create();
    }

    /// <summary>
    /// Queries out an entity set without keeping tracking of it against the context.
    /// This method should be used in read-only scenarios where changes are not intended to be refelected in the context.
    /// </summary>
    /// <typeparam name="T">Type to be queried out.</typeparam>
    /// <returns><see cref="IQueryable{T}"/> containing non-tracked entities.</returns>
    public IQueryable<T> Query<T>() where T : class
    {
      return this.DbContext.Set<T>().AsNoTracking();
    }

    /// <summary>
    /// Disposes the current context instance.
    /// </summary>
    public void Dispose()
    {
      this.DbContext?.Dispose();
    }

    /// <summary>
    /// Creates a new DbContext and saves a reference to the old one, which will be restored when the
    /// StackFrame object is disposed. This ensures that "attached" entities are not leaked outside of the service.
    /// </summary>
    /// <returns>
    /// The <see cref="StackFrame"/> object responsible for disposing the new DbContext and restoring the old one
    /// </returns>
    protected StackFrame NewDbContextScope()
    {
      var previousDbContext = this.DbContext;
      this.DbContext = dbContextFactory.Create();
      return new StackFrame(() =>
      {
        this.DbContext.Dispose();
        this.DbContext = previousDbContext;
      });
    }

    /// <summary>
    /// Simple abstraction of a debug stack frame.
    /// </summary>
    protected class StackFrame : IDisposable
    {
      private Action onDispose;

      /// <summary>
      /// Initializes an instance of <see cref="StackFrame"/> with an action to be performed once this very instance is disposed.
      /// </summary>
      /// <param name="onDispose"></param>
      public StackFrame(Action onDispose)
      {
        this.onDispose = onDispose;
      }

      /// <summary>
      /// Invokes onDispose to perform any cleanup routine.
      /// </summary>
      public void Dispose()
      {
        this.onDispose?.Invoke();
      }
    }
  }
}