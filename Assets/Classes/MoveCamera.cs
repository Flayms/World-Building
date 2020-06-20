using UnityEngine;

public class MoveCamera : MonoBehaviour {

  public const float SPEED = 7f;
  public const float HALF_SPEED = SPEED / 2;

  // Update is called once per frame
  public void Update() {
    var transform = this.transform;
    var time = Time.deltaTime;

    if (Input.GetKey(KeyCode.LeftArrow))
      transform.Translate(new Vector3(-SPEED * time, 0, 0));

    if (Input.GetKey(KeyCode.RightArrow))
      transform.Translate(new Vector3(SPEED * time, 0, 0));

    if (Input.GetKey(KeyCode.UpArrow))
      transform.position += new Vector3(-SPEED * time, 0, SPEED * time);

    if (Input.GetKey(KeyCode.DownArrow))
      transform.position += new Vector3(SPEED * time, 0, -SPEED * time);

  }
}
