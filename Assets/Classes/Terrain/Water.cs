using UnityEngine;
using System.Collections.Generic;
using Assets.Classes;
using System.Drawing;

public class Water : GeneratedObject {
  protected override string Name => "Water";
  protected override string MaterialName => "Water";
  public const float WATER_LEVEL = -1.65f;
  private readonly float[,] _heightMap;
  protected override bool AddMeshCollider => false;


  public Water(float[,] heightMap, Point location) {
    this.Location = location;
    this._heightMap = heightMap;
  }

  protected override void CreateMeshData() {
    var y = WATER_LEVEL;
    var heightMap = this._heightMap;
    var vertices = new List<Vector3>();
    var uvs = new List<Vector2>();
    var triangles = new List<int>();
    var i = 0;
    var waterUVs = new SpriteSelector(1, 1).GetUVs(0, 0);


    for (var z = 0; z < Chunk.DEPTH; ++z)
      for (var x = 0; x < Chunk.WIDTH; ++x) {
        if (heightMap[x, z] > y)
          continue;

        vertices.Add(new Vector3(x, y, z));
        vertices.Add(new Vector3(x + 1, y, z));
        vertices.Add(new Vector3(x, y, z + 1));
        vertices.Add(new Vector3(x + 1, y, z + 1));

        uvs.AddRange(waterUVs);
        AddTriangles(i, triangles);
        i += 4;
      }

    this.vertices = vertices.ToArray();
    this.triangles = triangles.ToArray();
    this.uv = uvs.ToArray();
  }

  public bool IsWater(int x, int z) => this._heightMap[x, z] <= WATER_LEVEL;
}
