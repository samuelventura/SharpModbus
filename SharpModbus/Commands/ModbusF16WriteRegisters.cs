using System;

namespace SharpModbus
{
    public class ModbusF16WriteRegisters : IModbusCommand
    {
        private readonly byte slave;
        private readonly ushort address;
        private readonly ushort[] values;

        public byte Code { get { return 16; } }
        public byte Slave { get { return slave; } }
        public ushort Address { get { return address; } }
        public ushort[] Values { get { return ModbusHelper.Clone(values); } }
        public int RequestLength { get { return 7 + ModbusHelper.BytesForWords(values.Length); } }
        public int ResponseLength { get { return 6; } }

        public ModbusF16WriteRegisters(byte slave, ushort address, ushort[] values)
        {
            this.slave = slave;
            this.address = address;
            this.values = values;
        }

        public void FillRequest(byte[] request, int offset)
        {
            FillResponse(request, offset, null);
            var bytes = ModbusHelper.EncodeWords(values);
            request[offset + 6] = (byte)bytes.Length;
            ModbusHelper.Copy(bytes, 0, request, offset + 7, bytes.Length);
        }

        public object ParseResponse(byte[] response, int offset)
        {
            Assert.Equal(response[offset + 0], slave, "Slave mismatch got {0} expected {1}");
            Assert.Equal(response[offset + 1], 16, "Function mismatch got {0} expected {1}");
            Assert.Equal(ModbusHelper.GetUShort(response, offset + 2), address, "Address mismatch got {0} expected {1}");
            Assert.Equal(ModbusHelper.GetUShort(response, offset + 4), values.Length, "Register count mismatch got {0} expected {1}");
            return null;
        }

        public object ApplyTo(IModbusModel model)
        {
            model.setWOs(slave, address, values);
            return null;
        }

        public void FillResponse(byte[] response, int offset, object value)
        {
            response[offset + 0] = slave;
            response[offset + 1] = 16;
            response[offset + 2] = ModbusHelper.High(address);
            response[offset + 3] = ModbusHelper.Low(address);
            response[offset + 4] = ModbusHelper.High(values.Length);
            response[offset + 5] = ModbusHelper.Low(values.Length);
        }

        public override string ToString()
        {
            return string.Format("[ModbusF16WriteRegisters Slave={0}, Address={1}, Values={2}]", slave, address, values);
        }
    }
}
