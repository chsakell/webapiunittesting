#region Using
using Microsoft.Owin.Hosting;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using UnitTestingWebAPI.API.Core.Filters;
using UnitTestingWebAPI.Data;
using UnitTestingWebAPI.Domain;
using UnitTestingWebAPI.Tests.Hosting;
#endregion

namespace UnitTestingWebAPI.Tests
{
    [TestFixture]
    public class ActionFilterTests
    {
        #region Variables
        private List<Article> _articles;
        #endregion

        #region Setup
        [SetUp]
        public void Setup()
        {
            _articles = BloggerInitializer.GetAllArticles();
        }
        #endregion

        #region Tests
        [Test]
        public void ShouldSortArticlesByTitle()
        {
            var filter = new ArticlesReversedFilter();
            var executedContext = new HttpActionExecutedContext(new HttpActionContext
            {
                Response = new HttpResponseMessage(),
            }, null);

            executedContext.Response.Content = new ObjectContent<List<Article>>(new List<Article>(_articles), new JsonMediaTypeFormatter());

            filter.OnActionExecuted(executedContext);

            var _returnedArticles = executedContext.Response.Content.ReadAsAsync<List<Article>>().Result;

            Assert.That(_returnedArticles.First(), Is.EqualTo(_articles.Last()));
        }

        [Test]
        public void ShouldCallToControllerActionReverseArticles()
        {
            //Arrange
            var address = "http://localhost:9000/";

            using (WebApp.Start<Startup>(address))
            {
                HttpClient _client = new HttpClient();
                var response = _client.GetAsync(address + "api/articles").Result;

                var _returnedArticles = response.Content.ReadAsAsync<List<Article>>().Result;

                Assert.That(_returnedArticles.First().Title, Is.EqualTo(BloggerInitializer.GetAllArticles().Last().Title));
            }
        }
        #endregion
    }
}
