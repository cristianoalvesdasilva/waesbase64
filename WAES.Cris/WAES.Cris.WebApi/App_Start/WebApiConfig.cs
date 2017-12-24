using System.Net.Http.Formatting;
using System.Web.Http;
using Newtonsoft.Json.Serialization;

namespace WAES.Cris.WebApi
{
  public static class WebApiConfig
  {
    public static void Register(HttpConfiguration config)
    {
      config.MapHttpAttributeRoutes();

      config.Routes.MapHttpRoute(
        name: "DefaultApi",
        routeTemplate: "api/{controller}/{id}",
        defaults: new { id = RouteParameter.Optional }
      );

      ConfigureJsonFormatter();
    }

    private static void ConfigureJsonFormatter()
    {
      JsonMediaTypeFormatter jsonFormatter = GlobalConfiguration.Configuration.Formatters.JsonFormatter;

      // 'Ignore' (do not render) null properties.
      jsonFormatter.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
      jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    }
  }
}