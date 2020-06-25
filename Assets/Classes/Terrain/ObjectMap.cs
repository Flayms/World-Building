using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

public class ObjectMap {
  private readonly int[,] _objectMap;
  private readonly List<GameObject> _objects = new List<GameObject>();
  private readonly Point _chunkPosition;
  private readonly Chunk _chunk;

  public static readonly IReadOnlyDictionary<string, ITileElement> TILE_ELEMENTS = new List<ITileElement> {
    new Building("house", new Size(3, 3), 0.5f),
    new Building("house 2", new Size(2, 3), 0.6f),
    new Building("tree", new Size(1, 1), 0.7f),
    new Building("tree 2", new Size(1, 1), 0.8f),
    new Street("street"),
  }.ToDictionary(x => x.Name);

  public ObjectMap(Point chunkPos, Chunk chunk) {
    this._objectMap = new int[Chunk.WIDTH, Chunk.DEPTH];
    this._chunkPosition = chunkPos;
    this._chunk = chunk;
  }

  //todo: shouldnt do 2 things
  public bool IsPlaceable(int posX, int posZ, ITileElement element, out bool[,] correctFields) {
    var width = element.Size.Width;
    var depth = element.Size.Height;
    var chunk = this._chunk;
    var heightMap = chunk.Terrain.HeightMap;
    var height = heightMap[posX, posZ];
    correctFields = new bool[width, depth];

    if (element is Street) {
      var result = this.GetTile(posX, posZ) != 1;
      correctFields[0, 0] = result;
      return result;
    }

    var endX = posX + width;
    var endZ = posZ + depth; 
    var isPlaceable = true;  

    //check tiles
    for (var z = posZ; z < endZ; ++z)
      for (var x = posX; x < endX; ++x) {

        if (chunk.Water.IsWater(x, z)) {
          correctFields = new bool[width, depth];
          return false;
        }

        if (this.GetTile(x, z) == 0 && heightMap[x, z] == height)
          correctFields[x - posX, z - posZ] = true;
        else
          isPlaceable = false;
      }

    return isPlaceable;
  }

  public void SetTile(int posX, int posZ, ITileElement element, Rotation rotation) {
    var chunk = this._chunk;
    var endX = posX + element.Size.Width;
    var endZ = posZ + element.Size.Height;
    int x;
    int z;
    var heightMap = chunk.Terrain.HeightMap;
    var height = heightMap[posX, posZ];

    if (!this.IsPlaceable(posX, posZ, element, out _))
      return;

    for (z = posZ; z < endZ; ++z)
      for (x = posX; x < endX; ++x) {
        this._objectMap[x, z] = 1;
      }

    var chunkPos = this._chunkPosition;

    switch (element) {
      case Building building:
        this._objects.Add(
          building.Create(new Vector3(posX + chunkPos.X, height, posZ + chunkPos.Y), rotation));
        break;

      case Street street:
        street.Create(posX, posZ, this._chunk);
        break;
    }
  }

  public int GetTile(int x, int z) {
    return this._objectMap[x, z];
  }

  public void Destroy() {
    foreach (var gObject in this._objects)
      Object.Destroy(gObject);
  }

}
