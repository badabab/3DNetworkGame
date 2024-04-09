using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class UI_Room : MonoBehaviour
{
    public TextMeshProUGUI RoomNameTextUI;
    public TextMeshProUGUI NicknameTextUI;
    public TextMeshProUGUI PlayerCountTextUI;

    private RoomInfo _roomInfo;

    public void Set(RoomInfo room)
    {
        _roomInfo = room;

        RoomNameTextUI.text = room.Name;
        NicknameTextUI.text = "방장 이름";
        PlayerCountTextUI.text = $"{room.PlayerCount}/{room.MaxPlayers}";
    }

    // 룸 UI를 클릭했을 때 호출되는 함수
    public void OnClickRoom()
    {
        PhotonNetwork.JoinRoom(_roomInfo.Name);
    }
}