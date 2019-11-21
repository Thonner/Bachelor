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

        public float[,] WeightsArray { get; set; }

        public float[] Bias { get; set; }

        public float[,,] Input { get; set; }

        public float[,,] Output { get; set; }



        public void CalcFC()
        {

            Output = new float[1, 1, OutputSize];



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

        private float Max(float v1, float v2)
        {
            if (v1 > v2) return v1;
            return v2;
        }
    }
}
