using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public enum CharacterType
{
    Male,
    Female,
}

public class UI_Lobby : MonoBehaviour
{
    public TMP_InputField NicknameInputFieldUI;
    public TMP_InputField RoomIDInputFieldUI;

    public static CharacterType SelectedCharacterType = CharacterType.Male;
    public GameObject MaleCharacter;
    public GameObject FemaleCharacter;

    private void Start()
    {
        MaleCharacter.SetActive(SelectedCharacterType == CharacterType.Male);
        FemaleCharacter.SetActive(SelectedCharacterType == CharacterType.Female);
    }

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
        roomOptions.MaxPlayers = 20;             // 입장 가능한 최대 플레이어 수
        roomOptions.IsVisible = true;            // 로비에서 방 목록에 노출할 것인가?
        roomOptions.IsOpen = true;               // 방이 열려있는 상태인가?
        roomOptions.EmptyRoomTtl = 1000 * 20;    // 비어있는 방 살아있는 시간(TimeToLive)
        // roomOptions.PlayerTtl = 100 * 30;

        PhotonNetwork.JoinOrCreateRoom(roomID, roomOptions, TypedLobby.Default);   // 방이 있다면 입장하고 없다면 만드는 것 
    }

    public void OnClickMaleButton() { OnClickCharacterTypeButton(CharacterType.Male); }
    public void OnClickFemaleButton() => OnClickCharacterTypeButton(CharacterType.Female);
    private void OnClickCharacterTypeButton(CharacterType characterType)
    {
        SelectedCharacterType = characterType;
        MaleCharacter.SetActive(SelectedCharacterType == CharacterType.Male);
        FemaleCharacter.SetActive(SelectedCharacterType == CharacterType.Female);
    }
}