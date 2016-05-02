using Halcyon.HAL;
using Halcyon.HAL.Attributes;
using Halcyon.Tests.HAL.Models;
using Xunit;

namespace Halcyon.Tests.HAL {
    public class LinkAttributeTests {
        [Fact]
        public void Link_Constructed_From_Attribute() {
            var model = new PersonModelWithAttributes();
            var converter = new HALAttributeConverter();

            var halResponse = converter.Convert(model);

            Assert.True(halResponse.HasSelfLink());
        }

        [Fact]
        public void Custom_Link_Constructed_From_Attribute() {
            var model = new PersonModelWithAttributes();
            var converter = new HALAttributeConverter();

            var halResponse = converter.Convert(model);

            Assert.True(halResponse.HasLink("person"));
        }
    }
}