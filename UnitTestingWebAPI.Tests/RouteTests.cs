#region Using
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using UnitTestingWebAPI.API.Core.Controllers;
using UnitTestingWebAPI.Domain;
using UnitTestingWebAPI.Tests.Helpers;
#endregion

namespace UnitTestingWebAPI.Tests
{
    [TestFixture]
    public class RouteTests
    {
        #region Variables
        HttpConfiguration _config;
        #endregion

        #region Setup
        [SetUp]
        public void Setup()
        {
            _config = new HttpConfiguration();
            _config.Routes.MapHttpRoute(name: "DefaultWebAPI", routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });
        }
        #endregion

        #region Tests
        [Test]
        public void RouteShouldControllerGetArticleIsInvoked()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "http://www.chsakell.com/api/articles/5");

            var _actionSelector = new ControllerActionSelector(_config, request);

            Assert.That(typeof(ArticlesController), Is.EqualTo(_actionSelector.GetControllerType()));
            Assert.That(GetMethodName((ArticlesController c) => c.GetArticle(5)),
                Is.EqualTo(_actionSelector.GetActionName()));
        }

        [Test]
        public void RouteShouldPostArticleActionIsInvoked()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://www.chsakell.com/api/articles/");

            var _actionSelector = new ControllerActionSelector(_config, request);

            Assert.That(GetMethodName((ArticlesController c) =>
                c.PostArticle(new Article())), Is.EqualTo(_actionSelector.GetActionName()));
        }

        [Test]
        public void RouteShouldInvalidRouteThrowException()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://www.chsakell.com/api/InvalidController/");

            var _actionSelector = new ControllerActionSelector(_config, request);

            Assert.Throws<HttpResponseException>(() => _actionSelector.GetActionName());
        }

        #endregion

        #region Helper methods
        public static string GetMethodName<T, U>(Expression<Func<T, U>> expression)
        {
            var method = expression.Body as MethodCallExpression;
            if (method != null)
                return method.Method.Name;

            throw new ArgumentException("Expression is wrong");
        }
        #endregion
    }
}
