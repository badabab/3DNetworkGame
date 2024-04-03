using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;

public class UI_PlayerRanking : MonoBehaviourPunCallbacks
{
    public List<UI_PlayerRankingSlot> Slots;    // 1~5등
    public UI_PlayerRankingSlot MySlot;     // 내 정보

    // 새로운 플레이어가 룸에 입장했을 때 호출되는 콜백 함수
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Refresh();
    }
    // 플레이어가 룸에서 퇴장했을 때 호출되는 콜백 함수
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Refresh();
    }
    // 플레이어의 커스텀 프로퍼티가 변경되면 호출되는 함수
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        Refresh();
    }
    public override void OnJoinedRoom()
    {
        Refresh();
    }

    private void Refresh()
    {
        Dictionary<int, Player> players = PhotonNetwork.CurrentRoom.Players;
        List<Player> playerList = players.Values.ToList();
        playerList.RemoveAll(player => !player.CustomProperties.ContainsKey("Score"));
        playerList.Sort((player1, player2) =>
        {
            int player1Score = (int)player1.CustomProperties["Score"];
            int player2Score = (int)player2.CustomProperties["Score"];
            return player2Score.CompareTo(player1Score);
        });

        int playerCount = Math.Min(playerList.Count, 5);
        foreach (UI_PlayerRankingSlot slot in Slots)
        {
            slot.gameObject.SetActive(false);
        }
        for (int i = 0; i < playerCount; i++)
        {
            Slots[i].gameObject.SetActive(true);
            Slots[i].Set(playerList[i]);
        }
        MySlot.Set(PhotonNetwork.LocalPlayer);
    }
}