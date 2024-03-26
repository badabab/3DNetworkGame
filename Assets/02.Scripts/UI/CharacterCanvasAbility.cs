using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCanvasAbility : CharacterAbility
{
    public Canvas MyCanvas;
    public Text NicknameTextUI;
    public Slider HealthSliderUI;
    public Slider StaminaSliderUI;

    private void Start()
    {
        NicknameTextUI.text = _owner.PhotonView.Controller.NickName;
    }
    private void Update()
    {
        transform.forward = Camera.main.transform.forward;       
    }
}
