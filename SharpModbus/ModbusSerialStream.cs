using System;
using System.Threading;
using System.IO.Ports;

namespace SharpModbus
{
    public class ModbusSerialStream : IModbusStream, IDisposable
    {
        private readonly SerialPort serialPort;
        private readonly Action<char, byte[], int> monitor;

        public ModbusSerialStream(SerialPort serialPort, int timeout, Action<char, byte[], int> monitor = null)
        {
            serialPort.ReadTimeout = timeout;
            serialPort.WriteTimeout = timeout;
            this.serialPort = serialPort;
            this.monitor = monitor;
        }

        public void Dispose()
        {
            Disposer.Dispose(serialPort);
        }

        public void Write(byte[] data)
        {
            if (monitor != null) monitor('>', data, data.Length);
            serialPort.Write(data, 0, data.Length);
        }

        public void Read(byte[] data)
        {
            var count = 0;
            var dl = DateTime.Now.AddMilliseconds(serialPort.ReadTimeout);
            while (DateTime.Now < dl && count < data.Length)
            {
                var available = serialPort.BytesToRead;
                if (available == 0) Thread.Sleep(1);
                else {
                    var size = (int)Math.Min(available, data.Length - count);
                    count += serialPort.Read(data, count, size);
                }
            }
            if (monitor != null) monitor('<', data, count);
            Assert.Equal(count, data.Length, "Partial read got {0} expected {1}");
        }
    }
}
