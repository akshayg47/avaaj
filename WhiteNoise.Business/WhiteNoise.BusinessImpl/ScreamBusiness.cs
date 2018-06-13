using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteNoise.Business;

namespace WhiteNoise.BusinessImpl
{
    public class ScreamBusiness : IScreamBusiness
    {
        public void ScreamMethod1()
        {
            int temp = 190;

            InternalScreamMethod(temp);
        }

        public bool ScreamMethod2(int temp)
        {
            return temp > 10;
        }

        private void InternalScreamMethod(int temp)
        {
            Console.WriteLine(temp);
        }
    }   
}
