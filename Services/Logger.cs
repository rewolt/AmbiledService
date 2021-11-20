using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Reflection;

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
            _writer = new StreamWriter(GetLogFullFilePath(), append: true, System.Text.Encoding.UTF8);
            
        }
        public void Log(string message)
        {
            var msg = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [INFO] {message}";
            Console.WriteLine(msg);
            _writer.WriteLine(msg);
            _writer.Flush();
        }

        public void Error(string message, Exception ex = null)
        {
            var msg = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [ERROR] {message}\n{ex?.ToString() ?? ""}";
            Console.WriteLine(msg);
            _writer.WriteLine(msg);
            _writer.Flush();
        }

        private string GetLogFullFilePath()
        {
            var partialFileName = _configuration.GetValue<string>("LogFileName");
            if (partialFileName == null)
            {
                partialFileName = "logs.txt";
            }

            var path = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;
            var actualFileName = partialFileName.Replace(".", "-" + DateTime.Now.ToString("yy-MM-dd") + ".");

            return Path.Combine(path, actualFileName);
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
