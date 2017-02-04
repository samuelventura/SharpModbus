using System;

namespace SharpModbus
{
	public class RtuModbusProtocol : IModbusProtocol
	{
		public ModbusCommand Wrap(ModbusCommand wrapped)
		{
			return new ModbusRTUWrapper(wrapped);
		}
	}
}

