using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using NUnit.Framework;
using SharpModbus.Serial;
using SharpModbus;

namespace SharpModbus.Test.Unix
{
    public class Socat: IDisposable
    {
        public const string Master = "/tmp/socat.master";
        public const string Slave = "/tmp/socat.slave";

        private readonly Process process;

        public Socat()
        {
            process = new Process();
            process.StartInfo = new ProcessStartInfo()
            {
                FileName = "socat",
                Arguments = $"-d -d pty,link={Master} pty,link={Slave}",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };
            process.Start();
            Dump(process.StandardOutput);
            Dump(process.StandardError);
            Wait(Master);
            Wait(Slave);
        }

        public void Dispose()
        {
            Tools.Try(() =>
            {
                process.StandardInput.Close();
                process.WaitForExit(200);
            });
            Tools.Try(process.Kill);
            Tools.Try(process.Dispose);
            File.Delete(Master);
            File.Delete(Slave);
        }

        [Conditional("DEBUG")]
        private void Dump(StreamReader reader)
        {
            Task.Factory.StartNew(() =>
            {
                var line = reader.ReadLine();
                while (line != null)
                {
                    Console.WriteLine(line);
                    line = reader.ReadLine();
                }
            });
        }

        private void Wait(string path) 
        {
            var dl  = DateTime.Now.AddMilliseconds(2000);
            while(!File.Exists(path)) 
            {
                if (DateTime.Now > dl) throw Tools.Make("Missing {0}", path);
                Thread.Sleep(100);
            }
        }
    }
}
