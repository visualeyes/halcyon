using Halcyon.HAL;
using Halcyon.Tests.HAL.Models;
using Xunit;

namespace Halcyon.Tests.HAL
{
    public class HALAttributeConverterTests
    {
        [Fact]
        public void Should_be_able_to_convert_HalReponse_with_attribute_marked_model()
        {
            var model = new PersonModelWithAttributes();
            var halResponse = new HALResponse(model);
            var converter = new HALAttributeConverter();

            Assert.True(converter.CanConvert(halResponse.GetType(), halResponse));
        }

        [Fact]
        public void Should_be_able_to_convert_attribute_marked_model()
        {
            var model = new PersonModelWithAttributes();
            var converter = new HALAttributeConverter();

            Assert.True(converter.CanConvert(model.GetType(), model));
        }

        [Fact]
        public void Should_not_be_able_to_convert_non_attribute_marked_model()
        {
            var model = new object();
            var converter = new HALAttributeConverter();

            Assert.False(converter.CanConvert(model.GetType(), model));
        }
    }
}