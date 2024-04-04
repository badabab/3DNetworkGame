using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class ItemObject : MonoBehaviourPun
{
    [Header("아이템 타입")]
    public ItemType ItemType;
    public float Value = 100;

    private void Start()
    {
        if (photonView.IsMine)
        {
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            Vector3 randomVector = UnityEngine.Random.insideUnitSphere;
            randomVector.y = 1f;
            randomVector.Normalize();
            randomVector *= UnityEngine.Random.Range(3, 7f);
            rigidbody.AddForce(randomVector, ForceMode.Impulse);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Character character = other.GetComponent<Character>();
            if (!character.PhotonView.IsMine || character.State == State.Death)
            {
                return;
            }

            character.GetComponent<CharacterEffectAbility>().RequestPlay((int)ItemType);

            switch (ItemType)
            {
                case ItemType.HealthPotion:
                {
                    character.Stat.Health += (int)Value;
                    if (character.Stat.Health >= character.Stat.MaxHealth)
                    {
                        character.Stat.Health = character.Stat.MaxHealth;
                    }
                    break;
                }
                case ItemType.StaminaPotion:
                {
                    character.Stat.Stamina += Value;
                    if (character.Stat.Stamina >= character.Stat.MaxStamina)
                    {
                        character.Stat.Stamina = character.Stat.MaxStamina;
                    }
                    break;
                }
                case ItemType.ScoreItem1:
                case ItemType.ScoreItem2:
                case ItemType.ScoreItem3:
                {
                    //character.Score += (int)Value;
                    character.AddPropertyIntValue("Score", (int)Value);
                    break;
                }
            }
            gameObject.SetActive(false);
            ItemObjectFactory.Instance.RequestDelete(photonView.ViewID);
        }
    }
}
