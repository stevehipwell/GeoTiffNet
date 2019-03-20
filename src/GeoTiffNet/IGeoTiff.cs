using System;
using System.Collections.Generic;

namespace GeoTiffNet
{
  public interface IGeoTiff
  {
    int GetImageCount();

    IList<IGeoTiffImage> GetImages();

    IGeoTiffImage GetImage(int index);
  }
}
