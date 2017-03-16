using Halcyon.HAL;
using Halcyon.HAL.Attributes;
using Halcyon.Tests.HAL.Models;
using Xunit;

namespace Halcyon.Tests.HAL {
    public class HALAttributeConverterTests {
        [Fact]
        public void Should_not_be_able_to_convert_HalReponse_with_attribute_marked_model() {
            var model = new PersonModelWithAttributes();
            var halResponse = new HALResponse(model);
            var converter = new HALAttributeConverter();

            Assert.False(converter.CanConvert(halResponse.GetType()));
        }

        [Fact]
        public void Should_be_able_to_convert_attribute_marked_model() {
            var model = new PersonModelWithAttributes();
            var converter = new HALAttributeConverter();

            Assert.True(converter.CanConvert(model.GetType()));
        }

        [Fact]
        public void Should_not_be_able_to_convert_non_attribute_marked_model() {
            var model = new object();
            var converter = new HALAttributeConverter();

            Assert.False(converter.CanConvert(model.GetType()));
        }

        [Fact]
        public void Should_use_default_model_config() {
            var model = new HalModelNoConfig();
            var defaultConfig = new HALModelConfig {
                LinkBase = "foo",
                ForceHAL = false
            };
            var converter = new HALAttributeConverter(defaultConfig);

            Assert.True(converter.CanConvert(model.GetType()));
            Assert.Equal(defaultConfig, converter.Convert(model).Config);
        }

        [Fact]
        public void Should_use_model_config_from_attribute()
        {
            var model = new HalModelWithConfig();
            var defaultConfig = new HALModelConfig
            {
                LinkBase = "foo",
                ForceHAL = false
            };
            var converter = new HALAttributeConverter(defaultConfig);

            Assert.True(converter.CanConvert(model.GetType()));
            var modelConfig = converter.Convert(model).Config;
            Assert.NotNull(modelConfig);
            Assert.NotEqual(defaultConfig, modelConfig);
        }
    }

    [HalModel]
    class HalModelNoConfig {}

    [HalModel("bar", true)]
    class HalModelWithConfig {}
}