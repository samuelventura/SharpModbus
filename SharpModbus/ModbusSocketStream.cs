using System;
using System.Threading;
using System.Net.Sockets;

namespace SharpModbus
{
    public class ModbusSocketStream : IModbusStream, IDisposable
    {
        private readonly TcpClient socket;
        private readonly Action<char, byte[], int> monitor;

        public ModbusSocketStream(TcpClient socket, int timeout, Action<char, byte[], int> monitor = null)
        {
            socket.ReceiveTimeout = timeout;
            socket.SendTimeout = timeout;
            this.monitor = monitor;
            this.socket = socket;
        }

        public void Dispose()
        {
            Disposer.Dispose(socket);
        }

        public void Write(byte[] data)
        {
            if (monitor != null) monitor('>', data, data.Length);
            socket.GetStream().Write(data, 0, data.Length);
        }

        public void Read(byte[] data)
        {
            var count = 0;
            var dl = DateTime.Now.AddMilliseconds(socket.ReceiveTimeout);
            while (DateTime.Now < dl && count < data.Length)
            {
                var available = socket.Available;
                if (available == 0) Thread.Sleep(1);
                else {
                    var size = (int)Math.Min(available, data.Length - count);
                    count += socket.GetStream().Read(data, count, size);
                }
            }
            if (monitor != null) monitor('<', data, count);
            Assert.Equal(count, data.Length, "Partial read got {0} expected {1}");
        }
    }
}

