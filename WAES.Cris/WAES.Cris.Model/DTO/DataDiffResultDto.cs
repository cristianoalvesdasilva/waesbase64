using System.Collections.Generic;

namespace WAES.Cris.Model.DTO
{
  /// <summary>
  /// Data transfer object holding the very properties that represent a diff outcome
  /// between <see cref="BinData.LeftContent"/> and <see cref="BinData.RightContent"/>
  /// </summary>
  public class DataDiffResultDto
  {
    /// <summary>
    /// Collection of offsets for every difference found.
    /// </summary>
    public List<int> DiffOffsets { get; set; }

    /// <summary>
    /// Length of the byte array representation of the base64 being compared.
    /// </summary>
    public int? Length { get; set; }

    /// <summary>
    /// Diff outcome message.
    /// </summary>
    public string Message { get; set; }
  }
}