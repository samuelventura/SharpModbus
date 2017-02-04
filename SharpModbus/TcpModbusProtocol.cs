using System;

namespace SharpModbus
{
	public class TcpModbusProtocol : IModbusProtocol
	{
		private int transactionId = 0;
		
		private int TransactionId { get { return transactionId; } }
		
		public ModbusCommand Wrap(ModbusCommand wrapped)
		{
			return new ModbusTCPWrapper(wrapped, transactionId++);
		}
	}
}
