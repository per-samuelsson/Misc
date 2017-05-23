
using System;

namespace Starcounter
{
    public class StatefulWebApplication : Json
    {
        public PartialHandlers Partials => new PartialHandlers();

        public class PartialHandlers
        {
            public void GET(string uri, Func<string, Response> response)
            {
                Handle.GET(uri, response);
            }
        }

        public void Use(IMiddleware middleware)
        {
            Application.Current.Use(middleware);
        }

        public void GET(string uri, Func<Response> response)
        {
            Handle.GET(uri, response);
        }
    }
}
