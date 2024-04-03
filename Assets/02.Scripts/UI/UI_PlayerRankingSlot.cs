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
        RankingTextUI.text = "-";
        NickmaneTextUI.text = player.NickName;
        if(player.CustomProperties != null )
        {
            KillCountTextUI.text = $"{player.CustomProperties["KillCount"]}";
            ScoreTextUI.text = $"{player.CustomProperties["Score"]}";
        }
        else
        {
            KillCountTextUI.text = "0";
            ScoreTextUI.text = "0";
        }        
    }
}