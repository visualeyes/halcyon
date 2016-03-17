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
        private const string TestHalProperties = "\"_links\":{\"self\":{\"href\":\"one\"}},\"_embedded\":{\"bars\":[{\"bar\":true}]}";
        private const string TestHalPropertiesLinkArray = "\"_links\":{\"self\":[{\"href\":\"one\"},{\"href\":\"two\"}]},\"_embedded\":{\"bars\":[{\"bar\":true}]}";


        [Theory]
        [MemberData("GetCollectionModels")]
        public void Hal_Model_Doesnt_Accept_Collections(object dto) {
            Assert.Throws<ArgumentException>("model", () => {
                new HALResponse(dto);
            });
        }

        [Theory]
        [MemberData("GetTestModels")]
        public void To_JObject(object model, Link linkOne, Link linkTwo, string embeddedName, object[] embedded) {
            string expected = GetExpectedJson(false);

            var serializer = new JsonSerializer();

            var response = new HALResponse(model)
                .AddLinks(linkOne)
                .AddEmbeddedCollection(embeddedName, embedded);

            var jObject = response.ToJObject(serializer);

            string actual = jObject.ToString(Formatting.None);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData("GetTestModels")]
        public void To_JObject_Link_Array(object model, Link linkOne, Link linkTwo, string embeddedName, object[] embedded) {
            string expected = GetExpectedJson(true);

            var serializer = new JsonSerializer();

            var response = new HALResponse(model)
                .AddLinks(linkOne)
                .AddLinks(linkTwo)
                .AddEmbeddedCollection(embeddedName, embedded);

            var jObject = response.ToJObject(serializer);

            string actual = jObject.ToString(Formatting.None);
            Assert.Equal(expected, actual);
        }

        private static string GetExpectedJson(bool linkArray) {
            string testJson = linkArray ? TestHalPropertiesLinkArray : TestHalProperties;
            var expectedPersonJson = "{" + PersonModel.TestModelJson + "," + testJson + "}";
            return expectedPersonJson;
        }

        public static object[] GetTestModels() {
            var personModel = PersonModel.GetTestModel();

            var linkOne = new Link("self", "one");
            var linkTwo = new Link("self", "two");

            string embeddedName = "bars";
            var embedded = new[] {
                new { bar = true }
            };

            return new object[] {
                new object[] { personModel, linkOne, linkTwo, embeddedName, embedded },
                new object[] { JObject.FromObject(personModel), linkOne, linkTwo, embeddedName, embedded },
            };
        }

        public static object[] GetCollectionModels() {
            return new object[] {
                new object[] { new int[0] },
                new object[] { Enumerable.Empty<int>() },
            };
        }
    }
}

