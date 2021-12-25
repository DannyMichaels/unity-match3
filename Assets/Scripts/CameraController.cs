using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// NOT USED
public class CameraController : MonoBehaviour
{
  private Board theBoard;
  public bool isRightAligned;

  void Awake()
  {
    theBoard = FindObjectOfType<Board>();
  }
  // Start is called before the first frame update
  void Start()
  {
    AlignCameraRelativeToBoard();
  }

  // Update is called once per frame
  void Update()
  {

  }

  void AlignCameraRelativeToBoard()
  {
    float x = isRightAligned ? (theBoard.width / 2) - 3.5f : theBoard.width / 2;
    transform.position = new Vector3(x, theBoard.height / 2, transform.position.z);
  }
}
