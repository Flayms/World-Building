using System.Drawing;
using UnityEngine;

//provides general information about one type of bulding
public struct BuildingInfo {
  public string Name { get; }
  public Size StandardSize { get; }
  public Vector3 PositionOffset { get; }
  public float ScaleFactor { get; }

  public BuildingInfo(string name, Size size, float scaleFactor) {
    this.Name = name;
    this.StandardSize = size;
    this.PositionOffset = new Vector3(size.Width * 0.5f, 0, size.Height * 0.5f);
    this.ScaleFactor = scaleFactor;
  }

  public Size GetRotatedSize(Rotation rotation) {
    if (rotation == Rotation.North || rotation == Rotation.South)
      return new Size(this.StandardSize.Height, this.StandardSize.Width);

    return this.StandardSize;
  }
}