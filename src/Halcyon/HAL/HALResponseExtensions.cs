using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Halcyon.HAL {
    public static class HALResponseExtensions {
        public static bool HasSelfLink(this HALResponse response) {
            return response.HasLink(Link.RelForSelf);
        }

        public static HALResponse AddLinks(this HALResponse halModel, params Link[] links) {
            return halModel.AddLinks(links);
        }

        public static HALResponse AddEmbeddedResource<T>(this HALResponse hyperMedia, string resourceName, T model, IEnumerable<Link> links = null) {
            if(links == null) {
                links = Enumerable.Empty<Link>();
            }

            var embedded = new HALResponse(model, hyperMedia.Config).AddLinks(links);
            hyperMedia.AddEmbeddedResource(resourceName, embedded);

            return hyperMedia;
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

        public static HALResponse AddEmbeddedResources<T>(this HALResponse response, IEnumerable<KeyValuePair<string, T>> resources) {
            foreach(var resource in resources) {
                response.AddEmbeddedResource(resource.Key, resource.Value);
            }

            return response;
        }

        public static HALResponse AddEmbeddedCollections<T>(this HALResponse response, IEnumerable<KeyValuePair<string, IEnumerable<T>>> embeddedCollections) {
            foreach(var embeddedCollection in embeddedCollections) {
                response.AddEmbeddedCollection(embeddedCollection.Key, embeddedCollection.Value);
            }

            return response;
        }

        public static HALResponse AddEmbeddedCollections(this HALResponse response, IEnumerable<KeyValuePair<string, IEnumerable<HALResponse>>> embeddedCollections) {

            foreach(var embeddedCollection in embeddedCollections) {
                response.AddEmbeddedCollection(embeddedCollection.Key, embeddedCollection.Value);
            }

            return response;
        }
    }
}
