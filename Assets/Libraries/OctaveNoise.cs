using SimplexNoise;

namespace Libaries {
  public static class OctaveNoise {

    public static bool NormalizeValues { get; set; } = true;
    public static int MaxValue { get; set; } = 255;

    /// <summary>
    /// Creates a noise map of layered simplex noise
    /// </summary>
    /// <param name="width">The width of the map.</param>
    /// <param name="height">The height of the map.</param>
    /// <param name="scale">How big the noise is gonna be scaled.</param>
    /// <param name="octaves">The amount of noise layers.</param>
    /// <param name="persistance">The change of impact/ amplitude each noise layer has (higher value = more impact).</param>
    /// <param name="lacunarity">The change of scale ich noise layer has (higher value = bigger scale).</param>
    /// <param name="offsetX">The x-offset of the map.</param>
    /// <param name="offsetY">The y-offset of the map.</param>
    /// <returns></returns>
    public static float[,] GenerateMap(
      int width, int height, int offsetX = 0, int offsetY = 0, int seed = 0, float scale = 1, int octaves = 4, float persistance = 0.5f,
      float lacunarity = 2f) {

      var map = new float[width, height];
      Noise.Seed = seed;

      for (var y = 0; y < height; ++y)
        for (var x = 0; x < width; ++x) {
          var value = 0F;
          var frequency = 1F;
          var amplitude = 1F;
          var posX = x + offsetX;
          var posY = y + offsetY;

          for (var i = 0; i < octaves; ++i) {
            var noise = Noise.CalcPixel2D(posX, posY, scale * frequency) * amplitude;

            value += noise;

            amplitude *= persistance;
            frequency *= lacunarity;
          }

          map[x, y] = value;
        }

      //normalize
      if (NormalizeValues)
        for (var y = 0; y < height; ++y)
        for (var x = 0; x < width; ++x) {
            var value = _Normalize(map[x, y], 0, 417) * MaxValue;
            map[x, y] = value;
        }

      return map;
    }

    private static float _Normalize(float value, float minValue, float maxValue) => (value - minValue) / (maxValue - minValue);

  }
}