using Photon.Pun;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    public Transform Character;
    public float YDistance = 20f;
    private Vector3 _initalEulerAngles;

    private void Start()
    {
        //if (_owner.PhotonView.IsMine)
            Character = GetComponent<Transform>();
        _initalEulerAngles = transform.eulerAngles;
    }
    private void LateUpdate()
    {
        Vector3 targetPosition = Character.position;
        targetPosition.y += YDistance;
        transform.position = targetPosition;

        Vector3 targetEulerAngles = Character.eulerAngles;
        targetEulerAngles.x = _initalEulerAngles.x;
        targetEulerAngles.z = _initalEulerAngles.z;
        transform.eulerAngles = targetEulerAngles;
    }
}
