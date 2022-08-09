using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIWeaponManager : MonoBehaviour
{
    public Image WeaponFrame;
    public Text BulletCountText;
    public Text WeaponNameText;
    public Image Wave;
    public TextMeshProUGUI TextWave;
    public TextMeshProUGUI TextZombies;
    public Image Health;
    public Image Healthbar;
    public TextMeshProUGUI HealthT;
    public TextMeshProUGUI Score;
    public Image Perk1;
    public Image Perk2;
    public Image Perk3;
    public Image WeaponArt;
    public Image Crosshair1;
    public Image Crosshair2;
    public Image Crosshair3;
    public Image Crosshair4;
    public GameObject Scope;
    public TextMeshProUGUI AmmoWarning;
    public GameObject AmmoW;
    public Image ReloadWarning;
    public GameObject ReloadWarn;

    [HideInInspector] public Weapons CurrentWeapon;
    [HideInInspector] public bool noWeapon;
    [HideInInspector] public bool noCross;
    [HideInInspector] public bool Scoping;
    [HideInInspector] public bool warn;
    // Start is called before the first frame update
    void Start()
    {
        warn = false;
        BulletCountText.text = "";
        WeaponNameText.text = "";
        WeaponArt.enabled = false;
        WeaponFrame.enabled = false;
        AmmoW.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.Find("Personaje").GetComponent<WeaponManager>().currWeapon != null)
        {
            if (noWeapon)
            {
                BulletCountText.CrossFadeAlpha(0, 0, false);
                WeaponNameText.CrossFadeAlpha(0, 0, false);
                WeaponFrame.CrossFadeAlpha(0, 0, false);
                WeaponArt.CrossFadeAlpha(0, 0, false);
                AmmoWarning.CrossFadeAlpha(0, 0, false);
                CurrentWeapon = GameObject.Find("Personaje").GetComponent<WeaponManager>().currWeapon;
                BulletCountText.text = CurrentWeapon.currentAmmo + "/" + CurrentWeapon.magazine;
                WeaponNameText.text = CurrentWeapon.name;
                WeaponArt.sprite = CurrentWeapon.weaponArt;
                WeaponArt.transform.localScale = CurrentWeapon.ArtScale;
                WeaponArt.enabled = true;
                WeaponFrame.enabled = true;
                BulletCountText.CrossFadeAlpha(1, 0.5f, false);
                WeaponNameText.CrossFadeAlpha(1, 0.5f, false);
                WeaponFrame.CrossFadeAlpha(1, 0.5f, false);
                WeaponArt.CrossFadeAlpha(1, 0.5f, false);
                noWeapon = false;
            }
            else
            {
                CurrentWeapon = GameObject.Find("Personaje").GetComponent<WeaponManager>().currWeapon;
                BulletCountText.text = CurrentWeapon.currentAmmo + "/" + CurrentWeapon.magazine;
                WeaponNameText.text = CurrentWeapon.name;
                WeaponArt.sprite = CurrentWeapon.weaponArt;
                WeaponArt.transform.localScale = CurrentWeapon.ArtScale;
                WeaponArt.enabled = true;
                WeaponFrame.enabled = true;
            }
            if (GameObject.Find("Personaje").GetComponent<WeaponManager>().aiming)
            {
                Crosshair1.CrossFadeAlpha(0, 0.2f, false);
                Crosshair2.CrossFadeAlpha(0, 0.2f, false);
                Crosshair3.CrossFadeAlpha(0, 0.2f, false);
                Crosshair4.CrossFadeAlpha(0, 0.2f, false);
                noCross = true;
                if (GameObject.Find("Personaje").GetComponent<WeaponManager>().currWeapon.shootType == "Sniper" && !Scoping && !GameObject.Find("Personaje").GetComponent<WeaponManager>().reloading)
                {
                    StartCoroutine("ScopeActive");
                    BulletCountText.CrossFadeAlpha(0, 0.2f, false);
                    WeaponNameText.CrossFadeAlpha(0, 0.2f, false);
                    WeaponFrame.CrossFadeAlpha(0, 0.2f, false);
                    WeaponArt.CrossFadeAlpha(0, 0.2f, false);
                    Health.CrossFadeAlpha(0, 0.2f, false);
                    Healthbar.CrossFadeAlpha(0, 0.2f, false);
                    Wave.CrossFadeAlpha(0, 0.2f, false);
                    TextWave.CrossFadeAlpha(0, 0.2f, false);
                    TextZombies.CrossFadeAlpha(0, 0.2f, false);
                    HealthT.CrossFadeAlpha(0, 0.2f, false);
                    Score.CrossFadeAlpha(0, 0.2f, false);
                    Perk1.CrossFadeAlpha(0, 0.2f, false);
                    Perk2.CrossFadeAlpha(0, 0.2f, false);
                    Perk3.CrossFadeAlpha(0, 0.2f, false);
                }
            }
            else
            {
                Scoping = false;
                BulletCountText.CrossFadeAlpha(1, 0.2f, false);
                WeaponNameText.CrossFadeAlpha(1, 0.2f, false);
                WeaponFrame.CrossFadeAlpha(1, 0.2f, false);
                WeaponArt.CrossFadeAlpha(1, 0.2f, false);
                Health.CrossFadeAlpha(1, 0.2f, false);
                Healthbar.CrossFadeAlpha(1, 0.2f, false);
                Wave.CrossFadeAlpha(1, 0.2f, false);
                TextWave.CrossFadeAlpha(1, 0.2f, false);
                TextZombies.CrossFadeAlpha(1, 0.2f, false);
                HealthT.CrossFadeAlpha(1, 0.2f, false);
                Score.CrossFadeAlpha(1, 0.2f, false);
                Perk1.CrossFadeAlpha(1, 0.2f, false);
                Perk2.CrossFadeAlpha(1, 0.2f, false);
                Perk3.CrossFadeAlpha(1, 0.2f, false);
                StopCoroutine("ScopeActive");
                Scope.SetActive(false);
            }
            if (GameObject.Find("Personaje").GetComponent<WeaponManager>().reloading)
            {
                Crosshair1.CrossFadeAlpha(0, 0.2f, false);
                Crosshair2.CrossFadeAlpha(0, 0.2f, false);
                Crosshair3.CrossFadeAlpha(0, 0.2f, false);
                Crosshair4.CrossFadeAlpha(0, 0.2f, false);
                noCross = true;
                Scoping = false; 
                StopCoroutine("ScopeActive");
                Scope.SetActive(false);
                BulletCountText.CrossFadeAlpha(1, 0.2f, false);
                WeaponNameText.CrossFadeAlpha(1, 0.2f, false);
                WeaponFrame.CrossFadeAlpha(1, 0.2f, false);
                WeaponArt.CrossFadeAlpha(1, 0.2f, false);
                if (GameObject.Find("Personaje").GetComponent<WeaponManager>().currWeapon.shootType != "Shotgun")
                {
                    ReloadWarn.SetActive(true);
                    ReloadWarning.fillAmount = Mathf.MoveTowards(ReloadWarning.fillAmount, 1f, Time.deltaTime / GameObject.Find("Personaje").GetComponent<WeaponManager>().reloadTime);
                }
                else if (GameObject.Find("Personaje").GetComponent<WeaponManager>().currWeapon.currentAmmo == 0)
                {
                    ReloadWarn.SetActive(true);
                    ReloadWarning.fillAmount = Mathf.MoveTowards(ReloadWarning.fillAmount, 1f, Time.deltaTime / (GameObject.Find("Personaje").GetComponent<WeaponManager>().reloadTime * 2));
                }
                else
                {
                    ReloadWarn.SetActive(true);
                    ReloadWarning.fillAmount = Mathf.MoveTowards(ReloadWarning.fillAmount, 1f, Time.deltaTime / GameObject.Find("Personaje").GetComponent<WeaponManager>().reloadTime);
                }
            }
            else
            {
                ReloadWarning.fillAmount = 0;
                ReloadWarn.SetActive(false);
            }
            if (!GameObject.Find("Personaje").GetComponent<WeaponManager>().reloading && !GameObject.Find("Personaje").GetComponent<WeaponManager>().aiming && noCross)
            {
                noCross = false;
                Crosshair1.CrossFadeAlpha(1, 0.2f, false);
                Crosshair2.CrossFadeAlpha(1, 0.2f, false);
                Crosshair3.CrossFadeAlpha(1, 0.2f, false);
                Crosshair4.CrossFadeAlpha(1, 0.2f, false);
            }
            if (GameObject.Find("Personaje").GetComponent<WeaponManager>().currWeapon.currentAmmo == 0 && GameObject.Find("Personaje").GetComponent<WeaponManager>().currWeapon.magazine == 0)
            {
                if(!warn)StartCoroutine("AmmoWarn");
            }
            else
            {
                StopCoroutine("AmmoWarn");
                AmmoWarning.CrossFadeAlpha(0f, 0.2f, false);
                warn = false;
            }
        }
        else
        {
            noWeapon = true;
            Crosshair1.CrossFadeAlpha(0, 0, false);
            Crosshair2.CrossFadeAlpha(0, 0, false);
            Crosshair3.CrossFadeAlpha(0, 0, false);
            Crosshair4.CrossFadeAlpha(0, 0, false);
            AmmoWarning.CrossFadeAlpha(0, 0, false);
            noCross = true;
        }
    }
    IEnumerator ScopeActive()
    {
        Scoping = true;
        yield return new WaitForSeconds(0.2f);
        Scope.SetActive(true);
    }
    IEnumerator AmmoWarn()
    { 
        warn = true;
        AmmoWarning.CrossFadeAlpha(1f, 0.6f, false);
        yield return new WaitForSeconds(0.6f);
        AmmoWarning.CrossFadeAlpha(0.6f, 0.6f, false);
        yield return new WaitForSeconds(0.6f);
        warn = false;
    }
}