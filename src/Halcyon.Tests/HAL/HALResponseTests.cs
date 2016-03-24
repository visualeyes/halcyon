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
        private const string TestHalProperties = "\"_links\":{\"a\":{\"href\":\"one\"}},\"_embedded\":{\"bars\":[{\"bar\":true}]}";
        private const string TestHalPropertiesLinkArray = "\"_links\":{\"a\":[{\"href\":\"one\"},{\"href\":\"two\"}]},\"_embedded\":{\"bars\":[{\"bar\":true}]}";


        [Theory]
        [MemberData("GetCollectionModels")]
        public void Hal_Model_Doesnt_Accept_Collections(object dto) {
            Assert.Throws<ArgumentException>("model", () => {
                new HALResponse(dto);
            });
        }

        [Theory]
        [MemberData("GetTestModels")]
        public void Has_Link(object model, Link linkAOne, Link linkATwo, Link linkB, string embeddedName, object[] embedded) {
            var response = new HALResponse(model)
                .AddLinks(linkAOne);

            Assert.True(response.HasLink(linkAOne.Rel));
            Assert.False(response.HasLink(linkB.Rel));
        }

        [Theory]
        [MemberData("GetTestModels")]
        public void To_JObject(object model, Link linkAOne, Link linkATwo, Link linkB, string embeddedName, object[] embedded) {
            string expected = GetExpectedJson(false);

            var serializer = new JsonSerializer();

            var response = new HALResponse(model)
                .AddLinks(linkAOne)
                .AddEmbeddedCollection(embeddedName, embedded);

            var jObject = response.ToJObject(serializer);

            string actual = jObject.ToString(Formatting.None);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData("GetTestModels")]
        public void To_JObject_Link_Array(object model, Link linkAOne, Link linkATwo, Link linkB, string embeddedName, object[] embedded) {
            string expected = GetExpectedJson(true);

            var serializer = new JsonSerializer();

            var response = new HALResponse(model)
                .AddLinks(linkAOne)
                .AddLinks(linkATwo)
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

            var linkAOne = new Link("a", "one");
            var linkATwo = new Link("a", "two");
            var linkB = new Link("b", "two");

            string embeddedName = "bars";
            var embedded = new[] {
                new { bar = true }
            };

            return new object[] {
                new object[] { personModel, linkAOne, linkATwo, linkB, embeddedName, embedded },
                new object[] { JObject.FromObject(personModel), linkAOne, linkATwo, linkB, embeddedName, embedded },
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

