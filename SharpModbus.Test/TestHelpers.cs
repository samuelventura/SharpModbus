using System;
using System.Net;
using System.Text;
using System.IO.Ports;
using System.Net.Sockets;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using NUnit.Framework;

namespace SharpModbus.Test
{
    public static class H
    {
        public static ushort[] us(params ushort[] args)
        {
            return args;
        }

        public static bool[] bo(params bool[] args)
        {
            return args;
        }

        public static byte[] by(params byte[] args)
        {
            return args;
        }

        public static string Hex(byte[] data, int size)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < size; i++) sb.Append(data[i].ToString("X2"));
            return sb.ToString();
        }

        public static void SharedExceptionTest(ModbusMaster master)
        {
            master.ReadCoil(1, 2); //should except on empty model
        }

        public static void SharedSlaveTest(ModbusModel model, ModbusMaster master)
        {
            master.WriteCoil(1, 2, true);
            Assert.AreEqual(true, master.ReadCoil(1, 2));
            master.WriteCoil(1, 3, false);
            Assert.AreEqual(false, master.ReadCoil(1, 3));
            master.WriteCoils(1, 4, H.bo(false, true));
            Assert.AreEqual(H.bo(true, false, false, true), master.ReadCoils(1, 2, 4));

            //race condition avoided by access order
            model.setDIs(11, 12, H.bo(true, true, false, false));
            Assert.AreEqual(true, master.ReadInput(11, 12));
            Assert.AreEqual(true, master.ReadInput(11, 13));
            Assert.AreEqual(false, master.ReadInput(11, 14));
            Assert.AreEqual(false, master.ReadInput(11, 15));
            Assert.AreEqual(H.bo(true, true, false, false), master.ReadInputs(11, 12, 4));

            master.WriteRegister(1, 2, 0xabcd);
            Assert.AreEqual(0xabcd, master.ReadHoldingRegister(1, 2));
            master.WriteRegister(1, 3, 0xcdab);
            Assert.AreEqual(0xcdab, master.ReadHoldingRegister(1, 3));
            master.WriteRegisters(1, 4, H.us(0xcda1, 0xcda2));
            Assert.AreEqual(H.us(0xabcd, 0xcdab, 0xcda1, 0xcda2), master.ReadHoldingRegisters(1, 2, 4));

            //race condition avoided by access order
            model.setWIs(11, 12, H.us(0xabcd, 0xcdab, 0xcda1, 0xcda2));
            Assert.AreEqual(0xabcd, master.ReadInputRegister(11, 12));
            Assert.AreEqual(0xcdab, master.ReadInputRegister(11, 13));
            Assert.AreEqual(0xcda1, master.ReadInputRegister(11, 14));
            Assert.AreEqual(0xcda2, master.ReadInputRegister(11, 15));
            Assert.AreEqual(H.us(0xabcd, 0xcdab, 0xcda1, 0xcda2), master.ReadInputRegisters(11, 12, 4));
        }
    }

    public class TcpServerModel : IDisposable
    {
        private readonly int port;
        private readonly IModbusScanner scanner;
        private readonly ModbusModel model;
        private readonly TcpListener server;
        public int delay;

        public TcpServerModel(ModbusModel model, IModbusScanner scanner)
        {
            this.model = model;
            this.scanner = scanner;
            server = new TcpListener(IPAddress.Loopback, 0);
            server.Start();
            port = ((IPEndPoint)server.LocalEndpoint).Port;
            Task.Factory.StartNew(Run);
        }

        public int Port { get { return port; } }

        private void Run()
        {
            //use <dotnet test -v n> for console output
            var client = server.AcceptTcpClient();
            try { using (client) Process(client); }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }

        private void Process(TcpClient client)
        {
            var buffer = new byte[256];
            client.GetStream().ReadTimeout = -1;
            //excepts in Linux/macOS
            //client.ReceiveTimeout = -1;
            while (true)
            {
                var size = client.GetStream().Read(buffer, 0, buffer.Length);
                if (size <= 0) break;
                scanner.Append(buffer, 0, size);
                var cmd = scanner.Scan();
                if (cmd != null)
                {
                    var response = new byte[cmd.ResponseLength];
                    try
                    {
                        var value = cmd.ApplyTo(model);
                        cmd.FillResponse(response, 0, value);
                    }
                    catch (Exception)
                    {
                        response = cmd.GetException(2);
                    }
                    if (delay > 0) Thread.Sleep(delay);
                    client.GetStream().Write(response, 0, response.Length);
                }
            }
        }

        public void Dispose()
        {
            server.Stop();
        }
    }

    public class SerialModel : IDisposable
    {
        private readonly SerialPort serial;
        private readonly IModbusScanner scanner;
        private readonly ModbusModel model;

        public SerialModel(string port, ModbusModel model, IModbusScanner scanner)
        {
            this.model = model;
            this.scanner = scanner;
            serial = new SerialPort(port);
            serial.Open();
            Task.Factory.StartNew(Run);
        }

        private void Run()
        {
            //use <dotnet test -v n> for console output
            try { Process(); }
            catch (Exception ex) { Log(ex.ToString()); }
        }

        [Conditional("DEBUG")]
        private void Log(string line)
        {
            Console.WriteLine(line);
        }

        private void Process()
        {
            var buffer = new byte[256];
            serial.ReadTimeout = -1;
            while (true)
            {
                var size = serial.Read(buffer, 0, buffer.Length);
                if (size <= 0) break;
                scanner.Append(buffer, 0, size);
                var cmd = scanner.Scan();
                if (cmd != null)
                {
                    var response = new byte[cmd.ResponseLength];
                    try
                    {
                        var value = cmd.ApplyTo(model);
                        cmd.FillResponse(response, 0, value);
                    }
                    catch (Exception)
                    {
                        response = cmd.GetException(2);
                    }
                    serial.Write(response, 0, response.Length);
                }
            }
        }

        public void Dispose()
        {
            serial.Close();
        }
    }

    public class StreamModel : IModbusStream
    {
        private readonly ModbusModel model;
        private readonly IModbusScanner scanner;
        private readonly List<byte> buffer = new List<byte>();

        public StreamModel(ModbusModel model, IModbusScanner scanner)
        {
            this.model = model;
            this.scanner = scanner;
        }

        public void Write(byte[] data)
        {
            scanner.Append(data, 0, data.Length);
            var cmd = scanner.Scan();
            if (cmd != null)
            {
                var response = new byte[cmd.ResponseLength];
                try
                {
                    var value = cmd.ApplyTo(model);
                    cmd.FillResponse(response, 0, value);
                }
                catch (Exception)
                {
                    response = cmd.GetException(2);
                }
                buffer.AddRange(response);
            }
        }

        public int Read(byte[] data)
        {
            var size = (int)Math.Min(buffer.Count, data.Length);
            for (var i = 0; i < size; i++) data[i] = buffer[i];
            buffer.RemoveRange(0, size);
            return size;
        }

        public void Dispose()
        {
        }
    }
}
