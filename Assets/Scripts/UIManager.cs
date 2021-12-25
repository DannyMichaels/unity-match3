using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
  public TMP_Text timeText, scoreText, winScore, winText;
  public GameObject roundOverScreen;
  public GameObject winStars1, winStars2, winStars3;

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

  }
}
