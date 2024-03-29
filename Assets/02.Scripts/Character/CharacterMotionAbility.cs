using UnityEngine;
using Photon.Pun;

public class CharacterMotionAbility : CharacterAbility
{
    private void Update()
    {
        if (_owner.State == State.Death || !_owner.PhotonView.IsMine)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _owner.PhotonView.RPC(nameof(PlayMotion), Photon.Pun.RpcTarget.All, 1);
        }
    }

    [PunRPC]
    public void PlayMotion(int number)
    {
        GetComponent<Animator>().SetTrigger($"Motion{number}");
    }
}