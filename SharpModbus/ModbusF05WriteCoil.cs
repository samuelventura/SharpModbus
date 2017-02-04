using System;

namespace SharpModbus
{
	public class ModbusF05WriteCoil : IModbusCommand
	{
		private readonly byte slave;
		private readonly ushort address;
		private readonly bool value;
		
		public byte Slave { get { return slave; } }
		public ushort Address { get { return address; } }
		public bool Value { get { return value; } }
		public int RequestLength { get { return 6; } }
		public int ResponseLength { get { return 6; } }
		
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
			request[offset + 4] = ModbusHelper.EncodeBool(value);
			request[offset + 5] = 0;			
		}
		
		public object ParseResponse(byte[] response, int offset)
		{
			Assert.Equal(response[offset + 0], slave, "Slave mismatch {0} expected:{1}");
			Assert.Equal(response[offset + 1], 5, "Function mismatch {0} expected:{1}");
			Assert.Equal(ModbusHelper.GetUShort(response, offset + 2), address, "Address mismatch {0} expected:{1}");
			Assert.Equal(response[offset + 4], ModbusHelper.EncodeBool(value), "Value mismatch {0} expected:{1}");
			Assert.Equal(response[offset + 5], 0, "Pad mismatch {0} expected:{1}");
			return null;
		}
		
		public override string ToString()
		{
			return string.Format("[ModbusF05WriteCoil Slave={0}, Address={1}, Value={2}]", slave, address, value);
		}
	}
}
