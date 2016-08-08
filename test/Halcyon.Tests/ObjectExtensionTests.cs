using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Halcyon.Tests {

    public class ObjectExtensionTests {
        private readonly int expectedNumber;
        private readonly string expectedHello;
        private readonly object expectedComplex;
        private readonly object testObj;

        public ObjectExtensionTests() {
            this.expectedNumber = 1;
            this.expectedHello = "world";
            this.expectedComplex = new {
                foo = "bar"
            };
            this.testObj = new {
                number = expectedNumber,
                hello = expectedHello,
                complex = expectedComplex
            };
        }

        [Fact]
        public void Object_To_Dictionary() {
            var actual = this.testObj.ToDictionary();

            Assert.Equal(3, actual.Count);
            Assert.Contains("number", actual.Keys);
            Assert.Contains("hello", actual.Keys);
            Assert.Contains("complex", actual.Keys);

            var actualNumber = actual["number"];
            var actualHello = actual["hello"];
            var actualComplex = actual["complex"];

            Assert.Equal(expectedNumber, actualNumber);
            Assert.Equal(expectedHello, actualHello);
            Assert.Equal(expectedComplex, actualComplex);
        }

        [Fact]
        public void Dictionary_To_Dictionary() {
            var expected = new Dictionary<string, object>();
            var actual = expected.ToDictionary();
            Assert.Same(expected, actual);
        }

        [Fact]
        public void JObject_To_Dictionary() {
            var jObj = JObject.FromObject(this.testObj);
            var actual = jObj.ToDictionary();

            Assert.Equal(3, actual.Count);
            Assert.Contains("number", actual.Keys);
            Assert.Contains("hello", actual.Keys);
            Assert.Contains("complex", actual.Keys);
        }
    }
}
