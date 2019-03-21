using System;
using System.Collections.Generic;

namespace GeoTiffNet
{
  public interface IGeoTiffImage : ITiffImage
  {
    IList<IGeoKey> GeoKeys { get; }
  }
}
