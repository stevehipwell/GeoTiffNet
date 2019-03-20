using System;
using FakeItEasy;
using Xunit;

using GeoTiffNet;

namespace GeoTiffNetTest
{
  public class TiffTest
  {
    [Fact]
    public void TiffsCanBeLoaded()
    {
      using (var tiff = new Tiff(@"C:\Data\OS\OpenMap\TM\data\TM47SE.tif"))
      {
        var index = 0;
        foreach (var image in tiff.GetImages())
        {
          Console.WriteLine("----------------------------------------------------");
          Console.WriteLine("Image: {0}", index++);
          Console.WriteLine("  Fields:");

          foreach (var field in image.Fields)
          {
            Console.WriteLine("    {0}: {1}", field.Tag, field.ValueOffset);
          }

          Console.WriteLine("----------------------------------------------------");
        }
      }

      Assert.True(true);
    }
  }
}
