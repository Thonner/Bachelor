using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mnist_classification
{
    class Filter
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public int Depth { get; set; }

        //weights are given as width times height times depth.
        public Fix8[,,] Weights { get; set; }

    }
}
