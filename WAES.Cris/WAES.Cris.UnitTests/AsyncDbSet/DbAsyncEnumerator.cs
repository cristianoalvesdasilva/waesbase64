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
      this.inner.Dispose();
    }

    public Task<bool> MoveNextAsync(CancellationToken cancellationToken)
    {
      return Task.FromResult(this.inner.MoveNext());
    }

    public T Current => this.inner.Current;

    object IDbAsyncEnumerator.Current => this.Current;
  }
}