using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Halcyon.Tests {

    public class ObjectExtensionTests {

        [Fact]
        public void Object_To_Dictionary() {
            int expectedNumber = 1;
            string expectedHello = "world";
            object expectedComplex = new {
                foo = "bar"
            };

            var obj = new {
                number = expectedNumber,
                hello = expectedHello,
                complex = expectedComplex
            };

            var dict = obj.ToDictionary();

            Assert.Equal(3, dict.Count);
            Assert.Contains("number", dict.Keys);
            Assert.Contains("hello", dict.Keys);
            Assert.Contains("complex", dict.Keys);

            var actualNumber = dict["number"];
            var actualHello = dict["hello"];
            var actualComplex = dict["complex"];

            Assert.Equal(expectedNumber, actualNumber);
            Assert.Equal(expectedHello, actualHello);
            Assert.Equal(expectedComplex, actualComplex);
        }

    }
}
