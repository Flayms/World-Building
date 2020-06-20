using Assets.Classes;
using Libaries;
using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Chunk {
  //todo: make index array for tile positions in array, so uvs can be easily accessed again

  public const int WIDTH = 50;
  public const int DEPTH = 50;
  private const int _HEIGHT = 8;
  private const float _STEP_SIZE = 0.25f;
  private const float _NOISE_SCALE = 0.035f;
  private const int _OCTAVES = 5;
  private const int _VERTICES_PER_FIELD = 4;
  private const int _TRIANGLES_PER_FIELD = 6;

  public float[,] HeightMap { get; private set; }
  public ObjectMap ObjectMap { get; }
  public GameObject GameObject { get; private set; }

  public Point Location { get; }
  private Vector3[] _vertices;
  private Vector2[] _uv;
  private int[] _triangles;
  private Mesh _mesh;
  private readonly Sprites[,] _spriteMap;
  private int[,] _squareIndexes; //the vertice/uv indexes for the cube tops
  public static AnimationCurve HeightCurve;

  private int _tempX;
  private int _tempZ;

  public enum Sprites {
    Grass,
    GrassDark,
    GrassSelector,
    GrassDarkSelector,
    Street
  }

  private readonly IReadOnlyDictionary<Sprites, Point> _spriteLocations = new Dictionary<Sprites, Point> {
    {Sprites.Grass, new Point(0, 1)},
    {Sprites.GrassDark, new Point(1, 1)},
    {Sprites.GrassSelector, new Point(0, 0)},
    {Sprites.GrassDarkSelector, new Point(1, 0)},
    {Sprites.Street, new Point(0, 2)},
  };

  private readonly SpriteSelector _spriteSelector = new SpriteSelector(2, 3);

  // Start is called before the first frame update
  public Chunk(Point location) {
    OctaveNoise.MaxValue = 1;
    this._spriteMap = new Sprites[WIDTH, DEPTH];
    this.Location = location;
    this.ObjectMap = new ObjectMap(this.Location, this);
  }

  public void Create() {
    this._GenerateTiles();
    this._CreateMesh();
    this._GenerateTrees();
  }

  //makes map checkered
  private void _GenerateTiles() {
    var isSecondTile = false;
    for (var z = 0; z < DEPTH; ++z) {
      isSecondTile = !isSecondTile;
      for (var x = 0; x < WIDTH; ++x) {
        isSecondTile = !isSecondTile;
        this._spriteMap[x, z] = isSecondTile ? Sprites.Grass : Sprites.GrassDark;
      }
    }
  }

  private void _CreateMesh() {
    this._CreateSquareNet();
    this._SetMesh();
  }

  //creates the vertice, triangle and uv data
  private void _CreateSquareNet() {
    var heightMap = this._CreateHeightMap();
    var tileAmount = WIDTH * DEPTH;
    var minVertices = tileAmount * _VERTICES_PER_FIELD;
    var vertices = new List<Vector3>(minVertices);
    var triangles = new List<int>(tileAmount * _TRIANGLES_PER_FIELD);
    var uvs = new List<Vector2>(minVertices);
    var squareIndexes = new int[WIDTH, DEPTH];
    var spriteSelector = this._spriteSelector;
    var spriteLocations = this._spriteLocations;
    var i = 0;

    for (var z = 0; z < DEPTH; ++z) {
      for (var x = 0; x < WIDTH; ++x) {
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
        _AddTriangles(i, triangles);
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
          _AddTriangles(i, triangles);

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
          _AddTriangles(i, triangles);
          i += 4;
        }
      }
    }

    this._vertices = vertices.ToArray();
    this._triangles = triangles.ToArray();
    this._squareIndexes = squareIndexes;
    this._uv = uvs.ToArray();
  }

  //creates triangles for 1 square
  private static void _AddTriangles(int index, List<int> triangles) {
    triangles.Add(index);
    triangles.Add(index + 2);
    triangles.Add(index + 1);

    triangles.Add(index + 1);
    triangles.Add(index + 2);
    triangles.Add(index + 3);
  }

  private float[,] _CreateHeightMap() {
    var width = WIDTH + 1; //need 1 more feel to predict height of next chunk for cube side
    var depth = DEPTH + 1;
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

  //index := the index in the uv array
  private void _SetUV(int index, Sprites sprite) {
    var loc = this._spriteLocations[sprite];
    var newUVs = this._spriteSelector.GetUVs(loc.X, loc.Y);
    var uvs = this._uv;
    uvs[index] = newUVs[0];
    uvs[index + 1] = newUVs[1];
    uvs[index + 2] = newUVs[2];
    uvs[index + 3] = newUVs[3];
  }

  public void Hover(int x, int z) {
    var oldIndex = this._squareIndexes[this._tempX, this._tempZ];
     var index = this._squareIndexes[x, z];

    //return last field
    if (this._spriteMap[this._tempX, this._tempZ] != Sprites.Street)
      this._SetUV(oldIndex, this._spriteMap[this._tempX, this._tempZ]);
    //update new field
    this._SetUV(index, this._spriteMap[x, z] + 2);
    this._mesh.uv = this._uv;
    this._tempX = x;
    this._tempZ = z;
  }

  public void MakeStreet(int x, int z) {
    var index = this._squareIndexes[x, z];
    this._spriteMap[x, z] = Sprites.Street;
    this._SetUV(index, Sprites.Street);
    this._mesh.uv = this._uv;
  }

  private void _SetMesh() {
    var mesh = new Mesh();
    var gObject = new GameObject("Chunk");
    var meshFilter = gObject.AddComponent<MeshFilter>();
    var meshRenderer = gObject.AddComponent<MeshRenderer>();
    var meshCollider = gObject.AddComponent<MeshCollider>();

    mesh.vertices = this._vertices;
    mesh.triangles = this._triangles;
    mesh.uv = this._uv;

    mesh.RecalculateNormals();
    meshRenderer.material = (Material)Resources.Load("Grass", typeof(Material));
    meshFilter.mesh = mesh;
    meshCollider.sharedMesh = mesh;
    //this._mesh.Optimize();

    this._mesh = mesh;
    this.GameObject = gObject;
    gObject.transform.position = new Vector3(this.Location.X, 0, this.Location.Y);
  }

  private void _GenerateTrees() {
    var threshhold = 0.6f;
    var spawnProbability = 0.3f;
    var standardProbality = 0.03f;
    var objMap = this.ObjectMap;
    var noiseMap = OctaveNoise.GenerateMap(WIDTH, DEPTH,
      this.Location.X, this.Location.Y, 45435734, 0.01f, 2);

    for (var z = 0; z < DEPTH; ++z)
      for (var x = 0; x < WIDTH; ++x) {
        var rnd = UnityEngine.Random.value;
        if ((noiseMap[x, z] > threshhold && rnd < spawnProbability) ||
          rnd < standardProbality)
          objMap.SetTile(x, z, ObjectMap.TILE_ELEMENTS["tree"]);
      }
  }

  public void Destroy() {
    UnityEngine.Object.Destroy(this.GameObject);
    this.ObjectMap.Destroy();
  }

}
