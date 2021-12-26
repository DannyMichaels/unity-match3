using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // extra stuff that lists can do

public class MatchFinder : MonoBehaviour
{
  private Board board;
  public List<Gem> currentMatches = new List<Gem>(); // a list is very easy to manipulate compared to an array.

  private void Awake()
  {
    board = FindObjectOfType<Board>();
  }

  public void FindAllMatches()
  {
    currentMatches.Clear(); // clear the list every time this is called because we want to make sure it's empty every-time it starts

    for (int x = 0; x < board.width; x++)
    {
      for (int y = 0; y < board.height; y++)
      {

        Gem currentGem = board.allGems[x, y];
        if (currentGem != null)
        {
          CheckMatchesHorizontal(x, y, currentGem); // left to right
          CheckMatchesVertical(x, y, currentGem); // up and down
        }
      }
    }

    // if there are any matches
    if (currentMatches.Count > 0)
    {
      // Distinct comes from System.Linq.

      currentMatches = currentMatches.Distinct().ToList();   // Distinct: generates another list that only holds unique objectss
    }

    CheckForBombs();
  }
  private void CheckMatchesHorizontal(int x, int y, Gem currentGem)
  {
    // make sure it's in bounds
    if (x > 0 && x < board.width - 1)
    {
      Gem leftGem = board.allGems[x - 1, y];
      Gem rightGem = board.allGems[x + 1, y];

      if (leftGem != null && rightGem != null)
      {
        CheckMatch3(currentGem, leftGem, rightGem);
      }
    }
  }

  private void CheckMatchesVertical(int x, int y, Gem currentGem)
  {
    // make sure it's in bounds
    if (y > 0 && y < board.height - 1)
    {
      Gem aboveGem = board.allGems[x, y + 1];
      Gem belowGem = board.allGems[x, y - 1];

      if (aboveGem != null && belowGem != null)
      {
        CheckMatch3(currentGem, belowGem, aboveGem);
      }
    }
  }

  private void CheckMatch3(Gem currentGem, Gem gem2, Gem gem3)
  {
    if (currentGem.type == Gem.GemType.stone) return;

    if (gem2.type == currentGem.type && gem3.type == currentGem.type)
    {
      // they're all the same type, we found a match
      currentGem.isMatched = true;
      gem2.isMatched = true;
      gem3.isMatched = true;

      // add the matching gems to the list
      currentMatches.Add(currentGem);
      currentMatches.Add(gem2);
      currentMatches.Add(gem3);
    }
  }


  // @desc  check each side of the matches to see if there is a bomb
  // and any gems that is around that bomb will explode
  public void CheckForBombs()
  {
    for (int i = 0; i < currentMatches.Count; i++)
    {
      Gem gem = currentMatches[i];
      int x = gem.posIndex.x;
      int y = gem.posIndex.y;

      // check to the left;
      if (x > 0)
      {
        if (board.allGems[x - 1, y] != null)
        {
          if (board.allGems[x - 1, y].type == Gem.GemType.bomb)
          {
            MarkBombArea(new Vector2Int(x - 1, y), board.allGems[x - 1, y]);
          }
        }
      }

      // check to the right
      if (x < board.width - 1)
      {
        if (board.allGems[x + 1, y] != null)
        {
          if (board.allGems[x + 1, y].type == Gem.GemType.bomb)
          {
            MarkBombArea(new Vector2Int(x + 1, y), board.allGems[x + 1, y]);
          }
        }
      }

      // check to the bottom;
      if (y > 0)
      {
        if (board.allGems[x, y - 1] != null)
        {
          if (board.allGems[x, y - 1].type == Gem.GemType.bomb)
          {
            MarkBombArea(new Vector2Int(x, y - 1), board.allGems[x, y - 1]);
          }
        }
      }

      // check upwards
      if (y < board.height - 1)
      {
        if (board.allGems[x, y + 1] != null)
        {
          if (board.allGems[x, y + 1].type == Gem.GemType.bomb)
          {
            MarkBombArea(new Vector2Int(x, y + 1), board.allGems[x, y + 1]);
          }
        }
      }
    }
  }

  public void MarkBombArea(Vector2Int bombPos, Gem theBomb)
  {
    // loop through all the gems that are around the bomb.
    for (int x = bombPos.x - theBomb.blastSize; x <= bombPos.x + theBomb.blastSize; x++)
    {
      for (int y = bombPos.y - theBomb.blastSize; y <= bombPos.y + theBomb.blastSize; y++)
      {
        if (x >= 0 && x < board.width && y >= 0 && y < board.height)
        {
          if (board.allGems[x, y] != null)
          {
            board.allGems[x, y].isMatched = true;
            currentMatches.Add(board.allGems[x, y]);
          }
        }
      }
    }

    currentMatches = currentMatches.Distinct().ToList();
  }
}
