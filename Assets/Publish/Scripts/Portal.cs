using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private Portal connectedPortal;

    private void Start()
    {
#if UNITY_EDITOR
        if (connectedPortal == null)
            Debug.LogError(gameObject.name + "와 연결된 포탈이 없습니다.");
#endif
    }

    public void TeleportCharacter(PlayerMove1 character)
    {
        StartCoroutine(Teleport(character));
    }

    private IEnumerator Teleport(PlayerMove1 character)
    {
        if (character == null || connectedPortal == null)
            yield break; 

        yield return new WaitForSeconds(0.1f);

        Vector3 teleportPos = connectedPortal.transform.position;
        teleportPos.z = character.transform.position.z;
        character.transform.position = teleportPos;

        yield return new WaitForSeconds(0.1f);

        character.CalculateRoute(character.routeList[character.routeList.Count - 1], character.routeList.Count);
    }

}
