using System;

namespace SharpModbus
{
	public class ModbusRTUProtocol : IModbusProtocol
	{
		public IModbusWrapper Wrap(IModbusCommand wrapped)
		{
			return new ModbusRTUWrapper(wrapped);
		}
		
		public IModbusWrapper Parse(byte[] request, int offset)
		{
			var wrapped = ModbusParser.Parse(request, offset);
			var crc = ModbusHelper.CRC16(request, offset, wrapped.RequestLength);
			Assert.Equal(crc, ModbusHelper.GetUShort(request, offset + wrapped.RequestLength), "CRC mismatch {0:X} expected:{1:X}");
			return new ModbusRTUWrapper(wrapped);
		}
	}
}
