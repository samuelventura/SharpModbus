using System;

namespace SharpModbus
{	
	public interface IModbusModel
	{
		void setDI(byte slave, ushort address, bool value);
		
		void setDIs(byte slave, ushort address, bool[] values);
		
		bool getDI(byte slave, ushort address);
		
		bool[] getDIs(byte slave, ushort address, int count);
		
		void setDO(byte slave, ushort address, bool value);
		
		void setDOs(byte slave, ushort address, bool[] values);
		
		bool getDO(byte slave, ushort address);
		
		bool[] getDOs(byte slave, ushort address, int count);

		void setWI(byte slave, ushort address, ushort value);
		
		void setWIs(byte slave, ushort address, ushort[] values);
		
		ushort getWI(byte slave, ushort address);
		
		ushort[] getWIs(byte slave, ushort address, int count);
		
		void setWO(byte slave, ushort address, ushort value);
		
		void setWOs(byte slave, ushort address, ushort[] values);
		
		ushort getWO(byte slave, ushort address);
		
		ushort[] getWOs(byte slave, ushort address, int count);
	}
}
