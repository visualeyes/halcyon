using System;

namespace Halcyon.HAL {
    public interface IHALConverter {
        bool CanConvert(Type type);
        HALResponse Convert(object model);
    }
}