using Halcyon.HAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Halcyon.Tests.HAL {
    public class JsonHALMediaTypeFormatterTests {

        [Fact]
        public void Adds_Default_Hal_Type() {
            var formatter = new JsonHALMediaTypeFormatter(
                halJsonMedaiTypes: null
            );
            
            Assert.NotNull(formatter);
            Assert.Contains("application/hal+json", formatter.SupportedMediaTypes.Select(m => m.MediaType));
        }

        [Fact]
        public void Adds_Hal_Types() {
            var formatter = new JsonHALMediaTypeFormatter(
                halJsonMedaiTypes: new string[] { "application/test" }
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


        [Fact]
        public async Task Write_To_Stream_test() {
            using (var stream = new MemoryStream()) {
                var content = new StringContent("");

                var formatter = new JsonHALMediaTypeFormatter();
                
                var model = new HALModel(new {
                    test = 1
                });

                Assert.NotNull(formatter);

                await formatter.WriteToStreamAsync(typeof(HALModel), model, stream, content, null);

                // Reset the position to ensure it can read
                stream.Position = 0;

                var reader = new StreamReader(stream);
                string result = await reader.ReadToEndAsync();

                Assert.Equal("{\"test\":1}", result);
            }
        }
    }
}
