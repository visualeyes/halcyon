using Halcyon.HAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Halcyon.Tests.HAL {
    public class LinkTests {

        [Theory]
        [InlineData(null, null, null, null)]
        [InlineData("", "", "", "")]
        [InlineData("rel", "href", "title", "method")]
        public void Link_Constructed(string rel, string href, string title, string method) {
            var link = new Link(
                rel: rel,
                href: href,
                title: title,
                method: method
            );

            Assert.Equal(rel, link.Rel);
            Assert.Equal(href, link.Href);
            Assert.Equal(method, link.Method);
            Assert.Equal(title, link.Title);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("", null)]
        [InlineData("href", null)]
        [InlineData("href/{one}", true)]
        public void Link_IsTemplated(string href, bool? isTemplated) {
            var link = new Link(
                rel: "self",
                href: href
            );
            
            Assert.Equal(href, link.Href);
            Assert.Equal(isTemplated, link.Templated);
        }

        [Theory]
        [InlineData(null, null, null)]
        [InlineData("", "", null)]
        [InlineData("href/{one}", "href/1", null)]
        [InlineData("href/{two}", "href/", null)]
        public void Create_Link(string href, string expectedHref, bool? isTemplated) {
            var parameters = new Dictionary<string, object> {
                { "one", 1 }
            };

            var link = new Link(
                rel: "self",
                href: href
            );

            var templatedLink = link.CreateLink(parameters);

            Assert.Equal(expectedHref, templatedLink.Href);
            Assert.Equal(isTemplated, templatedLink.Templated);
        }

        [Theory]
        [InlineData(null, null, null)]
        [InlineData("", "", "")]
        [InlineData("http://localhost/", "/api", "http://localhost/api")]
        [InlineData("http://localhost/", "http://otherhost/api", "http://otherhost/api")]
        public void Rebase_Link(string baseUriString, string href, string expectedHref) {
            
            var link = new Link(
                rel: "self",
                href: href
            );

            var rebasedLink = link.RebaseLink(baseUriString);

            Assert.Equal(expectedHref, rebasedLink.Href);
        }
    }
}
