using System;
using WhiteNoise.Business;

namespace WhiteNoise.BusinessImpl
{
    public class RussellBusiness : IRussellBusiness
    {
        public void RussellMethod1()
        {
            Console.WriteLine("Russell Method 1");
        }

        public int RussellMethod2(int temp)
        {
            if (temp < 10)
                return 0;
            return 1;
        }
    }
}
