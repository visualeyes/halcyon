using Halcyon.HAL;
using Halcyon.WebApi.HAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Halcyon.Tests.SelfHost.WebApi.Controllers {
    [RoutePrefix("api/foo")]
    public class FooController : ApiController {

        [HttpGet, Route("{id:int}")]
        public IHttpActionResult Get(int id) {
            // Any plain old object will do
            var fooModel = new {
                id = id,
                type = "foo"
            };

            // Return a simple resource with links to related resources
            return this.HAL(fooModel, new Link[] {
                new Link("foo:bar", "/api/foo/{id}/bar", replaceParameters: false)
            });
        }

        [HttpGet, Route("{fooId:int}/bars")]
        public IHttpActionResult GetBar(int fooId) {
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
                    new Link("other", "/api/foo/{fooId}/bar", replaceParameters: false)
                })
                .AddEmbeddedResource("baz", new { name = "bazza" }, new Link[] {
                    new Link("a", "test")
                })
                .AddEmbeddedCollection("bars", bars, new Link[] {
                    new Link("self", "/api/bar/{id}")
                });

            return this.Ok(response);
        }
    }
}
