using Halcyon.HAL;
using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Halcyon.Web.HAL {
    public static class HALResponseExtensions {

        public static IActionResult ToActionResult(this HALResponse model, Controller controller, HttpStatusCode statusCode = HttpStatusCode.OK) {
            return new ObjectResult(model) {
                StatusCode = (int)statusCode
            };
        }
    }
}
