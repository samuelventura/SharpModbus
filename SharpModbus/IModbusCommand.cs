
using System;

namespace SharpModbus
{
    public interface IModbusCommand
    {
        byte Code { get; }
        byte Slave { get; }
        ushort Address { get; }
        int RequestLength { get; }
        int ResponseLength { get; }
        void FillRequest(byte[] request, int offset);
        object ParseResponse(byte[] response, int offset);
        object ApplyTo(IModbusModel model);
        void FillResponse(byte[] response, int offset, object value);
    }
}
