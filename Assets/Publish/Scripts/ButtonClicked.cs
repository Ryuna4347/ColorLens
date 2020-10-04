using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonClicked : MonoBehaviour
{
    public void PlaySound(string name)
    {
        SoundManager.instance.Play(name);
    }
}
