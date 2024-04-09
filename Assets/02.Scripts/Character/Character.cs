using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

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

    [Header("Item Prefab")]
    public GameObject HealthPotionPrefab;
    public GameObject StaminaPotionPrefab;
    public GameObject[] ScoreItemPrefabs;

    private int _halfScore;

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

    private void Start()
    {
        if (PhotonView.IsMine)
        {
            UI_CharacterStat.Instance.MyCharacter = this;
        }
        if (!PhotonView.IsMine)
        {
            return;
        }
       
        SetRandomPositionAndRotation();

        Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
        hashtable.Add("Score", 0);
        hashtable.Add("KillCount", 0);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
    }

    [PunRPC]
    public void AddPropertyIntValue(string key, int value)
    {
        Hashtable myHashtable = PhotonNetwork.LocalPlayer.CustomProperties;
        myHashtable[key] = (int)myHashtable[key] + value;
        PhotonNetwork.LocalPlayer.SetCustomProperties(myHashtable);
        GetComponent<CharacterAttackAbility>().RefreshWeaponScale();
    }
    public void SetPropertyIntValue(string key, int value)
    {
        Hashtable myHashtable = PhotonNetwork.LocalPlayer.CustomProperties;
        myHashtable[key] = value;
        PhotonNetwork.LocalPlayer.SetCustomProperties(myHashtable);
        GetComponent<CharacterAttackAbility>().RefreshWeaponScale();
    }
    public int GetPropertyIntValue(string key)
    {
        Hashtable myHashtable = PhotonNetwork.LocalPlayer.CustomProperties;
        return (int)myHashtable[key];
    }

    public void DeathItem()
    {
        int killScore = (int)PhotonNetwork.LocalPlayer.CustomProperties["Score"] / 2;

        int itemCount = killScore / 100; // ScoreItem1
        int remainingScore = killScore % 100; // 남은 점수
        for (int i = 0; i < itemCount; i++)
        {
            ItemObjectFactory.Instance.RequestCreate(ItemType.ScoreItem1, transform.position);
        }
        itemCount = remainingScore / 50; // ScoreItem2
        remainingScore %= 50; // 남은 점수
        for (int i = 0; i < itemCount; i++)
        {
            ItemObjectFactory.Instance.RequestCreate(ItemType.ScoreItem2, transform.position);
        }
        itemCount = remainingScore / 20; // ScoreItem3
        for (int i = 0; i < itemCount; i++)
        {
            ItemObjectFactory.Instance.RequestCreate(ItemType.ScoreItem3, transform.position);
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
    public void AddLog(string logMessage)
    {
        UI_RoomInfo.Instance.AddLog(logMessage);
    }

    [PunRPC]
    public void Damaged(int damage, int actorNumber)
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
            if (PhotonView.IsMine)
            {
                OnDeath(actorNumber);
            }
            PhotonView.RPC(nameof(Death), RpcTarget.All);
        }
    }

    private void OnDeath(int actorNumber)
    {
        _halfScore = GetPropertyIntValue("Score") / 2;
        SetPropertyIntValue("Score", 0);

        if (actorNumber >= 0)
        {
            string nickName = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber).NickName;
            string logMessage = $"\n<color=#32CD32>{nickName}</color>님이 <color=#32CD32>{PhotonView.Owner.NickName}</color>을 <color=red>처치</color>하였습니다.";
            PhotonView.RPC(nameof(AddLog), RpcTarget.All, logMessage);

            Player targetPlayer = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber);
            PhotonView.RPC(nameof(AddPropertyIntValue), targetPlayer, "Score", _halfScore);
            PhotonView.RPC(nameof(AddPropertyIntValue), targetPlayer, "KillCount", 1);
        }
        else
        {
            string logMessage = $"\n<color=#32CD32>{PhotonView.Owner.NickName}</color>이 운명을 다했습니다.";
            PhotonView.RPC(nameof(AddLog), RpcTarget.All, logMessage);
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
        if (State == State.Death)
        {
            return;
        }
        State = State.Death;
        GetComponent<Animator>().SetTrigger("Death");
        GetComponent<CharacterAttackAbility>().InactiveCollider();

        if (PhotonView.IsMine)
        {
            DeathItem();
            // 팩토리패턴: 객체 생성과 사용 로직을 분리해서 캡슐화하는 패턴
            int num = UnityEngine.Random.Range(0, 10);
            if (num == 0) // 10%
            {
                ItemObjectFactory.Instance.RequestCreate(ItemType.StaminaPotion, transform.position);
            }
            else if (num > 0 && num < 3) // 20%
            {
                ItemObjectFactory.Instance.RequestCreate(ItemType.HealthPotion, transform.position);
            }
            else // 70%
            {
                int itemCount = UnityEngine.Random.Range(5, 15);
                for (int i = 0; i < itemCount; i++)
                {
                    int random = UnityEngine.Random.Range(0, 3);
                    switch (random)
                    {
                        case 0:
                            ItemObjectFactory.Instance.RequestCreate(ItemType.ScoreItem1, transform.position);
                            break;
                        case 1:
                            ItemObjectFactory.Instance.RequestCreate(ItemType.ScoreItem2, transform.position);
                            break;
                        case 2:
                            ItemObjectFactory.Instance.RequestCreate(ItemType.ScoreItem3, transform.position);
                            break;
                    }
                }             
            }
            StartCoroutine(Death_Coroutine());
        }
    }

    private IEnumerator Death_Coroutine()
    {
        yield return new WaitForSeconds(5f);
        SetRandomPositionAndRotation();
        PhotonView.RPC(nameof(Live), RpcTarget.All);
    }

    private void SetRandomPositionAndRotation()
    {
        Vector3 spawnPoint = BattleScene.Instance.GetRandomSpawnPoint();
        GetComponent<CharacterMoveAbility>().Teleport(spawnPoint);
        GetComponent<CharacterRotateAbility>().SetRandomRotation();
    }

    [PunRPC]
    private void Live()
    {
        Stat.Init();
        State = State.Live;
        GetComponent<Animator>().SetTrigger("Live");   
    }
}