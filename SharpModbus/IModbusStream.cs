using System;

namespace SharpModbus
{
    public interface IModbusStream : IDisposable
    {
        void Write(byte[] data);
        void Read(byte[] data);
    }
}
