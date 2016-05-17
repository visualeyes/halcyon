using Halcyon.HAL;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Routing;
using Microsoft.AspNet.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Halcyon.Web.HAL {
    public static class HALResponseExtensions {

        public static HALResponse AddSelfLinkIfNotExists(this HALResponse response, HttpRequest request) {
            if(!response.HasSelfLink()) {
                response.AddSelfLink(request);
            }

            return response;
        }

        public static HALResponse AddSelfLink(this HALResponse response, HttpRequest request) {
            var selfLink = new Link(Link.RelForSelf, request.Path, method: request.Method);
            response.AddLinks(selfLink);
            return response;
        }
        
        public static IActionResult ToActionResult(this HALResponse model, Controller controller, HttpStatusCode statusCode = HttpStatusCode.OK) {
            return new ObjectResult(model) {
                StatusCode = (int)statusCode
            };
        }
    }
}
