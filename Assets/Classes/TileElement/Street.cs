using System.Drawing;

public class Street : ITileElement {
  public string Name { get; }
  public Size Size { get; } = new Size(1, 1);

  public Street(string name) => this.Name = name;

  public void Create(int x, int z, Chunk chunk) => chunk.Terrain.MakeStreet(x, z);

}
