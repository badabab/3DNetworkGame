using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    private Transform _character;
    public float YDistance = 20f;
    private Vector3 _initalEulerAngles;

    private void Start()
    {
        _character = GetComponent<Transform>();
        _initalEulerAngles = transform.eulerAngles;
    }
    private void LateUpdate()
    {
        Vector3 targetPosition = _character.position;
        targetPosition.y += YDistance;
        transform.position = targetPosition;

        Vector3 targetEulerAngles = _character.eulerAngles;
        targetEulerAngles.x = _initalEulerAngles.x;
        targetEulerAngles.z = _initalEulerAngles.z;
        transform.eulerAngles = targetEulerAngles;
    }
}
