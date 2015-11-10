using Halcyon.HAL;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Http.Routing;
using Xunit;

namespace Halcyon.Tests.HAL {
    public class ControllerExtensionsTests {
        private readonly ApiController controller;
        private readonly Mock<ApiController> mockController;
        private readonly Mock<UrlHelper> mockUrl;

        private const string TestUrlBase = "http://localhost/";

        public ControllerExtensionsTests() {
            this.mockController = new Mock<ApiController>();
            this.mockUrl = new Mock<UrlHelper>();
                        
            this.mockUrl.Setup(u => u.Content(It.IsAny<string>())).Returns<string>((url) => {
                return url.Replace("~/", TestUrlBase);
            });

            this.controller = this.mockController.Object;
            this.controller.Url = this.mockUrl.Object;
        }

        [Theory]
        [InlineData(HttpStatusCode.OK, null, null)]
        [InlineData(HttpStatusCode.OK, "", null)]
        [InlineData(HttpStatusCode.OK, "~/api", TestUrlBase + "api")]
        [InlineData(HttpStatusCode.BadGateway, null, null)]
        public void Links_Only_Response(HttpStatusCode statusCode, string linkBase, string expectedLinkBase) {
            var links = new[] {
                new Link("self", "/api/foo")
            };

            var result = this.controller.HAL(links, relativeLinkBase: linkBase, statuscode: statusCode) as NegotiatedContentResult<HALResponse>;

            AssertHalModelResult(statusCode, expectedLinkBase, true, result);
        }

        [Theory]
        [InlineData(HttpStatusCode.OK, null, null)]
        [InlineData(HttpStatusCode.OK, "", null)]
        [InlineData(HttpStatusCode.OK, "~/api", TestUrlBase + "api")]
        [InlineData(HttpStatusCode.BadGateway, null, null)]
        public void Model_With_Links(HttpStatusCode statusCode, string linkBase, string expectedLinkBase) {
            var testModel = new {
                one = 1
            };

            var links = new[] {
                new Link("self", "/api/foo")
            };

            var result = this.controller.HAL(testModel, links, relativeLinkBase: linkBase, statuscode: statusCode) as NegotiatedContentResult<HALResponse>;

            AssertHalModelResult(statusCode, expectedLinkBase, false, result);
        }

        [Theory]
        [InlineData(HttpStatusCode.OK, null, null)]
        [InlineData(HttpStatusCode.OK, "", null)]
        [InlineData(HttpStatusCode.OK, "~/api", TestUrlBase + "api")]
        [InlineData(HttpStatusCode.BadGateway, null, null)]
        public void Model_With_Link(HttpStatusCode statusCode, string linkBase, string expectedLinkBase) {
            var testModel = new {
                one = 1
            };

            var link = new Link("self", "/api/foo");

            var result = this.controller.HAL(testModel, link, relativeLinkBase: linkBase, statuscode: statusCode) as NegotiatedContentResult<HALResponse>;

            AssertHalModelResult(statusCode, expectedLinkBase, false, result);
        }

        [Theory]
        [InlineData(HttpStatusCode.OK, true, null)]
        [InlineData(HttpStatusCode.OK, false, null)]
        [InlineData(HttpStatusCode.OK, false, TestUrlBase + "api")]
        [InlineData(HttpStatusCode.BadGateway, false, null)]
        public void Hal_Model(HttpStatusCode statusCode, bool forceHal, string linkBase) {
            
        
            var halModel = new HALResponse(new HALModelConfig {
                ForceHAL = forceHal,
                LinkBase = linkBase
            });
            
            var result = this.controller.HAL(halModel, statuscode: statusCode) as NegotiatedContentResult<HALResponse>;

            AssertHalModelResult(statusCode, linkBase, forceHal, result);
        }

        [Theory]
        [InlineData(HttpStatusCode.OK, null, null)]
        [InlineData(HttpStatusCode.OK, "", null)]
        [InlineData(HttpStatusCode.OK, "~/api", TestUrlBase + "api")]
        [InlineData(HttpStatusCode.BadGateway, null, null)]
        public void Model_With_Link_And_Embedded_And_EmbeddedLink(HttpStatusCode statusCode, string linkBase, string expectedLinkBase) {
            var testModel = new {
                one = 1
            };

            var link = new Link("self", "/api/foo");

            string embeddedKey = "test-embedded";
            var embedded = new[] {
                new { number = 1 },
                new { number = 2 }
            };

            var embeddedLink = new Link("self", "/api/foo");

            var result = this.controller.HAL(
                testModel,
                link,
                embeddedKey,
                embedded,
                embeddedLink,
                relativeLinkBase: linkBase,
                statuscode: statusCode
            ) as NegotiatedContentResult<HALResponse>;

            AssertHalModelResult(statusCode, expectedLinkBase, false, result);

            var model = result.Content;
        }

        [Theory]
        [InlineData(HttpStatusCode.OK, null, null)]
        [InlineData(HttpStatusCode.OK, "", null)]
        [InlineData(HttpStatusCode.OK, "~/api", TestUrlBase + "api")]
        [InlineData(HttpStatusCode.BadGateway, null, null)]
        public void Model_With_Link_And_Embedded_And_EmbeddedLinks(HttpStatusCode statusCode, string linkBase, string expectedLinkBase) {
            var testModel = new {
                one = 1
            };

            var link = new Link("self", "/api/foo");

            string embeddedKey = "test-embedded";
            var embedded = new[] {
                new { number = 1 },
                new { number = 2 }
            };

            var embeddedLinks = new[] {
                new Link("self", "/api/foo")
            };

            var result = this.controller.HAL(
                testModel,
                link,
                embeddedKey,
                embedded,
                embeddedLinks,
                relativeLinkBase: linkBase,
                statuscode: statusCode
            ) as NegotiatedContentResult<HALResponse>;

            AssertHalModelResult(statusCode, expectedLinkBase, false, result);

            var model = result.Content;
        }

        [Theory]
        [InlineData(HttpStatusCode.OK, null, null)]
        [InlineData(HttpStatusCode.OK, "", null)]
        [InlineData(HttpStatusCode.OK, "~/api", TestUrlBase + "api")]
        [InlineData(HttpStatusCode.BadGateway, null, null)]
        public void Model_With_Links_And_Embedded_And_EmbeddedLinks(HttpStatusCode statusCode, string linkBase, string expectedLinkBase) {
            var testModel = new {
                one = 1
            };

            var links = new[] {
                new Link("self", "/api/foo")
            };

            string embeddedKey = "test-embedded";
            var embedded = new[] {
                new { number = 1 },
                new { number = 2 }
            };

            var embeddedLinks = new[] {
                new Link("self", "/api/foo")
            };

            var result = this.controller.HAL(
                testModel, 
                links,
                embeddedKey,
                embedded,
                embeddedLinks,
                relativeLinkBase: linkBase, 
                statuscode: statusCode
            ) as NegotiatedContentResult<HALResponse>;

            AssertHalModelResult(statusCode, expectedLinkBase, false, result);
            
            var model = result.Content;
        }

        private static void AssertHalModelResult(HttpStatusCode statusCode, string expectedLinkBase, bool forceHal, NegotiatedContentResult<HALResponse> result) {
            Assert.NotNull(result);
            Assert.Equal(statusCode, result.StatusCode);

            var model = result.Content;

            Assert.NotNull(model);
            Assert.NotNull(model.Config);
            Assert.Equal(forceHal, model.Config.ForceHAL);
            Assert.Equal(expectedLinkBase, model.Config.LinkBase);
        }
    }
}
