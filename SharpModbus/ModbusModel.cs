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

    public class ModbusModel : IModbusModel
    {
        private readonly IDictionary<string, bool> digitals = new Dictionary<string, bool>();
        private readonly IDictionary<string, ushort> words = new Dictionary<string, ushort>();

        public void setDI(byte slave, ushort address, bool value)
        {
            var key = Key(ModbusIoType.DI, slave, address);
            digitals[key] = value;
        }

        public void setDIs(byte slave, ushort address, bool[] values)
        {
            for (var i = 0; i < values.Length; i++)
            {
                var key = Key(ModbusIoType.DI, slave, address + i);
                digitals[key] = values[i];
            }
        }

        public bool getDI(byte slave, ushort address)
        {
            var key = Key(ModbusIoType.DI, slave, address);
            return digitals[key];
        }

        public bool[] getDIs(byte slave, ushort address, int count)
        {
            var values = new bool[count];
            for (var i = 0; i < values.Length; i++)
            {
                var key = Key(ModbusIoType.DI, slave, address + i);
                values[i] = digitals[key];
            }
            return values;
        }

        public void setDO(byte slave, ushort address, bool value)
        {
            var key = Key(ModbusIoType.DO, slave, address);
            digitals[key] = value;
        }

        public void setDOs(byte slave, ushort address, bool[] values)
        {
            for (var i = 0; i < values.Length; i++)
            {
                var key = Key(ModbusIoType.DO, slave, address + i);
                digitals[key] = values[i];
            }
        }

        public bool getDO(byte slave, ushort address)
        {
            var key = Key(ModbusIoType.DO, slave, address);
            return digitals[key];
        }

        public bool[] getDOs(byte slave, ushort address, int count)
        {
            var values = new bool[count];
            for (var i = 0; i < values.Length; i++)
            {
                var key = Key(ModbusIoType.DO, slave, address + i);
                values[i] = digitals[key];
            }
            return values;
        }

        public void setWI(byte slave, ushort address, ushort value)
        {
            var key = Key(ModbusIoType.WI, slave, address);
            words[key] = value;
        }

        public void setWIs(byte slave, ushort address, ushort[] values)
        {
            for (var i = 0; i < values.Length; i++)
            {
                var key = Key(ModbusIoType.WI, slave, address + i);
                words[key] = values[i];
            }
        }

        public ushort getWI(byte slave, ushort address)
        {
            var key = Key(ModbusIoType.WI, slave, address);
            return words[key];
        }

        public ushort[] getWIs(byte slave, ushort address, int count)
        {
            var values = new ushort[count];
            for (var i = 0; i < values.Length; i++)
            {
                var key = Key(ModbusIoType.WI, slave, address + i);
                values[i] = words[key];
            }
            return values;
        }

        public void setWO(byte slave, ushort address, ushort value)
        {
            var key = Key(ModbusIoType.WO, slave, address);
            words[key] = value;
        }

        public void setWOs(byte slave, ushort address, ushort[] values)
        {
            for (var i = 0; i < values.Length; i++)
            {
                var key = Key(ModbusIoType.WO, slave, address + i);
                words[key] = values[i];
            }
        }

        public ushort getWO(byte slave, ushort address)
        {
            var key = Key(ModbusIoType.WO, slave, address);
            return words[key];
        }

        public ushort[] getWOs(byte slave, ushort address, int count)
        {
            var values = new ushort[count];
            for (var i = 0; i < values.Length; i++)
            {
                var key = Key(ModbusIoType.WO, slave, address + i);
                values[i] = words[key];
            }
            return values;
        }

        private string Key(ModbusIoType type, byte slave, int address)
        {
            return string.Format("{0},{1},{2}", slave, type, address);
        }
    }
}
