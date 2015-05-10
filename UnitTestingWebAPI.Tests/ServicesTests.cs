#region Using
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTestingWebAPI.Data;
using UnitTestingWebAPI.Data.Infrastructure;
using UnitTestingWebAPI.Data.Repositories;
using UnitTestingWebAPI.Domain;
using UnitTestingWebAPI.Service;
#endregion

namespace UnitTestingWebAPI.Tests
{
    [TestFixture]
    public class ServicesTests
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
                        oldArticle = x;
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
        public void ServiceShouldReturnAllArticles()
        {
            var articles = _articleService.GetArticles();

            Assert.That(articles, Is.EqualTo(_randomArticles));
        }

        [Test]
        public void ServiceShouldReturnRightArticle()
        {
            var wcfSecurityArticle = _articleService.GetArticle(2);

            Assert.That(wcfSecurityArticle,
                Is.EqualTo(_randomArticles.Find(a => a.Title.Contains("Secure WCF Services"))));
        }

        [Test]
        public void ServiceShouldAddNewArticle()
        {
            var _newArticle = new Article()
            {
                Author = "Chris Sakellarios",
                Contents = "If you are an ASP.NET MVC developer, you will certainly..",
                Title = "URL Rooting in ASP.NET (Web Forms)",
                URL = "http://chsakell.com/2013/12/15/url-rooting-in-asp-net-web-forms/"
            };

            int _maxArticleIDBeforeAdd = _randomArticles.Max(a => a.ID);
            _articleService.CreateArticle(_newArticle);

            Assert.That(_newArticle, Is.EqualTo(_randomArticles.Last()));
            Assert.That(_maxArticleIDBeforeAdd + 1, Is.EqualTo(_randomArticles.Last().ID));
        }

        [Test]
        public void ServiceShouldUpdateArticle()
        {
            var _firstArticle = _randomArticles.First();

            _firstArticle.Title = "OData feat. ASP.NET Web API"; // reversed :-)
            _firstArticle.URL = "http://t.co/fuIbNoc7Zh"; // short link
            _articleService.UpdateArticle(_firstArticle);

            Assert.That(_firstArticle.DateEdited, Is.Not.EqualTo(DateTime.MinValue));
            Assert.That(_firstArticle.URL, Is.EqualTo("http://t.co/fuIbNoc7Zh"));
            Assert.That(_firstArticle.ID, Is.EqualTo(1)); // hasn't changed
        }

        [Test]
        public void ServiceShouldDeleteArticle()
        {
            int maxID = _randomArticles.Max(a => a.ID); // Before removal
            var _lastArticle = _randomArticles.Last();

            // Remove last article
            _articleService.DeleteArticle(_lastArticle);

            Assert.That(maxID, Is.GreaterThan(_randomArticles.Max(a => a.ID))); // Max reduced by 1
        }

        #endregion
    }
}
