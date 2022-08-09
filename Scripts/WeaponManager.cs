using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeaponManager : MonoBehaviour
{
    [Header("Equip weapons and save them")]
    public Weapons[] loadout;
    public Transform weapon;
    public int current;
    public GameObject currentWeapon;
    public Weapons currWeapon;

    [field: SerializeField] public UnityEvent disparo { get; set; }

    //Aiming
    public bool aiming;
    public Transform t_anchor;
    public Transform t_ads;
    public Transform t_hip;
    public Transform t_resources;
    public bool scoping;

    [Header("Shooting")]
    public GameObject bullethole;
    public LayerMask Disparable;
    public LayerMask Atacable;
    public Camera cam;
    private float timeToFire = 0f;
    public bool shooting, shot;

    [Header("Spread")]
    private bool moving, jumping, sprinting, crouching, sliding, forward, crouchstarted;
    public float spreadMulti, shootSpread;
    public float spreadStabilizer;
    public Vector3 shootDirection;

    [Header("Reload")]
    public bool reloading = false;
    public Transform t_reloadState;
    public bool hastoreload = false;
    public float reloadTime;


    private AudioSource audioPlayer = null;
    void Start()
    {
        audioPlayer = GetComponent<AudioSource>();
        shootSpread = loadout[current].spread;
        current = 6;
        for (int i = 0; i <= 3; i++)
        {
            loadout[i].magazine = loadout[i].totalAmmo * 3;
            loadout[i].currentAmmo = loadout[i].totalAmmo;
            loadout[i].upgraded = false;
            loadout[i].SpeedCola = false;
        }
        loadout[0].bulletDamage = 20;
        loadout[1].bulletDamage = 15;
        loadout[2].bulletDamage = 10;
        loadout[3].bulletDamage = 100;
        loadout[0].reloadTime = 3f;
        loadout[1].reloadTime = 1.25f;
        loadout[2].reloadTime = 1f;
        loadout[3].reloadTime = 3f;
        loadout[0].name = "AK-47";
        loadout[1].name = "Colt 29";
        loadout[2].name = "Escopeta Recortada";
        loadout[3].name = "Barrett 50";
    }

    public AudioClip shootingClip = null;
    void Update()
    {
        if (currentWeapon != null)
        {
            currWeapon = loadout[current];
            if (Input.GetMouseButton(0) && loadout[current].shootType == "Auto" && !reloading) Shoot(); else if (Input.GetMouseButtonDown(0) && loadout[current].shootType == "Semi" && !reloading) Shoot(); else if (Input.GetMouseButtonDown(0) && loadout[current].shootType == "Shotgun" && !reloading) Shoot(); else if (Input.GetMouseButtonDown(0) && loadout[current].shootType == "Sniper" && !reloading) Shoot();
            if (Input.GetMouseButton(0) && loadout[current].shootType == "Auto" && loadout[current].currentAmmo > 0f && !reloading)
            {
                shooting = true;
            }
            else if (Input.GetMouseButtonDown(0) && loadout[current].shootType == "Semi" && loadout[current].currentAmmo > 0f && !reloading)
            {
                shooting = true;
            }
            else if (Input.GetMouseButtonDown(0) && loadout[current].shootType == "Shotgun" && loadout[current].currentAmmo > 0f && !reloading)
            {
                shooting = true;
            }
            else if (Input.GetMouseButtonDown(0) && loadout[current].shootType == "Shotgun" && loadout[current].currentAmmo > 0f && !reloading)
            {
                shooting = true;
            }
            else
            {
                shooting = false;
            }
            if (Input.GetMouseButtonDown(0) && loadout[current].magazine == 0 && loadout[current].currentAmmo == 0){
                audioPlayer.clip = loadout[current].NoAmmo;
                audioPlayer.Play();
            }
            t_anchor = currentWeapon.transform.Find("Anchor");
            t_ads = currentWeapon.transform.Find("States/ADS");
            t_hip = currentWeapon.transform.Find("States/Hip");
            t_reloadState = currentWeapon.transform.Find("States/ReloadState");
            t_resources = currentWeapon.transform.Find("Anchor/Resources");
            aiming = Input.GetMouseButton(1);
            Aim(aiming);
            if (loadout[current].magazine > 0 && Input.GetKeyDown(KeyCode.R) && !reloading && loadout[current].currentAmmo != loadout[current].totalAmmo) StartCoroutine("Reload"); else if (hastoreload && loadout[current].currentAmmo == 0 && Input.GetMouseButtonDown(0) && !reloading && loadout[current].magazine > 0) StartCoroutine("Reload");
            if (reloading) ReloadAnimation();
            if (!shooting) shootSpread = Mathf.Lerp(shootSpread, 1f, Time.deltaTime * 3f);
            SpreadCalc();
            //elasticidad de arma
            currentWeapon.transform.localPosition = Vector3.Lerp(currentWeapon.transform.localPosition, Vector3.zero, Time.deltaTime * 4f);
            //currentWeapon.transform.rotation = Quaternion.Lerp(currentWeapon.transform.rotation, new Quaternion(0,0,0,0), Time.deltaTime * 4f);
        }
        int actual = current;
        WeaponChooser();
        if (actual != current) Equip(current);
    }

    void WeaponChooser()
    {
        Debug.Log(hastoreload);
        if (!reloading)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                current = 0;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                current = 1;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                current = 2;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                current = 3;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") > 0f && current != 3) // Scroll adelante
            {
                current++;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f && current != 0) // Scroll atras
            {
                current--;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") > 0f && current == 3) // Scroll adelante
            {
                current = 0;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f && current == 0) // Scroll atras
            {
                current = 3;
            }
        }
    }
    void Equip(int p_ind)
    {
        shootingClip = loadout[current].Shot;
        if (currentWeapon != null) Destroy(currentWeapon);
        Debug.Log("Current weapon: " + p_ind);
        GameObject t_newEquipment = Instantiate(loadout[p_ind].prefab, weapon.position, weapon.rotation, weapon) as GameObject;
        t_newEquipment.transform.localPosition = Vector3.zero;
        t_newEquipment.transform.localEulerAngles = Vector3.zero;
        currentWeapon = t_newEquipment;
        shootingClip = loadout[current].Shot;
        reloadTime = loadout[current].reloadTime;

    }
    void Aim(bool aiming)
    {
        if (aiming && !reloading)
        {
            t_anchor.position = Vector3.Lerp(t_anchor.position, t_ads.position, Time.deltaTime * loadout[current].aimSpeed);
            if (loadout[current].shootType == "Sniper" && !scoping)
            {
                StartCoroutine("SniperHide");
            }
        }
        else if (!reloading)
        {
            t_anchor.position = Vector3.Lerp(t_anchor.position, t_hip.position, Time.deltaTime * loadout[current].aimSpeed);
        }
        if (loadout[current].shootType == "Sniper" && !aiming)
        {
            scoping = false;
            StopCoroutine("SniperHide");
            currentWeapon.SetActive(true);
        }
        if (loadout[current].shootType == "Semi" && aiming)
        {
            currWeapon.kickback = 0.025f;
        }
        else if (loadout[current].shootType == "Semi" && !aiming)
        {
            currWeapon.kickback = 0.05f;
        }
        if (loadout[current].shootType == "Auto" && aiming)
        {
            currWeapon.kickback = 0.025f;
        }
        else if (loadout[current].shootType == "Auto" && !aiming)
        {
            currWeapon.kickback = 0.05f;
        }
        if (loadout[current].shootType == "Shotgun" && aiming)
        {
            currWeapon.kickback = 0.1f;
        }
        else if (loadout[current].shootType == "Shotgun" && !aiming)
        {
            currWeapon.kickback = 0.4f;
        }
    }
    IEnumerator SniperHide()
    {
        scoping = true;
        yield return new WaitForSeconds(0.20f);
        currentWeapon.SetActive(false);
    }
    void Shoot()
    {
        Transform t_spawn = cam.transform;
        if (loadout[current].shootType == "Semi" && Time.time >= timeToFire && loadout[current].currentAmmo > 0f)
        {
            timeToFire = Time.time + 1f / loadout[current].fireRate;
            RaycastHit t_hit = new RaycastHit();
            loadout[current].currentAmmo--;
            Shootspread();
            ShotVFX();
            disparo?.Invoke();
            audioPlayer.clip = shootingClip;
            audioPlayer.Play();
            if (!aiming)
            {
                shootDirection = t_spawn.forward;
                shootDirection.x += Random.Range(spreadMulti * -shootSpread * spreadStabilizer, spreadMulti * shootSpread * spreadStabilizer);
                shootDirection.y += Random.Range(spreadMulti * -shootSpread * spreadStabilizer, spreadMulti * shootSpread * spreadStabilizer);

            }
            else
            {
                shootDirection = t_spawn.forward;
                shootDirection.x += Random.Range(-0.01f, 0.01f);
                shootDirection.y += Random.Range(-0.01f, 0.01f);
            }
            if (Physics.Raycast(t_spawn.position, shootDirection, out t_hit, 1000f, Atacable))
            {
                t_hit.collider.gameObject.GetComponent<Enemy>().GetHit(loadout[current].bulletDamage, this.gameObject);
            }
            else if (Physics.Raycast(t_spawn.position, shootDirection, out t_hit, 1000f, Disparable))
            {
                GameObject newBulletHole = Instantiate(bullethole, t_hit.point + t_hit.normal * 0.001f, Quaternion.identity) as GameObject;
                newBulletHole.transform.LookAt(t_hit.point + t_hit.normal);
                Destroy(newBulletHole, 5f);
            }
            if (loadout[current].currentAmmo == 1f) hastoreload = true; else hastoreload = false;
        }
        else if (loadout[current].shootType == "Auto" && Time.time >= timeToFire && loadout[current].currentAmmo > 0f)
        {
            timeToFire = Time.time + 1f / loadout[current].fireRate;
            RaycastHit t_hit = new RaycastHit();
            loadout[current].currentAmmo--;
            Shootspread();
            ShotVFX();
            disparo?.Invoke();
            audioPlayer.clip = shootingClip;
            audioPlayer.Play();
            if (!aiming)
            {
                shootDirection = t_spawn.forward;
                shootDirection.x += Random.Range(spreadMulti * -shootSpread * spreadStabilizer, spreadMulti * shootSpread * spreadStabilizer);
                shootDirection.y += Random.Range(spreadMulti * -shootSpread * spreadStabilizer, spreadMulti * shootSpread * spreadStabilizer);
            }
            else
            {
                shootDirection = t_spawn.forward;
                shootDirection.x += Random.Range(-0.01f, 0.01f);
                shootDirection.y += Random.Range(-0.01f, 0.01f);
            }
            if (Physics.Raycast(t_spawn.position, shootDirection, out t_hit, 1000f, Atacable))
            {
                t_hit.collider.gameObject.GetComponent<Enemy>().GetHit(loadout[current].bulletDamage, this.gameObject);
            }
            else if (Physics.Raycast(t_spawn.position, shootDirection, out t_hit, 1000f, Disparable))
            {
                GameObject newBulletHole = Instantiate(bullethole, t_hit.point + t_hit.normal * 0.001f, Quaternion.identity) as GameObject;
                newBulletHole.transform.LookAt(t_hit.point + t_hit.normal);
                Destroy(newBulletHole, 5f);
            }
            if (loadout[current].currentAmmo == 1f) hastoreload = true; else hastoreload = false;
        }
        else if (loadout[current].shootType == "Shotgun" && Time.time >= timeToFire && loadout[current].currentAmmo > 0f)
        {
            timeToFire = Time.time + 1f / loadout[current].fireRate;
            loadout[current].currentAmmo--;
            Shootspread();
            ShotVFX();
            disparo?.Invoke();
            audioPlayer.clip = shootingClip;
            audioPlayer.Play();
            for(int i = 0; i< loadout[current].pellets; i++)
            {
                RaycastHit t_hit = new RaycastHit();
                shootDirection = t_spawn.forward;
                shootDirection.x += Random.Range(spreadMulti * -shootSpread * spreadStabilizer, spreadMulti * shootSpread * spreadStabilizer);
                shootDirection.y += Random.Range(spreadMulti * -shootSpread * spreadStabilizer, spreadMulti * shootSpread * spreadStabilizer);
                if (Physics.Raycast(t_spawn.position, shootDirection, out t_hit, 1000f, Atacable))
                {
                    t_hit.collider.gameObject.GetComponent<Enemy>().GetHit(loadout[current].bulletDamage, this.gameObject);
                }
                else if (Physics.Raycast(t_spawn.position, shootDirection, out t_hit, 1000f, Disparable))
                {
                    GameObject newBulletHole = Instantiate(bullethole, t_hit.point + t_hit.normal * 0.001f, Quaternion.identity) as GameObject;
                    newBulletHole.transform.LookAt(t_hit.point + t_hit.normal);
                    Destroy(newBulletHole, 5f);
                }
                
            }
            if (loadout[current].currentAmmo == 1f) hastoreload = true; else hastoreload = false;
        }
        else if (loadout[current].shootType == "Sniper" && Time.time >= timeToFire && loadout[current].currentAmmo > 0f)
        {
            timeToFire = Time.time + 1f / loadout[current].fireRate;
            RaycastHit t_hit = new RaycastHit();
            loadout[current].currentAmmo--;
            Shootspread();
            ShotVFX();
            disparo?.Invoke();
            audioPlayer.clip = shootingClip;
            audioPlayer.Play();
            if (!aiming)
            {
                shootDirection = t_spawn.forward;
                shootDirection.x += Random.Range(spreadMulti * -shootSpread * spreadStabilizer, spreadMulti * shootSpread * spreadStabilizer);
                shootDirection.y += Random.Range(spreadMulti * -shootSpread * spreadStabilizer, spreadMulti * shootSpread * spreadStabilizer);
            }
            else
            {
                shootDirection = t_spawn.forward;
                shootDirection.x += Random.Range(-0.01f, 0.01f);
                shootDirection.y += Random.Range(-0.01f, 0.01f);
            }
            if (Physics.Raycast(t_spawn.position, shootDirection, out t_hit, 1000f, Atacable))
            {
                t_hit.collider.gameObject.GetComponent<Enemy>().GetHit(loadout[current].bulletDamage, this.gameObject);
            }
            else if (Physics.Raycast(t_spawn.position, shootDirection, out t_hit, 1000f, Disparable))
            {
                GameObject newBulletHole = Instantiate(bullethole, t_hit.point + t_hit.normal * 0.001f, Quaternion.identity) as GameObject;
                newBulletHole.transform.LookAt(t_hit.point + t_hit.normal);
                Destroy(newBulletHole, 5f);
            }
            if (loadout[current].currentAmmo == 1f) hastoreload = true; else hastoreload = false;
        }
    }
    void SpreadCalc()
    {
        
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) moving = true; else moving = false;
        sliding = GameObject.Find("Player").GetComponent<Movimiento>().sliding;
        jumping = GameObject.Find("Player").GetComponent<Movimiento>().grounded;
        sprinting = GameObject.Find("Player").GetComponent<Movimiento>().sprinting;
        forward = GameObject.Find("Player").GetComponent<Movimiento>().forward;
        crouchstarted = GameObject.Find("Player").GetComponent<Movimiento>().crouchstarted;
        crouching = GameObject.Find("Player").GetComponent<Movimiento>().crouching;
        if (!jumping && !crouching)
        {
            spreadMulti = 3f * 2;
        }
        else if (sprinting && !crouching && forward)
        {
            spreadMulti = 2.5f * 2;
        }
        else if (crouching && !sliding && moving)
        {
            spreadMulti = 1.2f * 2;
        }
        else if (crouching && !sliding && !moving)
        {
            spreadMulti = 1f;
        }
        else if (sliding)
        {
            spreadMulti = 2.5f * 2;
        }
        else if (moving && !crouching)
        {
            spreadMulti = 2f * 2;
        }
        else
        {
            spreadMulti = 2f;
        }
    }
    void Shootspread()
    {
        
        if (shootSpread < 2)
        {
            shot = true;
            shootSpread += loadout[current].spreadFactor;
            shot = false;
        }
        else
        {
            shot = true;
            shootSpread = loadout[current].maxSpread;
            shot = false;
        }
    }
    void ShotVFX()
    {
        //currentWeapon.transform.Rotate(-loadout[current].recoil, 0, 0);
        currentWeapon.transform.position -= currentWeapon.transform.forward * loadout[current].kickback;
        GameObject gunshot;
        GameObject gunshoteffect = Instantiate(currWeapon.FX, t_resources.position, weapon.rotation) as GameObject;
        if (!currWeapon.upgraded)
        {
            gunshot = Instantiate(currWeapon.FXgunshot, t_resources.position, t_resources.rotation) as GameObject;
        }
        else
        {
            gunshot = Instantiate(currWeapon.FXgunshotupgraded, t_resources.position, t_resources.rotation) as GameObject;
        }
        Destroy(gunshot, 1f);
        Destroy(gunshoteffect, 1f);
    }
    IEnumerator Reload()
    {
        Weapons arma = loadout[current];
        reloading = true;
        Debug.Log("detecta input");
        if (loadout[current].shootType == "Shotgun")
        {
            audioPlayer.clip = loadout[current].Reload;
            audioPlayer.Play();
            yield return new WaitForSeconds(loadout[current].reloadTime);
            loadout[current].magazine -= 1;
            loadout[current].currentAmmo += 1;
            if (loadout[current].magazine != 0 && loadout[current].currentAmmo == 1 && !Input.GetMouseButton(0))
            {
                audioPlayer.Play();
                yield return new WaitForSeconds(loadout[current].reloadTime);
                loadout[current].magazine -= 1;
                loadout[current].currentAmmo += 1;
            }
        }
        else
        {
            audioPlayer.clip = loadout[current].Reload;
            audioPlayer.Play();
            yield return new WaitForSeconds(loadout[current].reloadTime);
            if (loadout[current] == arma)
            {
                if (loadout[current].magazine - (loadout[current].totalAmmo - loadout[current].currentAmmo) > 0)
                {
                    loadout[current].magazine = loadout[current].magazine - (loadout[current].totalAmmo - loadout[current].currentAmmo);
                    loadout[current].currentAmmo = loadout[current].totalAmmo;
                }
                else
                {
                    loadout[current].currentAmmo = loadout[current].currentAmmo + loadout[current].magazine;
                    loadout[current].magazine = 0;
                }
            }
        }
            ReturnAnimation();
        Debug.Log("hace cosas");
        reloading = false;
    }
    void ReloadAnimation()
    {
        t_anchor.position = Vector3.Lerp(t_anchor.position, t_reloadState.position, Time.deltaTime * loadout[current].aimSpeed);
    }
    void ReturnAnimation()
    {
        t_anchor.position = Vector3.Lerp(t_anchor.position, t_hip.position, Time.deltaTime * loadout[current].aimSpeed);
    }
    public void GetAmmoBox(int weaponType)
    {
        Debug.Log(weaponType);
        if(weaponType== 2)
        {
            loadout[1].magazine += loadout[1].totalAmmo; 
        }
        else if (weaponType == 1)
        {
            loadout[0].magazine += loadout[0].totalAmmo;
        }
        else if (weaponType == 3)
        {
            loadout[2].magazine += loadout[2].totalAmmo;
        }
        else if (weaponType == 4)
        {
            loadout[3].magazine += loadout[3].totalAmmo;
        }
    }
    public void upgrade()
    {
        currWeapon.bulletDamage = currWeapon.bulletDamage + currWeapon.bulletDamage * 50 / 100;
        currWeapon.name = currWeapon.name + " V2";
    }
    public void presti()
    {
        for (int i = 0; i <= 3; i++)
        {
            currWeapon.SpeedCola = true;
            loadout[i].reloadTime -= currWeapon.reloadTime * 30 / 100;
        } 
    }
}
