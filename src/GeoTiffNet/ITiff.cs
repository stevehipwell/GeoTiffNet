using System;
using System.Collections.Generic;

namespace GeoTiffNet
{
  public interface ITiff
  {
    int GetImageCount();

    IList<ITiffImage> GetImages();

    ITiffImage GetImage(int index);
  }
}
