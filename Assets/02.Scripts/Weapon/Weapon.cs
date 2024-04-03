using Photon.Pun;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public CharacterAttackAbility MyCharacterAttackAbility;
    public int Score;
    private int _currentScore;

    private void OnTriggerEnter(Collider other)
    {
        MyCharacterAttackAbility.OnTriggerEnter(other);
    }

    private void Start()
    {
        gameObject.transform.localScale = Vector3.one;
        Score = (int)PhotonNetwork.LocalPlayer.CustomProperties["Score"];
        _currentScore = Score;
    }

    private void Update()
    {
        if (_currentScore >= 1000)
        {
            SetWeaponScale();
            _currentScore -= 1000;
        }
    }
    private void SetWeaponScale()
    {
        gameObject.transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
    }
}
