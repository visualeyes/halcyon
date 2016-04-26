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
            var converter = new HALAttributeConverter();
            var halResponse = new HALResponse(model);

            halResponse = converter.Convert(halResponse);

            Assert.True(halResponse.HasSelfLink());
        }

        [Fact]
        public void Custom_Link_Constructed_From_Attribute()
        {
            var model = new PersonModelWithAttributes();
            var converter = new HALAttributeConverter();
            var halResponse = new HALResponse(model);

            halResponse = converter.Convert(halResponse);

            Assert.True(halResponse.HasLink("person"));
        }
    }
}