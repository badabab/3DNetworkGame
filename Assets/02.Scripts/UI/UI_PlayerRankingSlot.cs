using Photon.Realtime;
using TMPro;
using UnityEngine;

public class UI_PlayerRankingSlot : MonoBehaviour
{
    public TextMeshProUGUI RankingTextUI;
    public TextMeshProUGUI NickmaneTextUI;
    public TextMeshProUGUI KillCountTextUI;
    public TextMeshProUGUI ScoreTextUI;

    public void Set(Player player)
    {
        RankingTextUI.text = "1";
        NickmaneTextUI.text = player.NickName;
        KillCountTextUI.text = "10";
        ScoreTextUI.text = "10000";
    }
}