using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using Map = Assets.Map;

public class TileHover {

  public Chunk Chunk { get; private set; }
  public Point Location { get; private set; } = Point.Empty;

  public Sprites Sprite {
    get => this._sprites[0, 0];
    set => this._sprites[0, 0] = value;
  }

  private Sprites[,] _sprites = new Sprites[1, 1];
  private readonly Map _map;
  private readonly MainLogic _logic;

  public TileHover(Map map, MainLogic logic) {
    this._map = map;
    this._logic = logic;

    //fills fields with first values so they aint null later on
    this.Chunk = map.Chunks.First();
    this._sprites[0, 0] = this.Chunk.Terrain.GetSprite(this.Location.X, this.Location.Y); 
  }

  public void HandleHover() {
    if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit))
      return;

    var gObject = hit.collider.gameObject;
    var location = hit.point;
    var terrain = this._map;
    var lastChunk = this.Chunk;
    var lastPos = this.Location;

    //todo: better way to do this
    //get current chunk and activate tile //todo: should put this logic into terrain
    foreach (var activeChunk in terrain.Chunks) {

      if (activeChunk.Terrain.GameObject != gObject)
        continue;

      var chunk = activeChunk;
      var size = this._logic.SelectedElement.Size;
      var cursorX = (int)(location.x - chunk.Location.X);
      var cursorY = (int)(location.z - chunk.Location.Y);

      var endX = cursorX + size.Width;
      var endY = cursorY + size.Height;

      int x;
      int y;

      //set last sprites back
      for (y = 0; y < this._sprites.GetLength(1); ++y)
        for (x = 0; x < this._sprites.GetLength(0); ++x) {
          lastChunk.Terrain.SetSprite(lastPos.X + x, lastPos.Y + y, this._sprites[x, y]);
        }      

      this._sprites = new Sprites[size.Width, size.Height];

      for (y = 0; y < size.Height; ++y)
        for (x = 0; x < size.Width; ++x) {
          //get new sprite data
          this._sprites[x, y] = chunk.Terrain.GetSprite(cursorX + x, cursorY + y);
          //set new sprite
          chunk.Terrain.SetSprite(cursorX + x, cursorY + y, Sprites.White);
        }


      this.Location = new Point(cursorX, cursorY);
      this.Chunk = chunk;
      
      break;
    }

  }

}