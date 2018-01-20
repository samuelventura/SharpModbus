using System;

namespace SharpModbus
{
	public class ModbusTCPWrapper : IModbusWrapper
	{
		private readonly IModbusCommand wrapped;
		private readonly int transactionId;
		
		public IModbusCommand Wrapped { get { return wrapped; } }
		public int TransactionId { get { return transactionId; } }
		public int RequestLength { get { return wrapped.RequestLength + 6; } }
		public int ResponseLength { get { return wrapped.ResponseLength + 6; } }
		
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
			Assert.Equal(ModbusHelper.GetUShort(response, offset + 0), transactionId, "TransactionId mismatch {0} {1}");
			Assert.Equal(ModbusHelper.GetUShort(response, offset + 2), 0, "Zero mismatch {0} {1}");
			Assert.Equal(ModbusHelper.GetUShort(response, offset + 4), wrapped.ResponseLength, "Length mismatch {0} {1}");
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
		
		public override string ToString()
		{
			return string.Format("[ModbusTCPWrapper Wrapped={0}, TransactionId={1}]", wrapped, transactionId);
		}
	}
}
