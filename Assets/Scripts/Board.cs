using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
  public int width, height;

  public GameObject bgTilePrefab;

  public Gem[] gems; // the gem variants to randomly pick from when spawning the gems

  // https://stackoverflow.com/questions/12567329/multidimensional-array-vs
  public Gem[,] allGems; // stored gems data on the board (2D Array), stores an X and a Y value for each gem.

  public float gemSpeed;


  // Start is called before the first frame update
  void Start()
  {
    allGems = new Gem[width, height];
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
        // add background tile
        Vector2 position = new Vector2(x, y);
        GameObject bgTile = Instantiate(bgTilePrefab, position, Quaternion.identity); // Quartenion: have 0 rotation
        bgTile.transform.parent = transform; // make it child of Board.
        bgTile.name = $"BG Tile - {x}, {y}";

        int randomGemIndex = Random.Range(0, gems.Length); // get a random gem variant from the gems array
        SpawnGem(new Vector2Int(x, y), gems[randomGemIndex]);
      }
    }
  }

  // Vector2Int: always whole numbers
  private void SpawnGem(Vector2Int position, Gem gemToSpawn)
  {
    Vector3 compatiblePos = new Vector3(position.x, position.y, 0f); // Vector2Int doesn't work with Instantiate, it wants a vector 3

    // create the gem and spawn it on the board
    Gem gem = Instantiate(gemToSpawn, compatiblePos, Quaternion.identity);
    gem.transform.parent = transform; // make it child of Board.
    gem.name = $"Gem - {position.x}, {position.y}";

    allGems[position.x, position.y] = gem; // store the new gem in the allGems array.

    gem.SetupGem(position, this); // let the gem know what is it's position and board
  }
}
