using System.Collections.Generic;
using UnityEngine;
using Color = UnityEngine.Color;

public class HexagonTerrain : MonoBehaviour {

  private Vector3[] _vertices;
  private int[] _triangles;
  private Color[] _colors;

  public int Width { get; private set; } = 10;
  public int Depth { get; private set; } = 10;
  private Mesh _mesh;

  // Start is called before the first frame update
  public void Start() {
    this._CreateMesh();
  }

  //creates the mesh for the terrain
  private void _CreateMesh() {
    this._CreateHexagonNext();
    this._SetColors();
    this._SetMesh();
  }

  //creates triangles and vertices of the mesh
  private void _CreateHexagonNext() {
    var width = Width;
    var depth = Depth;
    var triangles = new List<int>();
    var vertices = new List<Vector3>();
    var i = 0;

    for (var z = 0; z < depth; ++z) {
      var xOffset = z % 2 == 0 ? 0f : 0.5f;
      var zVal = z * 0.8f;

      for (var x = 0; x < width; ++x) {
        var xVal = x + xOffset;

        //create vertices for hexagon
        vertices.Add(new Vector3(xVal + 0.5f, 0, zVal + 0.5f)); //middle

        vertices.Add(new Vector3(xVal, 0, zVal + 0.2f));
        vertices.Add(new Vector3(xVal + 0.5f, 0, zVal));

        vertices.Add(new Vector3(xVal + 1, 0, zVal + 0.2f));
        vertices.Add(new Vector3(xVal + 1, 0, zVal + 0.8f));

        vertices.Add(new Vector3(xVal + 0.5f, 0, zVal + 1));
        vertices.Add(new Vector3(xVal, 0, zVal + 0.8f));


        for (var j = 1; j <= 6; ++j) {
          triangles.Add(i);
          triangles.Add(i + (j % 6) + 1);
          triangles.Add(i + j);
        }

        i += 7;
      }
    }

    this._vertices = vertices.ToArray();
    this._triangles = triangles.ToArray();
  }

  //sets the mesh hexagon colours
  private void _SetColors() {
    this._colors = new Color[this._vertices.Length];
    var colorIndex = 0;
    var colorRange = new Color[] { new Color(0, 1, 0), new Color(0, 0.8f, 0), new Color(0, 0.1f, 0),
    new Color(0, 0.5f, 0), new Color(0, 0.4f, 0),new Color(0, 0.2f, 0),};

    for (var y = 0; y < this.Depth; ++y)
      for (var x = 0; x < this.Width; ++x) {
        var color = colorRange[colorIndex];
        colorIndex = (colorIndex + 1) % colorRange.Length;
        this._SetColor(x, y, color);
      }
  }
  //sets the tile at the position active
  public void Activate(int x, int z) {

    //this._SetColor(this._tempX, this._tempZ, this._tempColor);
    //this._tempX = x;
    //this._tempZ = z;
    //this._tempColor = ;
    this._SetColor(x, z, new Color(1, 0, 1));
    this._mesh.colors = this._colors;
  }

  //assings colour to hexagon x, y in color array
  private void _SetColor(int x, int z, Color color) {
    var colors = this._colors;

    var index = (x + z * (this.Width)) * 7;
    for (var i = 0; i < 7; ++i) {
      colors[index + i] = color;
    }
  }

  private void _SetMesh() {
    var mesh = new Mesh();
    var gObject = new GameObject("Terrain");
    var meshFilter = gObject.AddComponent<MeshFilter>();
    var meshRenderer = gObject.AddComponent<MeshRenderer>();

    mesh.vertices = this._vertices;
    mesh.triangles = this._triangles;
    mesh.colors = this._colors;

    mesh.RecalculateNormals();
    meshRenderer.material = this.transform.GetComponent<Renderer>().material;
    meshFilter.mesh = mesh;
    //this._mesh.Optimize();

    this._mesh = mesh; 
  }
}