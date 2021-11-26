using AmbiledService.Models;
using Microsoft.Extensions.Hosting;
using System;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace AmbiledService.Services
{
    public class SenderService : BackgroundService
    {
        private SerialPort _serialPort;
        private bool _disposedValue;
        private readonly GlobalStateService _globalStateService;
        private readonly Logger _logger;

        public SenderService(GlobalStateService globalStateService, Logger logger)
        {
            _globalStateService = globalStateService;
            _logger = logger;
            SerialPortInitialize();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Run(async () =>
            {
                while(!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        if (!_serialPort?.IsOpen ?? false)
                        {
                            await Task.Delay(100);
                            continue;
                        }

                        string message = _serialPort.ReadLine();
                        if (message == "OK")
                        {
                            SendLedsData();
                        }  
                    }
                    catch(TimeoutException)
                    {
                        _logger.Error("Timeout exceeded when waiting for led driver.");
                    }
                    catch(Exception ex)
                    {
                        _logger.Error($"Exception thrown from {nameof(SenderService)} while running.", ex);
                    }
                }
            }, stoppingToken);
        }

        private void SendLedsData()
        {
            var ledsCopy = (RGB[])_globalStateService.RgbArray.Clone();
            byte[] buffer = new byte[ledsCopy.Length * 3];
            byte ledIterator = 0;
            for (byte i = 0; i < buffer.Length; i += 3)
            {
                buffer[i] = ledsCopy[ledIterator].red;
                buffer[i + 1] = ledsCopy[ledIterator].green;
                buffer[i + 2] = ledsCopy[ledIterator].blue;
                ledIterator++;
            }
            _serialPort.Write(buffer, 0, buffer.Length);
        }

        private void SerialPortInitialize()
        {
            try
            {
                _serialPort = new SerialPort
                {
                    PortName = "COM1",
                    BaudRate = 38400,
                    Parity = Parity.None,
                    DataBits = 8,
                    StopBits = StopBits.Two,
                    Handshake = Handshake.None,
                    DtrEnable = false,
                    RtsEnable = false,
                    NewLine = "\n",

                    ReadTimeout = 5000,
                    WriteTimeout = 1000
                };

                _serialPort.Open();
                _logger.Log($"Serial port {_serialPort.PortName} opened successfully.");
            }
            catch (Exception ex)
            {
                _logger.Error("Exception thrown while initializing serial port.", ex);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            _logger.Log($"Disposing {nameof(SenderService)}.");
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _serialPort.Close();
                    _serialPort.Dispose();
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
}
