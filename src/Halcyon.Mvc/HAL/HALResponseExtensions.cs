using Halcyon.HAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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
        
        public static IActionResult ToActionResult(this HALResponse model, ControllerBase controller, HttpStatusCode statusCode = HttpStatusCode.OK) {
            return new ObjectResult(model) {
                StatusCode = (int)statusCode
            };
        }
    }
}
