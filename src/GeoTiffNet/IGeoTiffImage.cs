using System;
using System.Collections.Generic;

namespace GeoTiffNet
{
  public interface IGeoTiffImage : ITiffImage
  {
    int Version { get; }

    IList<IGeoKey> GeoKeys { get; }
  }
}
