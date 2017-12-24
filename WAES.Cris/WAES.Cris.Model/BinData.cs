namespace WAES.Cris.Model
{
  /// <summary>
  /// Binary data model.
  /// </summary>
  public class BinData
  {
    /// <summary>
    /// Unique identifier property.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Right content property.
    /// </summary>
    public string RightContent { get; set; }

    /// <summary>
    /// Left content property.
    /// </summary>
    public string LeftContent { get; set; }
  }
}