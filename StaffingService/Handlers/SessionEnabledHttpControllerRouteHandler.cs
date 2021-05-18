using System;
using System.Collections.Generic;
using System.Linq;
using System.Dynamic;
using System.Web.Routing;
using System.Data;
using System.ComponentModel;
using System.Text;
using System.Configuration;
using System.Web.Http.WebHost;
using System.Web.SessionState;
using System.Web;
namespace WorkFlow.Handler
{

    public class SessionEnabledHttpControllerRouteHandler : HttpControllerRouteHandler
    {
        protected override IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new SessionEnabledControllerHandler(requestContext.RouteData);
        }
    }
}