using System;

namespace SharpModbus
{
	public interface IModbusProtocol
	{
		ModbusCommand Wrap(ModbusCommand wrapped);
	}
}
