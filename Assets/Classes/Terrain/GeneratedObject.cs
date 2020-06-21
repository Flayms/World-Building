using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public abstract class GeneratedObject {

  protected const int VERTICES_PER_FIELD = 4;
  protected const int TRIANGLES_PER_FIELD = 6;

  public GameObject GameObject { get; private set; }
  public Point Location { get; protected set; }
  protected abstract string Name { get; }
  protected abstract string MaterialName { get; }
  protected abstract bool AddMeshCollider { get; }

  protected Vector3[] vertices;
  protected Vector2[] uv;
  protected int[] triangles;
  protected Mesh mesh;

  public virtual void Create() {
    this.CreateMeshData();
    this.SetMesh();
  }

  protected abstract void CreateMeshData();


  protected static void AddTriangles(int index, List<int> triangles) {
    triangles.Add(index);
    triangles.Add(index + 2);
    triangles.Add(index + 1);

    triangles.Add(index + 1);
    triangles.Add(index + 2);
    triangles.Add(index + 3);
  }

  protected void SetMesh() {
    var mesh = new Mesh();
    var gObject = new GameObject(this.Name);
    var meshFilter = gObject.AddComponent<MeshFilter>();
    var meshRenderer = gObject.AddComponent<MeshRenderer>();
    

    mesh.vertices = this.vertices;
    mesh.triangles = this.triangles;
    mesh.uv = this.uv;

    mesh.RecalculateNormals();
    meshRenderer.material = (Material)Resources.Load(this.MaterialName, typeof(Material));
    meshFilter.mesh = mesh;

    if (this.AddMeshCollider) {
      var meshCollider = gObject.AddComponent<MeshCollider>();
      meshCollider.sharedMesh = mesh;
    }

    this.mesh = mesh;
    this.GameObject = gObject;
    gObject.transform.position = new Vector3(this.Location.X, 0, this.Location.Y);
  }
}
