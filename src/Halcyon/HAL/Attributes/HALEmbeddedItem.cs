using System.Collections.Generic;

namespace Halcyon.HAL.Attributes
{
    internal class HALEmbeddedItem
    {
        public HALEmbeddedItem(string resourceName, IEnumerable<HALResponse> halResponses, bool isCollection)
        {
            ResourceName = resourceName;
            HALResponses = halResponses;
            IsCollection = isCollection;
        }

        public string ResourceName { get; } 
        public bool IsCollection { get; }
        public IEnumerable<HALResponse> HALResponses { get;} 
    }
}