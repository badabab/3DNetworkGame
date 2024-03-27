using Photon.Pun;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    public static MinimapCamera Instance { get; private set; }
    public Character MyCharacter;
    public float YDistance = 20f;
    private Vector3 _initalEulerAngles;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        _initalEulerAngles = transform.eulerAngles;
    }
    private void LateUpdate()
    {
        if (MyCharacter == null)
        {
            return;
        }
        Vector3 targetPosition = MyCharacter.transform.position;
        targetPosition.y += YDistance;
        transform.position = targetPosition;

        Vector3 targetEulerAngles = MyCharacter.transform.eulerAngles;
        targetEulerAngles.x = _initalEulerAngles.x;
        targetEulerAngles.z = _initalEulerAngles.z;
        transform.eulerAngles = targetEulerAngles;
    }
}
