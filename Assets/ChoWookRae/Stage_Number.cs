using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Stage_Number : MonoBehaviour
{
    public Text Stage_Text;
    public Text Star_requirement;
    List<int> baseMoveCount;//각 맵마다 별을 획득할 수 있는 이동횟수 기준(2개)

    public GameObject PausePanel;
    public GameObject GameoverPanel;
    public GameObject ClearPanel;
    public GameObject G_Star_requirement;
    void Start()
    {
        baseMoveCount = GameObject.Find("Map").GetComponent<Map>().GetBaseMoveCount();
        Stage_Text.text = SceneManager.GetActiveScene().name;
    }

    private void Update()
    {
        if (PausePanel.active == true || GameoverPanel.active == true || ClearPanel.active == true)
            G_Star_requirement.SetActive(true);
        else
            G_Star_requirement.SetActive(false);
        Star_requirement.text = "별 3개: " +baseMoveCount[1] +"\n별 2개: "+ baseMoveCount[0];
    }
}
