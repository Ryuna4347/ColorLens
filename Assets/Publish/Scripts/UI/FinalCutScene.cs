using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FinalCutScene : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        ClearEffect.instance.Clear();
        PlayerPrefs.SetInt("FinalCutScene", 1);
        gameObject.SetActive(false);
    }
}
