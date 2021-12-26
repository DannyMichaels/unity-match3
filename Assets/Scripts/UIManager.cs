using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
  public TMP_Text timeText, scoreText, winScore, winText;
  public GameObject roundOverScreen;
  public GameObject winStars1, winStars2, winStars3;

  private Board theBoard;

  public string levelSelect;

  public GameObject pauseScreen;

  private void Awake()
  {
    theBoard = FindObjectOfType<Board>();
  }
  // Start is called before the first frame update
  void Start()
  {
    winStars1.SetActive(false);
    winStars2.SetActive(false);
    winStars3.SetActive(false);
  }

  // Update is called once per frame
  void Update()
  {
    if (Input.GetKeyDown(KeyCode.Escape))
    {
      PauseUnpause();
    }
  }

  public void PauseUnpause()
  {
    if (!pauseScreen.activeInHierarchy)
    {
      pauseScreen.SetActive(true);
      Time.timeScale = 0f;
    }
    else
    {
      pauseScreen.SetActive(false);
      Time.timeScale = 1f;
    }

  }

  public void ShuffleBoard()
  {
    theBoard.ShuffleBoard();
  }

  public void QuitGame()
  {
    Application.Quit();
  }

  public void GoToLevelSelect()
  {
    Time.timeScale = 1f;
    SceneManager.LoadScene(levelSelect);
  }

  public void TryAgain()
  {
    string currentScene = SceneManager.GetActiveScene().name;
    SceneManager.LoadScene(currentScene);
  }
}
