using System;

namespace AmbiledService.LedsEffects.Interfaces
{
    interface IEffect : IDisposable
    {
        event EventHandler EffectStarted;
        event EventHandler EffectEnded;

        void OnEffectStarted(EventArgs e);
        void OnEffectEnded(EventArgs e);
    }
}
