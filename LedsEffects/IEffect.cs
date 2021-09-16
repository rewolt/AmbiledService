using System;

namespace AmbiledService.LedsEffects
{
    interface IEffect
    {
        event EventHandler EffectStarted;
        event EventHandler EffectEnded;
    }
}
