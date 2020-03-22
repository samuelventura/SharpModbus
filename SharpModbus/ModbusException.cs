using System;

namespace SharpModbus
{
    public class ModbusException : Exception
    {
        private readonly byte code;

        public byte Code { get { return code; } }

        public ModbusException(byte code) :
            base(string.Format("Modbus exception {0}", code))
        {
            this.code = code;
        }
    }
}
