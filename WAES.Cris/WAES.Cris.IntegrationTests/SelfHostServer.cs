using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace WAES.Cris.IntegrationTests
{
  public class SelfHostHttpServer : IDisposable
  {
    private HttpSelfHostConfiguration config;
    private HttpSelfHostServer server;

    public SelfHostHttpServer(Action<HttpConfiguration> configurationHandler, string baseUrl)
    {
      if (configurationHandler == null)
      {
        throw new ArgumentNullException(nameof(configurationHandler), "Value cannot be null or empty.");
      }

      if (string.IsNullOrWhiteSpace(baseUrl))
      {
        throw new ArgumentException("Value cannot be null or empty.", nameof(baseUrl));
      }

      this.InitializeServer(configurationHandler, baseUrl);
    }

    public HttpResponseMessage Send(HttpRequestMessage request)
    {
      return new HttpClient().SendAsync(request).Result;
    }

    public void Dispose()
    {
      this.server?.CloseAsync()?.Wait();
      this.server?.Dispose();
      this.config?.Dispose();
    }

    private void InitializeServer(Action<HttpConfiguration> configurationHandler, string baseUrl)
    {
      this.config = new HttpSelfHostConfiguration(baseUrl);
      configurationHandler(config);
      this.server = new HttpSelfHostServer(config);
      this.server.OpenAsync().Wait();
    }
  }
}