using Halcyon.HAL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Http.Routing;

namespace Halcyon.WebApi.HAL {
    public static class ApiControllerExtensions {

        public static IHttpActionResult HAL(this ApiController controller, IEnumerable<Link> links, string relativeLinkBase = "~/", HttpStatusCode statuscode = HttpStatusCode.OK) {
            string linkBase = GetLinkBase(controller, relativeLinkBase);

            var hyperMedia = new HALResponse(new HALModelConfig {
                ForceHAL = true, // If we're only returning links, always return them
                LinkBase = linkBase
            });

            hyperMedia.AddLinks(links);

            return hyperMedia.ToActionResult(controller, statuscode);
        }

        public static IHttpActionResult HAL(this ApiController controller, HALResponse hyperMedia, HttpStatusCode statuscode = HttpStatusCode.OK) {
            return hyperMedia.ToActionResult(controller, statuscode);
        }

        public static IHttpActionResult HAL<T>(this ApiController controller, T model, Link link, string relativeLinkBase = "~/", bool addSelfLinkIfNotExists = true, HttpStatusCode statuscode = HttpStatusCode.OK) {
            return controller.HAL(model, new Link[] { link }, relativeLinkBase, addSelfLinkIfNotExists, statuscode);
        }

        public static IHttpActionResult HAL<T>(this ApiController controller, T model, IEnumerable<Link> links, string relativeLinkBase = "~/", bool addSelfLinkIfNotExists = true, HttpStatusCode statuscode = HttpStatusCode.OK) {
            string linkBase = GetLinkBase(controller, relativeLinkBase);

            var response = new HALResponse(model, new HALModelConfig {
                LinkBase = linkBase
            })
            .AddLinks(links);

            if(addSelfLinkIfNotExists) {
                response.AddSelfLinkIfNotExists(controller.Request);
            }

            return new NegotiatedContentResult<HALResponse>(statuscode, response, controller);
        }

        public static IHttpActionResult HAL<T, E>(this ApiController controller, T model, Link modelLink, string embeddedName, IEnumerable<E> embeddedModel, Link embeddedLink, string relativeLinkBase = "~/", HttpStatusCode statuscode = HttpStatusCode.OK) {
            return controller.HAL(model, new Link[] { modelLink }, embeddedName, embeddedModel, new Link[] { embeddedLink }, relativeLinkBase, statuscode);
        }

        public static IHttpActionResult HAL<T, E>(this ApiController controller, T model, Link modelLink, string embeddedName, IEnumerable<E> embeddedModel, IEnumerable<Link> embeddedLinks, string relativeLinkBase = "~/", HttpStatusCode statuscode = HttpStatusCode.OK) {
            return controller.HAL(model, new Link[] { modelLink }, embeddedName, embeddedModel, embeddedLinks, relativeLinkBase, statuscode);
        }

        public static IHttpActionResult HAL<T, E>(this ApiController controller, T model, IEnumerable<Link> modelLinks, string embeddedName, IEnumerable<E> embeddedModel, IEnumerable<Link> embeddedLinks, string relativeLinkBase = "~/", HttpStatusCode statuscode = HttpStatusCode.OK) {
            string linkBase = GetLinkBase(controller, relativeLinkBase);

            var hyperMedia = new HALResponse(model, new HALModelConfig {
                LinkBase = linkBase
            });

            hyperMedia
                .AddLinks(modelLinks)
                .AddEmbeddedCollection(embeddedName, embeddedModel, embeddedLinks);

            return hyperMedia.ToActionResult(controller, statuscode);
        }
        
                
        private static string GetLinkBase(ApiController controller, string relativeLinkBase) {
            string linkBase = null;

            if (!String.IsNullOrWhiteSpace(relativeLinkBase)) {
                linkBase = controller.Url.Content(relativeLinkBase);
            }

            return linkBase;
        }
    }
}