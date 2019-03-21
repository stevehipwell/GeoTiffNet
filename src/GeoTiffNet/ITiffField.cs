using System;

namespace GeoTiffNet
{
  public interface ITiffField
  {
    TiffTagEnum Tag { get; }

    TiffTagTypeEnum Type { get; }

    int Count { get; }

    byte[] Bytes { get; }

    ushort[] GetUInt16Values();

    double[] GetDoubleValues();
  }
}
