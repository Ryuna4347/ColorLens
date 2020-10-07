using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FinalCutScene : MonoBehaviour, IPointerClickHandler {
    private void Start()
    {
        if(PlayerPrefs.GetInt("4-12",0)>0 && PlayerPrefs.GetInt("FinalCutScene",0)==0)
        {
            transform.localPosition = new Vector3(0, 0, 0);
        }
        else //아직 4-12를 클리어하지 못했거나 컷씬을 본 경우 이 캔버스는 필요가 없으므로 Off로 둔다.(클릭 이벤트가 있으므로 꺼두는게 문제가 안 생길 것)
        {
            gameObject.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PlayerPrefs.SetInt("FinalCutScene", 1);
        gameObject.SetActive(false);
    }
}
