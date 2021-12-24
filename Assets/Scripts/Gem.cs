using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
  [HideInInspector] // hides it from inspector (not need to see)
  public Vector2Int posIndex; // position index.
  [HideInInspector]
  public Board board;

  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }

  public void SetupGem(Vector2Int position, Board theBoard)
  {
    posIndex = position;
    board = theBoard;
  }
}
