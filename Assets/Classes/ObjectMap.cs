using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class ObjectMap {
  private readonly int[,] _objectMap;
  private readonly List<GameObject> _objects = new List<GameObject>();
  private readonly Point _chunkPosition;
  private readonly Chunk _chunk;

  public static readonly IReadOnlyDictionary<string, TileElement> TILE_ELEMENTS = new Dictionary<string, TileElement>() {
    {"house", new TileElement("house", new Vector3(1, 0, 1), 0.24f)},
    {"tree", new TileElement("tree", new Vector3(0.5f, 0, 0.5f), 0.5f)},
    {"tree 2", new TileElement("tree 2", new Vector3(0.5f, 0, 0.5f), 0.5f)},
  };

  public ObjectMap(Point chunkPos, Chunk chunk) {
    this._objectMap = new int[Chunk.WIDTH, Chunk.DEPTH];
    this._chunkPosition = chunkPos;
    this._chunk = chunk;
  }

  public void SetTile(int x, int z, TileElement element) {
    this._objectMap[x, z] = 1;
    var chunkPos = this._chunkPosition;
    var heightMap = this._chunk.HeightMap;

    this._objects.Add(element.Create(new Vector3(x + chunkPos.X, heightMap[x, z], z + chunkPos.Y), (Rotation)((int)(Random.value * 4) * 90)));
  }

  public int GetTile(int x, int z) {
    return this._objectMap[x, z];
  }

  public void Destroy() {
    foreach (var gObject in this._objects)
      Object.Destroy(gObject);
  }

}
