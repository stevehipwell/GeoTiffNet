using System;

namespace GeoTiffNet
{
  public enum GeoKeyLocationEnum : ushort
  {
    Inline = 0,
    DoubleParams = TiffTagEnum.GeoDoubleParams,
    AsciiParams = TiffTagEnum.GeoAsciiParams
  }
}
