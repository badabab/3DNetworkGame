using UnityEngine;
using UnityEngine.UI;

public class UI_CharacterStat : CharacterAbility
{
    public Slider HealthSliderUI;
    public Slider StaminaSliderUI;
    private void Start()
    {
        Owner.Stat.Health = Owner.Stat.MaxHealth;
        Owner.Stat.Stamina = Owner.Stat.MaxStamina;
    }
    private void Update()
    {
        Owner.Stat.Health = Mathf.Clamp(Owner.Stat.Health, 0, Owner.Stat.MaxHealth);
        HealthSliderUI.value = Owner.Stat.Health / Owner.Stat.MaxHealth;

        Owner.Stat.Stamina = Mathf.Clamp(Owner.Stat.Stamina, 0, Owner.Stat.MaxStamina);
        StaminaSliderUI.value = Owner.Stat.Stamina / Owner.Stat.MaxStamina;
    }
}