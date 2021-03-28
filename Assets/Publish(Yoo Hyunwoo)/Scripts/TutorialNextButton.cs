using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TutorialNextButton : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    private Button button;
    public Image infoText;
    public Text stageText;
    public string lockedStageName;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(button.interactable == false)
        {
            infoText.gameObject.SetActive(true);
            stageText.text = lockedStageName;
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (button.interactable == false)
        {
            infoText.gameObject.SetActive(false);
        }
    }
}
