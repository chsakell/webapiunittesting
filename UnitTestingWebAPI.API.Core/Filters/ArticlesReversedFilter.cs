using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using UnitTestingWebAPI.Domain;

namespace UnitTestingWebAPI.API.Core.Filters
{
    public class ArticlesReversedFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            var objectContent = actionExecutedContext.Response.Content as ObjectContent;
            if (objectContent != null)
            {
                List<Article> _articles = objectContent.Value as List<Article>;
                if (_articles != null && _articles.Count > 0)
                {
                    _articles.Reverse();
                }
            }
        }
    }
}
