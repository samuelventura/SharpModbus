using System;

namespace SharpModbus
{
	public class ModbusTCPWrapper : ModbusCommand
	{
		private readonly ModbusCommand wrapped;
		private readonly int transactionId;
		
		public ModbusCommand Wrapped { get { return wrapped; } }
		public int TransactionId { get { return transactionId; } }
		public int RequestLength { get { return wrapped.RequestLength + 6; } }
		public int ResponseLength { get { return wrapped.ResponseLength + 6; } }
		
		public ModbusTCPWrapper(ModbusCommand wrapped, int transactionId)
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
		
		public object ParseResponse(byte[] request, byte[] response, int offset)
		{
			for (var i = 0; i < 4; i++)
				Assert.Equal(request[i], response[i], i, "Header mismatch {0}!={1} at pos:{2}");
			var declaredLength = ModbusHelper.GetUShort(response, offset + 4);
			Assert.Equal(declaredLength, wrapped.ResponseLength, "Length mismatch {0} {1}");
			return wrapped.ParseResponse(request, response, offset + 6);
		}
	}
}
