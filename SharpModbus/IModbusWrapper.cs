using System;

namespace SharpModbus
{
    public interface IModbusWrapper : IModbusCommand
    {
        IModbusCommand Wrapped { get; }
        byte[] GetException(byte code);
        void CheckException(byte[] respose, int count);
    }
}
