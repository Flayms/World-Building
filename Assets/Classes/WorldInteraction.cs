using System.Drawing;
using System.Linq;
using UnityEngine;
using Map = Assets.Map;

/// <summary>
/// displays and handles player interaction with the world
/// </summary>
public class WorldInteraction {

  public Chunk Chunk { get; private set; }
  public Point Location { get; private set; } = Point.Empty;
  private InteractionModes _interactionMode = InteractionModes.Interact;

  public Sprites Sprite {
    get => this._sprites[0, 0];
    set => this._sprites[0, 0] = value;
  }

  private Sprites[,] _sprites = new Sprites[1, 1];
  private readonly Map _map;
  private readonly MainLogic _logic;

  public WorldInteraction(Map map, MainLogic logic) {
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
      int x;
      int y;
      var cursorX = (int)(location.x - chunk.Location.X);
      var cursorY = (int)(location.z - chunk.Location.Y);
      var endX = this._sprites.GetLength(0);
      var endY = this._sprites.GetLength(1);

      chunk.ObjectMap.IsPlaceable(cursorX, cursorY, this._logic.SelectedElement, out var placeMap);

      //set last sprites back
      for (y = 0; y < endY; ++y)
        for (x = 0; x < endX; ++x)
          lastChunk.Terrain.SetSprite(x + lastPos.X, y + lastPos.Y, this._sprites[x, y]);

      endX = cursorX + size.Width;
      endY = cursorY + size.Height;

      this._sprites = new Sprites[size.Width, size.Height];

      for (y = cursorY; y < endY; ++y)
        for (x = cursorX; x < endX; ++x) {
          var sprite = placeMap[x - cursorX, y - cursorY] ? Sprites.White : Sprites.Red;

          //get new sprite data
          this._sprites[x -cursorX, y - cursorY] = chunk.Terrain.GetSprite(x, y);
          //set new sprite
          chunk.Terrain.SetSprite(x, y, sprite);
        }


      this.Location = new Point(cursorX, cursorY);
      this.Chunk = chunk;
      
      break;
    }

  }

}