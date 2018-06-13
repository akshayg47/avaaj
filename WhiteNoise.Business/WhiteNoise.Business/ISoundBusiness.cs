using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhiteNoise.Business
{
    public interface ISoundBusiness
    {
        void SoundMethod1(int temp);

        TheNewEntity SoundMethod2(TheOldEntity old);
    }
}
