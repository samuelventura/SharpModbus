﻿
using System;

namespace SharpModbus
{
	public interface IModbusWrapper : IModbusCommand
	{
		IModbusCommand Wrapped { get; }
	}
}
