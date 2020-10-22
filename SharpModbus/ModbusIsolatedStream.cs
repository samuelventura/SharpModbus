using System;
using SharpSerial;

namespace SharpModbus
{
    public class ModbusIsolatedStream : IModbusStream
    {
        private readonly Action<char, byte[], int> monitor;
        private readonly SerialProcess serialProcess;
        private readonly int timeout;

        public ModbusIsolatedStream(object settings, int timeout, Action<char, byte[], int> monitor = null)
        {
            this.serialProcess = new SerialProcess(settings);
            this.timeout = timeout;
            this.monitor = monitor;
        }

        public void Dispose()
        {
            Tools.Dispose(serialProcess);
        }

        public void Write(byte[] data)
        {
            if (monitor != null) monitor('>', data, data.Length);
            serialProcess.Write(data);
        }

        public int Read(byte[] data)
        {
            var response = serialProcess.Read(data.Length, -1, timeout);
            var count = response.Length;
            for (var i = 0; i < count; i++) data[i] = response[i];
            if (monitor != null) monitor('<', data, count);
            return count;
        }
    }
}
