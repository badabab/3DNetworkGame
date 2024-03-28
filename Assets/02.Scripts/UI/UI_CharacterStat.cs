using UnityEngine;
using UnityEngine.UI;

public class UI_CharacterStat : MonoBehaviour
{
    public static UI_CharacterStat Instance { get; private set; }
    public Character MyCharacter;
    public Slider HealthSliderUI;
    public Slider StaminaSliderUI;
    
    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        if (MyCharacter == null)
        {
            return;
        }
        //MyCharacter.Stat.Health = Mathf.Clamp(MyCharacter.Stat.Health, 0, MyCharacter.Stat.MaxHealth);
        //MyCharacter.Stat.Stamina = Mathf.Clamp(MyCharacter.Stat.Stamina, 0, MyCharacter.Stat.MaxStamina);
        HealthSliderUI.value = MyCharacter.Stat.Health / (float) MyCharacter.Stat.MaxHealth;
        StaminaSliderUI.value = MyCharacter.Stat.Stamina / MyCharacter.Stat.MaxStamina;
    }
}