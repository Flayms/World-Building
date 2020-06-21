using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class ObjectMap {
  private readonly int[,] _objectMap;
  private readonly List<GameObject> _objects = new List<GameObject>();
  private readonly Point _chunkPosition;
  private readonly Chunk _chunk;

  public static readonly IReadOnlyDictionary<string, TileElement> TILE_ELEMENTS = new Dictionary<string, TileElement>() {
    {"house", new TileElement("house", new Size(2, 2), new Vector3(1, 0, 1), 0.3f)},
    {"house 2", new TileElement("house 2", new Size(2, 2), new Vector3(1, 0, 1), 0.4f)},
    {"tree", new TileElement("tree", new Size(1, 1), new Vector3(0.5f, 0, 0.5f), 0.5f)},
    {"tree 2", new TileElement("tree 2", new Size(1, 1), new Vector3(0.5f, 0, 0.5f), 0.5f)},
  };

  public ObjectMap(Point chunkPos, Chunk chunk) {
    this._objectMap = new int[Chunk.WIDTH, Chunk.DEPTH];
    this._chunkPosition = chunkPos;
    this._chunk = chunk;
  }

  public void SetTile(int posX, int posZ, TileElement element) {
    var chunk = this._chunk;
    var endX = posX + element.Size.Width;
    var endZ = posZ + element.Size.Height;
    int x;
    int z;
    var heightMap = chunk.Terrain.HeightMap;
    var height = heightMap[posX, posZ];

    //check tiles
    for (z = posZ; z < endZ; ++z)
      for (x = posX; x < endX; ++x) {
        if (this.GetTile(x, z) == 1 || chunk.Water.IsWater(x, z) || heightMap[x, z] != height)
          return;
      }

    for (z = posZ; z < endZ; ++z)
      for (x = posX; x < endX; ++x) {
        this._objectMap[x, z] = 1;
      }

    var chunkPos = this._chunkPosition;

    this._objects.Add(element.Create(
      new Vector3(posX + chunkPos.X, height, posZ + chunkPos.Y),
      (Rotation)((int)(Random.value * 4) * 90))
      );
  }

  public int GetTile(int x, int z) {
    return this._objectMap[x, z];
  }

  public void Destroy() {
    foreach (var gObject in this._objects)
      Object.Destroy(gObject);
  }

}
