using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class Bear : MonoBehaviour
{
    // 곰 상태 상수(열거형)
    public enum BearState
    {
        Idle,
        Patrol,
        Trace,
        Return,
        Attack,
        Hit,
        Death,
    }

    private BearState _state = BearState.Idle;

    public Animator MyAnimatior;
    public NavMeshAgent Agent;

    private List<Character> _characterList = new List<Character>();
    public SphereCollider CharacterDetectCollider;
    private Character _targetCharacter;

    public Stat Stat;

    // [Idle]
    public float TraceDetectRange = 5f;
    public float IdleMaxTime = 5f;
    private float _idleTime = 0f;

    // [Patrol]
    public Transform PatrolDestination;

    // [Return]
    private Vector3 _startPosition;

    // [Attack]
    public float AttackDistance = 3f;
    private float _attackTimer = 0f;

    private void Start()
    {
        Agent.speed = Stat.MoveSpeed;
        _startPosition = transform.position;
        CharacterDetectCollider.radius = TraceDetectRange;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            Character character = col.GetComponent<Character>();
            if (!_characterList.Contains(character))
            {
                Debug.Log("새로운 인간을 찾았다!");
                _characterList.Add(character);
            }
        }
    }

    private void Update()
    {
        // 조기 반환
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        switch (_state)
        {
            case BearState.Idle:
            {
                Idle();
                break;
            }
            case BearState.Patrol:
            {
                Patrol();
                break;
            }
            case BearState.Trace:
            {
                Trace();
                break;
            }
            case BearState.Return:
            {
                Return();
                break;
            }
            case BearState.Attack:
            {
                Attack();
                break;
            }
        }
    }

    private void Idle()
    {
        _idleTime += Time.deltaTime;
        if (_idleTime >= IdleMaxTime)
        {
            _idleTime = 0f;
            SetRandomPatrolDestination();
            _state = BearState.Patrol;
            RequestPlayAnimation("Run");
            Debug.Log("Idle -> Patrol");
        }

        _targetCharacter = FindTarget(TraceDetectRange);
        if (_targetCharacter != null)
        {
            _startPosition = transform.position;
            SetRandomPatrolDestination();
            _state = BearState.Trace;
            RequestPlayAnimation("Run");
            Debug.Log("Idle -> Trace");
        }
    }

    private void Patrol()
    {
        if (PatrolDestination == null)
        {
            PatrolDestination = GameObject.Find("Patrol").transform;
        }
        Agent.destination = PatrolDestination.position;
        Agent.stoppingDistance = 0f;

        _targetCharacter = FindTarget(TraceDetectRange);
        if (_targetCharacter != null)
        {
            _state = BearState.Trace;
            RequestPlayAnimation("Run");
            Debug.Log("Patrol -> Trace");
        }

        if (!Agent.pathPending && Agent.remainingDistance <= 0.1f)
        {
            _state = BearState.Return;
            RequestPlayAnimation("Run");
            Debug.Log("Patrol -> Return");
        }
    }

    private void Return()
    {
        // [시작 위치]까지 간다.
        Agent.destination = _startPosition;
        Agent.stoppingDistance = 0f;

        if (!Agent.pathPending && Agent.remainingDistance <= 0.1f)
        {
            _state = BearState.Idle;
            RequestPlayAnimation("Idle");
            Debug.Log("Return -> Idle");
        }

        // IF [플레이어]가 [감지 범위]안에 들어오면 플레이어 (추적 상태로 전이)
        _targetCharacter = FindTarget(TraceDetectRange);
        if (_targetCharacter != null)
        {
            _state = BearState.Trace;
            RequestPlayAnimation("Run");
            Debug.Log("Return -> Trace");
        }
    }

    private void Trace()
    {
        // 타겟이 게임에서 나가면 복귀
        if (_targetCharacter == null)
        {
            Debug.Log("Trace -> Return");
            _state = BearState.Return;
            return;
        }

        // 타겟이 죽거나 너무 멀어지면 복귀
        Agent.destination = _targetCharacter.transform.position;
        if (_targetCharacter.State == State.Death || GetDistance(_targetCharacter.transform) > TraceDetectRange)
        {
            Debug.Log("Trace -> Patrol");
            _startPosition = transform.position;
            SetRandomPatrolDestination();
            _state = BearState.Patrol;
            return;
        }

        // 타겟이 가까우면 공격 상태로 전이
        if (GetDistance(_targetCharacter.transform) <= AttackDistance)
        {
            Agent.isStopped = true;
            Debug.Log("Trace -> Attack");
            MyAnimatior.Play("Idle");

            Agent.isStopped = true;
            Agent.ResetPath();
            Agent.stoppingDistance = AttackDistance;

            _state = BearState.Attack;
            return;
        }
    }

    private void Attack()
    {
        // 타겟이 게임에서 나가면 복귀
        if (_targetCharacter == null)
        {
            Debug.Log("Trace -> Return");
            Agent.isStopped = false;
            _startPosition = transform.position;
            SetRandomPatrolDestination();
            _state = BearState.Trace;
            return;
        }

        // 타겟이 죽거나 공격 범위에서 벗어나면 복귀
        Agent.destination = _targetCharacter.transform.position;
        if (_targetCharacter.State == State.Death || GetDistance(_targetCharacter.transform) > AttackDistance)
        {
            Debug.Log("Trace -> Return");
            Agent.isStopped = false;
            _startPosition = transform.position;
            _state = BearState.Idle;
            return;
        }

        _attackTimer += Time.deltaTime;
        if (_attackTimer >= Stat.AttackCoolTime)
        {
            transform.LookAt(_targetCharacter.transform);

            _attackTimer = 0f;
            RequestPlayAnimation("Attack");
        }
    }



    // 나와의 거리가 distance보다 짧은 플레이어를 반환
    private Character FindTarget(float distance)
    {
        _characterList.RemoveAll(c => c == null);

        Vector3 myPosition = transform.position;
        foreach (Character character in _characterList)
        {
            if (character.State == State.Death)
            {
                continue;
            }

            if (Vector3.Distance(character.transform.position, myPosition) <= distance)
            {
                return character;
            }
        }

        return null;
    }
    private List<Character> FindTargets(float distance)
    {
        _characterList.RemoveAll(c => c == null);

        List<Character> characters = new List<Character>();

        Vector3 myPosition = transform.position;
        foreach (Character character in _characterList)
        {
            if (character.State == State.Death)
            {
                continue;
            }

            if (Vector3.Distance(character.transform.position, myPosition) <= distance)
            {
                characters.Add(character);
            }
        }

        return characters;
    }


    private float GetDistance(Transform otherTransform)
    {
        return Vector3.Distance(transform.position, otherTransform.position);
    }


    public void AttackAction()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        Debug.Log("AttackAction!");
        // 일정 범위 안에 있는 모든 플레이어에게 데미지를 주고 싶다.
        List<Character> targets = FindTargets(AttackDistance + 0.1f);
        foreach (Character target in targets)
        {
            Vector3 dir = (target.transform.position - transform.position).normalized;
            int viewAngle = 160 / 2;
            float angle = Vector3.Angle(transform.forward, dir);
            Debug.Log(angle);
            if (Vector3.Angle(transform.forward, dir) < viewAngle)
            {
                target.PhotonView.RPC("Damaged", RpcTarget.All, Stat.Damage, -1);
            }

        }
    }

    private void RequestPlayAnimation(string animationName)
    {
        GetComponent<PhotonView>().RPC(nameof(PlayAnimation), RpcTarget.All, animationName);
    }

    [PunRPC]
    private void PlayAnimation(string animationName)
    {
        MyAnimatior.Play(animationName);
    }


    private void SetRandomPatrolDestination()
    {
        List<GameObject> randomPoints = GameObject.FindGameObjectsWithTag("PatrolPoint").ToList();
        PatrolDestination = randomPoints[UnityEngine.Random.Range(0, randomPoints.Count)].transform;
    }
}