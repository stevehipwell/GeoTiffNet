using System;
using System.Collections.Generic;

namespace GeoTiffNet
{
  public interface ITiffImage
  {
    IList<ITiffField> Fields { get; }
  }
}
