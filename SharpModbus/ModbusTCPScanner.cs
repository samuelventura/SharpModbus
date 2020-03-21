using System;
using System.Collections.Generic;

namespace SharpModbus
{
    public class ModbusTCPScanner : IModbusScanner
    {
        private readonly ModbusTCPProtocol protocol = new ModbusTCPProtocol();
        private readonly List<byte> buffer = new List<byte>();

        public void Append(byte[] data, int offset, int count)
        {
            for (var i = 0; i < count; i++) buffer.Add(data[offset + i]);
        }

        public IModbusWrapper Scan()
        {
            if (buffer.Count >= 6)
            {
                var length = ModbusHelper.GetUShort(buffer[4], buffer[5]);
                if (buffer.Count >= 6 + length)
                {
                    var request = buffer.GetRange(0, 6 + length).ToArray();
                    buffer.RemoveRange(0, 6 + length);
                    return protocol.Parse(request, 0);
                }
            }
            return null; //not enough data to parse
        }
    }
}
