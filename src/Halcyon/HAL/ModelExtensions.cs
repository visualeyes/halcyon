using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Halcyon.HAL {
    public static class ModelExtensions {

        public static HALModel AddLinks(this HALModel halModel, params Link[] links) {
            return halModel.AddLinks(links);
        }

        public static HALModel AddEmbeddedCollection<T>(this HALModel hyperMedia, string collectionName, IEnumerable<T> model, IEnumerable<Link> links = null) {
            if(links == null) {
                links = Enumerable.Empty<Link>();
            }

            var embedded = model
                            .Select(m => new HALModel(m, hyperMedia.Config).AddLinks(links))
                            .ToArray();

            hyperMedia.AddEmbeddedCollection(collectionName, embedded);

            return hyperMedia;
        }
    }
}
