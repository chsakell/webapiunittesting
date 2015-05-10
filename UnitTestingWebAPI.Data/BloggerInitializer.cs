using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTestingWebAPI.Domain;

namespace UnitTestingWebAPI.Data
{
    public class BloggerInitializer : DropCreateDatabaseIfModelChanges<BloggerEntities>
    {
        protected override void Seed(BloggerEntities context)
        {
            GetBlogs().ForEach(b => context.Blogs.Add(b));

            context.Commit();
        }

        public static List<Blog> GetBlogs()
        {
            List<Blog> _blogs = new List<Blog>();

            // Add two Blogs
            Blog _chsakellsBlog = new Blog()
            {
                Name = "chsakell's Blog",
                URL = "http://chsakell.com/",
                Owner = "Chris Sakellarios",
                Articles = GetChsakellsArticles()
            };

            Blog _dotNetCodeGeeks = new Blog()
            {
                Name = "DotNETCodeGeeks",
                URL = "dotnetcodegeeks",
                Owner = ".NET Code Geeks",
                Articles = GetDotNETGeeksArticles()
            };

            _blogs.Add(_chsakellsBlog);
            _blogs.Add(_dotNetCodeGeeks);

            return _blogs;
        }

        public static List<Article> GetChsakellsArticles()
        {
            List<Article> _articles = new List<Article>();

            Article _oData = new Article()
            {
                Author = "Chris S.",
                Title = "ASP.NET Web API feat. OData",
                URL = "http://chsakell.com/2015/04/04/asp-net-web-api-feat-odata/",
                Contents = @"OData is an open standard protocol allowing the creation and consumption of queryable 
                            and interoperable RESTful APIs. It was initiated by Microsoft and it’s mostly known to
                            .NET Developers from WCF Data Services. There are many other server platforms supporting
                            OData services such as Node.js, PHP, Java and SQL Server Reporting Services. More over, 
                            Web API also supports OData and this post will show you how to integrate those two.."
            };

            Article _wcfCustomSecurity= new Article()
            {
                Author = "Chris S.",
                Title = "Secure WCF Services with custom encrypted tokens",
                URL = "http://chsakell.com/2014/12/13/secure-wcf-services-with-custom-encrypted-tokens/",
                Contents = @"Windows Communication Foundation framework comes with a lot of options out of the box, 
                            concerning the security logic you will apply to your services. Different bindings can be
                            used for certain kind and levels of security. Even the BasicHttpBinding binding supports
                            some types of security. There are some times though where you cannot or don’t want to use
                            WCF security available options and hence, you need to develop your own authentication logic
                            accoarding to your business needs."
            };

            _articles.Add(_oData);
            _articles.Add(_wcfCustomSecurity);

            return _articles;
        }

        public static List<Article> GetDotNETGeeksArticles()
        {
            List<Article> _articles = new List<Article>();

            Article _angularFeatWebAPI = new Article()
            {
                Author = "Gordon Beeming",
                Title = "AngularJS feat. Web API",
                URL = "http://www.dotnetcodegeeks.com/2015/05/angularjs-feat-web-api.html",
                Contents = @"Developing Web applications using AngularJS and Web API can be quite amuzing. You can pick 
                            this architecture in case you have in mind a web application with limitted page refreshes or
                            post backs to the server while each application’s View is based on partial data retrieved from it."
            };

            _articles.Add(_angularFeatWebAPI);

            return _articles;
        }

        public static List<Article> GetAllArticles()
        {
            List<Article> _articles = new List<Article>();
            _articles.AddRange(GetChsakellsArticles());
            _articles.AddRange(GetDotNETGeeksArticles());

            return _articles;
        }
    }
}
