using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorKey : MonoBehaviour
{
    [SerializeField] private Door linkedDoor;

    private void Awake()
    {
        List<GameObject> doorList = new List<GameObject>(GameObject.FindGameObjectsWithTag("Door"));
        if (doorList.Count != 1)
        {
#if UNITY_EDITOR
            Debug.LogError("문이 없거나 2개 이상입니다.");
#endif
            //오류 출력!
        }
        else
        {
            linkedDoor = doorList[0].GetComponent<Door>();
        }
    }

    public void GetKey()
    {
        linkedDoor.CheckGainedKey(gameObject);
        GameManager.instance.UpdateObjectActive(gameObject, transform.position, false);
        gameObject.SetActive(false);
    }
}
