using System;
using System.Collections.Generic;
using GeoAPI.Geometries;

namespace GeoTiffNet
{
  public interface IGeoTiffImage : ITiffImage
  {
    IList<IGeoKey> GeoKeys { get; }

    Envelope GetBounds();
  }
}
