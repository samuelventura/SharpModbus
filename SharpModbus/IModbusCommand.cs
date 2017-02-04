
using System;

namespace SharpModbus
{
	public interface IModbusCommand
	{
		int RequestLength { get; }
		int ResponseLength { get; }
		void FillRequest(byte[] request, int offset);
		object ParseResponse(byte[] response, int offset);
		object ApplyTo(ModbusModel model);
	}
}
