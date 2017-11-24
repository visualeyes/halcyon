using Halcyon.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Halcyon.Tests.Templates {
    public class TemplateExtensionFacts {
        [Theory]
        [MemberData("GetLinks")]
        public void Substitute_Params(string link, Dictionary<string, object> parameters, string expected) {
            string actual = link.SubstituteParams(parameters);

            Assert.Equal(expected, actual);
        }

        public static IEnumerable<object[]> GetLinks() {
            return new object[][] {
                new object[] {
                    "api/test/{id}",
                    new Dictionary<string, object> {
                        { "id", 1 }
                    },
                    "api/test/1"
                },
                new object[] {
                    "api/search?query={query}",
                    new Dictionary<string, object> {
                        { "query", "happy c++" }
                    },
                    "api/search?query=happy%20c%2B%2B"
                }
            };
        }
    }
}
