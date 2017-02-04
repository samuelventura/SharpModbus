
using System;

namespace SharpModbus
{
	public interface ModbusCommand
	{
		int RequestLength { get; }
		int ResponseLength { get; }
		void FillRequest(byte[] request, int offset);
	}
}
