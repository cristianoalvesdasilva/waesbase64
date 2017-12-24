using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using WAES.Cris.Model;

namespace WAES.Cris.DataAccess
{
  /// <summary>
  /// Responsible for abstractring the interaction with the data layer.
  /// </summary>
  public class WaesDbContext : DbContext, IDataEntities
  {
    /// <summary>
    /// Initializes an instance of <see cref="WaesDbContext"/> using the connection string 'WaesDbContext'.
    /// </summary>
    public WaesDbContext() : base("name=WaesDbContext")
    {
      this.Configuration.LazyLoadingEnabled = false;
    }

    /// <summary>
    /// Provides access to the BinData dataset.
    /// </summary>
    public virtual IDbSet<BinData> BinDataSet { get; set; }

    /// <summary>
    /// Returns a System.Data.Entity.DbSet`1 instance for access to entities of the given type in the context and the underlying store.
    /// </summary>
    /// <typeparam name="TEntity">The type entity for which a set should be returned.</typeparam>
    /// <returns>A set for the given entity type.</returns>
    IDbSet<TEntity> IDataEntities.Set<TEntity>()
    {
      return this.Set<TEntity>();
    }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
      // Removing default table naming convention (plural).
      modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

      // Pulling any mapping present in the DataAccess project.
      modelBuilder.Configurations.AddFromAssembly(typeof(WaesDbContext).Assembly);
    }
  }
}