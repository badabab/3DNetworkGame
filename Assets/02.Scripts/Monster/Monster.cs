using UnityEngine;
using UnityEngine.AI;

public enum MonsterState
{
    Idle,
    Patrol,
    Trace,
    Return,
    Attack,
    Damaged,
    Die
}

public class Monster : MonoBehaviour
{
    private MonsterState _currentState;
    private NavMeshAgent _agent;
    private Animator _animator;

    private Transform _player;

    private Vector3 _startPosition;
    private Vector3 _destination;
    public float PatrolTime = 3f;
    private float _patrolTimer = 0f;
    public float MovementRange = 10f;
    public float TraceDistance = 10f;
    public float ReturnDistance = 20f;
    public const float TOLERANCE = 0.1f;
    public float AttackDistance = 2f;

    private void Start()
    {
        _currentState = MonsterState.Idle;
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _startPosition = transform.position;
        _destination = transform.position;
    }

    private void Update()
    {
        switch(_currentState)
        {
            case MonsterState.Idle:        Idle();      break;
            case MonsterState.Patrol:      Patrol();    break;
            case MonsterState.Trace:       Trace();     break;
            case MonsterState.Return:      Return();    break;
            case MonsterState.Attack:      Attack();    break;
            case MonsterState.Damaged:     Damaged();   break;
            case MonsterState.Die:         Die();       break;
        }
    }
    private void Idle()
    {
        _patrolTimer += Time.deltaTime;
        if (_patrolTimer > PatrolTime)
        {
            _patrolTimer = 0f;
            _currentState = MonsterState.Patrol;
            _animator.SetTrigger("IdleToPatrol");
            Debug.Log("Idle -> Patrol");
            MoveRandomPosition();
        }
        if (Vector3.Distance(_player.position, transform.position) <= TraceDistance)
        {
            _currentState = MonsterState.Trace;
            _animator.SetTrigger("IdleToTrace");
            Debug.Log("Idle -> Trace");
        }
    }
    private void Patrol()
    {
        //_animator.SetTrigger("Patrol");
        if (Vector3.Distance(transform.position, _destination) <= TOLERANCE)
        {
            MoveRandomPosition();
        }

        if (Vector3.Distance(transform.position, _startPosition) >= ReturnDistance)
        {
            _currentState = MonsterState.Return;
            _animator.SetTrigger("PatrolToReturn");
            Debug.Log("Patrol -> Return");
        }
        if (Vector3.Distance(_player.position, transform.position) <= TraceDistance)
        {
            _currentState = MonsterState.Trace;
            _animator.SetTrigger("PatrolToTrace");
            Debug.Log("Patrol -> Trace");
        }
    }
    private void Trace()
    {
        Vector3 dir = _player.position - transform.position;
        dir.Normalize();
        _agent.destination = _player.position;

        if (Vector3.Distance(transform.position, _startPosition) >= ReturnDistance)
        {
            _currentState = MonsterState.Return;
            Debug.Log("Trace -> Return");
        }
        if (Vector3.Distance(_player.position, transform.position) <= AttackDistance)
        {
            _currentState = MonsterState.Attack;
            Debug.Log("Trace -> Attack");
        }
    }
    private void Return()
    {
        _agent.SetDestination(_startPosition);

        if (Vector3.Distance(transform.position, _startPosition) <= TOLERANCE)
        {
            _currentState = MonsterState.Idle;
            _animator.SetTrigger("Idle");
            Debug.Log("Return -> Idle");
        }
    }
    private void Attack()
    {
        // Attack하는 코드

        if (Vector3.Distance(_player.position, transform.position) >= AttackDistance)
        {
            _currentState = MonsterState.Trace;
            Debug.Log("Attack -> Trace");
        }
    }
    private void Damaged()
    {
        //_animator.SetTrigger("Damaged");
        _currentState = MonsterState.Trace;
        Debug.Log("Damaged -> Trace");
    }
    private void Die()
    {
        //_animator.SetTrigger("Die");
    }

    private void MoveRandomPosition()
    {
        Vector3 randomDestination = Random.insideUnitSphere * MovementRange;
        randomDestination += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDestination, out hit, MovementRange, NavMesh.AllAreas);
        Vector3 targetPosition = hit.position;
        _agent.SetDestination(targetPosition);
        _destination = targetPosition;
    }
}