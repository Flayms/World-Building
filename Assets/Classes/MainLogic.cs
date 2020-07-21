using Assets.Classes;
using Libaries;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Map = Assets.Map;

//todo: 4 modes => building, street, interact, destruction
public class MainLogic : MonoBehaviour {

  public AnimationCurve HeightCurve;
  public bool GenerateTrees;
  public Map Map { get; private set; } 

  public BuildingInfo SelectedElement { get; set; }
  public Rotation Rotation;
  private Menu _menu;

  private WorldInteraction _worldInteraction;

  private Text fpsText;
  private float deltaTime;

  // Start is called before the first frame update
  public void Start() {
    OctaveNoise.MaxValue = 1; //todo: dunno if this should be here tbh
    this.fpsText = GameObject.Find("FPSText").GetComponent<Text>();
    this.SelectedElement = ObjectMap.TILE_ELEMENTS["tree"];

    //create terrain
    Chunk.GenerateTrees = this.GenerateTrees;
    Terrain.HeightCurve = this.HeightCurve;
    var map = new Map(Camera.main.transform.position);
    this.Map = map;
    this._menu = new Menu(this);

    this._worldInteraction = new WorldInteraction(map, this);
  }

  //todo: not the most elegant solution, menu button should call the method in menu directly
  public void OnMenuButtonClick() => this._menu.OnMenuButtonClick();

  // Update is called once per frame
  public void Update() {
    this._ShowFPS();
    Map.Update(Camera.main.transform.position);

    this._worldInteraction.HandleHover();

    if (Input.mouseScrollDelta.y > 0)
      this.Rotation = (Rotation)(((int)this.Rotation + 90) % 360);

    Debug.Log("scroll: " + this.Rotation);

    var interaction = this._worldInteraction;
    var chunk = interaction.Chunk;
    var location = interaction.Location;
    
    var x = location.X;
    var z = location.Y;

    if (Input.GetMouseButtonDown(0))
      interaction.HandleBuildingClick(x, z, this.SelectedElement, this.Rotation);


    if (Input.GetKey(KeyCode.S)) {
      chunk.Terrain.MakeStreet(x, z);
      interaction.Sprite = Sprites.Street;
    }
  }

  private void _ShowFPS() {
    deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
    float fps = 1.0f / deltaTime;
    fpsText.text = Mathf.Ceil(fps) + " fps";
  }
}