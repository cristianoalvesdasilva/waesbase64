using System.Threading.Tasks;
using WAES.Cris.Model;
using WAES.Cris.Model.DTO;

namespace WAES.Cris.Services
{
  /// <summary>
  /// Provides mechanisms for persisting and comparing <see cref="BinData"/> records.
  /// </summary>
  public interface IBinDataService
  {
    /// <summary>
    /// Asynchronously compares <see cref="BinData.LeftContent"/> against <see cref="BinData.RightContent"/>
    /// for an existing <see cref="BinData"/> record based off of a given <paramref name="id"/>.
    /// </summary>
    /// <param name="id"><see cref="BinData"/> unique identifier.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a single instance of <see cref="DataDiffResultDto"/></returns>
    /// <exception cref="System.InvalidOperationException">
    /// Thrown should
    /// <para>***No record be found for the given <paramref name="id"/></para>
    /// <para>***Either <see cref="BinData.LeftContent"/> or <see cref="BinData.LeftContent"/> be null/empty.</para>
    /// </exception>
    Task<DataDiffResultDto> CompareLeftAndRightAsync(long id);

    /// <summary>
    /// Asynchronously creates OR updates a record for the given <paramref name="data"/>.
    /// </summary>
    /// <param name="data"><see cref="BinData"/> instance containing a valid <see cref="BinData.Id"/>,
    /// and also either <see cref="BinData.LeftContent"/> or <see cref="BinData.RightContent"/> populated.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpsertAsync(BinData data);
  }
}