using Halcyon.HAL;
using Halcyon.Tests.HAL.Models;
using Xunit;

namespace Halcyon.Tests.HAL
{
    public class LinkAttributeTests
    {
        [Fact]
        public void Link_Constructed_From_Attribute()
        {
            var model = new PersonModelWithAttributes();
            var halResponse = new HALResponse(model);

            Assert.Equal(halResponse.HasSelfLink(), true);
        }

        [Fact]
        public void Custom_Link_Constructed_From_Attribute()
        {
            var model = new PersonModelWithAttributes();
            var halResponse = new HALResponse(model);

            Assert.Equal(halResponse.HasLink("person"), true);
        }
    }
}