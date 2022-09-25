using AmbiledService.Models;
using AmbiledService.Services;
using System;
using System.Threading.Tasks;

namespace AmbiledService.LedsEffects
{
    public class PulseEffect : IEffect
    {
        private readonly TransitionEffect _transform;
        private readonly GlobalStateService _globalState;
        private readonly Logger _logger;
        private bool _disposedValue;

        public event EventHandler EffectStarted;
        public event EventHandler EffectEnded;

        public PulseEffect(TransitionEffect transform, GlobalStateService globalState, Logger logger)
        {
            _transform = transform;
            _globalState = globalState;
            _logger = logger;
        }

        public async Task RunAsync(RGB pulseColor, TimeSpan brightTime, TimeSpan fadeTime, byte cycles = 1)
        {
            OnEffectStarted(null);

            var off = new RGB(0, 0, 0);
            var color = new RGB(pulseColor);

            for (byte i = 0; i < cycles; i++)
            {
                await _transform.RunAsync(off, color, brightTime);
                await _transform.RunAsync(color, off, fadeTime);
            }

            OnEffectEnded(null);
        }

        protected virtual void OnEffectStarted(EventArgs e)
        {
            EventHandler handler = EffectStarted;
            handler?.Invoke(this, e);
        }

        protected virtual void OnEffectEnded(EventArgs e)
        {
            EventHandler handler = EffectEnded;
            handler?.Invoke(this, e);
        }

        protected virtual void Dispose(bool disposing)
        {
            _logger.Log($"Disposing {nameof(PulseEffect)}.");
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _transform.Dispose();
                }
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
