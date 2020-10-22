using System;
using System.Net.Sockets;

namespace SharpModbus
{
    public class SerialSettings : SharpSerial.SerialSettings { }

    public static class Tools
    {
        public static void AssertEqual(int a, int b, string format)
        {
            if (a != b) Tools.Throw(format, a, b);
        }

        public static TcpClient ConnectWithTimeout(string host, int port, int timeout)
        {
            var socket = new TcpClient();
            try
            {
                var result = socket.BeginConnect(host, port, null, null);
                var connected = result.AsyncWaitHandle.WaitOne(timeout, true);
                if (!connected) Tools.Throw("Timeout connecting to {0}:{1}", host, port);
                socket.EndConnect(result);
                return socket;
            }
            catch (Exception ex)
            {
                Tools.Dispose(socket);
                throw ex;
            }
        }

        public static void Dispose(IDisposable disposable)
        {
            try { if (disposable != null) disposable.Dispose(); }
            catch (Exception) { }
        }

        public static Exception Make(string format, params object[] args)
        {
            var message = format;
            if (args.Length > 0) message = string.Format(format, args);
            return new Exception(message);
        }

        public static void Throw(string format, params object[] args)
        {
            var message = format;
            if (args.Length > 0) message = string.Format(format, args);
            throw new Exception(message);
        }

        public static void Throw(Exception inner, string format, params object[] args)
        {
            var message = format;
            if (args.Length > 0) message = string.Format(format, args);
            throw new Exception(message, inner);
        }
    }
}
