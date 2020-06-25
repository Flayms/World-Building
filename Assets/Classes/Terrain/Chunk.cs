using Libaries;
using System.Drawing;
using UnityEngine;

public class Chunk {
  //todo: make index array for tile positions in array, so uvs can be easily accessed again

  public const int WIDTH = 50;
  public const int DEPTH = 50;

  public static bool GenerateTrees { get; set; }
  public ObjectMap ObjectMap { get; }
  public Point Location { get; }
  public Terrain Terrain { get; }
  public Water Water { get; private set; }


  // Start is called before the first frame update
  public Chunk(Point location) {
    this.Terrain = new Terrain(location);
    this.Location = location;
    this.ObjectMap = new ObjectMap(this.Location, this);
  }

  public void Create() {
    this.Terrain.Create();
    this.Water = new Water(this.Terrain.HeightMap, this.Location);
    this.Water.Create();
    if (GenerateTrees)
      this._GenerateTrees();

  }

  //todo: put this in extra class
  private void _GenerateTrees() {
    var threshhold = 0.6f;
    var spawnProbability = 0.3f;
    var standardProbality = 0.03f;
    var objMap = this.ObjectMap;
    var heightMap = this.Terrain.HeightMap;
    var noiseMap = OctaveNoise.GenerateMap(WIDTH, DEPTH,
      this.Location.X, this.Location.Y, 45435734, 0.01f, 2);

    for (var z = 0; z < DEPTH; ++z)
      for (var x = 0; x < WIDTH; ++x) {
        var rnd = Random.value;

        if (((noiseMap[x, z] > threshhold && rnd < spawnProbability) ||
          rnd < standardProbality)
          && heightMap[x, z] > Water.WATER_LEVEL) {

          objMap.SetTile(x, z, ObjectMap.TILE_ELEMENTS["tree"], (Rotation)((int)(Random.value * 4) * 90));
        }
      }
  }

  public void Destroy() {
    Object.Destroy(this.Terrain.GameObject);
    Object.Destroy(this.Water.GameObject);
    this.ObjectMap.Destroy();
  }

}
