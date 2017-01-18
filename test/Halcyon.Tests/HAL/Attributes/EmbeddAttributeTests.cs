using Halcyon.HAL;
using Halcyon.HAL.Attributes;
using Halcyon.Tests.HAL.Models;
using Newtonsoft.Json;
using Xunit;

namespace Halcyon.Tests.HAL {
    public class EmbeddAttributeTests {
        [Fact]
        public void Embedded_Enumerable_Constructed_From_Attribute() {
            var model = new PersonModelWithAttributes();
            model.Pets.Add(new Pet { Id = 1, Name = "Fido" });
            var converter = new HALAttributeConverter();

            var halResponse = converter.Convert(model);
            var serializer = new JsonSerializer();
            var jObject = halResponse.ToJObject(serializer);

            var embedded = jObject["_embedded"]["pets"][0];
            var embeddedSelfLink = embedded["_links"]["self"];

            Assert.Equal("Fido", embedded["Name"]);
            Assert.Equal("1", embedded["Id"]);
            Assert.Equal("~/api/pets/1", embeddedSelfLink["href"].ToString());
        }

        [Fact]
        public void Embedded_Single_Property_Constructed_From_Attribute() {
            var model = new PersonModelWithAttributes();
            var converter = new HALAttributeConverter();

            var halResponse = converter.Convert(model);
            var serializer = new JsonSerializer();

            var jObject = halResponse.ToJObject(serializer);

            var embedded = jObject["_embedded"]["favouritePet"];
            Assert.Equal("Benji", embedded["Name"]);
            Assert.Equal("0", embedded["Id"]);
        }

        [Fact]
        public void Embedded_Single_Property_With_Embedded_Collection_Constructed_From_Attribute()
        {
            var model = new PersonModelWithAttributes();
            var converter = new HALAttributeConverter();

            var halResponse = converter.Convert(model);
            var serializer = new JsonSerializer();

            var jObject = halResponse.ToJObject(serializer);

            var embedded = jObject["_embedded"]["favouritePet"]["_embedded"]["toys"][0];
            Assert.Equal("Rubber Bone", embedded["Name"]);
        }

        [Fact]
        public void Paged_Links_Constructed_From_Attributes() {
            var model = new PagedPeopleModelWithAttributes() {
                PageIndex = 5
            };

            var converter = new HALAttributeConverter();

            var halResponse = converter.Convert(model);
            var serializer = new JsonSerializer();

            var jObject = halResponse.ToJObject(serializer);

            var links = jObject["_links"];

            Assert.Equal("~/api/person?index=5", links["self"]["href"]);
        }
    }
}