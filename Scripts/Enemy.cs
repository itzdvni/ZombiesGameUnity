using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using Random = System.Random;

public class Enemy : MonoBehaviour, iHittable, iAlertable
{

    [field: SerializeField]
    public EnemyDataSO enemyData { get; set; }
   [field: SerializeField] public int Health { get; private set; } = 4;

   [field: SerializeField] private EnemyAIBrain enemyAIBrain;
   
   
   [field: SerializeField]
   public EnemyAttack enemyAttack { get; set; }

   [field: SerializeField] private float timeWaitForDeath = 2.61f;

   private bool dead = false;
   
   
   [field: SerializeField ]
   public UnityEvent OnGetHit { get; set; }
   
   
   [field: SerializeField ]
   public UnityEvent doRagdoll { get; set; }
   
   
   [field: SerializeField ]
   public UnityEvent OnDie { get; set; }


   [SerializeField] private GameManager gameManager = null;

   [SerializeField] private int GeneralspawnChance;
   
   [SerializeField] private GameObject[] ammoItems = null;



   Random rand = new Random();

   
   private void Awake()
   {
       try
       {
           gameManager = FindObjectOfType<GameManager>();
       }catch (Exception exception){}
       
       enemyAIBrain = GetComponent<EnemyAIBrain>();
       if (enemyAttack == null)
       {
           enemyAttack = GetComponent<EnemyAttack>();
       }
   }

   private void Start()
   {
       SetValuesToData();
   }


   private void SetValuesToData()
   {
       Health = enemyData.maxHealth;
       enemyAIBrain.enemyMovement.navMeshAgent.speed = enemyData.Speed;
       enemyAIBrain.enemyMovement.navMeshAgent.acceleration = enemyData.Acceleration;
       enemyAIBrain.enemyMovement.navMeshAgent.angularSpeed = enemyData.RotationSpeed;
   }

   public void GetHit(int damage, GameObject damageDealer)
    {
        if (dead == false)
        {
            Health-=damage;
            OnGetHit?.Invoke(); // llamamos a lo que este esuchando al hit
            if (Health <= 0)
            {
                try
                {


                    gameManager.UpdateScore(200);
                }
                catch (Exception e)
                {

                }
                dead = true;
                int generalChance = rand.Next(100);
                if (generalChance <= 50)
                {
                    SpawnAmmo();
                }
                
                OnDie?.Invoke(); // llamamos a lo que este escuchando al ondie
                StartCoroutine(WaitToDie());
            }
        }
    }

   private void SpawnAmmo()
   {
       int ammoTypeChance = rand.Next(ammoItems.Length);

       Instantiate(ammoItems[ammoTypeChance], transform.position, quaternion.identity);
       
   }

   IEnumerator WaitToDie()
   {
       this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x,
           this.gameObject.transform.position.y - 5f,
           this.gameObject.transform.position.z);
       yield return new WaitForSeconds(3f);
       doRagdoll?.Invoke();
       this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x,
           this.gameObject.transform.position.y - 3f,
           this.gameObject.transform.position.z);
        this.gameObject.transform.DOMoveY(-5, 25);
        yield return new WaitForSeconds(15f);
        Destroy(gameObject);
   }
   


   public void PerformAttack()
   {
       if (dead == false)
       {
           enemyAttack.Attack(enemyData.Damage);
       }
   }

   public UnityEvent<GameObject> OnGetAlerted { get; set; }
   public void GetAlerted(GameObject alerterParent)
   {
       if (dead == false)
       {
           OnGetAlerted?.Invoke(alerterParent); // a lo que esté escuchando el evento y le pasamos el objeto que lo alerta
           // esto tiene que ir al estado de alerted
           if (enemyAIBrain.CurrentState.name != "ChaseState")
           {
               enemyAIBrain.enemyMovement.Move(alerterParent.transform.position);
               enemyAIBrain.aiActionData.alerted = true;
           }
           
       }
    
   }
}
