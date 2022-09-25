using System;

namespace AmbiledService.LedsEffects
{
    interface IEffect : IDisposable
    {
        event EventHandler EffectStarted;
        event EventHandler EffectEnded;
    }
}
