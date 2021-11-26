using Microsoft.Extensions.Hosting;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace AmbiledService.Services
{
    public class CpuUsageService : BackgroundService
    {
        long _idleTimeLast = 0, _kernelTimeLast = 0, _userTimeLast = 0;
        private bool _disposedValue;
        private readonly GlobalStateService _globalStateService;
        private readonly Logger _logger;

        public CpuUsageService(GlobalStateService globalStateService, Logger logger)
        {
            _globalStateService = globalStateService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var (idleTimeNow, kernelTimeNow, userTimeNow) = CPUInfo.GetInternalSystemTimes();

                var idle = idleTimeNow - _idleTimeLast;
                var kernel = kernelTimeNow - _kernelTimeLast;
                var user = userTimeNow - _userTimeLast;
                var system = user + kernel;

                var cpu = (int)((system - idle) * 100 / system);
                _globalStateService.CpuUsage = cpu;

                _idleTimeLast = idleTimeNow;
                _kernelTimeLast = kernelTimeNow;
                _userTimeLast = userTimeNow;

                await Task.Delay(1000, stoppingToken);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            _logger.Log($"Disposing {nameof(CpuUsageService)}.");
            if (!_disposedValue)
            {
                if (disposing)
                {
                }
                _disposedValue = true;
            }
        }

        public override void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    class CPUInfo
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private unsafe static extern bool QueryPerformanceCounter(long* lpPerformanceCount);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private unsafe static extern bool GetSystemTimes(long* lpIdleTime, long* lpKernelTime, long* lpUserTime);

        internal static long GetQueryPerformanceCounter()
        {
            long pc = 0;
            unsafe { QueryPerformanceCounter(&pc); }
            return pc;
        }

        internal static (long idleTime, long kernelTime, long userTime) GetInternalSystemTimes()
        {
            long lpIdleTime, lpKernelTime, lpUserTime;
            unsafe { GetSystemTimes(&lpIdleTime, &lpKernelTime, &lpUserTime); }
            return (lpIdleTime, lpKernelTime, lpUserTime);
        }
    }
}
