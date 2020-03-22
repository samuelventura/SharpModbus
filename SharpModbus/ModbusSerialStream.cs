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

        public int Read(byte[] data)
        {
            var count = 0;
            var to = 3.0 * 10.0 / serialPort.BaudRate; //spec says 1.5 chars
            var dl = DateTime.Now.AddMilliseconds(serialPort.ReadTimeout);
            while (count < data.Length)
            {
                var available = serialPort.BytesToRead;
                if (available == 0)
                {
                    if (DateTime.Now > dl) break;
                    Thread.Sleep(1);
                }
                else
                {
                    var size = (int)Math.Min(available, data.Length - count);
                    count += serialPort.Read(data, count, size);
                    dl = DateTime.Now.AddSeconds(to); //9600 -> 3.125ms
                }
            }
            if (monitor != null) monitor('<', data, count);
            return count;
        }
    }
}
