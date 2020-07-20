using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

public class ObjectMap {
  private readonly int[,] _objectMap;

  private readonly List<Building> _buildings = new List<Building>();
  private readonly Point _chunkPosition;
  private readonly Chunk _chunk;

  public static readonly IReadOnlyDictionary<string, BuildingInfo> TILE_ELEMENTS = new List<BuildingInfo> {
    new BuildingInfo("house", new Size(3, 3), 0.5f),
    new BuildingInfo("house 2", new Size(2, 3), 0.6f),
    new BuildingInfo("tree", new Size(1, 1), 0.7f),
    new BuildingInfo("tree 2", new Size(1, 1), 0.8f),
  }.ToDictionary(x => x.Name);

  public ObjectMap(Point chunkPos, Chunk chunk) {
    this._objectMap = new int[Chunk.WIDTH, Chunk.DEPTH];
    this._chunkPosition = chunkPos;
    this._chunk = chunk;
  }

  //todo: shouldnt do 2 things
  public bool[,] GetPlaceMap(int posX, int posZ, Size size) {
    var width = size.Width;
    var depth = size.Height;
    var chunk = this._chunk;
    var heightMap = chunk.Terrain.HeightMap;
    var height = heightMap[posX, posZ];
    var correctFields = new bool[width, depth];

    var endX = posX + width;
    var endZ = posZ + depth;  

    //check tiles
    for (var z = posZ; z < endZ; ++z)
      for (var x = posX; x < endX; ++x) {

        if (chunk.Water.IsWater(x, z)) {
          return new bool[width, depth];
        }

        if (!this.IsOccupied(x, z) && heightMap[x, z] == height)
          correctFields[x - posX, z - posZ] = true;
      }

    return correctFields;
  }

  public bool IsPlaceAble(int posX, int posZ, Size size) {
    var width = size.Width;
    var depth = size.Height;
    var chunk = this._chunk;
    var heightMap = chunk.Terrain.HeightMap;
    var height = heightMap[posX, posZ];

    var endX = posX + width;
    var endZ = posZ + depth;

    for (var z = posZ; z < endZ; ++z)
      for (var x = posX; x < endX; ++x) {
        if (chunk.Water.IsWater(x, z) || this.IsOccupied(x, z) || heightMap[x, z] != height)
          return false;
      }

    return true;
  }

  //need to check if placing is possible beforehand
  public void PlaceBuilding(int posX, int posZ, BuildingInfo buildingInfo, Rotation rotation) {
    var chunk = this._chunk;
    var size = buildingInfo.GetRotatedSize(rotation);
    var endX = posX + size.Width;
    var endZ = posZ + size.Height;
    var height = chunk.Terrain.HeightMap[posX, posZ];

    for (var z = posZ; z < endZ; ++z)
      for (var x = posX; x < endX; ++x) {
        this._objectMap[x, z] = 1;
      }

    var chunkPos = this._chunkPosition;

    this._buildings.Add(
      new Building(buildingInfo, new Vector3(posX + chunkPos.X, height, posZ + chunkPos.Y), rotation));
  }

  public bool IsOccupied(int x, int z) => this._objectMap[x, z] == 1 ? true : false;

  public void Destroy() {
    foreach (var building in this._buildings)
      Object.Destroy(building.GObject);
  }

}
