using Halcyon.HAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

namespace Halcyon.WebApi.HAL {
    public static class HALResponseExtensions {

        public static HALResponse AddSelfLinkIfNotExists(this HALResponse response, HttpRequestMessage request) {
            if(!response.HasSelfLink()) {
                response.AddSelfLink(request);
            }

            return response;
        }

        public static HALResponse AddSelfLink(this HALResponse response, HttpRequestMessage request) {
            var selfLink = new Link(Link.RelForSelf, request.RequestUri.LocalPath, method: request.Method.Method);
            response.AddLinks(selfLink);
            return response;
        }

        public static IHttpActionResult ToActionResult(this HALResponse model, ApiController controller, HttpStatusCode statusCode = HttpStatusCode.OK) {
            return new NegotiatedContentResult<HALResponse>(statusCode, model, controller);
        }
    }
}
