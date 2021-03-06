﻿using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;

namespace WAES.Cris.UnitTests.AsyncDbSet
{
  internal class DbAsyncEnumerable<T> : EnumerableQuery<T>, IDbAsyncEnumerable<T>, IQueryable<T>
  {
    public DbAsyncEnumerable(IEnumerable<T> enumerable)
        : base(enumerable)
    { }

    public DbAsyncEnumerable(Expression expression)
        : base(expression)
    { }

    public IDbAsyncEnumerator<T> GetAsyncEnumerator()
    {
      return new DbAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
    }

    IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
    {
      return this.GetAsyncEnumerator();
    }

    IQueryProvider IQueryable.Provider => new DbAsyncQueryProvider<T>(this);
  }
}