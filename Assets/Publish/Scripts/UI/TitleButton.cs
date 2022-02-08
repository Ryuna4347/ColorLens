using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TitleButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Text text;

    private void Awake()
    {
        text = transform.GetChild(0).gameObject.GetComponent<Text>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.color = new Color(0.5f, 0.5f, 0.5f);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        text.color = new Color(1,1,1);
    }
}
