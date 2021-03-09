using System;

namespace SharpModbus
{
    public class ModbusTCPProtocol : IModbusProtocol
    {
        private ushort transactionId = 0;

        public ushort TransactionId
        {
            get { return transactionId; }
            set { transactionId = value; }
        }

        public IModbusWrapper Wrap(IModbusCommand wrapped)
        {
            return new ModbusTCPWrapper(wrapped, transactionId++);
        }

        public IModbusWrapper Parse(byte[] request, int offset)
        {
            var wrapped = ModbusParser.Parse(request, offset + 6);
            Tools.AssertEqual(wrapped.RequestLength, ModbusHelper.GetUShort(request, offset + 4),
                "RequestLength mismatch got {0} expected {1}");
            var transaction = ModbusHelper.GetUShort(request, offset);
            return new ModbusTCPWrapper(wrapped, transaction);
        }
    }
}
