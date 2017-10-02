using System;
using System.Collections.Generic;

namespace SharpModbus
{
	public enum ModbusIoType
	{
		DI,
		DO,
		WO,
		WI
	}
	
	public class ModbusModel
	{
		private readonly IDictionary<string, bool> digitals = new Dictionary<string, bool>();
		private readonly IDictionary<string, ushort> words = new Dictionary<string, ushort>();
		
		public void setDI(byte slave, ushort address, bool value)
		{
			digitals[Key(ModbusIoType.DI, slave, address)] = value;
		}
		
		public void setDIs(byte slave, ushort address, bool[] values)
		{
			for (var i = 0; i < values.Length; i++)
				digitals[Key(ModbusIoType.DI, slave, address + i)] = values[i];
		}
		
		public bool getDI(byte slave, ushort address)
		{
			return digitals[Key(ModbusIoType.DI, slave, address)];
		}
		
		public bool[] getDIs(byte slave, ushort address, int count)
		{
			var values = new bool[count];
			for (var i = 0; i < values.Length; i++)
				values[i] = digitals[Key(ModbusIoType.DI, slave, address+i)];
			return values;
		}
		
		public void setDO(byte slave, ushort address, bool value)
		{
			digitals[Key(ModbusIoType.DO, slave, address)] = value;
		}
		
		public void setDOs(byte slave, ushort address, bool[] values)
		{
			for (var i = 0; i < values.Length; i++)
				digitals[Key(ModbusIoType.DO, slave, address + i)] = values[i];
		}
		
		public bool getDO(byte slave, ushort address)
		{
			return digitals[Key(ModbusIoType.DO, slave, address)];
		}
		
		public bool[] getDOs(byte slave, ushort address, int count)
		{
			var values = new bool[count];
			for (var i = 0; i < values.Length; i++)
				values[i] = digitals[Key(ModbusIoType.DO, slave, address+i)];
			return values;
		}

		public void setWI(byte slave, ushort address, ushort value)
		{
			words[Key(ModbusIoType.WI, slave, address)] = value;
		}
		
		public void setWIs(byte slave, ushort address, ushort[] values)
		{
			for (var i = 0; i < values.Length; i++)
				words[Key(ModbusIoType.WI, slave, address + i)] = values[i];
		}
		
		public ushort getWI(byte slave, ushort address)
		{
			return words[Key(ModbusIoType.WI, slave, address)];
		}
		
		public ushort[] getWIs(byte slave, ushort address, int count)
		{
			var values = new ushort[count];
			for (var i = 0; i < values.Length; i++)
				values[i] = words[Key(ModbusIoType.WI, slave, address+i)];
			return values;
		}
		
		public void setWO(byte slave, ushort address, ushort value)
		{
			words[Key(ModbusIoType.WO, slave, address)] = value;
		}
		
		public void setWOs(byte slave, ushort address, ushort[] values)
		{
			for (var i = 0; i < values.Length; i++)
				words[Key(ModbusIoType.WO, slave, address + i)] = values[i];
		}
		
		public ushort getWO(byte slave, ushort address)
		{
			return words[Key(ModbusIoType.WO, slave, address)];
		}
		
		public ushort[] getWOs(byte slave, ushort address, int count)
		{
			var values = new ushort[count];
			for (var i = 0; i < values.Length; i++)
				values[i] = words[Key(ModbusIoType.WO, slave, address+i)];
			return values;
		}
		
		private string Key(ModbusIoType type, byte slave, int address)
		{
			return string.Format("{0},{1},{2}", slave, type, address);
		}
	}
}
