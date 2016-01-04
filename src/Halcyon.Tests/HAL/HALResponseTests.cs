using Halcyon.HAL;
using Halcyon.Tests.HAL.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Halcyon.Tests.HAL {
    public class HALResponseTests {
        private const string TestHalProperties = "\"_links\":{\"self\":{\"href\":\"href\"}},\"_embedded\":{\"bars\":[{\"bar\":true}]}";

        [Theory]
        [MemberData("GetTestModels")]
        public void To_JObject(object model, Link link, string embeddedName, object[] embedded, string expected) {
            var serializer = new JsonSerializer();

            var response = new HALResponse(model)
                .AddLinks(link)
                .AddEmbeddedCollection(embeddedName, embedded);

            var jObject = response.ToJObject(serializer);

            string actual = jObject.ToString(Formatting.None);
            Assert.Equal(expected, actual);
        }
        
        public static object[] GetTestModels() {
            var personModel = PersonModel.GetTestModel();
            var expectedPersonJson = "{" + PersonModel.TestModelJson + "," + TestHalProperties + "}";

            var link = new Link("self", "href");
            string embeddedName = "bars";
            var embedded = new[] {
                new { bar = true }
            };

            return new object[] {
                new object[] { personModel, link, embeddedName, embedded, expectedPersonJson },
                new object[] { JObject.FromObject(personModel), link, embeddedName, embedded, expectedPersonJson }
            };
        }
    }
}

