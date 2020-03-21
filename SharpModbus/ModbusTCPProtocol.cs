using System;

namespace SharpModbus
{
    public class ModbusTCPProtocol : IModbusProtocol
    {
        private int transactionId = 0;

        private int TransactionId { get { return transactionId; } }

        private int NextTid()
        {
            var tid = transactionId++;
            transactionId &= 0xFFFF;
            return tid;
        }

        public IModbusWrapper Wrap(IModbusCommand wrapped)
        {
            return new ModbusTCPWrapper(wrapped, NextTid());
        }

        public IModbusWrapper Parse(byte[] request, int offset)
        {
            var wrapped = ModbusParser.Parse(request, offset + 6);
            Assert.Equal(wrapped.RequestLength, ModbusHelper.GetUShort(request, offset + 4), 
                "RequestLength mismatch got {0} expected {1}");
            var transaction = ModbusHelper.GetUShort(request, offset);
            return new ModbusTCPWrapper(wrapped, transaction);
        }
    }
}
