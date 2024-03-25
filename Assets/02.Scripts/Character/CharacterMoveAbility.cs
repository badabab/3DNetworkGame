using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class CharacterMoveAbility : CharacterAbility
{
    // 목표: [W][A][S][D] 및 방향키를 누르면 캐릭터를 그 방향으로 이동시키고 싶다.

    private float _gravity = -20; // 중력 변수
    private float _yVelocity = 0f; // 누적할 중력 변수
    private CharacterController _characterController;
    private Animator _animator;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }
    private void Update()
    {
        // 순서
        // 1. 사용자의 키보드 입력을 받는다.
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // 2. '캐릭터가 바라보는 방향'을 기준으로 방향을 설정한다.
        Vector3 dir = new Vector3 (h, 0, v);
        dir.Normalize();
        dir = Camera.main.transform.TransformDirection(dir);
        _animator.SetFloat("Move", dir.magnitude);  //magnitude: 벡터의 길이

        // 3. 중력 적용하세요.
        _yVelocity += _gravity * Time.deltaTime;
        dir.y = _yVelocity;
        // dir.y = -1f;

        /*float speed = MoveSpeed;
        if (Input.GetKey(KeyCode.LeftShift) && (h != 0 || v != 0))
        {
            speed = RunSpeed;
        }*/

        // 4. 이동속도에 따라 그 방향으로 이동한다.
        _characterController.Move(dir * Owner.Stat.MoveSpeed * Time.deltaTime);
    }
}