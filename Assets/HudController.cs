using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudController : MonoBehaviour
{
    public float health;
    [SerializeField] Slider healthSlider;
    public float stamina;
    [SerializeField] Slider staminaSlider;
    public float mana;
    [SerializeField] Slider manaSlider;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        manaSlider.value = mana;
        staminaSlider.value = stamina;
        healthSlider.value = health;
    }
}
