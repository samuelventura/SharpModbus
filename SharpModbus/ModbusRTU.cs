
using System;

namespace SharpModbus
{
	public static class ModbusRTU
	{
		public static byte[] Wrap(ModbusCommand cmd)
		{
			var request = new byte[cmd.RequestLength + 2];
			cmd.FillRequest(request, 0);
			var crc = ModbusHelper.CRC16(request, 0, cmd.RequestLength);
			request[cmd.RequestLength + 0] = ModbusHelper.High(crc);
			request[cmd.RequestLength + 1] = ModbusHelper.Low(crc);
			return request;
		}
	}
}
