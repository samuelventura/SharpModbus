
using System;

namespace SharpModbus
{
    public interface IModbusWrapper : IModbusCommand
    {
        IModbusCommand Wrapped { get; }
        int ExceptionLength { get; }
        void CheckException(byte[] respose);
    }
}
