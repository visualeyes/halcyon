using Halcyon.HAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Halcyon.Tests.HAL {
    public class JsonHALMediaTypeFormatterTests {

        [Fact]
        public void Adds_Default_Hal_Type() {
            var formatter = new JsonHALMediaTypeFormatter(
                halJsonMediaTypes: null
            );
            
            Assert.NotNull(formatter);
            Assert.Contains("application/hal+json", formatter.SupportedMediaTypes.Select(m => m.MediaType));
        }

        [Fact]
        public void Adds_Hal_Types() {
            var formatter = new JsonHALMediaTypeFormatter(
                halJsonMediaTypes: new string[] { "application/test" }
            );

            Assert.NotNull(formatter);
            Assert.Contains("application/test", formatter.SupportedMediaTypes.Select(m => m.MediaType));
        }

        [Fact]
        public void Adds_Default_Json_Type() {
            var formatter = new JsonHALMediaTypeFormatter(
                jsonMediaTypes: null
            );

            Assert.NotNull(formatter);
            Assert.Contains(JsonMediaTypeFormatter.DefaultMediaType.MediaType, formatter.SupportedMediaTypes.Select(m => m.MediaType));
        }

        [Fact]
        public void Adds_Json_Types() {
            var formatter = new JsonHALMediaTypeFormatter(
                jsonMediaTypes: new string[] { "application/test" }
            );

            Assert.NotNull(formatter);
            Assert.Contains("application/test", formatter.SupportedMediaTypes.Select(m => m.MediaType));
        }


        [Theory]
        [InlineData("application/json", false, "{\"test\":1}")]
        [InlineData("application/json", true, "{\"test\":1,\"_links\":{\"self\":{\"href\":\"href\"}}}")]
        [InlineData("application/hal+json", true, "{\"test\":1,\"_links\":{\"self\":{\"href\":\"href\"}}}")]
        public async Task Write_To_Stream_test(string contentType, bool forceHal, string expected) {
            using (var stream = new MemoryStream()) {
                var content = new StringContent("");
                content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

                var formatter = new JsonHALMediaTypeFormatter();

                var model = new { test = 1 };
                var link = new Link("self", "href");

                var halModel = new HALModel(model, new HALModelConfig {
                    ForceHAL = forceHal
                })
                .AddLinks(link);

                Assert.NotNull(formatter);

                await formatter.WriteToStreamAsync(typeof(HALModel), halModel, stream, content, null);

                // Reset the position to ensure it can read
                stream.Position = 0;

                var reader = new StreamReader(stream);
                string result = await reader.ReadToEndAsync();

                Assert.Equal(expected, result);
            }
        }
    }
}
