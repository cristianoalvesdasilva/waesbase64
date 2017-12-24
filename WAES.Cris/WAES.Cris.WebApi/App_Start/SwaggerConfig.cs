using System.IO;
using System.Web.Http;
using Swashbuckle.Application;
using WAES.Cris.WebApi;
using WebActivatorEx;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace WAES.Cris.WebApi
{
  public class SwaggerConfig
  {
    public static void Register()
    {
      GlobalConfiguration.Configuration
        .EnableSwagger(c =>
        {
          c.SingleApiVersion("v1", "WAES.Cris.WebApi");
          c.IncludeXmlComments(GetXmlCommentsPath());
        })
        .EnableSwaggerUi();
    }

    private static string GetXmlCommentsPath()
    {
      return Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/bin"), "ApiXmlDoc.xml");
    }
  }
}