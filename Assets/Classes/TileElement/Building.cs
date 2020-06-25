using System.Drawing;
using UnityEngine;

public struct Building : ITileElement {
  public string Name { get; }
  public Size Size { get; }

  private readonly Vector3 _positionOffset;
  private readonly float _scaleFactor;

  public Building(string name, Size size, float scaleFactor) {
    this.Name = name;
    this.Size = size;
    this._positionOffset = new Vector3(size.Width * 0.5f, 0, size.Height * 0.5f);
    this._scaleFactor = scaleFactor;
  }

  public GameObject Create(Vector3 location, Rotation rotation) {
    location += this._positionOffset;
    var gObject = (GameObject)Object.Instantiate(Resources.Load("Buildings/" + this.Name), location, Quaternion.identity);
    gObject.transform.localScale *= this._scaleFactor;
    gObject.transform.Rotate(Vector3.up * (int)rotation);

    return gObject;
  }
}