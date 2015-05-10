using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTestingWebAPI.Data.Infrastructure;
using UnitTestingWebAPI.Domain;

namespace UnitTestingWebAPI.Data.Repositories
{
    public class ArticleRepository : RepositoryBase<Article>, IArticleRepository
    {
        public ArticleRepository(IDbFactory dbFactory)
            : base(dbFactory) { }

        public Article GetArticleByTitle(string articleTitle)
        {
            var _article = this.DbContext.Articles.Where(b => b.Title == articleTitle).FirstOrDefault();

            return _article;
        }
    }

    public interface IArticleRepository : IRepository<Article>
    {
        Article GetArticleByTitle(string articleTitle);
    }
}
