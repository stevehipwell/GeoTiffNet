using System;

namespace GeoTiffNet
{
  public interface ITiffHeader
  {
    bool IsGeoTiff { get; }
    TiffByteOrderEnum ByteOrder { get; }

    IEndianHandler ByteHandler { get; }

    ushort VersionNumber { get; }

    long FirstImageOffset { get; }
  }
}
