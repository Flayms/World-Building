using UnityEngine;

public struct TileElement {
  public string Name;
  private Vector3 PositionOffset;
  private readonly float ScaleFactor;

  public TileElement(string name, Vector3 positionOffset, float scaleFactor) {
    this.Name = name;
    this.PositionOffset = positionOffset;
    this.ScaleFactor = scaleFactor;
  }

  public GameObject Create(Vector3 location, Rotation rotation) {
    location += this.PositionOffset;
    var gObject = (GameObject)Object.Instantiate(Resources.Load(this.Name), location, Quaternion.identity);
    gObject.transform.localScale *= this.ScaleFactor;
    gObject.transform.Rotate(Vector3.up * (int)rotation);

    return gObject;
  }
}

public enum Rotation {
  North = 0,
  East = 90,
  South = 180,
  West = 270
}
