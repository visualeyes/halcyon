using Halcyon.HAL;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;

namespace Halcyon.Web.HAL {
    public static class ControllerExtensions {

        public static IActionResult HAL(this ControllerBase controller, IEnumerable<Link> links, string relativeLinkBase = "~/", HttpStatusCode statuscode = HttpStatusCode.OK) {
            string linkBase = GetLinkBase(controller, relativeLinkBase);

            var hyperMedia = new HALResponse(new HALModelConfig {
                ForceHAL = true, // If we're only returning links, always return them
                LinkBase = linkBase
            });

            hyperMedia.AddLinks(links);

            return hyperMedia.ToActionResult(controller, statuscode);
        }

        public static IActionResult HAL(this ControllerBase controller, HALResponse hyperMedia, HttpStatusCode statuscode = HttpStatusCode.OK) {
            return hyperMedia.ToActionResult(controller, statuscode);
        }

        public static IActionResult HAL<T>(this ControllerBase controller, T model, Link link, string relativeLinkBase = "~/", bool addSelfLinkIfNotExists = true, HttpStatusCode statuscode = HttpStatusCode.OK) {
            return controller.HAL(model, new Link[] { link }, relativeLinkBase, addSelfLinkIfNotExists, statuscode);
        }

        public static IActionResult HAL<T>(this ControllerBase controller, T model, IEnumerable<Link> links, string relativeLinkBase = "~/", bool addSelfLinkIfNotExists = true, HttpStatusCode statuscode = HttpStatusCode.OK) {
            string linkBase = GetLinkBase(controller, relativeLinkBase);

            var response = new HALResponse(model, new HALModelConfig {
                LinkBase = linkBase
            })
            .AddLinks(links);

            if(addSelfLinkIfNotExists) {
                response.AddSelfLinkIfNotExists(controller.Request);
            }

            return response.ToActionResult(controller, statuscode);
        }

        public static IActionResult HAL<T, E>(this ControllerBase controller, T model, Link modelLink, string embeddedName, IEnumerable<E> embeddedModel, Link embeddedLink, string relativeLinkBase = "~/", HttpStatusCode statuscode = HttpStatusCode.OK) {
            return controller.HAL(model, new Link[] { modelLink }, embeddedName, embeddedModel, new Link[] { embeddedLink }, relativeLinkBase, statuscode);
        }

        public static IActionResult HAL<T, E>(this ControllerBase controller, T model, Link modelLink, string embeddedName, IEnumerable<E> embeddedModel, IEnumerable<Link> embeddedLinks, string relativeLinkBase = "~/", HttpStatusCode statuscode = HttpStatusCode.OK) {
            return controller.HAL(model, new Link[] { modelLink }, embeddedName, embeddedModel, embeddedLinks, relativeLinkBase, statuscode);
        }

        public static IActionResult HAL<T, E>(this ControllerBase controller, T model, IEnumerable<Link> modelLinks, string embeddedName, IEnumerable<E> embeddedModel, IEnumerable<Link> embeddedLinks, string relativeLinkBase = "~/", HttpStatusCode statuscode = HttpStatusCode.OK) {
            string linkBase = GetLinkBase(controller, relativeLinkBase);

            var hyperMedia = new HALResponse(model, new HALModelConfig {
                LinkBase = linkBase
            });

            hyperMedia
                .AddLinks(modelLinks)
                .AddEmbeddedCollection(embeddedName, embeddedModel, embeddedLinks);

            return hyperMedia.ToActionResult(controller, statuscode);
        }


        private static string GetLinkBase(ControllerBase controller, string relativeLinkBase) {
            string linkBase = null;

            if(!String.IsNullOrWhiteSpace(relativeLinkBase)) {
                linkBase = controller.Url.Content(relativeLinkBase);
            }

            return linkBase;
        }
    }
}