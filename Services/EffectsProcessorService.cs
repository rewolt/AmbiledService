using AmbiledService.LedsEffects;
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
        private readonly PulseEffect _pulse;
        private readonly Logger _logger;

        public EffectsProcessorService(GlobalStateService globalStateService, PulseEffect pulse, Logger logger)
        {
            _globalStateService = globalStateService;
            _pulse = pulse;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var color = ColorConverter.ConvertCpuUsageToRGB(_globalStateService.CpuUsage);
                    await _pulse.RunAsync(color, TimeSpan.FromMilliseconds(500), TimeSpan.FromMilliseconds(1000));
                }
            }
            catch (Exception ex)
            {
                _logger.Log("Error occured: " + ex.ToString());
            }
        }
    }
}
