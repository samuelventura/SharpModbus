using System;

namespace SharpModbus
{
	public class ModbusRTUWrapper : IModbusWrapper
	{
		private readonly IModbusCommand wrapped;
		
		public IModbusCommand Wrapped { get { return wrapped; } }
		public int RequestLength { get { return wrapped.RequestLength + 2; } }
		public int ResponseLength { get { return wrapped.ResponseLength + 2; } }
		
		public ModbusRTUWrapper(IModbusCommand wrapped)
		{
			this.wrapped = wrapped;
		}
		
		public void FillRequest(byte[] request, int offset)
		{
			wrapped.FillRequest(request, offset);
			var crc = ModbusHelper.CRC16(request, offset, wrapped.RequestLength);
			request[offset + wrapped.RequestLength + 0] = ModbusHelper.High(crc);
			request[offset + wrapped.RequestLength + 1] = ModbusHelper.Low(crc);
		}
		
		public object ParseResponse(byte[] response, int offset)
		{
			var crc1 = ModbusHelper.CRC16(response, offset, wrapped.ResponseLength);
			var crc2 = ModbusHelper.GetUShort(response, offset + wrapped.ResponseLength);
			Assert.Equal(crc2, crc1, "CRC mismatch {0} {1}");
			return wrapped.ParseResponse(response, offset);
		}
		
		public override string ToString()
		{
			return string.Format("[ModbusRTUWrapper Wrapped={0}]", wrapped);
		}
	}
}
