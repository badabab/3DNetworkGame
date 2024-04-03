using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;

public class UI_PlayerRanking : MonoBehaviourPunCallbacks
{
    public List<UI_PlayerRankingSlot> Slots;    // 1~5등
    public UI_PlayerRankingSlot MySlot;     // 내 정보

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Refresh();
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
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