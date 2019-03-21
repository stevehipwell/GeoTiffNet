using System;

namespace GeoTiffNet
{
  public interface ITiffHeader
  {
    bool IsGeoTiff { get; }

    TiffByteOrderEnum ByteOrder { get; }

    IByteHandler ByteHandler { get; }

    ushort VersionNumber { get; }

    long FirstImageOffset { get; }
  }
}
