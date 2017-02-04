
using System;

namespace SharpModbus
{
	public class ModbusF05WriteCoil : ModbusCommand
	{
		private readonly byte slave;
		private readonly ushort address;
		private readonly bool value;
		
		public byte Slave { get { return slave; } }
		public ushort Address { get { return address; } }
		public bool Value { get { return value; } }
		public ushort RequestLength { get { return 6; } }
		public ushort ResponseLength { get { return 6; } }
		
		public ModbusF05WriteCoil(byte slave, ushort address, bool state)
		{
			this.slave = slave;
			this.address = address;
			this.value = state;
		}
		
		public void FillRequest(byte[] request, int offset)
		{
			request[offset + 0] = slave;
			request[offset + 1] = 5;
			request[offset + 2] = ModbusHelper.High(address);
			request[offset + 3] = ModbusHelper.Low(address);
			request[offset + 4] = (byte)(value ? 0xFF : 0x00);
			request[offset + 5] = 0;			
		}
	}
}
