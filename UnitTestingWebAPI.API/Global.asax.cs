using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using System.Web.SessionState;
using UnitTestingWebAPI.Data;

namespace UnitTestingWebAPI.API
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            // Init database
            System.Data.Entity.Database.SetInitializer(new BloggerInitializer());
        }
    }
}