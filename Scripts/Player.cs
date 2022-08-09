using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;


public class Player : MonoBehaviour, IAgent, iHittable
{
    [field: SerializeField]
    public int Health { get; set; }

    [SerializeField]
    private bool dead = false;
    
    [field: SerializeField]
    public UnityEvent OnDie { get; set; }
    
    [field: SerializeField] 
    public UnityEvent OnGetHit { get; set; }
    

    [SerializeField] private GameObject dieScreen = null;
    


    [SerializeField] private Transform CenterPosition;
    
    private Animator animatorController;
    

    [SerializeField] private SkinnedMeshRenderer meshed;
    

    [SerializeField] private float timeToResurrect = 5f;
    public bool mejorado = false;
    
    
    [SerializeField]
    private GameObject enemyPrefabTest;
    private void Awake()
    {
        animatorController = GetComponent<Animator>();
        meshed = GetComponentInChildren<SkinnedMeshRenderer>(); // ya lo llamamos abajo en la funcion pertinente
    }
    

    public void GetHit(int damage, GameObject damageDealer)
    {
        if (dead == false)
        {
            Health-=damage;
            OnGetHit?.Invoke();
            if (Health <= 0)
            {
                OnDie?.Invoke();
                dead = true;
                Cursor.visible = true;
                StartCoroutine(DeathCoroutine());
            }
        }

    }

    

    public bool areYouAlive()
    {
        return !dead; 
    }

    IEnumerator DeathCoroutine() // para que de tiempo a playear el sound que queramos
    {

        gameObject.GetComponent<Movimiento>().enabled = false;
        gameObject.GetComponentInParent<WeaponManager>().enabled = false;
        
        dieScreen.SetActive(true);
        dieScreen.GetComponent<fadeInOut>().startFadeIn();
        
        
        yield return new WaitForSeconds(timeToResurrect);
        //if (CenterPosition == null) CenterPosition = gameObject.transform;

        //var respawnedZombie = Instantiate(enemyPrefabTest, CenterPosition.position,Quaternion.Inverse(this.transform.rotation));

        //SkinPicker respawnedPicker = respawnedZombie.GetComponentInChildren<SkinPicker>();
        //respawnedPicker.isEated = true;
        //meshed = GetComponentInChildren<SkinnedMeshRenderer>();
        //respawnedPicker.ChangeMesh(meshed);

        //Destroy(gameObject);
    }
    public void titan()
    {
        mejorado = true;
        Health = 150;
    }
    private void Update()
    {
        if (!mejorado)
        {
            if (Health > 100)
            {
                Health = 100;
            }
        }
        else
        {
            if (Health > 150)
            {
                Health = 150;
            }
        }
    }
}
