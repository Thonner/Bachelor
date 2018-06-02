using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mnist_classification
{

    class Fix32 : System.Object
    {
        public int value;

        public const int right = 16; //16
        public const int left = 16; //16

        const int multiplier = 65536; //2^right;

        private Fix32(int value)
        {
            this.value = value;
        }


        public Fix32(double value)
        {
            int temp = (int)(value * multiplier);


            if (temp < Int32.MinValue)
            {
                //Underflow occured, set to min value.
                temp = Int32.MinValue;
            }
            else if (temp > Int32.MaxValue)
            {
                //Overflow occured, set to max value.
                temp = Int32.MaxValue;
            }


            this.value = (int)temp;
        }



        public static Fix32 operator +(Fix32 f1, Fix32 f2)
        {
            if (f1 == null)
            {
                return f2;
            }

            int temp = (f1.value + f2.value);

            //If both are negative, result should be negative
            if (f1.value < 0 && f2.value < 0)
            {
                if (temp < Int32.MinValue)
                {
                    //Underflow occured, set to min value.
                    temp = Int32.MinValue;
                }
                //If both are positive, result should be positive.
            }
            else if (f1.value >= 0 && f2.value >= 0)
            {
                if (temp > Int32.MaxValue)
                {
                    //Overflow occured, set to max value.
                    temp = Int32.MaxValue;
                }
            }
            //If one is negative and the other is positive, it will not overflow.
            if (temp > Int32.MaxValue || temp < Int32.MinValue)
            {
                throw new Exception("SOMETHING WENT WRONG!");

            }
            return new Fix32((int)temp);
        }

        public static Fix32 operator -(Fix32 f1, Fix32 f2)
        {

            //We invert the right side and can use our adder.
            Fix32 temp = new Fix32((int)(0 - f2.value));


            return f1 + temp;
        }


        public static Fix32 operator *(Fix32 f1, Fix8 f2)
        {


            int temp = f1.value * f2.value;

            temp = temp >> (right + Fix8.right - right);


            if (temp < Int32.MinValue)
            {
                //Underflow occured, set to min value.
                temp = Int32.MinValue;
            }
            else if (temp > Int32.MaxValue)
            {
                //Overflow occured, set to max value.
                temp = Int32.MaxValue;
            }


            return new Fix32((int)temp);
        }

        public static Fix32 operator *(Fix32 f1, Fix32 f2)
        {


            int temp = f1.value * f2.value;

            temp = temp >> right;


            if (temp < Int32.MinValue)
            {
                //Underflow occured, set to min value.
                temp = Int32.MinValue;
            }
            else if (temp > Int32.MaxValue)
            {
                //Overflow occured, set to max value.
                temp = Int32.MaxValue;
            }


            return new Fix32((int)temp);
        }

        public static explicit operator Fix32(double v)
        {
            return new Fix32(v);
        }

        public static implicit operator Fix32(int v)
        {
            return new Fix32(v);
        }

        public static implicit operator Double(Fix32 v)
        {
            return v.Double();
        }

        public static implicit operator float(Fix32 v)
        {
            return (float)v.Double();
        }

        public double Double()
        {
            double temp = value;

            temp /= multiplier;

            return temp;
        }

        public override string ToString()
        {
            return this.Double().ToString();
        }
    }
}
