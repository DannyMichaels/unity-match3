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
    // currentMatches.Clear();

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
}
