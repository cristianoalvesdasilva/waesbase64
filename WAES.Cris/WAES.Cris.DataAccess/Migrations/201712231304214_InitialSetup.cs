namespace WAES.Cris.DataAccess.Migrations
{
  using System.Data.Entity.Migrations;

  public partial class InitialSetup : DbMigration
  {
    public override void Up()
    {
      CreateTable(
          "dbo.BinData",
          c => new
          {
            Id = c.Long(nullable: false),
            RightContent = c.String(unicode: false),
            LeftContent = c.String(unicode: false),
          })
          .PrimaryKey(t => t.Id);
    }

    public override void Down()
    {
      DropTable("dbo.BinData");
    }
  }
}