using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public TextMeshProUGUI healthText;
    public Image healthBar;

    [SerializeField] private Player playerController;
    float health, maxHealth = 100;
    float lerpSpeed;
    

    private void Start()
    {
        
        health = maxHealth;
    }

    private void Update()
    {
        health = playerController.Health;
        healthText.text = health.ToString();
        if (health > maxHealth) health = maxHealth;

        lerpSpeed = 3f * Time.deltaTime;

        HealthBarFiller();
        ColorChanger();
    }

    void HealthBarFiller()
    {
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, (health / maxHealth), lerpSpeed);

    }

    void ColorChanger()
    {
        Color healthColor = Color.Lerp(Color.red, Color.green, (health / maxHealth));
        healthBar.color = healthColor;
    }

    public void titan()
    {
        maxHealth = 150;
    }
}
