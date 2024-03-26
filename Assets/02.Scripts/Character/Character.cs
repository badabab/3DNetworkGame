using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(CharacterMoveAbility))]
[RequireComponent(typeof(CharacterRotateAbility))]
[RequireComponent(typeof(CharacterAttackAbility))]
public class Character : MonoBehaviour, IPunObservable
{
    public Stat Stat;
    public PhotonView PhotonView { get; private set; }

    private void Awake()
    {
        Stat.Init();
        PhotonView = GetComponent<PhotonView>();
        if (PhotonView.IsMine)
        {
            UI_CharacterStat.Instance.MyCharacter = this;
        }
    }
    private Vector3 _receivedPosition;
    private Quaternion _receivedRotation;
    private void Update()
    {
        if (!PhotonView.IsMine)
        {
            transform.position = Vector3.Lerp(transform.position, _receivedPosition, Time.deltaTime * 20f);
            transform.rotation = Quaternion.Slerp(transform.rotation, _receivedRotation, Time.deltaTime * 20f);
        }
    }

    // 데이터 동기화를 위해 데이터 전송 및 수신 기능을 가진 약속
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // stream(통로)은 서버에서 주고받을 데이터가 담겨있는 변수
        if (stream.IsWriting)   // 데이터를 전송하는 상황
        {
            /*stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);*/

            stream.SendNext(Stat.Health);
            stream.SendNext(Stat.Stamina);
        }
        else if (stream.IsReading)  // 데이터를 수신하는 상황
        {
            // 데이터를 전송한 순서와 똑같이 받은 데이터를 캐스팅해야된다.
            /* // Photon Transform View
            _receivedPosition = (Vector3)stream.ReceiveNext();
            _receivedRotation = (Quaternion)stream.ReceiveNext();*/
            if (!PhotonView.IsMine)
            {
                Stat.Health = (int)stream.ReceiveNext();
                Stat.Stamina = (float)stream.ReceiveNext();
            }              
        }
        // info는 송수신 성공/실패 여부에 대한 메시지 담겨있다.
    }
}
