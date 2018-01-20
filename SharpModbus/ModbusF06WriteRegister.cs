using System;

namespace SharpModbus
{
	public class ModbusF06WriteRegister : IModbusCommand
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
		
		public object ParseResponse(byte[] response, int offset)
		{
			Assert.Equal(response[offset + 0], slave, "Slave mismatch {0} expected:{1}");
			Assert.Equal(response[offset + 1], 6, "Function mismatch {0} expected:{1}");
			Assert.Equal(ModbusHelper.GetUShort(response, offset + 2), address, "Address mismatch {0} expected:{1}");
			Assert.Equal(ModbusHelper.GetUShort(response, offset + 4), value, "Value mismatch {0} expected:{1}");
			return null;
		}
		
		public object ApplyTo(IModbusModel model)
		{
			model.setWO(slave, address, value);
			return null;
		}
		
		public void FillResponse(byte[] response, int offset, object value)
		{
			FillRequest(response, offset);
		}
		
		public override string ToString()
		{
			return string.Format("[ModbusF06WriteRegister Slave={0}, Address={1}, Value={2}]", slave, address, value);
		}
	}
}