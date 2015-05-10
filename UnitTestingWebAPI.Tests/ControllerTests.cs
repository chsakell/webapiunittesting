#region Usings
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Http.Routing;
using UnitTestingWebAPI.API.Core.Controllers;
using UnitTestingWebAPI.Data;
using UnitTestingWebAPI.Data.Infrastructure;
using UnitTestingWebAPI.Data.Repositories;
using UnitTestingWebAPI.Domain;
using UnitTestingWebAPI.Service;
#endregion

namespace UnitTestingWebAPI.Tests
{
    [TestFixture]
    public class ControllerTests
    {
        #region Variables
        IArticleService _articleService;
        IArticleRepository _articleRepository;
        IUnitOfWork _unitOfWork;
        List<Article> _randomArticles;
        #endregion

        #region Setup
        [SetUp]
        public void Setup()
        {
            _randomArticles = SetupArticles();

            _articleRepository = SetupArticleRepository();
            _unitOfWork = new Mock<IUnitOfWork>().Object;
            _articleService = new ArticleService(_articleRepository, _unitOfWork);
        }

        public List<Article> SetupArticles()
        {
            int _counter = new int();
            List<Article> _articles = BloggerInitializer.GetAllArticles();

            foreach (Article _article in _articles)
                _article.ID = ++_counter;

            return _articles;
        }

        public IArticleRepository SetupArticleRepository()
        {
            // Init repository
            var repo = new Mock<IArticleRepository>();

            // Setup mocking behavior
            repo.Setup(r => r.GetAll()).Returns(_randomArticles);

            repo.Setup(r => r.GetById(It.IsAny<int>()))
                .Returns(new Func<int, Article>(
                    id => _randomArticles.Find(a => a.ID.Equals(id))));

            repo.Setup(r => r.Add(It.IsAny<Article>()))
                .Callback(new Action<Article>(newArticle =>
                {
                    dynamic maxArticleID = _randomArticles.Last().ID;
                    dynamic nextArticleID = maxArticleID + 1;
                    newArticle.ID = nextArticleID;
                    newArticle.DateCreated = DateTime.Now;
                    _randomArticles.Add(newArticle);
                }));

            repo.Setup(r => r.Update(It.IsAny<Article>()))
                .Callback(new Action<Article>(x =>
                {
                    var oldArticle = _randomArticles.Find(a => a.ID == x.ID);
                    oldArticle.DateEdited = DateTime.Now;
                    oldArticle.URL = x.URL;
                    oldArticle.Title = x.Title;
                    oldArticle.Contents = x.Contents;
                    oldArticle.BlogID = x.BlogID;
                }));

            repo.Setup(r => r.Delete(It.IsAny<Article>()))
                .Callback(new Action<Article>(x =>
                {
                    var _articleToRemove = _randomArticles.Find(a => a.ID == x.ID);

                    if (_articleToRemove != null)
                        _randomArticles.Remove(_articleToRemove);
                }));

            // Return mock implementation
            return repo.Object;
        }

        #endregion

        #region Tests

        [Test]
        public void ControlerShouldReturnAllArticles()
        {
            var _articlesController = new ArticlesController(_articleService);

            var result = _articlesController.GetArticles();

            CollectionAssert.AreEqual(result, _randomArticles);
        }

        [Test]
        public void ControlerShouldReturnLastArticle()
        {
            var _articlesController = new ArticlesController(_articleService);

            var result = _articlesController.GetArticle(3) as OkNegotiatedContentResult<Article>;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Content.Title, _randomArticles.Last().Title);
        }

        [Test]
        public void ControlerShouldPutReturnBadRequestResult()
        {
            var _articlesController = new ArticlesController(_articleService)
            {
                Configuration = new HttpConfiguration(),
                Request = new HttpRequestMessage
                {
                    Method = HttpMethod.Put,
                    RequestUri = new Uri("http://localhost/api/articles/-1")
                }
            };

            var badresult = _articlesController.PutArticle(-1, new Article() { Title = "Unknown Article" });
            Assert.That(badresult, Is.TypeOf<BadRequestResult>());
        }

        [Test]
        public void ControlerShouldPutUpdateFirstArticle()
        {
            var _articlesController = new ArticlesController(_articleService)
            {
                Configuration = new HttpConfiguration(),
                Request = new HttpRequestMessage
                {
                    Method = HttpMethod.Put,
                    RequestUri = new Uri("http://localhost/api/articles/1")
                }
            };

            IHttpActionResult updateResult = _articlesController.PutArticle(1, new Article()
            {
                ID = 1,
                Title = "ASP.NET Web API feat. OData",
                URL = "http://t.co/fuIbNoc7Zh",
                Contents = @"OData is an open standard protocol.."
            }) as IHttpActionResult;

            Assert.That(updateResult, Is.TypeOf<StatusCodeResult>());

            StatusCodeResult statusCodeResult = updateResult as StatusCodeResult;

            Assert.That(statusCodeResult.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

            Assert.That(_randomArticles.First().URL, Is.EqualTo("http://t.co/fuIbNoc7Zh"));
        }

        [Test]
        public void ControlerShouldPostNewArticle()
        {
            var article = new Article
            {
                Title = "Web API Unit Testing",
                URL = "http://chsakell.com/web-api-unit-testing",
                Author = "Chris Sakellarios",
                DateCreated = DateTime.Now,
                Contents = "Unit testing Web API.."
            };

            var _articlesController = new ArticlesController(_articleService)
            {
                Configuration = new HttpConfiguration(),
                Request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("http://localhost/api/articles")
                }
            };

            _articlesController.Configuration.MapHttpAttributeRoutes();
            _articlesController.Configuration.EnsureInitialized();
            _articlesController.RequestContext.RouteData = new HttpRouteData(
            new HttpRoute(), new HttpRouteValueDictionary { { "_articlesController", "Articles" } });
            var result = _articlesController.PostArticle(article) as CreatedAtRouteNegotiatedContentResult<Article>;

            Assert.That(result.RouteName, Is.EqualTo("DefaultApi"));
            Assert.That(result.Content.ID, Is.EqualTo(result.RouteValues["id"]));
            Assert.That(result.Content.ID, Is.EqualTo(_randomArticles.Max(a => a.ID)));
        }

        [Test]
        public void ControlerShouldNotPostNewArticle()
        {
            var article = new Article
            {
                Title = "Web API Unit Testing",
                URL = "http://chsakell.com/web-api-unit-testing",
                Author = "Chris Sakellarios",
                DateCreated = DateTime.Now,
                Contents = null
            };

            var _articlesController = new ArticlesController(_articleService)
            {
                Configuration = new HttpConfiguration(),
                Request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("http://localhost/api/articles")
                }
            };

            _articlesController.Configuration.MapHttpAttributeRoutes();
            _articlesController.Configuration.EnsureInitialized();
            _articlesController.RequestContext.RouteData = new HttpRouteData(
            new HttpRoute(), new HttpRouteValueDictionary { { "Controller", "Articles" } });
            _articlesController.ModelState.AddModelError("Contents", "Contents is required field");

            var result = _articlesController.PostArticle(article) as InvalidModelStateResult;

            Assert.That(result.ModelState.Count, Is.EqualTo(1));
            Assert.That(result.ModelState.IsValid, Is.EqualTo(false));
        }

        #endregion
    }
}
