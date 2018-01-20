using System;

namespace SharpModbus
{
	public class ModbusF02ReadInputs : IModbusCommand
	{
		private readonly byte slave;
		private readonly ushort address;
		private readonly ushort count;
		
		public byte Slave { get { return slave; } }
		public ushort Address { get { return address; } }
		public ushort Count { get { return count; } }
		public int RequestLength { get { return 6; } }
		public int ResponseLength { get { return 3 + ModbusHelper.BytesForBools(count); } }
		
		public ModbusF02ReadInputs(byte slave, ushort address, ushort count)
		{
			this.slave = slave;
			this.address = address;
			this.count = count;
		}
		
		public void FillRequest(byte[] request, int offset)
		{
			request[offset + 0] = slave;
			request[offset + 1] = 2;
			request[offset + 2] = ModbusHelper.High(address);
			request[offset + 3] = ModbusHelper.Low(address);
			request[offset + 4] = ModbusHelper.High(count);
			request[offset + 5] = ModbusHelper.Low(count);			
		}
		
		public object ParseResponse(byte[] response, int offset)
		{
			var bytes = ModbusHelper.BytesForBools(count);
			Assert.Equal(response[offset + 0], slave, "Slave mismatch {0} expected:{1}");
			Assert.Equal(response[offset + 1], 2, "Function mismatch {0} expected:{1}");
			Assert.Equal(response[offset + 2], bytes, "Bytes mismatch {0} expected:{1}");
			return ModbusHelper.DecodeBools(response, offset + 3, count);
		}
		
		public object ApplyTo(IModbusModel model)
		{
			return model.getDIs(slave, address, count);
		}
		
		public void FillResponse(byte[] response, int offset, object value)
		{
			var bytes = ModbusHelper.BytesForBools(count);
			response[offset + 0] = slave;
			response[offset + 1] = 2;
			response[offset + 2] = bytes;
			var data = ModbusHelper.EncodeBools((bool[])value);
			ModbusHelper.Copy(data, 0, response, offset + 3, bytes);
		}
		
		public override string ToString()
		{
			return string.Format("[ModbusF02ReadInputs Slave={0}, Address={1}, Count={2}]", slave, address, count);
		}
	}
}