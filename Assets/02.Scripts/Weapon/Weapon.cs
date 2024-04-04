using UnityEngine;

public class Weapon : MonoBehaviour
{
    public CharacterAttackAbility MyCharacterAttackAbility;

    private void OnTriggerEnter(Collider other)
    {
        MyCharacterAttackAbility.OnTriggerEnter(other);
    }

    public void SetWeaponScale()
    {
        gameObject.transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
        Debug.Log($"무기 크기 {gameObject.transform.localScale}");
    }
}