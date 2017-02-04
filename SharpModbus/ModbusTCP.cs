
using System;

namespace SharpModbus
{
	public static class ModbusTCP
	{
		public static byte[] Wrap(ModbusCommand cmd, ushort transactionId)
		{
			var request = new byte[cmd.RequestLength + 6];
			request[0] = ModbusHelper.High(transactionId);
			request[1] = ModbusHelper.Low(transactionId);
			request[2] = 0;
			request[3] = 0;
			request[4] = ModbusHelper.High(cmd.RequestLength);
			request[5] = ModbusHelper.Low(cmd.RequestLength);			
			cmd.FillRequest(request, 6);
			return request;	
		}
	}
}
