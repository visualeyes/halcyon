using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

namespace Halcyon.HAL {
    public static class HALResponseExtensions {

        public static IHttpActionResult ToActionResult(this HALResponse model, ApiController controller, HttpStatusCode statusCode = HttpStatusCode.OK) {
            return new NegotiatedContentResult<HALResponse>(statusCode, model, controller);
        }

        public static HALResponse AddLinks(this HALResponse halModel, params Link[] links) {
            return halModel.AddLinks(links);
        }

        public static HALResponse AddEmbeddedCollection<T>(this HALResponse hyperMedia, string collectionName, IEnumerable<T> model, IEnumerable<Link> links = null) {
            if(links == null) {
                links = Enumerable.Empty<Link>();
            }

            var embedded = model
                            .Select(m => new HALResponse(m, hyperMedia.Config).AddLinks(links))
                            .ToArray();

            hyperMedia.AddEmbeddedCollection(collectionName, embedded);

            return hyperMedia;
        }
    }
}
