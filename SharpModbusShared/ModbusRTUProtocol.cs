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
			Assert.Equal(crc, ModbusHelper.GetUShortLittleEndian(request, offset + wrapped.RequestLength), 
			             "CRC mismatch {0:X4} expected {1:X4}");
			return new ModbusRTUWrapper(wrapped);
		}
	}
}
