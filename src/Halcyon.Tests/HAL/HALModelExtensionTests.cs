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
    public class HALModelExtensionTests {

        private readonly ApiController controller;
        private readonly Mock<ApiController> mockController;
        private readonly Mock<UrlHelper> mockUrl;

        private const string TestUrlBase = "http://localhost/";

        public HALModelExtensionTests() {
            this.mockController = new Mock<ApiController>();
            this.mockUrl = new Mock<UrlHelper>();

            this.mockUrl.Setup(u => u.Content(It.IsAny<string>())).Returns<string>((url) => {
                return url.Replace("~/", TestUrlBase);
            });

            this.controller = this.mockController.Object;
            this.controller.Url = this.mockUrl.Object;
        }

        [Theory]
        [InlineData(HttpStatusCode.OK)]
        [InlineData(HttpStatusCode.BadRequest)]
        public void To_Action_Result(HttpStatusCode statuscode) {
            var halModel = new HALResponse(null);

            var result = halModel.ToActionResult(this.controller, statuscode) as NegotiatedContentResult<HALResponse>;

            Assert.NotNull(result);
            Assert.Equal(statuscode, result.StatusCode);

            var response = result.Content;

            Assert.NotNull(response);
            Assert.Same(halModel, response);
        }
    }
}
