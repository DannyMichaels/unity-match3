using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
  public static SFXManager instance;

  public AudioSource gemSound, explodeSound, stoneSound, roundOverSound;

  private void Awake()
  {
    instance = this;
  }


  private static void PlaySound(AudioSource sound)
  {
    sound.Stop();

    sound.pitch = Random.Range(.8f, 1.2f);

    sound.Play();
  }

  public void PlayGemBreak()
  {
    PlaySound(gemSound);
  }

  public void PlayExplode()
  {
    PlaySound(explodeSound);
  }

  public void PlayStoneBreak()
  {
    PlaySound(stoneSound);
  }

  public void PlayRoundOver()
  {
    roundOverSound.Play();
  }
}
