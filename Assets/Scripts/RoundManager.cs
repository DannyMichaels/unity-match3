using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
  public float roundTime = 60f;
  private UIManager uiManager;

  private bool endingRound = false;
  private Board board;

  public int currentScore;
  public float displayScore;
  public float scoreSpeed = 5f;

  public int scoreTarget1, scoreTarget2, scoreTarget3;

  void Awake()
  {
    uiManager = FindObjectOfType<UIManager>();
    board = FindObjectOfType<Board>();
  }

  // Update is called once per frame
  void Update()
  {
    SetRoundTime();

    if (endingRound && board.currentState == Board.BoardState.move)
    {
      WinCheck();
      endingRound = false;
    }

    SetUIScore();
  }


  void SetRoundTime()
  {
    if (roundTime > 0)
    {
      roundTime -= Time.deltaTime;

      if (roundTime <= 0)
      {
        roundTime = 0;

        endingRound = true;
      }
    }


    uiManager.timeText.text = roundTime.ToString("0.0") + "s"; // set text
  }

  private void SetUIScore()
  {
    displayScore = Mathf.Lerp(displayScore, currentScore, scoreSpeed * Time.deltaTime); // lerp from current score to new score. instead of instantly going to new score

    uiManager.scoreText.text = displayScore.ToString("0"); // no decimal places, whole number
  }

  private void WinCheck()
  {
    uiManager.roundOverScreen.SetActive(true);

    uiManager.winScore.text = currentScore.ToString();

    if (currentScore >= scoreTarget3)
    {
      uiManager.winText.text = "Congratulations! You earned 3 stars!";
      uiManager.winStars3.SetActive(true);
    }
    else if (currentScore >= scoreTarget2)
    {
      uiManager.winText.text = "Congratulations! You earned 2 stars!";
      uiManager.winStars2.SetActive(true);
    }

    else if (currentScore >= scoreTarget1)
    {
      uiManager.winText.text = "Congratulations! You earned 1 star!";
      uiManager.winStars1.SetActive(true);
    }

    else
    {
      uiManager.winText.text = "Oh no, No stars for you! Try again?";
    }
  }
}
