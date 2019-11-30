using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mnist_classification
{
    class FCLayer
    {


        public int InputWidth { get; set; }

        public int InputHeight { get; set; }

        public int InputDepth { get; set; }

        public int OutputSize { get; set; }

        public int Weights { get; set; }

        public Fix32[,] WeightsArray { get; set; }

        public Fix32[] Bias { get; set; }

        public Fix32[,,] Input { get; set; }

        public Fix32[,,] Output { get; set; }



        public void CalcFC()
        {

            Output = new Fix32[1, 1, OutputSize];



            for (int output = 0; output < OutputSize; output++)
            {
                int counter = 0;


                //For all the nodes in the input
                for (int k = 0; k < InputDepth; k++)
                {
                    for (int j = 0; j < InputHeight; j++)
                    {
                        for (int i = 0; i < InputWidth; i++)
                        {
                            Output[0,0,output] += Input[i, j, k] * WeightsArray[output,counter] ;
                            counter++;
                        }
                    }
                }
                Output[0, 0, output] = Max(Output[0, 0, output] + Bias[output],  0);
            }
        }

        private Fix32 Max(Fix32 v1, Fix32 v2)
        {
            if (v1 > v2) return v1;
            return v2;
        }
    }
}
