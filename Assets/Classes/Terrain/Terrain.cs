using Assets.Classes;
using Libaries;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Terrain : GeneratedObject {

  protected override string Name => "Terrain";
  protected override string MaterialName => "Grass";
  public float[,] HeightMap { get; private set; }

  private const int _HEIGHT = 9;
  private const float _STEP_SIZE = 0.25f;
  private const float _NOISE_SCALE = 0.03f;
  private const int _OCTAVES = 5;

  private readonly SpriteSelector _spriteSelector = new SpriteSelector(2, 3);

  public static AnimationCurve HeightCurve { get; set; }

  protected override bool AddMeshCollider => true;

  public Terrain(Point location) {
    this.Location = location;
    this._spriteMap = new Sprites[Chunk.WIDTH, Chunk.DEPTH];
  }

  public override void Create() {
    this._GenerateTiles();
    base.Create();
  }

  private readonly Sprites[,] _spriteMap;

  private int[,] _squareIndexes; //the vertice/uv indexes for the cube tops

  private readonly IReadOnlyDictionary<Sprites, Point> _spriteLocations = new Dictionary<Sprites, Point> {
    {Sprites.Grass, new Point(0, 1)},
    {Sprites.GrassDark, new Point(1, 1)},
    {Sprites.GrassSelector, new Point(0, 0)},
    {Sprites.GrassDarkSelector, new Point(1, 0)},
    {Sprites.Street, new Point(0, 2)},
    {Sprites.White, new Point(1, 2) }
  };

  //makes map checkered
  private void _GenerateTiles() {
    var isSecondTile = false;
    for (var z = 0; z < Chunk.DEPTH; ++z) {
      isSecondTile = !isSecondTile;
      for (var x = 0; x < Chunk.WIDTH; ++x) {
        isSecondTile = !isSecondTile;
        this._spriteMap[x, z] = isSecondTile ? Sprites.Grass : Sprites.GrassDark;
      }
    }
  }

  //creates the vertice, triangle and uv data
  protected override void CreateMeshData() {
    var heightMap = this._CreateHeightMap();
    var width = Chunk.WIDTH;
    var depth = Chunk.DEPTH;
    var tileAmount = width * depth;
    var minVertices = tileAmount * VERTICES_PER_FIELD;
    var vertices = new List<Vector3>(minVertices);
    var triangles = new List<int>(tileAmount * TRIANGLES_PER_FIELD);
    var uvs = new List<Vector2>(minVertices);
    var squareIndexes = new int[width, depth];
    var spriteSelector = this._spriteSelector;
    var spriteLocations = this._spriteLocations;
    var i = 0;

    for (var z = 0; z < depth; ++z) {
      for (var x = 0; x < width; ++x) {
        var y = heightMap[x, z];
        var nextX = x + 1;
        var nextZ = z + 1;
        var spriteLoc = spriteLocations[this._spriteMap[x, z]];
        var newUVs = spriteSelector.GetUVs(spriteLoc.X, spriteLoc.Y);

        //create vertices for top side
        vertices.Add(new Vector3(x, y, z));
        vertices.Add(new Vector3(x + 1, y, z));
        vertices.Add(new Vector3(x, y, z + 1));
        vertices.Add(new Vector3(x + 1, y, z + 1));


        uvs.AddRange(newUVs);
        AddTriangles(i, triangles);
        squareIndexes[x, z] = i;
        i += 4;

        //if x-axe height difference, create vertical square side
        var nextY = heightMap[x + 1, z];
        if (y != nextY) {
          vertices.Add(new Vector3(nextX, y, z));
          vertices.Add(new Vector3(nextX, nextY, z));
          vertices.Add(new Vector3(nextX, y, nextZ));
          vertices.Add(new Vector3(nextX, nextY, nextZ));

          uvs.AddRange(newUVs);
          AddTriangles(i, triangles);

          i += 4;
        }

        //if y - axe height difference, create vertical square side
        nextY = heightMap[x, z + 1];
        if (y != nextY) {
          vertices.Add(new Vector3(x, y, nextZ));
          vertices.Add(new Vector3(nextX, y, nextZ));
          vertices.Add(new Vector3(x, nextY, nextZ));
          vertices.Add(new Vector3(nextX, nextY, nextZ));


          uvs.AddRange(newUVs);
          AddTriangles(i, triangles);
          i += 4;
        }
      }
    }

    this.vertices = vertices.ToArray();
    this.triangles = triangles.ToArray();
    this._squareIndexes = squareIndexes;
    this.uv = uvs.ToArray();
  }

  private float[,] _CreateHeightMap() {
    var width = Chunk.WIDTH + 1; //need 1 more feel to predict height of next chunk for cube side
    var depth = Chunk.DEPTH + 1;
    var heightOffset = _HEIGHT / 2;
    //1 in every direction bigger to predict side of next tile 
    var heightMap = OctaveNoise.GenerateMap(width, depth,
      this.Location.X, this.Location.Y, 0, _NOISE_SCALE, _OCTAVES);

    for (var z = 0; z < width; ++z)
      for (var x = 0; x < depth; ++x) {
        var temp = HeightCurve.Evaluate(heightMap[x, z]) * _HEIGHT - heightOffset;
        temp = (int)(temp / _STEP_SIZE); //todo: maybe exchange with modulo in float numbers
        heightMap[x, z] = temp * _STEP_SIZE;
      }

    this.HeightMap = heightMap;
    return heightMap;
  }

  public void SetSprite(int x, int z, Sprites sprite) => this._SetUV(this._squareIndexes[x, z], sprite);
  public Sprites GetSprite(int x, int z) => this._spriteMap[x, z];

  //index := the index in the uv array
  private void _SetUV(int index, Sprites sprite) {
    var loc = this._spriteLocations[sprite];
    var newUVs = this._spriteSelector.GetUVs(loc.X, loc.Y);
    var uvs = this.uv;
    uvs[index] = newUVs[0];
    uvs[index + 1] = newUVs[1];
    uvs[index + 2] = newUVs[2];
    uvs[index + 3] = newUVs[3];

    this.mesh.uv = this.uv;
  }

  public void MakeStreet(int x, int z) {
    var index = this._squareIndexes[x, z];
    this._spriteMap[x, z] = Sprites.Street;
    this._SetUV(index, Sprites.Street);
    this.mesh.uv = this.uv;
  }
}
