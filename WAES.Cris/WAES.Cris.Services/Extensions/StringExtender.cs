namespace WAES.Cris.Services.Extensions
{
  public static class StringExtender
  {
    /// <summary>
    /// Checks whether a string is either null, empty or contains only white spaces.
    /// </summary>
    /// <param name="value">String to be validated.</param>
    /// <returns>True should the string be 'blank', otherwise false.</returns>
    public static bool IsBlank(this string value)
    {
      return string.IsNullOrWhiteSpace(value);
    }
  }
}