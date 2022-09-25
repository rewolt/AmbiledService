using AmbiledService.LedsEffects;
using AmbiledService.Models;
using AmbiledService.Utilities;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AmbiledService.Services
{
    public class EffectsProcessorService : BackgroundService
    {
        private readonly GlobalStateService _globalStateService;
        private readonly TransitionEffect _transition;
        private readonly Logger _logger;

        public EffectsProcessorService(GlobalStateService globalStateService, TransitionEffect transition, Logger logger)
        {
            _globalStateService = globalStateService;
            _transition = transition;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var darkBlue = new RGB(0, 0, 50);

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var cpuColor = ColorConverter.ConvertCpuUsageToRGB(_globalStateService.CpuUsage);

                    await _transition.RunAsync(darkBlue, cpuColor, TimeSpan.FromMilliseconds(500));
                    await _transition.RunAsync(cpuColor, darkBlue, TimeSpan.FromMilliseconds(1500));
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Error occured while running {nameof(EffectsProcessorService)}.", ex);
            }
        }
    }
}
