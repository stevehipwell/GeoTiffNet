using System;

namespace GeoTiffNet
{
  public interface ITiffField
  {
    TiffTagEnum Tag { get; }

    TiffTagTypeEnum Type { get; }

    int Count { get; }

    uint ValueOffset { get; }

    ushort GetUInt16Value();

    ushort[] GetUInt16Values();

    double GetDoubleValue();

    double[] GetDoubleValues();

    ReadOnlySpan<byte> GetBytes();
  }
}
