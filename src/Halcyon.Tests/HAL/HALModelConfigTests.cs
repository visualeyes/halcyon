using Halcyon.HAL;
using Halcyon.Tests.HAL.Models;
using Xunit;

namespace Halcyon.Tests.HAL
{
    public class HALModelConfigTests
    {

        [Fact]
        public void Sets_LinkBase(){
            var hal = new HALResponse(new HALModelConfig { LinkBase = "some-link-base" });
            Assert.Equal(hal.Config.LinkBase, "some-link-base");
        }
        [Fact]
        public void Sets_ForceHAL(){
            var hal = new HALResponse(new HALModelConfig { ForceHAL = true });
            Assert.Equal(hal.Config.ForceHAL, true);
        }

        [Fact]
        public void Sets_Config_From_Attribute()
        {
            var model = new PersonModelWithAttributes();
            var hal = new HALResponse(model);
            Assert.Equal(hal.Config.ForceHAL, true);
            Assert.Equal(hal.Config.LinkBase, "~/api");
        }
    }
}
