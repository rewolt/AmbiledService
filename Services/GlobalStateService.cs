using AmbiledService.Models;
using Microsoft.Extensions.Configuration;
using System.Threading;

namespace AmbiledService.Services
{
    public sealed class GlobalStateService
    {
        private RGB[] _rgbArray;
        private volatile int _cpuUsage;
        private volatile bool _isTransformEffectRunning;
        public bool IsTransformEffectRunning { get => _isTransformEffectRunning; set => _isTransformEffectRunning = value; }
        public RGB[] RgbArray { get => _rgbArray; set => Interlocked.Exchange(ref _rgbArray, value); }
        public int CpuUsage { get => _cpuUsage; set => _cpuUsage = value; }

        public GlobalStateService(IConfiguration configuration)
        {
            _rgbArray = new RGB[configuration.GetValue<int>("LedsNumber")];

            for (int i = 0; i < _rgbArray.Length; i++)
            {
                _rgbArray[i] = new RGB(0, 0, 0);
            }
        }
    }
}
