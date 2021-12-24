using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
  // [HideInInspector] // hides it from inspector (not need to see)
  public Vector2Int posIndex; // position index.
  [HideInInspector]
  public Board board;

  private Vector2 firstTouchPosition;
  private Vector2 finalTouchPosition;

  private bool mousePressed;
  private float swipeAngle = 0;

  private Gem otherGem;

  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {


    HandleMousePressed();
  }

  public void SetupGem(Vector2Int position, Board theBoard)
  {
    posIndex = position;
    board = theBoard;
  }

  // need to add a box collider 2D for this to be clickable
  private void OnMouseDown()
  {
    // convert the mouse position into a position in the world based on the camera that we have
    firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); // camera.main is a way to check and find the main camera in the scene
    mousePressed = true;
  }

  private void HandleMousePressed()
  {
    // Input.GetMouseButtonUp(0): 0 is left click
    if (mousePressed && Input.GetMouseButtonUp(0)) // ButtonUp: if we let go of left click
    {
      mousePressed = false;

      finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
      CalculateAngle(); // calculate the angle that the swipe is happening in
    }
  }

  private void CalculateAngle()
  {
    // https://docs.unity3d.com/ScriptReference/Mathf.Atan2.html
    swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x);
    swipeAngle = swipeAngle * 180 / Mathf.PI;
    Debug.Log(swipeAngle);


    if (Vector3.Distance(firstTouchPosition, finalTouchPosition) > .5f)
    {

      MovePieces();
    }
  }

  private void MovePieces()
  {
    bool isInBoundsX = posIndex.x < board.width - 1; // make sure we're not swiping outside of range
    bool isInBoundsY = posIndex.y < board.height - 1; // make sure we're not swiping outside of range

    bool swipingRight = swipeAngle < 45 && swipeAngle > -45;
    bool swipingLeft = swipeAngle > 135 || swipeAngle < -135;

    bool swipingUpwards = swipeAngle > 45 && swipeAngle <= 135;
    bool swipingDown = swipeAngle < -45 && swipeAngle >= -135;

    // move gem to the right
    if (swipingRight && isInBoundsX)
    {
      // get the other gem where we swap places with
      otherGem = board.allGems[posIndex.x + 1, posIndex.y]; // next gem is just X + 1 to the right

      otherGem.posIndex.x--; // move it's X negative 1 because we're swapping locations (move left 1)
      this.posIndex.x++; // add 1 in the x to this gem (swapping) (move right 1)
    }

    else if (swipingUpwards && isInBoundsY)
    {
      // get the other gem where we swap places with
      otherGem = board.allGems[posIndex.x, posIndex.y + 1]; // next gem is just Y + 1 so up one

      otherGem.posIndex.y--; // move it's X negative 1 because we're swapping locations (move down 1)
      this.posIndex.y++; // add 1 in the y to this gem (swapping) (move up 1)
    }


    else if (swipingDown && posIndex.y > 0)
    {
      // get the other gem where we swap places with
      otherGem = board.allGems[posIndex.x, posIndex.y - 1];

      otherGem.posIndex.y++;
      this.posIndex.y--;
    }


    else if (swipingLeft && posIndex.x > 0)
    {
      // get the other gem where we swap places with
      otherGem = board.allGems[posIndex.x - 1, posIndex.y];

      otherGem.posIndex.x++;
      this.posIndex.x--;
    }

    board.allGems[posIndex.x, posIndex.y] = this; // the one that is stored in this location is now this gem
    board.allGems[otherGem.posIndex.x, otherGem.posIndex.y] = otherGem;
  }

}
