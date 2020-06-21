using Libaries;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using Map = Assets.Map;

//todo: 4 modes => building, street, interact, destruction
public class MainLogic : MonoBehaviour {

  public AnimationCurve HeightCurve;
  public bool GenerateTrees;
  public Map Map { get; private set; } 

  public TileElement SelectedElement { get; private set; }

  private TileHover _tileHover;

  private Text fpsText;
  private float deltaTime;
  private readonly List<GameObject> _buttons = new List<GameObject>(); //todo: could use array

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

    this._tileHover = new TileHover(map, this);

    this._CreateButtons();   
  }

  private void _CreateButtons() {
    //create buttons
    var canvas = GameObject.Find("Canvas");
    var prefab = canvas.transform.Find("Button Prefab").gameObject;
    var width = prefab.GetComponent<RectTransform>().rect.width;
    var position = prefab.transform.position;
    position.x -= width;

    foreach (var pair in ObjectMap.TILE_ELEMENTS) {
      position.x += width;
      this._CreateButton(prefab, canvas, pair.Key, position);
    }
  }

  private void _CreateButton(GameObject prefab, GameObject canvas, string name, Vector3 position) { 
    var gObject = Instantiate(prefab, position, Quaternion.identity);

    gObject.SetActive(true);
    gObject.transform.SetParent(canvas.transform);
    gObject.name = name;     
    this._buttons.Add(gObject);

    //set text
    gObject.transform.Find("Text").gameObject.transform.GetComponent<Text>().text = name;
    //set method
    gObject.GetComponent<Button>().onClick.AddListener(delegate { OnButtonClick(name); });
  }

  // Update is called once per frame
  public void Update() {
    this._ShowFPS();
    Map.Update(Camera.main.transform.position);

    if (Input.GetKey(KeyCode.T))
      Debug.Break();

    this._tileHover.HandleHover();

    var chunk = this._tileHover.Chunk;
    var location = this._tileHover.Location;
    var x = location.X;
    var z = location.Y;
    var objectMap = chunk.ObjectMap;

    //if keydown and tile not yet set
    if (Input.GetMouseButtonDown(0) && objectMap.GetTile(x, z) != 1 && !chunk.Water.IsWater(x, z))
      objectMap.SetTile(x, z, this.SelectedElement);


    if (Input.GetKey(KeyCode.S)) {
      chunk.Terrain.MakeStreet(x, z);
      this._tileHover.Sprite = Sprites.Street;
    }
  }

  private void _ShowFPS() {
    deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
    float fps = 1.0f / deltaTime;
    fpsText.text = Mathf.Ceil(fps) + " fps";
  }

  public void OnButtonClick(string objectName) {

    //change selected button colour
    foreach (var button in this._buttons) {
      var text = button.transform.Find("Text").gameObject.transform.GetComponent<Text>().text;
      button.GetComponent<Image>().color = text == objectName
        ? Color.green
        : Color.white;
    }

    this.SelectedElement = ObjectMap.TILE_ELEMENTS[objectName];
  }
}