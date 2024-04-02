using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class UI_RoomInfo : MonoBehaviourPunCallbacks
{
    public static UI_RoomInfo Instance {  get; private set; }

    public TextMeshProUGUI RoomNameTextUI;
    public TextMeshProUGUI PlayerCountTextUI;
    public TextMeshProUGUI LogTextUI;

    private string _logText = string.Empty;
    private bool _init = false;

    private void Awake()
    {
        Instance = this;
    }

    public override void OnJoinedRoom()
    {
        if (!_init)
        {
            Init();
        }
    }
    void Start()
    {
        if (!_init && PhotonNetwork.InRoom)
        {
            Init();
        }
    }
    private void Init()
    {
        _init = true;
        RoomNameTextUI.text = PhotonNetwork.CurrentRoom.Name;
        PlayerCountTextUI.text = $"{PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";

        _logText += "방에 <color=#87CEEB>입장</color>했습니다.";
        Refresh();
    }

    // 새로운 플레이어가 룸에 입장했을 때 호출되는 콜백 함수
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        _logText += $"\n<color=#32CD32>{newPlayer.NickName}</color>님이 <color=#87CEEB>입장</color>하였습니다.";
        Refresh();
    }

    // 플레이어가 룸에서 퇴장했을 때 호출되는 콜백 함수
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        _logText += $"\n<color=#32CD32>{otherPlayer.NickName}</color>님이 <color=#FFC0CB>퇴장</color>하였습니다.";
        Refresh();
    }

    private void Refresh()
    {
        LogTextUI.text = _logText;
        PlayerCountTextUI.text = $"{PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";
    }

    public void AddLog(string logMessage)
    {
        _logText += logMessage;
        Refresh();
    }
}