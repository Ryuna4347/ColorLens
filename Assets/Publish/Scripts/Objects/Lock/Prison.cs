using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prison : LockBase
{
    public float disappearTime;
    [SerializeField] private GameObject holdingCharcter;
    [SerializeField] private List<string> colorCombination; //colorNow 색상 안에 들어가는 3원색 리스트
    private SpriteRenderer spriteRenderer;
    [SerializeField] protected string keyObjName;

    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
#if UNITY_EDITOR
        if (keyObjName == null)
            Debug.LogError(gameObject.name + " : 해제 오브젝트가 등록되어있지 않습니다");
#endif
    }
    private void Start()
    {
        colorCombination = GameManager.instance.GetColorCombination(keyObjName);
        RefreshPrisonColor();
    }

    private void OnDisable()
    {
        if (holdingCharcter != null)
            holdingCharcter.GetComponent<PlayerMove1>().holdByPrison = false;
    }
    private void RefreshPrisonColor()
    {
        Color newColor = new Color(0, 0, 0, 1);
        newColor.r = colorCombination.Contains("Red") ? 1 : 0;
        newColor.g = colorCombination.Contains("Green") ? 1 : 0;
        newColor.b = colorCombination.Contains("Blue") ? 1 : 0;
        spriteRenderer.color = newColor;
    }

    public void EraseColor(string colorName)
    {
        List<string> compareColorList = GameManager.instance.GetColorCombination(colorName);

        if (keyObjName.Equals(colorName))
        {
            StartCoroutine("DisappearPrison");
        }
        else
        {
            foreach (string compareColor in compareColorList)
            {
                if (colorCombination.Contains(compareColor)) //겹치는 색이 있으면 제거
                {
                    colorCombination.Remove(compareColor);
                }

                if (colorCombination.Count == 0)
                {
                    StartCoroutine("DisappearPrison");
                    return;
                }
            }
            keyObjName = GameManager.instance.GetColorName(colorCombination); //현재 남아있는 색상 조합은 무슨 색인지 갱신
            RefreshPrisonColor();
        }
    }


    private IEnumerator DisappearPrison()
    {
        float time = 0f;
        Color color = gameObject.GetComponent<SpriteRenderer>().color;

        while (time < disappearTime)
        {
            color.a -= 0.05f;
            time += 0.05f;
            yield return new WaitForSeconds(0.05f);
        }
        gameObject.SetActive(false);
    }
}
