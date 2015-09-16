using Halcyon.HAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Halcyon.Tests.HAL {


    public class HALModelTests {
        private readonly int expectedNumber = 1;
        private readonly string expectedHello = "world";
        private readonly object expectedComplex = new {
            foo = "bar"
        };

        private readonly object model = null;

        private readonly IEnumerable<Link> modelLinks = new List<Link> {
            new Link(Link.RelForSelf, "test"),
            new Link("number", "number/{number}")
        };

        public HALModelTests() {
            model = new {
                number = expectedNumber,
                hello = expectedHello,
                complex = expectedComplex
            };
        }

        [Fact]
        public void To_HAL_Model_With_No_Links() {
            dynamic halModel = new HALModel(model);

            AssertModelProperties(halModel);
            Assert.Empty(halModel._links);
        }

        [Fact]
        public void To_HAL_Model_With_Links() {
            var halModel = new HALModel(model);

            halModel.AddLinks(modelLinks);

            AssertModelProperties(halModel);
            AssertModelLinks(halModel);
        }

        [Fact]
        public void To_HAL_Model_With_Embedded_No_Links() {
            var halModel = new HALModel(model);

            var embeddedList = new List<object> { model };

            halModel.AddEmbeddedCollection("one", embeddedList, null);

            dynamic dyn = halModel;

            AssertModelProperties(halModel);
            Assert.Empty(dyn._links);

            var embedded = dyn._embedded as Dictionary<string, IEnumerable<HALModel>>;

            Assert.NotNull(embedded);

            var embeddedCollection = embedded.SingleOrDefault();

            Assert.NotNull(embeddedCollection);
            Assert.Equal("one", embeddedCollection.Key);

            dynamic embeddedModel = embeddedCollection.Value.SingleOrDefault();

            Assert.NotNull(embeddedModel);
            AssertModelProperties(embeddedModel);
            Assert.Empty(embeddedModel._links);
        }

        [Fact]
        public void To_HAL_Model_With_Embedded_With_Links() {
            var halModel = new HALModel(model);

            var embeddedList = new List<object> { model };

            halModel.AddEmbeddedCollection("one", embeddedList, modelLinks);

            AssertModelProperties(halModel);

            dynamic dyn = halModel;

            var embedded = dyn._embedded as Dictionary<string, IEnumerable<HALModel>>;

            Assert.NotNull(embedded);

            var embeddedCollection = embedded.SingleOrDefault();

            Assert.NotNull(embeddedCollection);
            Assert.Equal("one", embeddedCollection.Key);

            dynamic embeddedModel = embeddedCollection.Value.SingleOrDefault();

            Assert.NotNull(embeddedModel);
            AssertModelProperties(embeddedModel);
            AssertModelLinks(embeddedModel);
        }

        private void AssertModelProperties(dynamic halModel) {
            Assert.Equal(expectedNumber, halModel.number);
            Assert.Equal(expectedHello, halModel.hello);
            Assert.Equal(expectedComplex, halModel.complex);
        }

        private static void AssertModelLinks(dynamic dyn) {
            var links = dyn._links as Dictionary<string, Link>;

            Assert.NotNull(links);

            var selfLink = links[Link.RelForSelf];

            Assert.NotNull(selfLink);
            Assert.Equal(Link.RelForSelf, selfLink.Rel);
            Assert.Equal("test", selfLink.Href);

            var numberLink = links["number"];
            Assert.NotNull(numberLink);
            Assert.Equal("number", numberLink.Rel);
            Assert.Equal("number/1", numberLink.Href);
        }
    }
}

