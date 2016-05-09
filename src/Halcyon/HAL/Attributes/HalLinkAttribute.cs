using System;

namespace Halcyon.HAL.Attributes {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class HalLinkAttribute : Attribute {
        public string Rel { get; }
        public string Href { get; }
        public string Title { get; }
        public string Method { get; }

        public HalLinkAttribute(string rel, string href, string title = null, string method = null) {
            Rel = rel;
            Href = href;
            Title = title;
            Method = method;
        }
    }
}