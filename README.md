# SharpModbus

C# Modbus Tools

## Quick Start
```csharp
//Modbus RTU over serial
var master = ModbusMaster.RTU(new SerialPort()
{
    PortName = "COM10",
    BaudRate = 57600,
});
master.WriteCoil(1, 3000, false);
//Modbus TCP over socket
var master = ModbusMaster.TCP("10.77.0.2", 502);
master.WriteCoils(1, 4, new bool[] { false, true });
```

## Documentation

No documentation yet. Resort to tests at SharpModbusTest and SharpModbusSpecialTest subprojects for guidance. A couple of real device communication samples are available there.

[SharpMaster](https://github.com/samuelventura/SharpMaster) uses SharpModbus too.

## Development Setup

- Windows 10 Pro 64x / macOS 10.13.2
- VS Code (bash terminal from Git4Win)
- Net Core SDK 3.1.200
- dotnet CLI

## Development CLI

```bash
#packing for nuget
dotnet clean -c Release
dotnet pack SharpModbus -c Release
#cross platform test cases
dotnet test SharpModbusTest
#compile check special tests
dotnet build SharpModbusSpecialTest
#com0com test cases need COM98 and COM99
dotnet test SharpModbusSpecialTest --filter FullyQualifiedName~Com0ComSlaveTest
#comfile modport connected on COM10 see details inside
dotnet test SharpModbusSpecialTest --filter FullyQualifiedName~ComfileModportTest
#opto22 snappac connected on 10.77.0.2:502 see details inside
dotnet test SharpModbusSpecialTest --filter FullyQualifiedName~Opto22SnapPacTest
```

## Multiplatform

- 1.0.6 at nuget confirmed to work on MacOS/Ubuntu with .Net Core 2.0.2
- 1.0.6 at nuget confirmed to work on Windows10 with NetFramework40/.Net Core 2.0.2

## TODO

- [ ] Improve documentation and samples
- [ ] Support Modbus ASCII