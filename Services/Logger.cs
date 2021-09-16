using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace AmbiledService.Services
{
    public class Logger : IDisposable
    {
        private StreamWriter _writer;
        private bool _disposedValue;
        private readonly IConfiguration _configuration;

        public Logger(IConfiguration configuration)
        {
            _configuration = configuration;
            _writer = new StreamWriter(_configuration.GetValue<string>("LogFileName"));
            
        }
        public void Log(string message)
        {
            var msg = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
            Console.WriteLine(msg);
            _writer.WriteLine(msg);
            _writer.Flush();
        }

        protected virtual void Dispose(bool disposing)
        {
            Log($"Disposing {nameof(Logger)}");
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _writer.Flush();
                    _writer.Dispose();
                }
                _writer = null;
                _disposedValue = true;
            }
        }
        ~Logger()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
