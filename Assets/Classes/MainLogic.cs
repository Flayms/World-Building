using System;
using UnityEngine;
using UnityEngine.UI;
using Terrain = Assets.Terrain;

public class MainLogic : MonoBehaviour {

  public AnimationCurve HeightCurve;
  private int _lastKeyAmount;
  public Terrain Terrain { get; private set; } 

  private string _selectedObjectName = "tree";

  // Start is called before the first frame update
  public void Start() {
    this.fpsText = GameObject.Find("FPSText").GetComponent<Text>();

    //create terrain
    Chunk.HeightCurve = this.HeightCurve;
    var terrain = new Terrain(Camera.main.transform.position);
    this.Terrain = terrain;
    

    //create buttons
    var canvas = GameObject.Find("Canvas");
    var prefab = canvas.transform.Find("Button Prefab").gameObject;
    var width = prefab.GetComponent<RectTransform>().rect.width;
    var position = prefab.transform.position;
    position.x -= width;

    foreach (var pair in ObjectMap.TILE_ELEMENTS) {
      var name = pair.Key;
      position.x += width;
      var button = Instantiate(prefab, position, Quaternion.identity);
      button.SetActive(true);
      button.transform.SetParent(canvas.transform);
      button.name = name;
      //set text
      button.transform.Find("Text").gameObject.transform.GetComponent<Text>().text = name;
      //set method
      button.GetComponent<Button>().onClick.AddListener(delegate { OnButtonClick(name); });
    }
  }



  // Update is called once per frame
  public void Update() {
    this.ShowFPS();
    Terrain.Update(Camera.main.transform.position);

    //todo: totally braindead but works for debug
    var keys = this.HeightCurve.keys.Length;
    if (keys!= this._lastKeyAmount) {
      this._lastKeyAmount = keys;
      //this.Terrain.Dispose();
      //this.Start();
    }

    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit)) 
      this._HandleGroundClick(hit.point, hit.collider.gameObject);
  }


  public Text fpsText;
  public float deltaTime;
  private void ShowFPS() {
    deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
    float fps = 1.0f / deltaTime;
    fpsText.text = Mathf.Ceil(fps) + " fps";
  }

  public void OnButtonClick(string objectName) {
    this._selectedObjectName = objectName;
  }

  private void _HandleGroundClick(Vector3 location, GameObject gObject) {
    var x = 0;
    var z = 0;
    var terrain = this.Terrain;
    Chunk chunk = null;

    //todo: better way to do this
    //get current chunk and activate tile //todo: should put this logic into terrain
    foreach(var activeChunk in terrain.Chunks) {

      if (activeChunk.GameObject == gObject) {
        chunk = activeChunk;
        x = (int)(location.x - chunk.Location.X);
        z = (int)(location.z - chunk.Location.Y);
        chunk.Hover(x, z);
        break;
      }
    }

    if (chunk == null) //todo: might be removable
      throw new Exception("No Fitting Chunk found!");

    var objectMap = chunk.ObjectMap;

    if (Input.GetMouseButtonDown(0)) {
      if (objectMap.GetTile(x, z) == 1)
        return;

      if (!ObjectMap.TILE_ELEMENTS.TryGetValue(this._selectedObjectName, out var tileElement))
        throw new IndexOutOfRangeException();

      objectMap.SetTile(x, z, tileElement);
    }

    if (Input.GetKey(KeyCode.S)) {
      chunk.MakeStreet(x, z);
    }

  }
}
