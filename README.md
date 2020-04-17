# SharpModbus

C# Modbus Tools

## Quick Start

```csharp
var settings = new SerialSettings()
{
    PortName = "COM3",
    BaudRate = 57600,
};
//Modbus RTU over serial
using (var master = ModbusMaster.RTU(settings))
{
    master.WriteCoil(1, 3000, false);
    master.WriteCoils(1, 3001, false, true);
}
//Modbus RTU over isolated serial
using (var master = ModbusMaster.IsolatedRTU(settings))
{
    master.WriteCoil(1, 3000, false);
    master.WriteCoils(1, 3001, false, true);
}
//Modbus TCP over socket
using (var master = ModbusMaster.TCP("10.77.0.2", 502))
{
    master.WriteCoils(1, 4, false, true);
}
```

## Documentation

No documentation yet. Resort to tests at SharpModbus.Test and SharpModbus.Test.Special subprojects for guidance. A couple of real device communication samples are available there.

[SharpMaster](https://github.com/samuelventura/SharpMaster) uses SharpModbus too.

## Development Setup

- Windows 10 Pro 64x (Windows only)
- VS Code (bash terminal from Git4Win)
- Net Core SDK 3.1.201
- com0com-2.2.2.0-x64-fre-signed COM98/99
- For Comfile Modport Test
  - FTDI USB-RS485-WE-1800-BT COM3
  - Comfile MD-H485+MD-DOSO8+5SlotBoard
  - CUI SWI25-24-N-P5
  - B&B BB-UH401 

## Development CLI

```bash
#packing for nuget
dotnet clean SharpModbus -c Release
dotnet pack SharpModbus -c Release
#cross platform test cases
dotnet test SharpModbus.Test
#console output for test cases
dotnet test SharpModbus.Test -v n
#com0com test cases need COM98 and COM99
dotnet test SharpModbus.Test.Special --filter FullyQualifiedName~Com0ComSlaveTest
#comfile modport connected on COM10 see details inside
dotnet test SharpModbus.Test.Special --filter FullyQualifiedName~ComfileModportTest
#opto22 snappac connected on 10.77.0.2:502 see details inside
dotnet test SharpModbus.Test.Special --filter FullyQualifiedName~Opto22SnapPacTest
#compile check special tests
dotnet build SharpModbus.Test.Special
```

## TODO

- [ ] Support Linux / macOS
- [ ] Improve documentation and samples
- [ ] Support Modbus ASCII