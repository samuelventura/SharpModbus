
using System;

namespace SharpModbus
{
	public class ModbusF06WriteRegister : ModbusCommand
	{
		private readonly byte slave;
		private readonly ushort address;
		private readonly ushort value;
		
		public byte Slave { get { return slave; } }
		public ushort Address { get { return address; } }
		public ushort Value { get { return value; } }
		public int RequestLength { get { return 6; } }
		public int ResponseLength { get { return 6; } }
		
		public ModbusF06WriteRegister(byte slave, ushort address, ushort value)
		{
			this.slave = slave;
			this.address = address;
			this.value = value;
		}
		
		public void FillRequest(byte[] request, int offset)
		{
			request[offset + 0] = slave;
			request[offset + 1] = 6;
			request[offset + 2] = ModbusHelper.High(address);
			request[offset + 3] = ModbusHelper.Low(address);
			request[offset + 4] = ModbusHelper.High(value);
			request[offset + 5] = ModbusHelper.Low(value);			
		}
	}
}