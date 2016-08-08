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
        [InlineData(null, null, null, null, null, null, null, null, null)]
        [InlineData("", "", "", "", "", "", "", "", "")]
        [InlineData("rel", "href", "title", "method", "type", "deprecation", "name", "profile", "hrefLang")]
        public void Link_Created(string rel, string href, string title, string method, string type, string deprecation, string name, string profile, string hrefLang) {
            var link = new Link(
                rel: rel,
                href: href,
                title: title,
                method: method
            ) {
                Type = type,
                Deprecation = deprecation,
                Name = name,
                Profile = profile,
                HrefLang = hrefLang
            };

            Assert.Equal(rel, link.Rel);
            Assert.Equal(href, link.Href);
            Assert.Equal(method, link.Method);
            Assert.Equal(title, link.Title);
            Assert.Equal(type, link.Type);
            Assert.Equal(deprecation, link.Deprecation);
            Assert.Equal(name, link.Name);
            Assert.Equal(hrefLang, link.HrefLang);
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
        [InlineData(null, null, null, null, null, true)]
        [InlineData("", "", "", "", null, true)]
        [InlineData("href/{one}", "href/1", "book-{one}", "book-1", null, true)]
        [InlineData("href/{one}", "href/{one}", "book-{one}", "book-{one}", true, false)]
        [InlineData("href/{two}", "href/", "book-{two}", "book-", null, true)]
        public void Create_Link(string href, string expectedHref, string title, string expectedTitle, bool? isTemplated, bool replaceParameters) {
            var parameters = new Dictionary<string, object> {
                { "one", 1 }
            };

            var link = new Link(
                rel: "self",
                title: title,
                href: href,
                replaceParameters: replaceParameters
            );

            var templatedLink = link.CreateLink(parameters);

            Assert.Equal(expectedHref, templatedLink.Href);
            Assert.Equal(expectedTitle, templatedLink.Title);
            Assert.Equal(isTemplated, templatedLink.Templated);
        }

        [Theory]
        [InlineData(null, null, null)]
        [InlineData("", "", "")]
        [InlineData("/", "/api", "/api")]
        [InlineData("/app/", "/api", "/app/api")]
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
