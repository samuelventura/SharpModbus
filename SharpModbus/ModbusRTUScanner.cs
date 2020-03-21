using System;
using System.Collections.Generic;

namespace SharpModbus
{
    public class ModbusRTUScanner : IModbusScanner
    {
        private readonly ModbusRTUProtocol protocol = new ModbusRTUProtocol();
        private readonly List<byte> buffer = new List<byte>();

        public void Append(byte[] data, int offset, int count)
        {
            for (var i = 0; i < count; i++) buffer.Add(data[offset + i]);
        }

        public IModbusWrapper Scan()
        {
            //01,02,03,04,05,06 have 6 + 2(CRC)
            //15,16 have 6 + 1(len) + len + 2(CRC)
            if (buffer.Count >= 8)
            {
                var code = buffer[1];
                CheckCode(code);
                var length = 8;
                if (HasBytesAt6(code)) length += 1 + buffer[6];
                if (buffer.Count >= length)
                {
                    var request = buffer.GetRange(0, length).ToArray();
                    buffer.RemoveRange(0, length);
                    return protocol.Parse(request, 0);
                }
            }
            return null; //not enough data to parse
        }

        bool HasBytesAt6(byte code)
        {
            return "15,16".Contains(code.ToString("00"));
        }

        void CheckCode(byte code)
        {
            var valid = "01,02,03,04,05,06,15,16".Contains(code.ToString("00"));
            if (!valid) Thrower.Throw("Unsupported code {0}", code);
        }
    }
}
