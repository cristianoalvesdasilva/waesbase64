using System.Threading.Tasks;
using System.Web.Http;
using WAES.Cris.Model;
using WAES.Cris.Model.DTO;
using WAES.Cris.Services;

namespace WAES.Cris.WebApi.Controllers
{
  /// <summary>
  /// Responsible for persisting and comparing <see cref="Model.BinData"/> records.
  /// </summary>
  [RoutePrefix("v1/diff")]
  public sealed class DiffsController : BaseController
  {
    private readonly IBinDataService binDataService;

    /// <summary>
    /// Initializes an instance of <see cref="DiffsController"/> with an inject instance of <see cref="IBinDataService"/>.
    /// </summary>
    /// <param name="binDataService"></param>
    public DiffsController(IBinDataService binDataService)
    {
      this.binDataService = binDataService;
    }

    /// <summary>
    /// Gets the differences between 'left' and 'right' content for an existing record.
    /// </summary>
    /// <param name="id">Unique identifier associated with an existing record.</param>
    /// <returns><see cref="Task{IHttpActionResult}"/> wrapping a <see cref="DataDiffResultDto"/> instance for a successful comparison.
    /// <para>
    /// Should and error happen, then <see cref="System.Net.HttpStatusCode.BadRequest"/> is returned along with the error message.
    /// </para>
    /// </returns>
    [HttpGet]
    [Route("{id}")]
    public async Task<IHttpActionResult> GetDiffAsync(long id)
    {
      return await this.ExecAndHandleAsync(() => this.binDataService.CompareLeftAndRightAsync(id));
    }

    /// <summary>
    /// Creates a record should none be found for the given <paramref name="id"/>
    /// and sets its <see cref="Model.BinData.LeftContent"/>, otherwise simply performs the latter.
    /// </summary>
    /// <param name="id">Unique identifier associated with an existing <see cref="Model.BinData"/> record, or to be used should a new record be created.</param>
    /// <param name="data">Json with a 'data' property holding a base64 string.</param>
    /// <returns><see cref="System.Net.HttpStatusCode.OK"/> for a successful execution, otherwise <see cref="System.Net.HttpStatusCode.BadRequest"/></returns>
    [HttpPost]
    [Route("{id}/left")]
    public async Task<IHttpActionResult> UpsertLeftAsync([FromUri] long id, PostBinDataDto data)
    {
      return await this.ExecAndHandleAsync(() => this.binDataService.UpsertAsync(new BinData
      {
        Id = id,
        LeftContent = data.Data
      }));
    }

    /// <summary>
    /// Creates a <see cref="Model.BinData"/> record should none be found for the given <paramref name="id"/>
    /// and sets its <see cref="Model.BinData.RightContent"/>, otherwise simply performs the latter.
    /// </summary>
    /// <param name="id">Unique identifier associated with an existing <see cref="Model.BinData"/> record, or to be used should a new record be created.</param>
    /// <param name="data">Json with a 'data' property holding a base64 string.</param>
    /// <returns><see cref="System.Net.HttpStatusCode.OK"/> for a successful execution, otherwise <see cref="System.Net.HttpStatusCode.BadRequest"/></returns>
    [HttpPost]
    [Route("{id}/right")]
    public async Task<IHttpActionResult> UpsertRightAsync([FromUri] long id, PostBinDataDto data)
    {
      return await this.ExecAndHandleAsync(() => this.binDataService.UpsertAsync(new BinData
      {
        Id = id,
        RightContent = data.Data
      }));
    }
  }
}