using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCtrl : MonoBehaviour
{
  public static MoveCtrl instance;

  private void Awake()
  {
    instance = this;
  }

  public bool canMove = true;
}
