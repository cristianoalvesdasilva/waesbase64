using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using WAES.Cris.Model;

namespace WAES.Cris.DataAccess.Config
{
  internal class BinDataConfig : EntityTypeConfiguration<BinData>
  {
    public BinDataConfig()
    {
      // Id will be manually generated.
      this.Property(_ => _.Id)
          .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

      // Base64 string is ASCII, so going with varchar instead of nvarchar (default).
      this.Property(_ => _.LeftContent)
          .HasColumnType("varchar(max)");

      this.Property(_ => _.RightContent)
         .HasColumnType("varchar(max)");
    }
  }
}