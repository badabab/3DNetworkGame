using UnityEngine;
using UnityEngine.UI;

public class UI_CharacterStat : MonoBehaviour
{
    public Character MyCharacter;
    public Slider HealthSliderUI;
    public Slider StaminaSliderUI;
    private void Update()
    {
        if (MyCharacter == null)
        {
            return;
        }
        //MyCharacter.Stat.Health = Mathf.Clamp(MyCharacter.Stat.Health, 0, MyCharacter.Stat.MaxHealth);
        //MyCharacter.Stat.Stamina = Mathf.Clamp(MyCharacter.Stat.Stamina, 0, MyCharacter.Stat.MaxStamina);
        HealthSliderUI.value = MyCharacter.Stat.Health / MyCharacter.Stat.MaxHealth;
        StaminaSliderUI.value = MyCharacter.Stat.Stamina / MyCharacter.Stat.MaxStamina;
    }
}