
using System;

namespace SharpModbus
{
	public class ModbusF16WriteRegisters : ModbusCommand
	{
		private readonly byte slave;
		private readonly ushort address;
		private readonly ushort[] values;
		
		public byte Slave { get { return slave; } }
		public ushort Address { get { return address; } }
		public ushort[] Values { get { return ModbusHelper.Clone(values); } }
		public ushort RequestLength { get { return (ushort)(7 + ModbusHelper.BytesForWords(values.Length)); } }
		public ushort ResponseLength { get { return 6; } }
		
		public ModbusF16WriteRegisters(byte slave, ushort address, ushort[] values)
		{
			this.slave = slave;
			this.address = address;
			this.values = values;
		}
		
		public void FillRequest(byte[] request, int offset)
		{
			var bytes = ModbusHelper.EncodeWords(values);
			request[offset + 0] = slave;
			request[offset + 1] = 16;
			request[offset + 2] = ModbusHelper.High(address);
			request[offset + 3] = ModbusHelper.Low(address);
			request[offset + 4] = ModbusHelper.High((ushort)values.Length);
			request[offset + 5] = ModbusHelper.Low((ushort)values.Length);			
			request[offset + 6] = (byte)bytes.Length;
			ModbusHelper.Copy(bytes, 0, request, offset + 7, bytes.Length);
		}
	}
}
