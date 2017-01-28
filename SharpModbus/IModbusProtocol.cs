using System;

namespace SharpModbus
{
	public interface IModbusProtocol : IDisposable
	{
		byte[] Query(byte[] request, ushort length);
	}
}
