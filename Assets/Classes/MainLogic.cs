using System;
using UnityEngine;
using UnityEngine.UI;
using Terrain = Assets.Terrain;

public class MainLogic : MonoBehaviour {

  public AnimationCurve HeightCurve;
  public bool GenerateTrees;
  private int _lastKeyAmount;
  public Terrain Terrain { get; private set; } 

  private string _selectedObjectName = "tree";

  private TileHover _tileHover;

  private Text fpsText;
  private float deltaTime;

  // Start is called before the first frame update
  public void Start() {
    this.fpsText = GameObject.Find("FPSText").GetComponent<Text>();

    //create terrain
    Chunk.GenerateTrees = this.GenerateTrees;
    Chunk.HeightCurve = this.HeightCurve;
    var terrain = new Terrain(Camera.main.transform.position);
    this.Terrain = terrain;

    this._tileHover = new TileHover(terrain);

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
    var button = Instantiate(prefab, position, Quaternion.identity);
    button.SetActive(true);
    button.transform.SetParent(canvas.transform);
    button.name = name;
    //set text
    button.transform.Find("Text").gameObject.transform.GetComponent<Text>().text = name;
    //set method
    button.GetComponent<Button>().onClick.AddListener(delegate { OnButtonClick(name); });
  }

  // Update is called once per frame
  public void Update() {
    this._ShowFPS();
    Terrain.Update(Camera.main.transform.position);

    //todo: totally braindead but works for debug
    var keys = this.HeightCurve.keys.Length;
    if (keys!= this._lastKeyAmount) {
      this._lastKeyAmount = keys;
      //this.Terrain.Dispose();
      //this.Start();
    }


    this._tileHover.HandleHover();

    var chunk = this._tileHover.Chunk;
    // if (chunk == null) //todo: might be removable
    //  throw new Exception("No Fitting Chunk found!");

    var location = this._tileHover.Location;
    var x = location.X;
    var z = location.Y;
    var objectMap = chunk.ObjectMap;

    //if keydown and tile not yet set
    if (Input.GetMouseButtonDown(0) && objectMap.GetTile(x, z) != 1) {
      if (!ObjectMap.TILE_ELEMENTS.TryGetValue(this._selectedObjectName, out var tileElement))
        throw new IndexOutOfRangeException();

      objectMap.SetTile(x, z, tileElement);
    }

    if (Input.GetKey(KeyCode.S)) {
      chunk.MakeStreet(x, z);
      this._tileHover.Sprite = Sprites.Street;
    }
  }

  private void _ShowFPS() {
    deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
    float fps = 1.0f / deltaTime;
    fpsText.text = Mathf.Ceil(fps) + " fps";
  }

  public void OnButtonClick(string objectName) {
    this._selectedObjectName = objectName;
  }


}
