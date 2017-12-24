namespace WAES.Cris.Model.DTO
{
  /// <summary>
  /// Data transfer object holding the very properties necessary to upsert a <see cref="BinData"/> record.
  /// </summary>
  public class PostBinDataDto
  {
    /// <summary>
    /// Base64 string to be associated with a <see cref="BinData"/> instance.
    /// </summary>
    public string Data { get; set; }
  }
}