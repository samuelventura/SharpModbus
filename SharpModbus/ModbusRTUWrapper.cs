using System;

namespace SharpModbus
{
    public class ModbusRTUWrapper : IModbusWrapper
    {
        private readonly IModbusCommand wrapped;

        public byte Code { get { return wrapped.Code; } }
        public byte Slave { get { return wrapped.Slave; } }
        public ushort Address { get { return wrapped.Address; } }
        public IModbusCommand Wrapped { get { return wrapped; } }
        public int RequestLength { get { return wrapped.RequestLength + 2; } }
        public int ResponseLength { get { return wrapped.ResponseLength + 2; } }
        public int ExceptionLength { get { return 3 + 2; } }

        public ModbusRTUWrapper(IModbusCommand wrapped)
        {
            this.wrapped = wrapped;
        }

        public void FillRequest(byte[] request, int offset)
        {
            wrapped.FillRequest(request, offset);
            var crc = ModbusHelper.CRC16(request, offset, wrapped.RequestLength);
            request[offset + wrapped.RequestLength + 0] = ModbusHelper.Low(crc);
            request[offset + wrapped.RequestLength + 1] = ModbusHelper.High(crc);
        }

        public object ParseResponse(byte[] response, int offset)
        {
            var crc1 = ModbusHelper.CRC16(response, offset, wrapped.ResponseLength);
            //crc is little endian page 13 http://modbus.org/docs/Modbus_over_serial_line_V1_02.pdf
            var crc2 = ModbusHelper.GetUShortLittleEndian(response, offset + wrapped.ResponseLength);
            Assert.Equal(crc2, crc1, "CRC mismatch got {0:X4} expected {1:X4}");
            return wrapped.ParseResponse(response, offset);
        }

        public object ApplyTo(IModbusModel model)
        {
            return wrapped.ApplyTo(model);
        }

        public void FillResponse(byte[] response, int offset, object value)
        {
            wrapped.FillResponse(response, offset, value);
            var crc = ModbusHelper.CRC16(response, offset, wrapped.ResponseLength);
            response[offset + wrapped.ResponseLength + 0] = ModbusHelper.Low(crc);
            response[offset + wrapped.ResponseLength + 1] = ModbusHelper.High(crc);
        }

        public byte[] GetException(byte code)
        {
            var exception = new byte[ExceptionLength];
            exception[0] = wrapped.Slave;
            exception[1] = (byte)(wrapped.Code | 0x80);
            exception[2] = code;
            var crc = ModbusHelper.CRC16(exception, 0, 3);
            exception[3] = ModbusHelper.Low(crc);
            exception[4] = ModbusHelper.High(crc);
            return exception;
        }

        public void CheckException(byte[] response)
        {
            var offset = 0;
            var code = response[offset + 1];
            if ((code & 0x80) != 0)
            {
                Assert.Equal(response[offset + 0], wrapped.Slave, "Slave mismatch got {0} expected {1}");
                Assert.Equal(code & 0x7F, wrapped.Code, "Code mismatch got {0} expected {1}");
                var crc1 = ModbusHelper.CRC16(response, offset, 3);
                //crc is little endian page 13 http://modbus.org/docs/Modbus_over_serial_line_V1_02.pdf
                var crc2 = ModbusHelper.GetUShortLittleEndian(response, offset + 3);
                Assert.Equal(crc2, crc1, "CRC mismatch got {0:X4} expected {1:X4}");
                throw new ModbusException(response[offset + 2]);
            }
        }

        public override string ToString()
        {
            return string.Format("[ModbusRTUWrapper Wrapped={0}]", wrapped);
        }
    }
}
