
using System;
using Starcounter;

namespace StarcounterApplication46
{
    [Database]
    public class Mail { }

    class MailApplication : StatefulWebApplication
    {
        public string Title { get; set; }
        public Json Focused { get; set; }
        
        static void Main()
        {
            var app = new MailApplication();
            app.Title = "Inbox";

            app.Use(new HtmlFromJsonProvider());
            app.Use(new PartialToStandaloneHtmlProvider());

            // Web interface entrypoint. By convention, always the
            // first handler registered in Main(), and must return the
            // app itself. The runtime will react on the fact that we are
            // returning an instance of StatefulWebApplication
            // and this is what triggers use of Session, patching,
            // client-server synchronization, etc (as opposed to
            // raw "Handle.GET", that is stateless).

            // www.mails.com
            app.GET("/", () => {
                app.Focused.Data = Db.SQL<Mail>("SELECT m FROM Mail m").First;
                return app;
            });

            // When using stateful applications, utilizing patching,
            // all access goes via the app instance, not the global,
            // stateless Handle.GET.
            // Using a certain app instance ("Partials") have the benefit of
            // that we can introduce special code regarding blending there, and
            // also make it very obvious what blending points any application
            // actually expose (for other applications to blend with).
            // Would eliminate need for stuff such as new HandlerOptions() { SelfOnly = true }

            // www.mails.com/mails/123
            app.Partials.GET("/mails/{?}", (string id) => {
                // We morph the app and return it.
                app.Focused.Data = Db.SQL<Mail>("SELECT m FROM Mail m WHERE ObjectID=?", id).First;
                return app;
            });

            // Finally, if Self.GET is still needed (probably), the proposed API
            // is `app.GetPartial`, no longer forcing /partial in the URI and removing
            // the need for an additionl "Self" class

            var partial = app.GetPartial("/settings");
        }
    }
}