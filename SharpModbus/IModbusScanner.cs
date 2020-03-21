using System;

namespace SharpModbus
{
    public interface IModbusScanner
    {
        void Append(byte[] data, int offset, int count);
        IModbusWrapper Scan();
    }
}
