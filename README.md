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

Setup 1
- Windows 10 Pro 64x VM
- VirtualBox 7.0.6 on Kubuntu 20.04
- VS Code 1.74.3
- .NET SDK 6.0.405
- com0com-2.2.2.0-x64-fre-signed COM98/99
- For Special Test subproject
  - FTDI USB-RS485-WE-1800-BT COM3
  - Comfile MD-H485+MD-DOSO8+5SlotBoard
  - CUI SWI25-24-N-P5
  - B&B BB-UH401 

Setup 2
- Mac mini - Ventura 13.1
- VS Code 1.74.3
- .NET SDK 6.0.405 + 7.0.2

Setup 3
- Kubuntu 20.04
- VS Code 1.74.2
- asdf dotnet 6.0.405

## Development CLI

```bash
#packing for nuget
dotnet clean SharpModbus -c Release
dotnet pack SharpModbus -c Release
#cross platform test cases
dotnet test SharpModbus.Test
#avoid complains on missing mono on MacOS/Linux
dotnet test SharpModbus.Test -f net6.0
#console output for test cases
dotnet test SharpModbus.Test -v n

#com0com based serial test for Windows
#com0com pair required as COM98 and COM99
#ignore yellow stderr and stdout
dotnet test SharpModbus.Test.Windows
#test in release to avoid yellow output
dotnet test SharpModbus.Test.Windows -c Release

#socat based serial test for MacOS/Linux
#brew install socat
#ignore yellow stderr and stdout
dotnet test SharpModbus.Test.Unix
#test in release to avoid yellow output
dotnet test SharpModbus.Test.Unix -c Release

#tests below require a hard to replicate environment
#do not bother unless really important to you

#comfile modport connected on COM10 see details inside
dotnet test SharpModbus.Test.Special --filter FullyQualifiedName~ComfileModportTest
#opto22 snappac connected on 10.77.0.2:502 see details inside
dotnet test SharpModbus.Test.Special --filter FullyQualifiedName~Opto22SnapPacTest
#compile check special tests
dotnet build SharpModbus.Test.Special
```

## Roadmap

- [x] Support Linux/MacOS as well
- [ ] Improve documentation and samples
- [ ] Support Modbus ASCII
