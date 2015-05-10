using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestingWebAPI.Data.Infrastructure
{
    public class DbFactory : Disposable, IDbFactory
    {
        BloggerEntities dbContext;

        public BloggerEntities Init()
        {
            return dbContext ?? (dbContext = new BloggerEntities());
        }

        protected override void DisposeCore()
        {
            if (dbContext != null)
                dbContext.Dispose();
        }
    }
}
