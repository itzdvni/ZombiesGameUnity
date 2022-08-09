using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthPlayerUI : MonoBehaviour
{
    
    [SerializeField] private TextMeshProUGUI healthText = null;
    private Player playerScript = null;


    private void Awake()
    {
        playerScript = GetComponent<Player>();
    }

    private void Start()
    {
        healthText.text = "Health: " + playerScript.Health.ToString();
    }

    public void UpdatePlayerHealth()
    {
        healthText.text = "Health: " + playerScript.Health.ToString();
    }
}


