using System;

namespace GeoTiffNet
{
  public interface IGeoKey
  {
    GeoKeyEnum Tag { get; }

    int? Int32Value { get; }

    double? DoubleValue { get; }

    string StringValue { get; }
  }
}
