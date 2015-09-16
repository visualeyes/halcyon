using Halcyon.HAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HalcyonExample.Controllers {
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
                new Link("self", "/api/foo/{id}"),
                new Link("foo:bar", "/api/foo/{id}/bar")
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
            return this.HAL(
                fooBarModel,
                new Link[] {
                    new Link("self", "/api/foo/{fooId}/bar")
                },
                "bars",
                bars,
                new Link[] {
                    new Link("self", "/api/bar/{id}")
                }
            );
        }
    }
}

