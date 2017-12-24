using Ninject.Extensions.Conventions;
using Ninject.Extensions.NamedScope;
using Ninject.Infrastructure;
using Ninject.Modules;
using Ninject.Syntax;
using Ninject.Web.Common;
using WAES.Cris.DataAccess;

namespace WAES.Cris.WebApi.Injection
{
  public class NinjectBindings : NinjectModule
  {
    public override void Load()
    {
      // Binding factories.
      this.Bind(x => x.FromAssembliesMatching("WAES.Cris.*")
        .SelectAllInterfaces()
        .Where(t => t.Name.EndsWith("Factory"))
        .BindToFactory());

      // Binding non-factory classes to its default interfaces in request scope.
      this.Bind(_ => _.From("WAES.Cris.DataAccess", "WAES.Cris.Services")
        .SelectAllClasses()
        .Where(c => !c.Name.EndsWith("Factory"))
        .BindDefaultInterfaces()
        .Configure(b => b.InRequestScope()));

      this.Bind(x =>
        x.From("WAES.Cris.DataAccess", "WAES.Cris.Services")
        .SelectAllClasses()
        .Where(t => !t.Name.EndsWith("Factory"))
        .BindDefaultInterface()
        .Configure(b => b
          .WhenAnyAncestorInSingletonScope()
          .InParentScope()
        ));

      this.Bind<IDataEntities>().To<WaesDbContext>();
    }
  }

  public static class NinjectExtender
  {
    public static IBindingInNamedWithOrOnSyntax<T> WhenAnyAncestorInSingletonScope<T>(this IBindingWhenSyntax<T> syntax)
    {
      return syntax.WhenAnyAncestorMatches(ctx => ctx.Binding.ScopeCallback == StandardScopeCallbacks.Singleton);
    }
  }
}