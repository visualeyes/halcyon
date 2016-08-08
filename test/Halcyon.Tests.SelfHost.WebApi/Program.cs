using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Halcyon.Tests.SelfHost.WebApi {
    class Program {
        static void Main(string[] args) {
            string url = "http://localhost:5000";

            var server = WebApp.Start(url, app => {
                HttpConfiguration config = new HttpConfiguration();
                config.MapHttpAttributeRoutes();

                app.UseWebApi(config);
            });

            Console.WriteLine("Web API listening at " + url);

            Console.ReadLine();
        }
    }
}
