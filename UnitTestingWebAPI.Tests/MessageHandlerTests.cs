#region Using
using Microsoft.Owin.Hosting;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using UnitTestingWebAPI.API.Core.Controllers;
using UnitTestingWebAPI.API.Core.MessageHandlers;
using UnitTestingWebAPI.Data;
using UnitTestingWebAPI.Data.Infrastructure;
using UnitTestingWebAPI.Data.Repositories;
using UnitTestingWebAPI.Domain;
using UnitTestingWebAPI.Service;
using UnitTestingWebAPI.Tests.Hosting;
#endregion

namespace UnitTestingWebAPI.Tests
{
    [TestFixture]
    public class MessageHandlerTests
    {
        #region Variables
        private EndRequestHandler _endRequestHandler;
        private HeaderAppenderHandler _headerAppenderHandler;
        #endregion

        #region Setup
        [SetUp]
        public void Setup()
        {
            // Direct MessageHandler test
            _endRequestHandler = new EndRequestHandler();
            _headerAppenderHandler = new HeaderAppenderHandler()
            {
                InnerHandler = _endRequestHandler
            };
        }
        #endregion

        #region Tests
        [Test]
        public async void ShouldAppendCustomHeader()
        {
            var invoker = new HttpMessageInvoker(_headerAppenderHandler);
            var result = await invoker.SendAsync(new HttpRequestMessage(HttpMethod.Get, 
                new Uri("http://localhost/api/test/")), CancellationToken.None);

            Assert.That(result.Headers.Contains("X-WebAPI-Header"), Is.True);
            Assert.That(result.Content.ReadAsStringAsync().Result, 
                Is.EqualTo("Unit testing message handlers!"));
        }

        [Test]
        public void ShouldCallToControllerActionAppendCustomHeader()
        {
            //Arrange
            var address = "http://localhost:9000/";

            using (WebApp.Start<Startup>(address))
            {
                HttpClient _client = new HttpClient();
                var response = _client.GetAsync(address + "api/articles").Result;

                Assert.That(response.Headers.Contains("X-WebAPI-Header"), Is.True);

                var _returnedArticles = response.Content.ReadAsAsync<List<Article>>().Result;
                Assert.That(_returnedArticles.Count, Is.EqualTo( BloggerInitializer.GetAllArticles().Count));
            }
        }
        #endregion
    }
}
