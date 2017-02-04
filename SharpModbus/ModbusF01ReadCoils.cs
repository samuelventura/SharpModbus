
using System;

namespace SharpModbus
{
	public class ModbusF01ReadCoils : ModbusCommand
	{
		private readonly byte slave;
		private readonly ushort address;
		private readonly ushort count;
		
		public byte Slave { get { return slave; } }
		public ushort Address { get { return address; } }
		public ushort Count { get { return count; } }
		public ushort RequestLength { get { return 6; } }
		public ushort ResponseLength { get { return (ushort)(3 + ModbusHelper.BytesForBools(count)); } }
		
		public ModbusF01ReadCoils(byte slave, ushort address, ushort count)
		{
			this.slave = slave;
			this.address = address;
			this.count = count;
		}
		
		public void FillRequest(byte[] request, int offset)
		{
			request[offset + 0] = slave;
			request[offset + 1] = 01;
			request[offset + 2] = ModbusHelper.High(address);
			request[offset + 3] = ModbusHelper.Low(address);
			request[offset + 4] = ModbusHelper.High(count);
			request[offset + 5] = ModbusHelper.Low(count);			
		}
	}
}