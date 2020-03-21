using System;

namespace SharpModbus
{
    public class ModbusTCPWrapper : IModbusWrapper
    {
        private readonly IModbusCommand wrapped;
        private readonly int transactionId;

        public byte Code { get { return wrapped.Code; } }
        public byte Slave { get { return wrapped.Slave; } }
        public ushort Address { get { return wrapped.Address; } }
        public IModbusCommand Wrapped { get { return wrapped; } }
        public int TransactionId { get { return transactionId; } }
        public int RequestLength { get { return wrapped.RequestLength + 6; } }
        public int ResponseLength { get { return wrapped.ResponseLength + 6; } }
        public int ExceptionLength { get { return 3 + 6; } }

        public ModbusTCPWrapper(IModbusCommand wrapped, int transactionId)
        {
            this.wrapped = wrapped;
            this.transactionId = transactionId;
        }

        public void FillRequest(byte[] request, int offset)
        {
            request[offset + 0] = ModbusHelper.High(transactionId);
            request[offset + 1] = ModbusHelper.Low(transactionId);
            request[offset + 2] = 0;
            request[offset + 3] = 0;
            request[offset + 4] = ModbusHelper.High(wrapped.RequestLength);
            request[offset + 5] = ModbusHelper.Low(wrapped.RequestLength);
            wrapped.FillRequest(request, offset + 6);
        }

        public object ParseResponse(byte[] response, int offset)
        {
            Assert.Equal(ModbusHelper.GetUShort(response, offset + 0), transactionId, "TransactionId mismatch got {0} expected {1}");
            Assert.Equal(ModbusHelper.GetUShort(response, offset + 2), 0, "Zero mismatch got {0} expected {1}");
            Assert.Equal(ModbusHelper.GetUShort(response, offset + 4), wrapped.ResponseLength, "Length mismatch got {0} expected {1}");
            return wrapped.ParseResponse(response, offset + 6);
        }

        public object ApplyTo(IModbusModel model)
        {
            return wrapped.ApplyTo(model);
        }

        public void FillResponse(byte[] response, int offset, object value)
        {
            response[offset + 0] = ModbusHelper.High(transactionId);
            response[offset + 1] = ModbusHelper.Low(transactionId);
            response[offset + 2] = 0;
            response[offset + 3] = 0;
            response[offset + 4] = ModbusHelper.High(wrapped.ResponseLength);
            response[offset + 5] = ModbusHelper.Low(wrapped.ResponseLength);
            wrapped.FillResponse(response, offset + 6, value);
        }

        public void CheckException(byte[] response)
        {
            var offset = 6;
            var code = response[offset + 1];
            if ((code & 0x80) != 0)
            {
                Assert.Equal(response[offset + 0], wrapped.Slave, "Slave mismatch got {0} expected {1}");
                Assert.Equal(code & 0x7F, wrapped.Code, "Code mismatch got {0} expected {1}");
                Thrower.Throw("Modbus exception {0}", response[offset + 2]);
            }
        }

        public override string ToString()
        {
            return string.Format("[ModbusTCPWrapper Wrapped={0}, TransactionId={1}]", wrapped, transactionId);
        }
    }
}
