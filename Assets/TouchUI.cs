using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchUI : MonoBehaviour
{
    [SerializeField] private RectTransform childTransform;
    [SerializeField] private Vector2 origin;
    [SerializeField] private Vector2 nowPos;

    private void Awake()
    {
        if (Application.platform != RuntimePlatform.Android || Application.platform != RuntimePlatform.IPhonePlayer)
        {
            Destroy(this.gameObject);
        }
        childTransform = transform.GetChild(0) as RectTransform;
    }

    void OnEnable()
    {
        if (Application.platform != RuntimePlatform.Android || Application.platform != RuntimePlatform.IPhonePlayer)
        {
            Destroy(this.gameObject);
        }
        if (Input.touchCount == 1)
        {
            origin = transform.position;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.touchCount == 1)
        {
            Touch finger = Input.GetTouch(0);
            if (finger.phase == TouchPhase.Ended || finger.phase == TouchPhase.Canceled)
            {

            }
            else
            {
                nowPos = Input.GetTouch(0).position;

                float distance = (nowPos - origin).magnitude;
                Vector2 clampedPos = Mathf.Clamp(distance, 0, 1) * (nowPos - origin).normalized + origin;
                transform.position = clampedPos;
            }
        }
    }
}
