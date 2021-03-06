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

  [HideInInspector]
  public MatchFinder matchFinder;

  public enum BoardState { wait, move };
  public BoardState currentState = BoardState.move;

  public Gem bomb;
  public float bombChance = 2f;

  [HideInInspector]
  public RoundManager roundManager;

  private float bonusMulti;
  public float bonusAmount = .5f;

  private BoardLayout boardLayout;
  private Gem[,] layoutStore;

  void Awake()
  {
    matchFinder = FindObjectOfType<MatchFinder>();
    roundManager = FindObjectOfType<RoundManager>();
    boardLayout = GetComponent<BoardLayout>(); // if it can't find it it will just leave boardLayout as null
  }

  void Start()
  {
    allGems = new Gem[width, height];

    layoutStore = new Gem[width, height];

    Setup();
  }

  void Update()
  {
    // matchFinder.FindAllMatches();

    if (Input.GetKeyDown(KeyCode.S))
    {
      ShuffleBoard();
    }
  }

  private void Setup()
  {
    if (boardLayout != null)
    {
      layoutStore = boardLayout.GetLayout();
    }

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

        if (layoutStore[x, y] != null)
        {
          SpawnGem(new Vector2Int(x, y), layoutStore[x, y]);
        }
        else
        {
          int randomGemIndex = Random.Range(0, gems.Length); // get a random gem variant from the gems array

          int iterations = 0;
          // if there is a match, pick a new random gem index (don't start game with matches)
          while (MatchesAt(new Vector2Int(x, y), gems[randomGemIndex]) && iterations < 100)
          {
            randomGemIndex = Random.Range(0, gems.Length); // get a random gem variant from the gems array
            iterations++; // stop infinite looping (when only 2 gem variants there will always be a match)
          }

          SpawnGem(new Vector2Int(x, y), gems[randomGemIndex]);
        }
      }
    }
  }

  // Vector2Int: always whole numbers
  private void SpawnGem(Vector2Int position, Gem gemToSpawn)
  {
    // if the random number is less than the bombChange, spawn a bomb instead.
    if (Random.Range(0f, 100f) < bombChance)
    {
      gemToSpawn = bomb;
    }

    // add height of board in position.y so gem "slides" in when spawned
    Vector3 compatiblePos = new Vector3(position.x, position.y + height, 0f); // Vector2Int doesn't work with Instantiate, it wants a vector 3

    // create the gem and spawn it on the board
    Gem gem = Instantiate(gemToSpawn, compatiblePos, Quaternion.identity);
    gem.transform.parent = transform; // make it child of Board.
    gem.name = $"Gem - {position.x}, {position.y}";

    allGems[position.x, position.y] = gem; // store the new gem in the allGems array.

    gem.SetupGem(position, this); // let the gem know what is it's position and board
  }

  bool MatchesAt(Vector2Int posToCheck, Gem gemToCheck)
  {
    // check to left as long as it's greater than 1
    if (posToCheck.x > 1)
    {
      if (allGems[posToCheck.x - 1, posToCheck.y].type == gemToCheck.type && allGems[posToCheck.x - 2, posToCheck.y].type == gemToCheck.type)
      {
        return true;
      }
    }

    // check below
    if (posToCheck.y > 1)
    {
      if (allGems[posToCheck.x, posToCheck.y - 1].type == gemToCheck.type && allGems[posToCheck.x, posToCheck.y - 2].type == gemToCheck.type)
      {
        return true;
      }
    }

    return false;
  }

  // specifically destroy individual gem
  private void DestroyMatchedGemAt(Vector2Int pos)
  {
    Gem gemToDestroy = allGems[pos.x, pos.y];

    if (gemToDestroy != null)
    {
      if (gemToDestroy.isMatched)
      {
        gemToDestroy.PlayDestroyedSound();

        Instantiate(gemToDestroy.destroyEffect, new Vector2(pos.x, pos.y), Quaternion.identity); // create particle effects

        Destroy(gemToDestroy.gameObject);
        allGems[pos.x, pos.y] = null; // make sure gemToDestroy is null (don't use the var because it won't mutate the array.)
      }
    }
  }

  // destroy any matches on the board
  public void DestroyMatches()
  {
    for (int i = 0; i < matchFinder.currentMatches.Count; i++)
    {
      if (matchFinder.currentMatches[i] != null)
      {
        ScoreCheck(matchFinder.currentMatches[i]);

        DestroyMatchedGemAt(matchFinder.currentMatches[i].posIndex);
      }
    }

    StartCoroutine(DecreaseRowCo());
  }


  // @method DecreaseRowCo
  // @desc: make gems fall after matches have been destroyed
  private IEnumerator DecreaseRowCo()
  {
    yield return new WaitForSeconds(.2f);

    int nullCounter = 0;

    for (int x = 0; x < width; x++)
    {
      for (int y = 0; y < height; y++)
      {
        if (allGems[x, y] == null)
        {
          nullCounter++;
        }
        else if (nullCounter > 0)
        {
          allGems[x, y].posIndex.y -= nullCounter;
          allGems[x, y - nullCounter] = allGems[x, y];
          allGems[x, y] = null;
        }
      }

      nullCounter = 0;
    }

    StartCoroutine(FillBoardCo());
  }


  // @method FillBoardCo
  // @desc fill the board with gems after matches have been destroyed and gems have fell, then check for any new matches.
  private IEnumerator FillBoardCo()
  {
    yield return new WaitForSeconds(.5f);

    RefillBoard();

    yield return new WaitForSeconds(.5f);

    // after board has been refilled, check for any new matches to destroy (cascading effect that is so loved in Match 3 games)
    matchFinder.FindAllMatches();
    if (matchFinder.currentMatches.Count > 0)
    {
      bonusMulti++;

      yield return new WaitForSeconds(.5f);
      DestroyMatches();
    }
    else
    {
      yield return new WaitForSeconds(.5f);
      currentState = BoardState.move;

      bonusMulti = 0f;
    }
  }

  // @method RefillBoard
  // @desc: gets called in FillBoardCo after matches have been destroyed
  private void RefillBoard()
  {
    for (int x = 0; x < width; x++)
    {
      for (int y = 0; y < height; y++)
      {
        // only spawn gem at that spot if it's null
        if (allGems[x, y] == null)
        {
          int gemVariantIdx = Random.Range(0, gems.Length);

          SpawnGem(new Vector2Int(x, y), gems[gemVariantIdx]);
        }
      }
    }

    CheckMisplacedGems();
  }

  // EDGECASE: find any gems that haven't been assigned to board and just remove them.
  private void CheckMisplacedGems()
  {
    List<Gem> foundGems = new List<Gem>();

    // find all gems in the scene and add them to the list
    foundGems.AddRange(FindObjectsOfType<Gem>());

    for (int x = 0; x < width; x++)
    {
      for (int y = 0; y < height; y++)
      {
        // foundGems should be all the gems that aren't included in the board. So if it's in the board (allGems). remove it
        if (foundGems.Contains(allGems[x, y]))
        {
          foundGems.Remove(allGems[x, y]); // remove it from foundGems because it's on the board
        }
      }

    }

    // after the loops we will be left with only gems that haven't been assigned to the board
    foreach (Gem gem in foundGems)
    {
      // destroy all misplaced gems
      Destroy(gem.gameObject);
    }
  }

  public void ShuffleBoard()
  {
    /* Get all the gems that currently exist on the board and add them to a list. 
      loop through all the available spaces and place the gems into place

      try to make sure there are no matches    
    */

    if (currentState != BoardState.wait)
    {
      currentState = BoardState.wait;
      List<Gem> gemsFromBoard = new List<Gem>();

      for (int x = 0; x < width; x++)
      {
        for (int y = 0; y < height; y++)
        {
          gemsFromBoard.Add(allGems[x, y]);
          allGems[x, y] = null;
        }
      }

      for (int x = 0; x < width; x++)
      {
        for (int y = 0; y < height; y++)
        {
          int randomGemIdx = Random.Range(0, gemsFromBoard.Count);
          int iterations = 0;

          while (MatchesAt(new Vector2Int(x, y), gemsFromBoard[randomGemIdx]) && iterations < 100 && gemsFromBoard.Count > 1)
          {
            randomGemIdx = Random.Range(0, gemsFromBoard.Count);
            iterations++;
          }

          gemsFromBoard[randomGemIdx].SetupGem(new Vector2Int(x, y), this);
          allGems[x, y] = gemsFromBoard[randomGemIdx];
          gemsFromBoard.RemoveAt(randomGemIdx);
        }
      }

      StartCoroutine(FillBoardCo());
    }
  }

  public void ScoreCheck(Gem gemToCheck)
  {
    roundManager.currentScore += gemToCheck.scoreValue;

    if (bonusMulti > 0)
    {
      float bonusToAdd = gemToCheck.scoreValue * bonusMulti * bonusAmount;
      roundManager.currentScore += Mathf.RoundToInt(bonusToAdd);
    }
  }
}
