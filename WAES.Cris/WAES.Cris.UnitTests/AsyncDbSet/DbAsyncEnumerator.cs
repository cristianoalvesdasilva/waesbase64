using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

namespace WAES.Cris.UnitTests.AsyncDbSet
{
  internal class DbAsyncEnumerator<T> : IDbAsyncEnumerator<T>
  {
    private readonly IEnumerator<T> inner;

    public DbAsyncEnumerator(IEnumerator<T> inner)
    {
      this.inner = inner;
    }

    public void Dispose()
    {
      inner.Dispose();
    }

    public Task<bool> MoveNextAsync(CancellationToken cancellationToken)
    {
      return Task.FromResult(inner.MoveNext());
    }

    public T Current => inner.Current;

    object IDbAsyncEnumerator.Current => Current;
  }
}