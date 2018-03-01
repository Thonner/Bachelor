using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mnist_classification
{
    class Layers
    {
        public ConvLayer Conv { get; set; }

        public FCLayer FC { get; set; }

        public MaxLayer Max { get; set; }

        public string LayerType { get; set; }
    }
}
