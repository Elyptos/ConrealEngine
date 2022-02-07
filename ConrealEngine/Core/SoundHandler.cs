using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace ConrealEngine
{
    public class SoundHandler : Submodule
    {
        private static readonly SoundHandler instance = new SoundHandler();

        public static SoundHandler Instance { get { return instance; } }

        private SoundPlayer loopingSoundPlayer = new SoundPlayer();

        public override void Start()
        {
            if(!active)
            {
                active = true;
            }
        }

        public override void Stop()
        {
            if(active)
            {
                active = false;
            }
        }

        public void PlayLoopingSound(string soundFile)
        {
            
        }

        public void StopLoopingSound()
        {

        }
    }
}
