using System;

namespace GeoTiffNet
{
  public interface ITiffField
  {
    TiffTagEnum Tag { get; }

    TiffTagTypeEnum Type { get; }

    int Count { get; }

    byte[] Bytes { get; }

    string GetAsciiValue();

    ushort[] GetShortValues();

    uint[] GetLongValues();

    Tuple<uint, uint>[] GetRationalValues();

    sbyte[] GetSByteValues();

    byte[] GetUndefinedValues();

    short[] GetSShortValues();

    int[] GetSLongValues();

    Tuple<int, int>[] GetSRationalValues();

    float[] GetFloatValues();

    double[] GetDoubleValues();
  }
}
