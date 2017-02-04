using System;

namespace SharpModbus
{
	public class ModbusRTUWrapper : ModbusCommand
	{
		private readonly ModbusCommand wrapped;
		
		public ModbusCommand Wrapped { get { return wrapped; } }
		public int RequestLength { get { return wrapped.RequestLength + 2; } }
		public int ResponseLength { get { return wrapped.ResponseLength + 2; } }
		
		public ModbusRTUWrapper(ModbusCommand wrapped)
		{
			this.wrapped = wrapped;
		}
		
		public void FillRequest(byte[] request, int offset)
		{
			wrapped.FillRequest(request, offset);
			var crc = ModbusHelper.CRC16(request, 0, wrapped.RequestLength);
			request[offset + wrapped.RequestLength + 0] = ModbusHelper.High(crc);
			request[offset + wrapped.RequestLength + 1] = ModbusHelper.Low(crc);
		}
	}
}
