using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhiteNoise.Business
{
    public interface IRussellBusiness
    {
        void RussellMethod1();

        int RussellMethod2(int temp);

        int RussellMethodComplex3(List<string> list);

        bool RussellMethodComplex4(List<TheOldEntity> entities, Dictionary<string, string> dict);
    }
}
