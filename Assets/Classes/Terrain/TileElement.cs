using System.Drawing;
using UnityEngine;

public struct TileElement {
  public string Name;
  public Size Size;
  private Vector3 PositionOffset;
  private readonly float ScaleFactor;

  public TileElement(string name, Size size, Vector3 positionOffset, float scaleFactor) {
    this.Name = name;
    this.Size = size;
    this.PositionOffset = positionOffset;
    this.ScaleFactor = scaleFactor;
  }

  public GameObject Create(Vector3 location, Rotation rotation) {
    location += this.PositionOffset;
    var gObject = (GameObject)Object.Instantiate(Resources.Load("Buildings/" + this.Name), location, Quaternion.identity);
    gObject.transform.localScale *= this.ScaleFactor;
    gObject.transform.Rotate(Vector3.up * (int)rotation);

    return gObject;
  }
}