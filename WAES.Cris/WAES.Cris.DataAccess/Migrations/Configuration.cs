namespace WAES.Cris.DataAccess.Migrations
{
  using System.Data.Entity.Migrations;

  internal sealed class Configuration : DbMigrationsConfiguration<WaesDbContext>
  {
    public Configuration()
    {
      AutomaticMigrationsEnabled = false;
    }

    protected override void Seed(WaesDbContext context)
    {
    }
  }
}