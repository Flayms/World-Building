using System.Drawing;
using System.Linq;
using UnityEngine;
using Terrain = Assets.Terrain;

public class TileHover {

  public Chunk Chunk { get; private set; }
  public Point Location { get; private set; } = Point.Empty;

  public Sprites Sprite { get; set; }
  private readonly Terrain _terrain;

  public TileHover(Terrain terrain) {
    this._terrain = terrain;

    //fills fields with first values so they aint null later on
    this.Chunk = terrain.Chunks.First();
    this.Sprite = this.Chunk.GetSprite(this.Location.X, this.Location.Y);
  }

  public void HandleHover() {
    if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit))
      return;

    var gObject = hit.collider.gameObject;
    var location = hit.point;
    var terrain = this._terrain;
    var lastChunk = this.Chunk;
    var lastPos = this.Location;

    //todo: better way to do this
    //get current chunk and activate tile //todo: should put this logic into terrain
    foreach (var activeChunk in terrain.Chunks) {

      if (activeChunk.GameObject != gObject)
        continue;

      var chunk = activeChunk;
      var x = (int)(location.x - chunk.Location.X);
      var z = (int)(location.z - chunk.Location.Y);

      //set last sprite back - but only if it wasnt changed by other influence 
       lastChunk.SetSprite(lastPos.X, lastPos.Y, this.Sprite);

      //get new sprite data     
      this.Sprite = chunk.GetSprite(x, z);
      this.Location = new Point(x, z);
      this.Chunk = chunk;

      //set new sprite
      chunk.SetSprite(x, z, Sprites.White);

      break;
    }

  }

}