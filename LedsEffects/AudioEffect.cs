using AmbiledService.LedsEffects.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AmbiledService.LedsEffects
{
    sealed class AudioEffect : IEffect
    {
        public event EventHandler EffectStarted;
        public event EventHandler EffectEnded;


        public void OnEffectEnded(EventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnEffectStarted(EventArgs e)
        {
            throw new NotImplementedException();
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    sealed class SoundCapturer
    {
        

    }
}
