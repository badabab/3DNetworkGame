using UnityEngine;

public class CampFire : MonoBehaviour
{
    public float FireDamageTime = 1f;
    private float timer = 0f;
    public int FireDamage = 20;
    private void Start()
    {
        timer = 0;
    }
    private void OnTriggerStay(Collider other)
    {      
        timer += Time.deltaTime;

        Character character = other.GetComponent<Character>();
        if (timer > FireDamageTime && character != null)
        {
            character.Damaged(FireDamage);
            timer = 0;
        }
    }
}