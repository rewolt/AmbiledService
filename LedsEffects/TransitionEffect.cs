using AmbiledService.LedsEffects.Interfaces;
using AmbiledService.Models;
using AmbiledService.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace AmbiledService.LedsEffects
{
    public sealed class TransitionEffect : IEffect
    {
        private readonly IConfiguration _configuration;
        private readonly GlobalStateService _globalStateService;
        private readonly TimeSpan _resolution;
        private readonly Logger _logger;
        private bool _disposedValue;

        public event EventHandler EffectEnded;
        public event EventHandler EffectStarted;

        public TransitionEffect(IConfiguration configuration, GlobalStateService globalStateService, Logger logger)
        {
            _configuration = configuration;
            _globalStateService = globalStateService;
            _logger = logger;
            _resolution = TimeSpan.FromMilliseconds(25);
        }

        public async Task RunAsync(RGB startingColor, RGB endingColor, TimeSpan transitionTime, byte cycles = 1)
        {
            if (transitionTime.TotalSeconds > 214)
                throw new ArgumentException($"Maximum {nameof(transitionTime)} cannot be longer than 214 seconds (3,5 min).");

            OnEffectStarted(null);

            PreciseRGB actualColor = new (startingColor);
            PreciseRGB finalColor = new (endingColor);

            long initialSteps = transitionTime.Ticks / _resolution.Ticks;
            long actualSteps = initialSteps;

            float redColorChangeSpan = (finalColor.red - startingColor.red) / initialSteps;
            float greenColorChangeSpan = (finalColor.green - startingColor.green) / initialSteps;
            float blueColorChangeSpan = (finalColor.blue - startingColor.blue) / initialSteps;

            while (cycles > 0 && actualSteps > 0 )
            {
                actualColor.red += redColorChangeSpan;
                actualColor.green += greenColorChangeSpan;
                actualColor.blue += blueColorChangeSpan;

                var leds = new RGB[_configuration.GetValue<int>("LedsNumber")];
                for (byte i = 0; i < leds.Length; i++)
                {
                    leds[i].red = (byte)actualColor.red;
                    leds[i].green = (byte)actualColor.green;
                    leds[i].blue = (byte)actualColor.blue;
                } 

                _globalStateService.RgbArray = leds;

                if (cycles >= 1 && actualSteps == 1)
                {
                    actualSteps = initialSteps;
                    cycles--;
                }
                else
                    actualSteps--;

                await Task.Delay(_resolution.Milliseconds);
            }

            OnEffectEnded(null);
        }

        public void OnEffectStarted(EventArgs e)
        {
            _globalStateService.IsTransformEffectRunning = true;
            EventHandler handler = EffectStarted;
            handler?.Invoke(this, e);
        }

        public void OnEffectEnded(EventArgs e)
        {
            _globalStateService.IsTransformEffectRunning = false;
            EventHandler handler = EffectEnded;
            handler?.Invoke(this, e);
        }

        private void Dispose(bool disposing)
        {
            _logger.Log($"Disposing {nameof(TransitionEffect)}.");
            if (!_disposedValue)
            {
                if (disposing)
                {
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
