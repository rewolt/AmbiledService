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
            var msg = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [ERROR] {message}{(ex is null ? "" : $"\n{ex}")}";
            Console.WriteLine(msg);
            _writer.WriteLine(msg);
            _writer.Flush();
        }

        private string GetLogFullFilePath()
        {
            var baseFileName = _configuration.GetValue<string>("LogBaseFileName");
            if (baseFileName == null)
            {
                baseFileName = "logs.txt";
            }

            var assemblyDirectory = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;
            var actualFileName = baseFileName.Replace(".", "-" + DateTime.Now.ToString("yy-MM-dd") + ".");
            var fullPath = Path.Combine(assemblyDirectory, "Logs", actualFileName);

            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            return fullPath;
        }

        protected virtual void Dispose(bool disposing)
        {
            Log($"Disposing {nameof(Logger)}.");
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
