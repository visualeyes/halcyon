using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Halcyon.Web.HAL;
using Halcyon.HAL;

namespace Halcyon.Tests.SelfHost.Mvc.Controllers {
    [Route("api/[controller]")]
    public class FooController : Controller {

        [HttpGet("{id:int}")]
        public IActionResult Get(int id) {
            // Any plain old object will do
            var fooModel = new {
                id,
                type = "foo"
            };

            // Return a simple resource with links to related resources
            return this.HAL(fooModel, new Link[] {
                new Link("self", "/api/foo/{id}"),
                new Link("foo:bar", "/api/foo/{id}/bar")
            });
        }

        [HttpGet("{fooId:int}/bars")]
        public IActionResult GetBar(int fooId) {
            // A collection of bars related to foo
            var bars = new List<object> {
                new { id = 1, fooId = fooId, type = "bar" },
                new { id = 2, fooId = fooId, type = "bar" }
            };

            // data about the bars related to foo
            var fooBarModel = new {
                fooId = fooId,
                count = bars.Count
            };

            // Return a fooBar resource with embedded bars
            var response = new HALResponse(fooBarModel)
                .AddSelfLink(this.Request)
                .AddLinks(new Link[] {
                    new Link("self", "/api/foo/{fooId}/bar")
                })
                .AddEmbeddedCollection("bars", bars, new Link[] {
                    new Link("self", "/api/bar/{id}")
                });

            return this.Ok(response);
        }
    }
}
