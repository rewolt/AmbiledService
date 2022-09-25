using AmbiledService.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace AmbiledService.Services
{
    public sealed class SenderService : BackgroundService
    {
        private SerialPort _serialPort;
        private bool _disposedValue;
        private byte[] _buffer;
        private readonly GlobalStateService _globalStateService;
        private readonly Logger _logger;

        public SenderService(GlobalStateService globalStateService, IConfiguration configuration, Logger logger)
        {
            _globalStateService = globalStateService;
            _logger = logger;
            _buffer = new byte[configuration.GetValue<int>("LedsNumber") * 3];
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
            byte ledIterator = 0;
            for (byte i = 0; i < _buffer.Length; i += 3)
            {
                _buffer[i] = ledsCopy[ledIterator].red;
                _buffer[i + 1] = ledsCopy[ledIterator].green;
                _buffer[i + 2] = ledsCopy[ledIterator].blue;
                ledIterator++;
            }
            _serialPort.Write(_buffer, 0, _buffer.Length);
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

        private void Dispose(bool disposing)
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
