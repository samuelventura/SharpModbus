using System;

namespace SharpModbus
{
	public class ModbusTCPWrapper : ModbusCommand
	{
		private readonly ModbusCommand wrapped;
		private readonly int transactionId;
		
		public ModbusCommand Wrapped { get { return wrapped; } }
		public int TransactionId { get { return transactionId; }}
		public int RequestLength { get { return wrapped.RequestLength + 6; } }
		public int ResponseLength { get { return wrapped.ResponseLength + 6; } }
		
		public ModbusTCPWrapper(ModbusCommand wrapped)
		{
			this.transactionId = 0;
			this.wrapped = wrapped;
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
	}
}
