
using System;

namespace SharpModbus
{
    public static class ModbusParser
    {
        public static IModbusCommand Parse(byte[] request, int offset)
        {
            var slave = request[offset + 0];
            var code = request[offset + 1];
            var address = ModbusHelper.GetUShort(request, offset + 2);
            switch (code)
            {
                case 1:
                    return Parse01(slave, code, address, request, offset);
                case 2:
                    return Parse02(slave, code, address, request, offset);
                case 3:
                    return Parse03(slave, code, address, request, offset);
                case 4:
                    return Parse04(slave, code, address, request, offset);
                case 5:
                    return Parse05(slave, code, address, request, offset);
                case 6:
                    return Parse06(slave, code, address, request, offset);
                case 15:
                    return Parse15(slave, code, address, request, offset);
                case 16:
                    return Parse16(slave, code, address, request, offset);
            }
            throw Thrower.Make("Unsupported function code {0}", code);
        }

        private static IModbusCommand Parse01(byte slave, byte code, ushort address, byte[] request, int offset)
        {
            var count = ModbusHelper.GetUShort(request, offset + 4);
            return new ModbusF01ReadCoils(slave, address, count);
        }

        private static IModbusCommand Parse02(byte slave, byte code, ushort address, byte[] request, int offset)
        {
            var count = ModbusHelper.GetUShort(request, offset + 4);
            return new ModbusF02ReadInputs(slave, address, count);
        }

        private static IModbusCommand Parse03(byte slave, byte code, ushort address, byte[] request, int offset)
        {
            var count = ModbusHelper.GetUShort(request, offset + 4);
            return new ModbusF03ReadHoldingRegisters(slave, address, count);
        }

        private static IModbusCommand Parse04(byte slave, byte code, ushort address, byte[] request, int offset)
        {
            var count = ModbusHelper.GetUShort(request, offset + 4);
            return new ModbusF04ReadInputRegisters(slave, address, count);
        }

        private static IModbusCommand Parse05(byte slave, byte code, ushort address, byte[] request, int offset)
        {
            var value = ModbusHelper.DecodeBool(request[offset + 4]);
            var zero = request[offset + 5];
            Assert.Equal(zero, 0, "Zero mismatch got {0} expected {1}");
            return new ModbusF05WriteCoil(slave, address, value);
        }

        private static IModbusCommand Parse06(byte slave, byte code, ushort address, byte[] request, int offset)
        {
            var value = ModbusHelper.GetUShort(request, offset + 4);
            return new ModbusF06WriteRegister(slave, address, value);
        }

        private static IModbusCommand Parse15(byte slave, byte code, ushort address, byte[] request, int offset)
        {
            var count = ModbusHelper.GetUShort(request, offset + 4);
            var values = ModbusHelper.DecodeBools(request, offset + 7, count);
            var bytes = request[offset + 6];
            Assert.Equal(ModbusHelper.BytesForBools(count), bytes, "Byte count mismatch got {0} expected {1}");
            return new ModbusF15WriteCoils(slave, address, values);
        }

        private static IModbusCommand Parse16(byte slave, byte code, ushort address, byte[] request, int offset)
        {
            var count = ModbusHelper.GetUShort(request, offset + 4);
            var values = ModbusHelper.DecodeWords(request, offset + 7, count);
            var bytes = request[offset + 6];
            Assert.Equal(ModbusHelper.BytesForWords(count), bytes, "Byte count mismatch got {0} expected {1}");
            return new ModbusF16WriteRegisters(slave, address, values);
        }
    }
}
