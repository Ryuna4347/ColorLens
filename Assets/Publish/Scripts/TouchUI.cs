using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchUI : MonoBehaviour
{
    public Transform cursor;
    private float maxDistance; //cursor의 반경

    private void OnEnable() //Start인 경우 Instantiate된 직후 꺼지게 되어서 Start가 불려오지 않는다.
    {
        if (PlayerPrefs.GetInt("TouchType", 0) == 0)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            cursor = transform.GetChild(1).GetChild(0);
            maxDistance = ((RectTransform)transform.GetChild(0)).sizeDelta.x / 2;
        }
    }

    private void OnDisable()
    {
        if (PlayerPrefs.GetInt("TouchType", 0) == 0)
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            cursor.transform.position = new Vector2(0, 0);
            maxDistance = 0;
            transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    #region 스마트폰 관련 기능

    private void Update()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (PlayerPrefs.GetInt("TouchType", 0) == 1 && !GameManager.instance.IsGameOver && GameManager.instance.CanMove)
        {
            if (Input.touchCount > 0)
            {
                if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                {
                    
                    Touch touch = Input.GetTouch(0);
                    if (touch.phase == TouchPhase.Began)
                    {
                        transform.GetChild(1).gameObject.SetActive(true);
                        transform.position = touch.position;
                        StartCoroutine("CheckTouchDirection");
                    }
                }
            }
        }
#endif
    }

    private IEnumerator CheckTouchDirection()
    {
        Vector2 lastPos = new Vector2(0, 0);
        yield return null; //아래 phase 검사시 이게 없으면 해당 프레임은 Began이라서 한 프레임을 쉬고 들어간다.
        while (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
            {
                lastPos = touch.position;
                float vectorDist = (lastPos - (Vector2)transform.position).magnitude;
                cursor.localPosition = Mathf.Clamp(vectorDist,0,maxDistance) * (lastPos - (Vector2)transform.position).normalized;
            }
            else
            {
                break;
            }
            yield return null;
        }
        if ((lastPos - (Vector2)transform.position).magnitude <= 50) //기능하기 위한 최소거리
        {
            transform.GetChild(1).gameObject.SetActive(false);
            yield break;
        }

        Vector2 vec = (lastPos - (Vector2)transform.position).normalized;

        var angleRadians = Mathf.Atan2(vec.y, vec.x);
        var angleDegrees = angleRadians * Mathf.Rad2Deg;
        if (angleDegrees < 0)
            angleDegrees += 360;

        int dir = 0;
        if (angleDegrees <= 45 || angleDegrees > 315) //동쪽
            dir = 3;
        else if (angleDegrees > 45 && angleDegrees <= 135) //북쪽
            dir = 1;
        else if (angleDegrees > 135 && angleDegrees <= 225) //서쪽
            dir = 7;
        else //남쪽
            dir = 5;

        GameManager.instance.MoveCharacters(dir);
        transform.GetChild(1).gameObject.SetActive(false);
    }

    private IEnumerator CheckTouchDirectionMouse()
    {
        Vector2 lastPos = new Vector2(0, 0);

        while (Input.GetMouseButton(0))
        {
            lastPos = Input.mousePosition;
            float vectorDist = (lastPos - (Vector2)transform.position).magnitude;
            cursor.localPosition = Mathf.Clamp(vectorDist, 0, maxDistance) * (lastPos - (Vector2)transform.position).normalized;

            yield return null;
        }

        Vector2 vec = (lastPos - (Vector2)transform.position);
        if (vec.magnitude <= 100f)
            yield break;

        Debug.Log(vec.magnitude);

        //use atan2 to get the angle; Atan2 returns radians
        var angleRadians = Mathf.Atan2(vec.y, vec.x);

        //convert to degrees
        var angleDegrees = angleRadians * Mathf.Rad2Deg;

        //angleDegrees will be in the range (-180,180].
        //I like normalizing to [0,360) myself, but this is optional..
        if (angleDegrees < 0)
            angleDegrees += 360;

        int dir = 0;
        if (angleDegrees <= 45 || angleDegrees > 315)
        { //동쪽
            dir = 3;
        }
        else if (angleDegrees > 45 && angleDegrees <= 135)
        { //북쪽
            dir = 1;
        }
        else if (angleDegrees > 135 && angleDegrees <= 225)
        { //서쪽
            dir = 7;
        }
        else
        { //남쪽
            dir = 5;
        }
        GameManager.instance.MoveCharacters(dir);
    }

    public void MoveButtonClicked(int dir)
    {
        if(PlayerPrefs.GetInt("TouchType",0) == 0)
        {
            GameManager.instance.MoveCharacters(dir);
        }
    }

    #endregion
}
