using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletCounter : MonoBehaviour
{

    public Text contador;
    public Weapons currentWeapon;

    // Start is called before the first frame update
    void Start()
    {
        contador.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.Find("Personaje").GetComponent<WeaponManager>().currWeapon != null)
        {
            currentWeapon = GameObject.Find("Personaje").GetComponent<WeaponManager>().currWeapon;
            contador.text = currentWeapon.currentAmmo+"/"+currentWeapon.totalAmmo;
        }
        
    }
}
