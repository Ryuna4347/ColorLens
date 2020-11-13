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

    public void TeleportCharacter(GameObject charac, Direction dir, int remainMoveCount)
    {
        StartCoroutine(Teleport(charac, dir, remainMoveCount));
    }

    public IEnumerator Teleport(GameObject charac, Direction dir, int remainMoveCount)
    {
        PlayerMove1 characMove;
        if ((characMove = charac.GetComponent<PlayerMove1>()) == null || connectedPortal == null)
            yield break; 

        yield return new WaitForSeconds(0.1f);

        Vector3 teleportPos = connectedPortal.transform.position;
        teleportPos.z = charac.transform.position.z;
        charac.transform.position = teleportPos;

        yield return new WaitForSeconds(0.1f);

        characMove.CalculateRoute(dir,remainMoveCount);
    }

}
