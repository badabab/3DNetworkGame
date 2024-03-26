using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(CharacterMoveAbility))]
[RequireComponent(typeof(CharacterRotateAbility))]
[RequireComponent(typeof(CharacterAttackAbility))]
public class Character : MonoBehaviour
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
}
