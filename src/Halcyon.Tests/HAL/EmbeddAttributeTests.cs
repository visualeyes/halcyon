using Halcyon.HAL;
using Halcyon.Tests.HAL.Models;
using Newtonsoft.Json;
using Xunit;

namespace Halcyon.Tests.HAL
{
    public class EmbeddAttributeTests
    {
        [Fact]
        public void Embedded_Enumerable_Constructed_From_Attribute()
        {
            var model = new PersonModelWithAttributes();
            model.Pets.Add(new Pet { Id = 1, Name = "Fido" });
            var halResponse = new HALResponse(model);
            var serializer = new JsonSerializer();
            var jObject = halResponse.ToJObject(serializer);

            var embedded = jObject["_embedded"]["pets"][0];
            Assert.Equal("Fido", embedded["Name"]);
            Assert.Equal("1", embedded["Id"]);
        }

        [Fact]
        public void Embedded_Single_Property_Constructed_From_Attribute()
        {
            var model = new PersonModelWithAttributes();
            var halResponse = new HALResponse(model);
            var serializer = new JsonSerializer();

            var jObject = halResponse.ToJObject(serializer);

            var embedded = jObject["_embedded"]["favouritePet"][0];
            Assert.Equal("Benji", embedded["Name"]);
            Assert.Equal("0", embedded["Id"]);
        }
    }
}