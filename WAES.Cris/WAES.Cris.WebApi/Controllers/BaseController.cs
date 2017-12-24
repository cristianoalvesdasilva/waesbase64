using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace WAES.Cris.WebApi.Controllers
{
  /// <summary>
  /// Holds a set of common functions for API controllers.
  /// </summary>
  public class BaseController : ApiController
  {
    /// <summary>
    /// Executes a given action and handles any thrown exception.
    /// </summary>
    /// <param name="action">Async action to be executed and handled.</param>
    /// <returns><see cref="Task{IHttpActionResult}"/> instance for a successful execution.</returns>
    protected async Task<IHttpActionResult> ExecAndHandleAsync(Func<Task> action)
    {
      return await this.ExecAndHandleAsync<object>(async () =>
      {
        await action();
        return null;
      });
    }

    /// <summary>
    /// Executes a given action and handles any thrown exception.
    /// Returns <see cref="Task{IHttpActionResult}"/> instance for a successful execution.
    /// <para>
    /// Should <paramref name="action"/> run successfully, then <see cref="System.Net.HttpStatusCode.OK"/> is returned along with its result.
    /// Otherwise <see cref="System.Net.HttpStatusCode.BadRequest"/> along with the very error message.
    /// </para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="action">Async action to be executed and handled.</param>
    /// <returns><see cref="Task{IHttpActionResult}"/> instance for a successful execution.</returns>
    protected async Task<IHttpActionResult> ExecAndHandleAsync<T>(Func<Task<T>> action)
    {
      try
      {
        T result = await action();
        if (result == null)
        {
          return this.Ok();
        }

        return this.Ok(result);
      }
      catch (Exception ex)
      {
        return this.BadRequest(ex.Message);
      }
    }
  }
}