using System;
using System.IO.Ports;
using System.Net.Sockets;

namespace SharpModbus
{
    public class SerialSettings
    {
        public SerialSettings() { init(null); }
        public SerialSettings(string portName) { init(portName); }
        public string PortName { get; set; }
        public int BaudRate { get; set; }
        public int DataBits { get; set; }
        public Parity Parity { get; set; }
        public StopBits StopBits { get; set; }
        public Handshake Handshake { get; set; }

        private void init(string portName)
        {
            var sp = new SerialPort();
            PortName = portName ?? sp.PortName;
            BaudRate = sp.BaudRate;
            DataBits = sp.DataBits;
            Parity = sp.Parity;
            StopBits = sp.StopBits;
            Handshake = sp.Handshake;
        }
    }

    static class Assert
    {
        public static void Equal(int a, int b, string format)
        {
            if (a != b) Thrower.Throw(format, a, b);
        }
    }

    public static class TcpTools
    {
        public static TcpClient ConnectWithTimeout(string host, int port, int timeout)
        {
            var socket = new TcpClient();
            using (var disposer = new Disposer(socket))
            {
                var result = socket.BeginConnect(host, port, null, null);
                var connected = result.AsyncWaitHandle.WaitOne(timeout, true);
                if (!connected) Thrower.Throw("Timeout connecting to {0}:{1}", host, port);
                socket.EndConnect(result);
                disposer.Clear();
                return socket;
            }
        }
    }

    public class Disposer : IDisposable
    {
        private IDisposable payload;

        public Disposer(IDisposable payload)
        {
            this.payload = payload;
        }

        public void Dispose()
        {
            Disposer.Dispose(payload);
        }

        public void Clear()
        {
            payload = null;
        }

        public static void Dispose(IDisposable closeable)
        {
            try { if (closeable != null) closeable.Dispose(); }
            catch (Exception) { }
        }
    }

    public static class Thrower
    {
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
