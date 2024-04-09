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

        // [ 룸 옵션 설정 ]
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;             // 입장 가능한 최대 플레이어 수
        roomOptions.IsVisible = true;            // 로비에서 방 목록에 노출할 것인가?
        roomOptions.IsOpen = true;               // 방이 열려있는 상태인가?
        roomOptions.EmptyRoomTtl = 1000 * 20;    // 비어있는 방 살아있는 시간(TimeToLive)
        // roomOptions.PlayerTtl = 100 * 30;
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable()  // 룸 커스텀 프로퍼티 (플레이어 커스텀 프로퍼티)
        {
            {"MasterNickname", nickname}
        };
        // 로비에서 공개적으로 표시될 룸 커스텀 프로퍼티의 키를 정의
        // -> 방을 검색하거나 선택할 때 사용자에게 유용한 정보를 제공하기 위해 사용
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "MasterNickname" };

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

    public void OnNicknameValueChanged(string newValue)
    {
        if (string.IsNullOrEmpty(newValue))
        {
            return;
        }
        PhotonNetwork.NickName = newValue;
    }
}