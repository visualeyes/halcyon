using System;
using System.Web.Http.Filters;
using Halcyon.HAL.Filters;

namespace Halcyon.WebApi.HAL.Filters
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class HalModelAttribute : ActionFilterAttribute, IHalModelAttribute
    {
        public string LinkBase { get; }
        public bool ForceHal { get; }

        public HalModelAttribute(string linkBase, bool forceHAL = false)
        {
            LinkBase = linkBase;
            ForceHal = forceHAL;
        }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class HalLinkAttribute : ActionFilterAttribute, IHalLinkAttribute
    {
        public string Rel { get; }
        public string Href { get; }
        public string Title { get; }
        public string Method { get; }

        public HalLinkAttribute(string rel, string href, string title = null, string method = null)
        {
            Rel = rel;
            Href = href;
            Title = title;
            Method = method;
        }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class HalEmbeddedAttribute : ActionFilterAttribute
    {
        public string Rel { get; }
        public string Href { get; }
        public string Title { get; }
        public string Method { get; }

        public HalEmbeddedAttribute(string rel, string href, string title = null, string method = null)
        {
            Rel = rel;
            Href = href;
            Title = title;
            Method = method;
        }
    }
}
