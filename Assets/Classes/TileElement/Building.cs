using System.Drawing;
using UnityEngine;
  public class Building {
  public GameObject GObject { get; }
  public BuildingInfo Info { get; }
  public Rotation Rotation { get; }
  public Size Size { get; }

  public Vector3 Location => GObject.transform.position;

  public Building (BuildingInfo info, Vector3 location, Rotation rotation) {
    this.Info = info;
    this.Rotation = rotation;

    this.Size = this.Info.GetRotatedSize(rotation);

    var gObject = (GameObject)Object.Instantiate(Resources.Load("Buildings/" + info.Name), location + info.PositionOffset, Quaternion.identity);
    gObject.transform.localScale *= info.ScaleFactor;
    gObject.transform.Rotate(Vector3.up * (int)rotation);
    this.GObject = gObject;
  }
}