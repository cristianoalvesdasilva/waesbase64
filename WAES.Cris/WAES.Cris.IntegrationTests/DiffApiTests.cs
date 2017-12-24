using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Ninject;
using Ninject.Web.WebApi;
using WAES.Cris.DataAccess;

namespace WAES.Cris.IntegrationTests
{
  [TestClass]
  public class DiffApiTests : IDisposable
  {
    private const string ApiBaseUrl = "http://127.0.0.1:3000";

    private IKernel kernel;
    private SelfHostHttpServer server;
    private IDataEntities dbContext;

    public DiffApiTests()
    {
      // Injection container.
      this.kernel = new StandardKernel();
      this.kernel.Load<WebApi.Injection.NinjectBindings>();

      // Host.
      this.server = new SelfHostHttpServer((c) =>
      {
        WebApi.WebApiConfig.Register(c);
        c.DependencyResolver = new NinjectDependencyResolver(this.kernel);
      }, ApiBaseUrl);

      this.dbContext = this.kernel.Get<IDataEntities>();
    }

    [TestMethod]
    public void Post_Right_Should_Return_Http_200_When_Content_Successfully_Upserted()
    {
      int recordId = 9999;
      this.PostAndAssertHttpCode(path: $"/v1/diff/{recordId}/right", base64String: null, additionalAssertion: (base64String, httpResponse) =>
      {
        this.dbContext.BinDataSet.Any(_ => _.Id == recordId && _.RightContent == base64String)
          .Should()
          .BeTrue();
      });
    }

    [TestMethod]
    public void Post_Right_Should_Return_Http_400_When_Right_Content_Is_Invalid()
    {
      // Act & Assert
      this.PostAndAssert(path: "/v1/diff/1/right", statusCodeAssertion: (httpCode) =>
      {
        httpCode.Should().Be(HttpStatusCode.BadRequest);
      },
      base64String: string.Empty);
    }

    [TestMethod]
    public void Post_Left_Should_Return_Http_200_When_Content_Successfully_Upserted()
    {
      int recordId = 9999;
      this.PostAndAssertHttpCode(path: $"/v1/diff/{recordId}/left", base64String: null, additionalAssertion: (base64String, httpResponse) =>
      {
        this.dbContext.BinDataSet.Any(_ => _.Id == recordId && _.LeftContent == base64String)
          .Should()
          .BeTrue();
      });
    }

    [TestMethod]
    public void Post_Left_Should_Return_Http_400_When_Content_Is_Invalid()
    {
      // Act & Assert
      this.PostAndAssert(path: "/v1/diff/1/left", statusCodeAssertion: (httpCode) =>
      {
        httpCode.Should().Be(HttpStatusCode.BadRequest);
      },
      base64String: string.Empty);
    }

    [TestMethod]
    public void Get_Diff_Should_Return_Http_200_And_Message_When_Left_And_Right_Are_Same()
    {
      // Arrange
      int recordId = 9999;
      string base64String = this.GenerateBase64String();

      // Setting left & right with same string.
      this.PostAndAssertHttpCode($"/v1/diff/{recordId}/right", base64String);
      this.PostAndAssertHttpCode($"/v1/diff/{recordId}/left", base64String);

      // Act
      string jsonResponse = this.GetAndAssertHttpCode($"/v1/diff/{recordId}");
      dynamic diffResult = JsonConvert.DeserializeObject(jsonResponse);

      // Assert
      string message = diffResult.Message;
      message.Should().Be("Left and Right are the same.");
    }

    [TestMethod]
    public void Get_Diff_Should_Return_Http_200_And_Message_When_Left_And_Right_Have_Different_Sizes()
    {
      // Arrange
      int recordId = 9999;
      string rightBase64String = this.GenerateBase64String("RIGHT!!");
      string leftBase64String = this.GenerateBase64String("LEFT!");

      // Setting left & right different size strings.
      this.PostAndAssertHttpCode($"/v1/diff/{recordId}/right", rightBase64String);
      this.PostAndAssertHttpCode($"/v1/diff/{recordId}/left", leftBase64String);

      // Act
      string jsonResponse = this.GetAndAssertHttpCode($"/v1/diff/{recordId}");
      dynamic diffResult = JsonConvert.DeserializeObject(jsonResponse);

      // Assert
      string message = diffResult.Message;
      message.Should().Be("Left and Right have different sizes.");
    }

    [TestMethod]
    public void Get_Diff_Should_Return_Http_200_And_Message_When_Left_And_Right_Have_Same_Sizes()
    {
      // Arrange
      int recordId = 9999;
      string rightBase64String = this.GenerateBase64String("TEXT!!");
      string leftBase64String = this.GenerateBase64String("text!!");

      // Setting left & right different size strings.
      this.PostAndAssertHttpCode($"/v1/diff/{recordId}/right", rightBase64String);
      this.PostAndAssertHttpCode($"/v1/diff/{recordId}/left", leftBase64String);

      // Act
      string jsonResponse = this.GetAndAssertHttpCode($"/v1/diff/{recordId}");
      dynamic diffResult = JsonConvert.DeserializeObject(jsonResponse);

      // Assert
      string message = diffResult.Message;
      message.Should().Be("Left and Right have same size, but with different content.");

      List<int> offsets = diffResult.DiffOffsets.ToObject<List<int>>();
      offsets.Should().NotBeNullOrEmpty();

      int length = diffResult.Length;
      length.Should().BeGreaterThan(0);
    }

    [TestMethod]
    public void Get_Diff_Should_Return_Http_400_When_Id_Not_Found()
    {
      // Arrange
      long recordId = this.dbContext.BinDataSet.Max(_ => _.Id) + 1;

      // Act & Assert
      string jsonResponse = this.GetAndAssert($"/v1/diff/{recordId}", (httpCode) =>
      {
        httpCode.Should().Be(HttpStatusCode.BadRequest);
      });
    }

    /// <summary>
    /// Disposes underlying resources.
    /// </summary>
    public void Dispose()
    {
      this.server.Dispose();
      this.kernel.Dispose();
      this.dbContext.Dispose();
    }

    /// <summary>
    /// Generates a base64 string. Should <paramref name="text"/> be provided, then the operation happens over it, otherwise a random text is picked.
    /// </summary>
    /// <param name="text">Text to be converted to a base64 string. This parameter is optional.</param>
    /// <returns>Base64 string</returns>
    private string GenerateBase64String(string text = null)
    {
      if (string.IsNullOrWhiteSpace(text))
      {
        text = $"Hello World!__{DateTime.Now.Ticks}";
      }

      var bytes = Encoding.UTF8.GetBytes(text);
      return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// Posts data over HTTP against the self-hosted API and asserts if HTTP 200 code was returned.
    /// </summary>
    /// <param name="path">API path (without ApiBaseUrl) to be posted against.</param>
    /// <param name="base64String">Base64 string to be posted. Should it be null, a random string will be provided.</param>
    /// <param name="additionalAssertion">Function to perform additional assertion.
    /// The first argument is the base64 string used during the post,
    /// whereas the second is an instance of<see cref="HttpResponseMessage"/> returned from the server.
    /// </param>
    private void PostAndAssertHttpCode(string path, string base64String = null, Action<string, HttpResponseMessage> additionalAssertion = null)
    {
      this.PostAndAssert(path, (httpCode) => httpCode.Should().Be(HttpStatusCode.OK), base64String, additionalAssertion);
    }

    /// <summary>
    /// Posts data over HTTP against the self-hosted API and asserts if HTTP 200 code was returned.
    /// </summary>
    /// <param name="path">API path (without ApiBaseUrl) to be posted against.</param>
    /// <param name="statusCodeAssertion">Function to assert the very http response code returned from the server.</param>
    /// <param name="base64String">Base64 string to be posted. Should it be null, a random string will be provided.</param>
    /// <param name="additionalAssertion">Function to perform additional assertion.
    /// The first argument is the base64 string used during the post,
    /// whereas the second is an instance of<see cref="HttpResponseMessage"/> returned from the server.
    /// </param>
    private void PostAndAssert(string path, Action<HttpStatusCode> statusCodeAssertion, string base64String = null, Action<string, HttpResponseMessage> additionalAssertion = null)
    {
      if (base64String == null)
      {
        base64String = this.GenerateBase64String();
      }

      using (var request = new HttpRequestMessage(HttpMethod.Post, ApiBaseUrl + path))
      {
        request.Content = new StringContent($"{{\"data\":\"{base64String}\"}}", Encoding.UTF8, "application/json");
        using (HttpResponseMessage response = server.Send(request))
        {
          statusCodeAssertion(response.StatusCode);
          additionalAssertion?.Invoke(base64String, response);
        }
      }
    }

    /// <summary>
    /// Gets data over HTTP from the self-hosted API and asserts if HTTP 200 code was returned.
    /// </summary>
    /// <param name="path">API path (without ApiBaseUrl) to get data from.</param>
    /// <returns>Response body content.</returns>
    private string GetAndAssertHttpCode(string path)
    {
      return this.GetAndAssert(path, (httpCode) => httpCode.Should().Be(HttpStatusCode.OK));
    }

    /// <summary>
    /// Gets data over HTTP from the self-hosted API.
    /// </summary>
    /// <param name="path">API path (without ApiBaseUrl) to get data from.</param>
    /// <param name="statusCodeAssertion">Function to assert the very http response code returned from the server.</param>
    /// <returns>Response body content.</returns>
    private string GetAndAssert(string path, Action<HttpStatusCode> statusCodeAssertion)
    {
      using (var httpClient = new HttpClient())
      {
        using (HttpResponseMessage response = httpClient.GetAsync(ApiBaseUrl + path).Result)
        {
          statusCodeAssertion(response.StatusCode);
          return response.Content.ReadAsStringAsync().Result;
        }
      }
    }
  }
}