using UnityEngine;

namespace Assets.Classes {
  public class SpriteSelector {
    private readonly double _factorX;
    private readonly double _factorY;
    private const float _correctionOffset = 0.005f;
    private const float _correctionOffset2 = _correctionOffset * 2;

    public SpriteSelector(int amountX, int amountY) {
      this._factorX = (double)1 / amountX;
      this._factorY = (double)1 / amountY;
    }

    //returns the uvs for sprite xy
    public Vector2[] GetUVs(int x, int y) {
      var factorX = this._factorX;
      var factorY = this._factorY;

      var xStart = (float)(x * factorX) + _correctionOffset;
      var yStart = (float)(y * factorY) + _correctionOffset;
      var xEnd = (float)((x + 1) * factorX) - _correctionOffset2;
      var yEnd = (float)((y + 1) * factorY) - _correctionOffset2;
      
      var uvs = new Vector2[4];
      uvs[0] = new Vector2(xStart, yStart);
      uvs[1] = new Vector2(xEnd, yStart);
      uvs[2] = new Vector2(xStart, yEnd);
      uvs[3] = new Vector2(xEnd, yEnd);

      return uvs;
    }

  }
}
