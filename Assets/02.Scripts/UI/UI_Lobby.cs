using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class UI_Lobby : MonoBehaviour
{
    public TMP_InputField NicknameInputFieldUI;
    public TMP_InputField RoomIDInputFieldUI;

    public void OnClickMakeRoomButton()
    {
        string nickname = NicknameInputFieldUI.text;
        string roomID = RoomIDInputFieldUI.text;

        if (string.IsNullOrEmpty(nickname) || string.IsNullOrEmpty(roomID))
        {
            Debug.Log("입력하세요.");
            return;
        }

        PhotonNetwork.NickName = nickname;

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;       // 입장 가능한 최대 플레이어 수
        roomOptions.IsVisible = true;      // 로비에서 방 목록에 노출할 것인가?
        roomOptions.IsOpen = true;         // 방이 열려있는 상태인가?

        PhotonNetwork.JoinOrCreateRoom(roomID, roomOptions, TypedLobby.Default);   // 방이 있다면 입장하고 없다면 만드는 것 
    }
}
