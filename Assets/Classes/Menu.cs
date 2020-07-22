using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Classes {

  //todo: this is only ui, need to seperate ui and logic!!
  //todo: havent refactored this yet
  public class Menu {

    private readonly List<GameObject> _buttons = new List<GameObject>(); //todo: could use array
    private readonly MainLogic _logic;

    public Menu(MainLogic logic) {
      this._logic = logic;
    }

    private void _CreateBuildingMenu() {
      //create buttons
      var canvas = GameObject.Find("Canvas");
      var menu = canvas.transform.Find("buildingMenu");
      menu.transform.gameObject.SetActive(true);
      var prefab = menu.transform.Find("pnlButtons").transform.Find("btnPrefabBuilding").gameObject;

      var width = prefab.GetComponent<RectTransform>().rect.width;
      var position = prefab.transform.position;
      position.x -= width;

      foreach (var pair in ObjectMap.TILE_ELEMENTS) {
        position.x += width;
        this._CreateBuildingButton(prefab, canvas, pair.Key, position);
      }
    }


    //todo: as well into extra class
    private void _CloseBuildingMenu() {
      var menu = GameObject.Find("Canvas").transform.Find("buildingMenu");
      menu.transform.gameObject.SetActive(false);

      foreach (var button in this._buttons)
        Object.Destroy(button);

      this._buttons.Clear();
    }

    public void OnMenuButtonClick() {
      if (this._buttons.Count == 0)
        this._CreateBuildingMenu();
      else
        this._CloseBuildingMenu();
    }

    private void _CreateBuildingButton(GameObject prefab, GameObject canvas, string name, Vector3 position) {
      var gObject = Object.Instantiate(prefab, position, Quaternion.identity);

      gObject.SetActive(true);
      gObject.transform.SetParent(canvas.transform);
      gObject.name = name;
      this._buttons.Add(gObject);

      //set text
      gObject.transform.Find("Text").gameObject.transform.GetComponent<Text>().text = name;
      //set method
      gObject.GetComponent<Button>().onClick.AddListener(delegate { OnButtonClick(name); });
    }

    public void OnButtonClick(string objectName) {

      //change selected button colour
      foreach (var button in this._buttons) {
        var text = button.transform.Find("Text").gameObject.transform.GetComponent<Text>().text;
        button.GetComponent<Image>().color = text == objectName
          ? Color.green
          : Color.white;
      }
      this._CloseBuildingMenu();

      this._logic.SelectedElement = ObjectMap.TILE_ELEMENTS[objectName];
    }
  }
}
