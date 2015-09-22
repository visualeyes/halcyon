using Halcyon.HAL;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
