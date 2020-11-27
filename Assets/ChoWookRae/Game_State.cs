using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///인게임 별이동 UI
/// </summary>
public class Game_State : MonoBehaviour
{
    public Text MoveText;//인게임 텍스트UI
    public Animator[] StarAnims;//인게임 별이동 

    private void Update()
    {
        InGameStates();
    }

    void InGameStates()
    {
        if (GameManager.instance.moveCount > GameManager.instance.baseMoveCount[0])
            StarAnims[1].Play("Star_Delete");//세번째 별
         else if (GameManager.instance.moveCount > GameManager.instance.baseMoveCount[1])
            StarAnims[0].Play("Star_Delete");//세번째 별
        MoveText.text = "이동횟수 : " + GameManager.instance.moveCount;

    }
}
