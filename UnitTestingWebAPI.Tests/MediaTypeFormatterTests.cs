using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnitTestingWebAPI.API.Core.MediaTypeFormatters;
using UnitTestingWebAPI.Data;
using UnitTestingWebAPI.Domain;

namespace UnitTestingWebAPI.Tests
{
    [TestFixture]
    public class MediaTypeFormatterTests
    {
        #region Variables
        Blog _blog;
        Article _article;
        ArticleFormatter _formatter;
        #endregion

        #region Setup
        [SetUp]
        public void Setup()
        {
            _blog = BloggerInitializer.GetBlogs().First();
            _article = BloggerInitializer.GetChsakellsArticles().First();
            _formatter = new ArticleFormatter();
        }
        #endregion

        #region Tests
        [Test]
        public void FormatterShouldThrowExceptionWhenUnsupportedType()
        {
            Assert.Throws<InvalidOperationException>(() => new ObjectContent<Blog>(_blog, _formatter));
        }

        [Test]
        public void FormatterShouldNotThrowExceptionWhenArticle()
        {
            Assert.DoesNotThrow(() => new ObjectContent<Article>(_article, _formatter));
        }

        [Test]
        public void FormatterShouldHeaderBeSetCorrectly()
        {
            var content = new ObjectContent<Article>(_article, new ArticleFormatter());

            Assert.That(content.Headers.ContentType.MediaType, Is.EqualTo("application/article"));
        }

        [Test]
        public async void FormatterShouldBeAbleToDeserializeArticle()
        {
            var content = new ObjectContent<Article>(_article, _formatter);

            var deserializedItem = await content.ReadAsAsync<Article>(new[] { _formatter });

            Assert.That(_article, Is.SameAs(deserializedItem));
        }

        [Test]
        public void FormatterShouldNotBeAbleToWriteUnsupportedType()
        {
            var canWriteBlog = _formatter.CanWriteType(typeof(Blog));
            Assert.That(canWriteBlog, Is.False);
        }

        [Test]
        public void FormatterShouldBeAbleToWriteArticle()
        {
            var canWriteArticle = _formatter.CanWriteType(typeof(Article));
            Assert.That(canWriteArticle, Is.True);
        }
        #endregion
    }
}
