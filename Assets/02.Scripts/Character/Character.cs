using Cinemachine;
using Photon.Pun;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterMoveAbility))]
[RequireComponent(typeof(CharacterRotateAbility))]
[RequireComponent(typeof(CharacterAttackAbility))]
[RequireComponent(typeof(CharacterShakeAbility))]
public class Character : MonoBehaviour, IPunObservable, IDamaged
{
    public Stat Stat;
    public State State { get; private set; } = State.Live;
    public PhotonView PhotonView { get; private set; }
    private Animator _animator;
    public GameObject[] SpawnPoints;

    private void Awake()
    {
        SpawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        Stat.Init();
        _animator = GetComponent<Animator>();
        PhotonView = GetComponent<PhotonView>();
        if (PhotonView.IsMine)
        {
            UI_CharacterStat.Instance.MyCharacter = this;
            MinimapCamera.Instance.MyCharacter = this;
        }
    }

    // 데이터 동기화를 위해 데이터 전송 및 수신 기능을 가진 약속
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // stream(통로)은 서버에서 주고받을 데이터가 담겨있는 변수
        if (stream.IsWriting)   // 데이터를 전송하는 상황
        {
            stream.SendNext(Stat.Health);
            stream.SendNext(Stat.Stamina);
        }
        else if (stream.IsReading)  // 데이터를 수신하는 상황
        {
            if (!PhotonView.IsMine)
            {
                Stat.Health = (int)stream.ReceiveNext();
                Stat.Stamina = (float)stream.ReceiveNext();
            }              
        }
        // info는 송수신 성공/실패 여부에 대한 메시지 담겨있다.
    }

    [PunRPC]
    public void Damaged(int damage)
    {
        if (State == State.Death)
        {
            return;
        }
        Stat.Health -= damage;
        GetComponent<CharacterShakeAbility>().Shake();
        if (PhotonView.IsMine)
        {
            OnDamagedMine();
        }   
        if (Stat.Health <= 0)
        {
            State = State.Death;
            PhotonView.RPC(nameof(Death), RpcTarget.All);
        }
    }

    private void OnDamagedMine()
    {
        CinemachineImpulseSource impulseSource;
        if (TryGetComponent<CinemachineImpulseSource>(out impulseSource))
        {
            float strength = 0.4f;
            impulseSource.GenerateImpulseWithVelocity(UnityEngine.Random.insideUnitSphere.normalized * strength);
        }

        UI_DamagedEffect.Instance.Show(0.5f);
    }

    [PunRPC]
    public void Death()
    {
        State = State.Death;
        GetComponent<Animator>().SetTrigger("Death");
        GetComponent<CharacterAttackAbility>().InactiveCollider();

        if (PhotonView.IsMine)
        {
            GetComponent<CharacterController>().enabled = false;
            StartCoroutine(Death_Coroutine());
        }
    }

    private IEnumerator Death_Coroutine()
    {
        yield return new WaitForSeconds(5f);

        PhotonView.RPC(nameof(Live), RpcTarget.All);
    }

    [PunRPC]
    private void Live()
    {
        Stat.Init();
        State = State.Live;
        transform.position = BattleScene.Instance.GetRandomSpawnPoint();
        GetComponent<CharacterController>().enabled = true;
        GetComponent<Animator>().SetTrigger("Live");    
    }
}