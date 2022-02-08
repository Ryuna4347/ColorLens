using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorKey : MonoBehaviour
{
    [SerializeField] private List<Door> linkedDoor;

    private void Awake()
    {
        List<GameObject> doorList = new List<GameObject>(GameObject.FindGameObjectsWithTag("Door"));
        if (doorList.Count == 0)
        {
#if UNITY_EDITOR
            Debug.LogError("문이 없습니다.");
#endif
            //오류 출력!
        }
        else
        {
            linkedDoor = doorList.ConvertAll(new System.Converter<GameObject, Door>(ConvertToDoor));
        }
    }

    Door ConvertToDoor(GameObject obj)
    {
        return obj.GetComponent<Door>();
    }

    public void GetKey()
    {
        foreach (Door door in linkedDoor)
        {
            door.CheckGainedKey(gameObject);
        }
        GameManager.instance.UpdateObjectActive(gameObject, transform.position, false);
        gameObject.SetActive(false);
    }
}
