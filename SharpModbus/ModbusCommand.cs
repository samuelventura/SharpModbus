
using System;

namespace SharpModbus
{
	public interface ModbusCommand
	{
		int RequestLength { get; }
		int ResponseLength { get; }
		void FillRequest(byte[] request, int offset);
		object ParseResponse(byte[] request, byte[] response, int offset);
	}
}
