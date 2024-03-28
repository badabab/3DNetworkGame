using UnityEngine;
using Cinemachine;

public class CameraShakeOnDamage : MonoBehaviour
{
    public float ShakeTime = 0.3f;
    public float Amplitude = 2f;

    private CinemachineImpulseSource impulseSource;
    private CinemachineVirtualCamera Camera;

    private void Start()
    {
        GameObject followCamera = GameObject.FindWithTag("FollowCamera");
        Camera = followCamera.GetComponent<CinemachineVirtualCamera>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }
    public void ShakeCamera()
    {
        Vector3 randomDirection = Random.onUnitSphere;  // 랜덤 방향 벡터 생성
        Vector2 impulse = new Vector2(randomDirection.x, randomDirection.y) * Amplitude;

        impulseSource.GenerateImpulse(impulse);
        Invoke("StopShake", ShakeTime);
    }
    private void StopShake()
    {
        impulseSource.GenerateImpulse(Vector2.zero);
    }
}
