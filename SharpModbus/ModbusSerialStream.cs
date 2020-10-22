using System;
using SharpSerial;

namespace SharpModbus
{
    public class ModbusSerialStream : IModbusStream
    {
        private readonly Action<char, byte[], int> monitor;
        private readonly SerialDevice serialDevice;
        private readonly int timeout;

        public ModbusSerialStream(SerialSettings settings, int timeout, Action<char, byte[], int> monitor = null)
        {
            this.serialDevice = new SerialDevice(settings);
            this.timeout = timeout;
            this.monitor = monitor;
        }

        public void Dispose()
        {
            Tools.Dispose(serialDevice);
        }

        public void Write(byte[] data)
        {
            if (monitor != null) monitor('>', data, data.Length);
            serialDevice.Write(data);
        }

        public int Read(byte[] data)
        {
            var response = serialDevice.Read(data.Length, -1, timeout);
            var count = response.Length;
            for (var i = 0; i < count; i++) data[i] = response[i];
            if (monitor != null) monitor('<', data, count);
            return count;
        }
    }
}
