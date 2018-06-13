using System;
using WhiteNoise.Business;

namespace WhiteNoise.BusinessImpl
{
    public class SoundBusiness : ISoundBusiness
    {
        private IScreamBusiness scream;

        private IRussellBusiness russell;

        public SoundBusiness()
        {
            this.scream = new ScreamBusiness();
            this.russell = new RussellBusiness();
        }

        public void SoundMethod1(int temp)
        {
            var flag = this.scream.ScreamMethod2(temp);

            if (flag)
                this.russell.RussellMethod1();

            var value = InternalSoundMethod();

            if (value == 100)
                this.scream.ScreamMethod1();
        }

        public TheNewEntity SoundMethod2(TheOldEntity old)
        {
            var value = InternalSoundMethod();
            int flag = 0;
            if (value == 101)
                flag = this.russell.RussellMethod2(value);

            if (flag > 0)
                return new TheNewEntity()
                {
                    prop1 = 1,
                    prop2 = true,
                    prop3 = 3.14
                };

            return new TheNewEntity()
            {
                prop1 = 2,
                prop2 = false,
                prop3 = 2.71
            };
        }

        private int InternalSoundMethod()
        {
            return 100;
        }
    }
}
