using System;
using System.Web.Http.Filters;
using Halcyon.HAL.Filters;

namespace Halcyon.WebApi.HAL.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class HalModelAttribute : ActionFilterAttribute, IHalModelAttribute
    {
        public string LinkBase { get; private set; }
        public bool ForceHal { get; private set; }

        public HalModelAttribute(string linkBase, bool forceHAL = false)
        {
            LinkBase = linkBase;
            ForceHal = forceHAL;
        }
    }
}
