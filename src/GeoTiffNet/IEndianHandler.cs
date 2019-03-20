using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Buffers.Binary;

namespace GeoTiffNet
{
  public interface IEndianHandler
  {
    ushort ReadUInt16(ReadOnlySpan<byte> bytes);

    uint ReadUInt32(ReadOnlySpan<byte> bytes);

    ReadOnlySpan<byte> ToByteArray(uint value);
  }
}
