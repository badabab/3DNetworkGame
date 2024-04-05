using Photon.Pun;
using System.Collections.Generic;
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

public class Monster : MonoBehaviourPunCallbacks
{
    public MonsterStat MStat;
    public PhotonView PhotonView { get; private set; }
    private MonsterState _monsterState;
    private NavMeshAgent _agent;
    private Animator _animator;

    private List<GameObject> _playersList = new List<GameObject>();
    private Transform _playerTransform;
    private Character _playerCharacter;

    private Vector3 _startPosition;
    private Vector3 _destination;
    private float _patrolTimer = 0f;
    public const float TOLERANCE = 0.1f;
    private float _attackTimer = 0f;

    private void Start()
    {
        gameObject.SetActive(false);
        PhotonNetwork.AddCallbackTarget(this);
    }
    public override void OnJoinedRoom()
    {
        Init();
        gameObject.SetActive(true);
    }
    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
    private void Init()
    {
        MStat.Init();
        _monsterState = MonsterState.Idle;
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _startPosition = transform.position;
        _destination = transform.position;

        foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList)
        {
            _playersList.Add(GetPlayer(p));
        }
        FindClosePlayer();
    }

    private void Update()
    {
        switch(_monsterState)
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

    private GameObject GetPlayer(Photon.Realtime.Player player)
    {
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in playerObjects)
        {
            PhotonView photonView = p.GetComponent<PhotonView>();
            if (photonView != null && photonView.OwnerActorNr == player.ActorNumber)
            {
                return p;
            }
        }
        return null;
    }
    private void FindClosePlayer()
    {
        float targetDistance = Mathf.Infinity;
        GameObject targetPlayer = null;
        Vector3 currentPosition = transform.position;
        foreach (GameObject p in _playersList)
        {
            float distance = Vector3.Distance(p.transform.position, currentPosition);
            if (distance < targetDistance)
            {
                targetDistance = distance;
                targetPlayer = p;
            }
        }
        _playerTransform = targetPlayer.transform;
        _playerCharacter = targetPlayer.GetComponent<Character>();
    }

    private void Idle()
    {
        _patrolTimer += Time.deltaTime;
        if (_patrolTimer > MStat.PatrolTime)
        {
            _patrolTimer = 0f;
            _monsterState = MonsterState.Patrol;
            _animator.SetTrigger("IdleToPatrol");
            Debug.Log("Idle -> Patrol");
            MoveRandomPosition();
        }
        if (Vector3.Distance(_playerTransform.position, transform.position) <= MStat.TraceDistance)
        {
            _monsterState = MonsterState.Trace;
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

        if (Vector3.Distance(transform.position, _startPosition) >= MStat.ReturnDistance)
        {
            _monsterState = MonsterState.Return;
            _animator.SetTrigger("PatrolToReturn");
            Debug.Log("Patrol -> Return");
        }
        if (Vector3.Distance(_playerTransform.position, transform.position) <= MStat.TraceDistance)
        {
            _monsterState = MonsterState.Trace;
            _animator.SetTrigger("PatrolToTrace");
            Debug.Log("Patrol -> Trace");
        }
    }
    private void Trace()
    {
        Vector3 dir = _playerTransform.position - transform.position;
        dir.Normalize();
        _agent.destination = _playerTransform.position;

        if (Vector3.Distance(transform.position, _startPosition) >= MStat.ReturnDistance)
        {
            _monsterState = MonsterState.Return;
            _animator.SetTrigger("TraceToReturn");
            Debug.Log("Trace -> Return");
        }
        if (Vector3.Distance(_playerTransform.position, transform.position) <= MStat.AttackDistance)
        {
            _monsterState = MonsterState.Attack;
            _animator.SetTrigger($"Attack{UnityEngine.Random.Range(1,3)}");
            _playerCharacter.Damaged(MStat.Damage, -2);
            Debug.Log("Trace -> Attack");
        }
    }
    private void Return()
    {
        _agent.SetDestination(_startPosition);

        if (Vector3.Distance(transform.position, _startPosition) <= TOLERANCE)
        {
            _monsterState = MonsterState.Idle;
            _animator.SetTrigger("Idle");
            Debug.Log("Return -> Idle");
        }
    }
    private void Attack()
    {
        _attackTimer += Time.deltaTime;
        if (_attackTimer > MStat.AttackDelay)
        {
            _animator.SetTrigger($"Attack{UnityEngine.Random.Range(1, 3)}");
            _playerCharacter.Damaged(MStat.Damage, -2);
            _attackTimer = 0;
        }

        if (Vector3.Distance(_playerTransform.position, transform.position) >= MStat.AttackDistance)
        {
            _monsterState = MonsterState.Trace;
            _animator.SetTrigger("AttackToTrace");
            Debug.Log("Attack -> Trace");
        }
    }
    private void Damaged()
    {
        if (_monsterState == MonsterState.Die)
        {
            return;
        }
        //MStat.Health -= _playerCharacter.Stat.Damage;
        _animator.SetTrigger("Damaged");
        if (MStat.Damage <= 0)
        {
            _monsterState = MonsterState.Die;
        }
        else
        {
            // 0.5초 후??
            _monsterState = MonsterState.Trace;
            _animator.SetTrigger("DamagedToTrace");
            Debug.Log("Damaged -> Trace");
        }       
    }
    private void Die()
    {
        _animator.SetTrigger("Die");
    }
    private void OnDeath(int actorNumber)
    {
        if (actorNumber >= 0)
        {
            string nickName = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber).NickName;
            string logMessage = $"\n<color=#32CD32>{nickName}</color>님이 <color=#32CD32>Monster</color>를 <color=red>처치</color>하였습니다.";
            PhotonView.RPC(nameof(AddLog), RpcTarget.All, logMessage);
        }
    }
    [PunRPC]
    public void AddLog(string logMessage)
    {
        UI_RoomInfo.Instance.AddLog(logMessage);
    }

    private void MoveRandomPosition()
    {
        Vector3 randomDestination = Random.insideUnitSphere * MStat.MovementRange;
        randomDestination += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDestination, out hit, MStat.MovementRange, NavMesh.AllAreas);
        Vector3 targetPosition = hit.position;
        _agent.SetDestination(targetPosition);
        _destination = targetPosition;
    }
}