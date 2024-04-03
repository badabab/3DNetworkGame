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

    [Header("Item Prefab")]
    public GameObject HealthPotionPrefab;
    public GameObject StaminaPotionPrefab;
    public GameObject[] ScoreItemPrefabs;

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
        if (!PhotonView.IsMine)
        {
            return;
        }
       
        SetRandomPositionAndRotation();

        ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
        hashtable.Add("Score", 0);
        hashtable.Add("KillCount", 0);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
    }

    public void AddScore(int score)
    {
        ExitGames.Client.Photon.Hashtable myHashtable = PhotonNetwork.LocalPlayer.CustomProperties;
        myHashtable["Score"] = (int)myHashtable["Score"] + score;
        PhotonNetwork.LocalPlayer.SetCustomProperties(myHashtable);
    }
    public void ResetScore()
    {
        ExitGames.Client.Photon.Hashtable myHashtable = PhotonNetwork.LocalPlayer.CustomProperties;
        myHashtable["Score"] = 0;
        PhotonNetwork.LocalPlayer.SetCustomProperties(myHashtable);
    }
    public void KillScore(int actorNumber)
    {
        int killScore = (int)PhotonNetwork.LocalPlayer.CustomProperties["Score"] / 2;
        ExitGames.Client.Photon.Hashtable playerHashtable = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber).CustomProperties;
        playerHashtable["Score"] = (int)playerHashtable["Score"] + killScore;
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
    public void AddKillCount (int actorNumber)
    {
        ExitGames.Client.Photon.Hashtable playerHashtable = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber).CustomProperties;
        playerHashtable["KillCount"] = (int)playerHashtable["KillCount"] + 1;
        PhotonNetwork.CurrentRoom.GetPlayer(actorNumber).SetCustomProperties(playerHashtable);
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
        if (actorNumber >= 0)
        {
            string nickName = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber).NickName;
            string logMessage = $"\n<color=#32CD32>{nickName}</color>님이 <color=#32CD32>{PhotonView.Owner.NickName}</color>을 <color=red>처치</color>하였습니다.";
            PhotonView.RPC(nameof(AddLog), RpcTarget.All, logMessage);
            KillScore(actorNumber);
            AddKillCount(actorNumber);
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
            ResetScore();
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