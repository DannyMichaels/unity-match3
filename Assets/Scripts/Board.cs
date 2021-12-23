using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
  public int width, height;

  public GameObject bgTilePrefab;

  // Start is called before the first frame update
  void Start()
  {
    Setup();
  }


  private void Setup()
  {
    // create a x by y Board, example: 7x7 
    // note: Camera x should be board width divided by 2 (rounded down to nearest whole number), same with camera Y
    for (int x = 0; x < width; x++)
    {
      for (int y = 0; y < height; y++)
      {
        Vector2 position = new Vector2(x, y);
        GameObject bgTile = Instantiate(bgTilePrefab, position, Quaternion.identity); // Quartenion: have 0 rotation
        bgTile.transform.parent = transform; // make it child of Board.
        bgTile.name = $"BG Tile - {x}, {y}";
      }
    }
  }
}
