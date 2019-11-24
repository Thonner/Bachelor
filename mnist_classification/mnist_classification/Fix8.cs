using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mnist_classification
{
    class Fix8
    {

        public SByte value;
        

        //More bytes on the right side than possible, due to shifted value.
        // FORM:
        // s.ssXXXXXXX
        // Where s it MSB, the two zeroes are implied and the seven X are the lower 7 bits.
        public const int right = 4;
        public const int left = 4;

        const int multiplier = 16; //2^right;

        private Fix8(SByte value)
        {
            this.value = value;
        }


        public Fix8(double value)
        {
            int temp =(int) (value * multiplier);


            if (temp < SByte.MinValue)
            {
                //Underflow occured, set to min value.
                temp = SByte.MinValue;
            }
            else if (temp > SByte.MaxValue)
            {
                //Overflow occured, set to max value.
                temp = SByte.MaxValue;
            }


            this.value = (SByte)temp;
        }

        public Fix8(int value)
        {
            int temp = (int)(value * multiplier);


            if (temp < SByte.MinValue)
            {
                //Underflow occured, set to min value.
                temp = SByte.MinValue;
            }
            else if (temp > SByte.MaxValue)
            {
                //Overflow occured, set to max value.
                temp = SByte.MaxValue;
            }

            this.value = (SByte)temp;
        }



        public static Fix8 operator +(Fix8 f1, Fix8 f2)
        {
            if (f1 == null)
            {
                return f2;
            }

            int temp = (f1.value + f2.value);

            //If both are negative, result should be negative
            if (f1.value < 0 && f2.value < 0)
            {
                if (temp < SByte.MinValue)
                {
                    //Underflow occured, set to min value.
                    temp = SByte.MinValue;
                }
                //If both are positive, result should be positive.
            }
            else if (f1.value >= 0 && f2.value >= 0)
            {
                if (temp > SByte.MaxValue)
                {
                    //Overflow occured, set to max value.
                    temp = SByte.MaxValue;
                }
            }
            //If one is negative and the other is positive, it will not overflow.
            if(temp > SByte.MaxValue || temp < SByte.MinValue)
            {
                throw new Exception("SOMETHING WENT WRONG!");

            }
            return new Fix8((SByte)temp);
        }

        public static Fix8 operator -(Fix8 f1, Fix8 f2)
        {

            //We invert the right side and can use our adder.
            Fix8 temp = new Fix8((int)(0 - f2.value));


            return f1 + temp;
        }

        public static Fix8 operator *(Fix8 f1, Fix8 f2)
        {
            int temp = f1.value * f2.value;

            temp = temp >> right;


            if (temp < SByte.MinValue)
            {
                //Underflow occured, set to min value.
                temp = SByte.MinValue;
            }
            else if (temp > SByte.MaxValue)
            {
                //Overflow occured, set to max value.
                temp = SByte.MaxValue;
            }


            return new Fix8((SByte)temp);
        }

        public static explicit operator Fix8(double v)
        {
            return new Fix8(v);
        }

        public static implicit operator Fix8(int v)
        {
            return new Fix8(v);
        }

        public static implicit operator Double(Fix8 v)
        {
            return v.Double();
        }

        public static implicit operator float(Fix8 v)
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
