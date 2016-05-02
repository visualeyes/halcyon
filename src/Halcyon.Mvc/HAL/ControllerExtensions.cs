using Halcyon.HAL;
using Microsoft.AspNet.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Web;

namespace Halcyon.Web.HAL {
    public static class ControllerExtensions {

        public static IActionResult HAL(this Controller controller, IEnumerable<Link> links, string relativeLinkBase = "~/", HttpStatusCode statuscode = HttpStatusCode.OK) {
            string linkBase = GetLinkBase(controller, relativeLinkBase);

            var hyperMedia = new HALResponse(new HALModelConfig {
                ForceHAL = true, // If we're only returning links, always return them
                LinkBase = linkBase
            });

            hyperMedia.AddLinks(links);

            return hyperMedia.ToActionResult(controller, statuscode);
        }

        public static IActionResult HAL(this Controller controller, HALResponse hyperMedia, HttpStatusCode statuscode = HttpStatusCode.OK) {
            return hyperMedia.ToActionResult(controller, statuscode);
        }

        public static IActionResult HAL<T>(this Controller controller, T model, Link link, string relativeLinkBase = "~/", HttpStatusCode statuscode = HttpStatusCode.OK) {
            return controller.HAL(model, new Link[] { link }, relativeLinkBase, statuscode);
        }

        public static IActionResult HAL<T>(this Controller controller, T model, IEnumerable<Link> links, string relativeLinkBase = "~/", HttpStatusCode statuscode = HttpStatusCode.OK) {
            if(!links.Any()) {
                return new ObjectResult(model) { StatusCode = (int)statuscode };
            }

            string linkBase = GetLinkBase(controller, relativeLinkBase);

            var hyperMedia = new HALResponse(model, new HALModelConfig {
                LinkBase = linkBase
            })
            .AddLinks(links);

            return hyperMedia.ToActionResult(controller, statuscode);
        }

        public static IActionResult HAL<T, E>(this Controller controller, T model, Link modelLink, string embeddedName, IEnumerable<E> embeddedModel, Link embeddedLink, string relativeLinkBase = "~/", HttpStatusCode statuscode = HttpStatusCode.OK) {
            return controller.HAL(model, new Link[] { modelLink }, embeddedName, embeddedModel, new Link[] { embeddedLink }, relativeLinkBase, statuscode);
        }

        public static IActionResult HAL<T, E>(this Controller controller, T model, Link modelLink, string embeddedName, IEnumerable<E> embeddedModel, IEnumerable<Link> embeddedLinks, string relativeLinkBase = "~/", HttpStatusCode statuscode = HttpStatusCode.OK) {
            return controller.HAL(model, new Link[] { modelLink }, embeddedName, embeddedModel, embeddedLinks, relativeLinkBase, statuscode);
        }

        public static IActionResult HAL<T, E>(this Controller controller, T model, IEnumerable<Link> modelLinks, string embeddedName, IEnumerable<E> embeddedModel, IEnumerable<Link> embeddedLinks, string relativeLinkBase = "~/", HttpStatusCode statuscode = HttpStatusCode.OK) {
            string linkBase = GetLinkBase(controller, relativeLinkBase);

            var hyperMedia = new HALResponse(model, new HALModelConfig {
                LinkBase = linkBase
            });

            hyperMedia
                .AddLinks(modelLinks)
                .AddEmbeddedCollection(embeddedName, embeddedModel, embeddedLinks);

            return hyperMedia.ToActionResult(controller, statuscode);
        }


        private static string GetLinkBase(Controller controller, string relativeLinkBase) {
            string linkBase = null;

            if(!String.IsNullOrWhiteSpace(relativeLinkBase)) {
                linkBase = controller.Url.Content(relativeLinkBase);
            }

            return linkBase;
        }
    }
}