using AmbiledService.LedsEffects;
using AmbiledService.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AmbiledServices
{
    public sealed class Program
    {
        public static void Main(string[] args)
        {
            CreateHost(args).Run();
        }

        public static IHost CreateHost(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService(options => options.ServiceName = "Ambiled Service")
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<PulseEffect>();
                    services.AddSingleton<TransitionEffect>();
                    services.AddSingleton<Logger>();
                    services.AddSingleton<GlobalStateService>();

                    services.AddHostedService<EffectsProcessorService>();
                    services.AddHostedService<SenderService>();
                    services.AddHostedService<CpuUsageService>();
                })
            .Build();
    }
}
