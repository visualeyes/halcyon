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

        [Theory]
        [MemberData("GetCollectionModels")]
        public void Hal_Model_Doesnt_Accept_Collections(object dto) {
            Assert.Throws<ArgumentException>("model", () => {
                new HALResponse(dto);
            });
        }

        [Theory]
        [MemberData("GetLinkTestModels")]
        public void Has_Link(object model, Link[] links, string expectedHal, string expectedPlain) {
            var response = new HALResponse(model)
                .AddLinks(links);
                
            Assert.True(response.HasLink("a"));
            Assert.True(response.HasLink("b"));
            Assert.False(response.HasLink("c"));
        }

        [Fact]
        public void JObject_To_JObject() {
            string expected = GetExpectedJson("");

            var personModel = PersonModel.GetTestModel();
            var model = JObject.FromObject(personModel);

            var response = new HALResponse(model);

            var serializer = new JsonSerializer();
            var jObject = response.ToJObject(serializer);

            string actual = jObject.ToString(Formatting.None);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData("GetLinkTestModels")]
        public void Links_To_JObject(object model, Link[] links, string expectedHal, string expectedPlain) {
            var serializer = new JsonSerializer();

            var response = new HALResponse(model)
                .AddLinks(links);

            var jObject = response.ToJObject(serializer);

            string actual = jObject.ToString(Formatting.None);
            Assert.Equal(expectedHal, actual);
        }

        [Theory]
        [MemberData("GetLinkTestModels")]
        public void Links_To_ToPlainResponse(object model, Link[] links, string expectedHal, string expectedPlain) {
            var serializer = new JsonSerializer();

            var response = new HALResponse(model)
                .AddLinks(links);

            var jObject = response.ToPlainResponse(serializer);

            string actual = jObject.ToString(Formatting.None);
            Assert.Equal(expectedPlain, actual);
        }

        [Theory]
        [MemberData("GetEmbeddedCollectionTestModels")]
        public void Embedded_Resource_Collections_To_JObject(object model, Dictionary<string, IEnumerable<object>> embeddedCollections, string expectedHal, string expectedPlain) {
            var serializer = new JsonSerializer();

            var response = new HALResponse(model);
            response.AddEmbeddedCollections(embeddedCollections);

            var jObject = response.ToJObject(serializer);

            string actual = jObject.ToString(Formatting.None);
            Assert.Equal(expectedHal, actual);
        }

        [Theory]
        [MemberData("GetEmbeddedCollectionTestModels")]
        public void Embedded_Resource_Collections_ToPlainResponse(object model, Dictionary<string, IEnumerable<object>> embeddedCollections, string expectedHal, string expectedPlain) {
            var serializer = new JsonSerializer();

            var response = new HALResponse(model);
            response.AddEmbeddedCollections(embeddedCollections);

            var jObject = response.ToPlainResponse(serializer);

            string actual = jObject.ToString(Formatting.None);
            Assert.Equal(expectedPlain, actual);
        }

        [Theory]
        [MemberData("GetEmbeddedResourceTestModels")]
        public void Embedded_Resource_To_JObject(object model, Dictionary<string, object> embeddedResources, string expectedHal, string expectedPlain) {
            var serializer = new JsonSerializer();

            var response = new HALResponse(model);
            response.AddEmbeddedResources(embeddedResources);

            var jObject = response.ToJObject(serializer);

            string actual = jObject.ToString(Formatting.None);
            Assert.Equal(expectedHal, actual);
        }

        [Theory]
        [MemberData("GetEmbeddedResourceTestModels")]
        public void Embedded_Resource_To_ToPlainResponse(object model, Dictionary<string, object> embeddedResources, string expectedHal, string expectedPlain) {
            var serializer = new JsonSerializer();

            var response = new HALResponse(model);
            response.AddEmbeddedResources(embeddedResources);

            var jObject = response.ToPlainResponse(serializer);

            string actual = jObject.ToString(Formatting.None);
            Assert.Equal(expectedPlain, actual);
        }

        public static IEnumerable<object[]> GetLinkTestModels() {
            var personModel = PersonModel.GetTestModel();
                        
            return new object[][] {
                new object[] { 
                    personModel, new Link[] { new Link("a", "one"), new Link("b", "three") },
                    GetExpectedJson("\"_links\":{\"a\":{\"href\":\"one\"},\"b\":{\"href\":\"three\"}}"),
                    GetExpectedJson("")
                },
                new object[] {
                    personModel, new Link[] { new Link("a", "one"), new Link("a", "two"), new Link("b", "three") }, 
                    GetExpectedJson("\"_links\":{\"b\":{\"href\":\"three\"},\"a\":[{\"href\":\"one\"},{\"href\":\"two\"}]}"),
                    GetExpectedJson("")
                },
                new object[] {
                    personModel, new Link[] { new Link("a", "one"), new Link("a", "two"), new Link("b", "three", isRelArray:true) },
                    GetExpectedJson("\"_links\":{\"a\":[{\"href\":\"one\"},{\"href\":\"two\"}],\"b\":[{\"href\":\"three\"}]}"),
                    GetExpectedJson("")
                },
                new object[] {
                    personModel, new Link[] { new Link("a", "one", isRelArray:true), new Link("a", "two"), new Link("b", "three") },
                    GetExpectedJson("\"_links\":{\"b\":{\"href\":\"three\"},\"a\":[{\"href\":\"one\"},{\"href\":\"two\"}]}"),
                    GetExpectedJson("")
                }
            };
        }

        public static IEnumerable<object[]> GetEmbeddedCollectionTestModels() {
            var personModel = PersonModel.GetTestModel();

            return new object[][] {
                new object[] {
                    personModel, new Dictionary<string, IEnumerable<object>> { 
                        { "bars", new object[] { new { bar = true } } }
                    },
                    GetExpectedJson("\"_embedded\":{\"bars\":[{\"bar\":true}]}"),
                    GetExpectedJson("\"bars\":[{\"bar\":true}]")
                },
                new object[] {
                    personModel, new Dictionary<string, IEnumerable<object>> {
                        { 
                            "bars", new object[] { 
                                new { name = "one" },
                                new { name = "two" }
                            } 
                        }
                    },
                    GetExpectedJson("\"_embedded\":{\"bars\":[{\"name\":\"one\"},{\"name\":\"two\"}]}"),
                    GetExpectedJson("\"bars\":[{\"name\":\"one\"},{\"name\":\"two\"}]")
                },
            };
        }

        public static IEnumerable<object[]> GetEmbeddedResourceTestModels() {
            var personModel = PersonModel.GetTestModel();

            return new object[][] {
                new object[] {
                    personModel, new Dictionary<string, object> {
                        { "bar", new { bar = true } }
                    },
                    GetExpectedJson("\"_embedded\":{\"bar\":{\"bar\":true}}"),
                    GetExpectedJson("\"bar\":{\"bar\":true}")
                },
                new object[] {
                    personModel, new Dictionary<string, object> {
                        { "bar", new { name = "one" } },
                        { "baz", new { name = "two" } }
                    },
                    GetExpectedJson("\"_embedded\":{\"bar\":{\"name\":\"one\"},\"baz\":{\"name\":\"two\"}}"),
                    GetExpectedJson("\"bar\":{\"name\":\"one\"},\"baz\":{\"name\":\"two\"}")
                },
            };
        }
        
        private static string GetExpectedJson(string halProperties) {
            if(!String.IsNullOrWhiteSpace(halProperties)) halProperties = "," + halProperties;

            var expectedPersonJson = "{" + PersonModel.TestModelJson + halProperties + "}";
            return expectedPersonJson;
        }

        public static IEnumerable<object[]> GetCollectionModels() {
            return new object[][] {
                new object[] { new int[0] },
                new object[] { Enumerable.Empty<int>() },
            };
        }
    }
}

