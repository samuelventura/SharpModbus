
using System;

namespace SharpModbus
{
	public interface ModbusCommand
	{
		ushort RequestLength { get; }
		ushort ResponseLength { get; }
		void FillRequest(byte[] request, int offset);
	}
}
