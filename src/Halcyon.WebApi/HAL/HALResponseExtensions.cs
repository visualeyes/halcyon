using Halcyon.HAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

namespace Halcyon.WebApi.HAL {
    public static class HALResponseExtensions {

        public static IHttpActionResult ToActionResult(this HALResponse model, ApiController controller, HttpStatusCode statusCode = HttpStatusCode.OK) {
            return new NegotiatedContentResult<HALResponse>(statusCode, model, controller);
        }
    }
}
