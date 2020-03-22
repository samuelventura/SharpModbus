using System;
using System.IO.Ports;

namespace SharpModbus
{
    public class ModbusMaster : IDisposable
    {
        public static ModbusMaster RTU(SerialPort serial, int timeout = 400)
        {
            if (!serial.IsOpen) serial.Open();
            var stream = new ModbusSerialStream(serial, timeout);
            var protocol = new ModbusRTUProtocol();
            return new ModbusMaster(stream, protocol);
        }

        public static ModbusMaster TCP(string ip, int port, int timeout = 400)
        {
            var socket = TcpTools.ConnectWithTimeout(ip, port, timeout);
            var stream = new ModbusSocketStream(socket, timeout);
            var protocol = new ModbusTCPProtocol();
            return new ModbusMaster(stream, protocol);
        }

        private readonly IModbusProtocol protocol;
        private readonly IModbusStream stream;

        public ModbusMaster(IModbusStream stream, IModbusProtocol protocol)
        {
            this.stream = stream;
            this.protocol = protocol;
        }

        public void Dispose()
        {
            Disposer.Dispose(stream);
        }

        public bool ReadCoil(byte slave, ushort address)
        {
            return ReadCoils(slave, address, 1)[0]; //there is no code for single read
        }

        public bool ReadInput(byte slave, ushort address)
        {
            return ReadInputs(slave, address, 1)[0]; //there is no code for single read
        }

        public ushort ReadInputRegister(byte slave, ushort address)
        {
            return ReadInputRegisters(slave, address, 1)[0]; //there is no code for single read
        }

        public ushort ReadHoldingRegister(byte slave, ushort address)
        {
            return ReadHoldingRegisters(slave, address, 1)[0]; //there is no code for single read
        }

        public bool[] ReadCoils(byte slave, ushort address, ushort count)
        {
            return Execute(new ModbusF01ReadCoils(slave, address, count)) as bool[];
        }

        public bool[] ReadInputs(byte slave, ushort address, ushort count)
        {
            return Execute(new ModbusF02ReadInputs(slave, address, count)) as bool[];
        }

        public ushort[] ReadInputRegisters(byte slave, ushort address, ushort count)
        {
            return Execute(new ModbusF04ReadInputRegisters(slave, address, count)) as ushort[];
        }

        public ushort[] ReadHoldingRegisters(byte slave, ushort address, ushort count)
        {
            return Execute(new ModbusF03ReadHoldingRegisters(slave, address, count)) as ushort[];
        }

        public void WriteCoil(byte slave, ushort address, bool value)
        {
            Execute(new ModbusF05WriteCoil(slave, address, value));
        }

        public void WriteRegister(byte slave, ushort address, ushort value)
        {
            Execute(new ModbusF06WriteRegister(slave, address, value));
        }

        public void WriteCoils(byte slave, ushort address, bool[] values)
        {
            Execute(new ModbusF15WriteCoils(slave, address, values));
        }

        public void WriteRegisters(byte slave, ushort address, ushort[] values)
        {
            Execute(new ModbusF16WriteRegisters(slave, address, values));
        }

        private object Execute(IModbusCommand cmd)
        {
            var wrapper = protocol.Wrap(cmd);
            var request = new byte[wrapper.RequestLength];
            var response = new byte[wrapper.ResponseLength];
            wrapper.FillRequest(request, 0);
            stream.Write(request);
            var count = stream.Read(response);
            if (count < response.Length) wrapper.CheckException(response, count);
            return wrapper.ParseResponse(response, 0);
        }
    }
}