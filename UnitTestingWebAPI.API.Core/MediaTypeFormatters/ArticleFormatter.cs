#region Using
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnitTestingWebAPI.Domain;
#endregion

namespace UnitTestingWebAPI.API.Core.MediaTypeFormatters
{
    public class ArticleFormatter : BufferedMediaTypeFormatter
    {
        public ArticleFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/article"));
        }

        public override bool CanReadType(Type type)
        {
            return false;
        }

        public override bool CanWriteType(Type type)
        {
            //for single article object
            if (type == typeof(Article))
                return true;
            else
            {
                // for multiple article objects
                Type _type = typeof(IEnumerable<Article>);
                return _type.IsAssignableFrom(type);
            }
        }

        public override void WriteToStream(Type type,
                                           object value,
                                           Stream writeStream,
                                           HttpContent content)
        {
            using (StreamWriter writer = new StreamWriter(writeStream))
            {
                var articles = value as IEnumerable<Article>;
                if (articles != null)
                {
                    foreach (var article in articles)
                    {
                        writer.Write(String.Format("[{0},\"{1}\",\"{2}\",\"{3}\",\"{4}\"]",
                                                    article.ID,
                                                    article.Title,
                                                    article.Author,
                                                    article.URL,
                                                    article.Contents));
                    }
                }
                else
                {
                    var _article = value as Article;
                    if (_article == null)
                    {
                        throw new InvalidOperationException("Cannot serialize type");
                    }
                    writer.Write(String.Format("[{0},\"{1}\",\"{2}\",\"{3}\",\"{4}\"]",
                                                    _article.ID,
                                                    _article.Title,
                                                    _article.Author,
                                                    _article.URL,
                                                    _article.Contents));
                }
            }
        }
    }
}
