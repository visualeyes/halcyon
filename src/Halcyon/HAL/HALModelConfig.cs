using System;
using System.Collections.Generic;
using System.Linq;

namespace Halcyon.HAL {
    public class HALModelConfig : IHALModelConfig {
        public string LinkBase { get; set; }
        public bool ForceHAL { get; set; }
    }
}