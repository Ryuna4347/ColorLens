using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Stage_Number : MonoBehaviour
{
    public Text Stage_Text;
    [SerializeField]List<int> baseMoveCount;//각 맵마다 별을 획득할 수 있는 이동횟수 기준(2개)

    public GameObject PausePanel;
    public GameObject GameoverPanel;
    public GameObject ClearPanel;
    public Text G_Star_requirement;
    void Start()
    {
        baseMoveCount = GameObject.Find("Map").GetComponent<Map>().GetBaseMoveCount();
        Stage_Text.text = SceneManager.GetActiveScene().name;
    }

    private void Update()
    {
        if (PausePanel.activeSelf == true || GameoverPanel.activeSelf == true || ClearPanel.activeSelf == true)
        {
            GameObject activeParent = PausePanel.activeSelf ? PausePanel : (GameoverPanel.activeSelf ? GameoverPanel : ClearPanel);
            G_Star_requirement = activeParent.transform.Find("Star requirement").gameObject.GetComponent<Text>();
            G_Star_requirement.text = "별 3개: " + baseMoveCount[1] + "\n별 2개: " + baseMoveCount[0];
        }
    }
}
