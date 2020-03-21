using System;

namespace SharpModbus
{
    public interface IModbusProtocol
    {
        IModbusWrapper Wrap(IModbusCommand wrapped);
        IModbusWrapper Parse(byte[] request, int offset);
    }
}
