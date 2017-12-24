using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Moq;
using WAES.Cris.UnitTests.AsyncDbSet;

namespace WAES.Cris.UnitTests
{
  public static class MockHelper
  {
    public static Mock<IDbSet<T>> MockDbSet<T>(params T[] items) where T : class
    {
      var queryable = items.AsQueryable();
      var mockSet = new Mock<IDbSet<T>>();
      mockSet.As<IDbAsyncEnumerable<T>>()
        .Setup(m => m.GetAsyncEnumerator())
        .Returns(new DbAsyncEnumerator<T>(queryable.GetEnumerator()));

      mockSet.As<IQueryable<T>>()
        .Setup(m => m.Provider)
        .Returns(new DbAsyncQueryProvider<T>(queryable.Provider));

      mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
      mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
      mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

      return mockSet;
    }
  }
}