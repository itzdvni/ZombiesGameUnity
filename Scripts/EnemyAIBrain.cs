using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class EnemyAIBrain : MonoBehaviour
{
    [field: SerializeField] public AgentSounds soundsZombie = null;
    
    [field: SerializeField] private bool displayInfo; 
    [field: SerializeField] public GameObject target { get; set; }
    
    [field: SerializeField] public Animator anim { get; set; }

    [field: SerializeField] public UnityEvent attacking { get; set; }
    
    [field: SerializeField] public AIState CurrentState { get; private set; }
    [field: SerializeField] public AIState NextState { get; private set; }

    [field: SerializeField] public EnemyMovement enemyMovement { get; set; }
    
    
    [field: SerializeField] public TextMeshProUGUI currentStateText { get; set; }
    
    [field: SerializeField] public TextMeshProUGUI nextStateText { get; set; }

    
    [field: SerializeField] public TextMeshProUGUI targetSpottedText { get; set; }

    [field: SerializeField] public TextMeshProUGUI FirstTimeSpottedText { get; set; }
    
    [field: SerializeField] public TextMeshProUGUI AliveText { get; set; }

    [field: SerializeField] public TextMeshProUGUI TargetNameText { get; set; }


    [field: SerializeField] public TextMeshProUGUI canmove { get; set; }

    [field: SerializeField] public TextMeshProUGUI ArrivedText { get; set; }

    [field: SerializeField] public TextMeshProUGUI interestingText { get; set; }
    
    [field: SerializeField] public TextMeshProUGUI alertedText { get; set; }
    
    [field: SerializeField] public TextMeshProUGUI readyPatrolText { get; set; }
    
    [field: SerializeField] public TextMeshProUGUI isPatrolingText { get; set; }

    [field: SerializeField] public TextMeshProUGUI attackFinishedText { get; set; }

    [field: SerializeField] public AIActionData aiActionData { get; set; }
    
    [field: SerializeField] public EnemyAnimator enemyAnimator { get; set; }


    [field: SerializeField]
    public float Distance { get; set; } = 5f;
    
    
    protected void OnDrawGizmos()
    {
        #if UNITY_EDITOR
        if (UnityEditor.Selection.activeObject == gameObject)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, Distance);
            Gizmos.color = Color.white;
            

        }
        #endif
    }
    
    
    
    private GameObject[] allPossibleTargets;
    private void Awake()
    {
        aiActionData = GetComponentInChildren<AIActionData>();
        anim = FindObjectOfType<Animator>();
        target = FindObjectOfType<Player>().gameObject;
        //target = FindObjectOfType<Player>().gameObject // si revertimos que sea a esta
        enemyMovement = GetComponent<EnemyMovement>();
        enemyAnimator = GetComponent<EnemyAnimator>();
    }


    private GameObject GetClosestEnemy(GameObject[] possibles)
    {
        GameObject bestTarget = null;
        GameObject closestFood = null;
        GameObject overAllClosestEnemy = null;

        
        float closestOverAll = Mathf.Infinity;
        float closestDistance = Mathf.Infinity;
        float closestFoodDistance = Mathf.Infinity;
        
        Vector3 currentPosition = transform.position;
        Vector3 enemyPosition;
        
        float currentDistance;
        
        bool isAlive;
        
        
        foreach (var target in possibles)
        {
            enemyPosition = target.transform.position;
            currentDistance = Vector3.Distance(currentPosition, enemyPosition);

            if (currentDistance < closestOverAll)
            {
                closestOverAll = currentDistance;
                overAllClosestEnemy = target;
            }
            
            isAlive = target.GetComponent<Player>().areYouAlive();

            if (isAlive == true)
            {
                if (currentDistance < closestDistance)
                {
                    closestDistance = currentDistance;
                    bestTarget = target;
                }
                
            }
            if(isAlive == false)
            {
                if (currentDistance < closestFoodDistance)
                {
                    closestFoodDistance = currentDistance;
                    closestFood = target;
                }
               
            }
        }
        
        // SI EL ELEMENTO VIVO ESTÁ DENTRO DEL RANGO QUE PEDIMOS ENTONCES DEJARA DE COMER E IRA HACIA EL QUE ESTA VIVO, SINO AL CADAVER
        
        if (closestFood != null)
        {
            if (VerifyDistanceRange(bestTarget) == false) // si el best target esta demasiado lej
            {
                if (VerifyDistanceRange(closestFood))
                {
                    aiActionData.isAlive = closestFood.GetComponent<Player>().areYouAlive();
                    return closestFood;  
                }
            }
        }
        

        if (bestTarget != null & closestFood != null)
        {
            if (VerifyDistanceRange(bestTarget) == false && VerifyDistanceRange(closestFood) == false)
            {
                aiActionData.isAlive = overAllClosestEnemy.GetComponent<Player>().areYouAlive();
                return overAllClosestEnemy;
            } // como ni el vivo ni el muerto esta cerca fijamos el target en el que esta mas close 

            if (VerifyDistanceRange(bestTarget) && VerifyDistanceRange(closestFood))
            {
                aiActionData.isAlive = bestTarget.GetComponent<Player>().areYouAlive();
                return bestTarget;
            }
        }
        
        aiActionData.isAlive = bestTarget.GetComponent<Player>().areYouAlive();
        return bestTarget;
    }

    private bool VerifyDistanceRange(GameObject aliveElement)
    {
        if (Vector3.Distance(aliveElement.transform.position, transform.position) < Distance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    private void showInfo()
    {
        TargetNameText.text = "targetName " + target.gameObject.name.ToString();
        if (target.gameObject.name == "survivor") TargetNameText.color = Color.yellow;
        else if (target.gameObject.name == "Player") TargetNameText.color = Color.magenta;

        alertedText.text = "alerted " + aiActionData.alerted.ToString();
        if (aiActionData.alerted) alertedText.color = Color.green;
        else alertedText.color = Color.red;
        
        
        attackFinishedText.text = "attack finished " + aiActionData.AttackFinished.ToString();
        if (aiActionData.AttackFinished) attackFinishedText.color = Color.green;
        else attackFinishedText.color = Color.blue;

        
        
            AliveText.text = "isalive " + aiActionData.isAlive.ToString();
            if (aiActionData.isAlive) AliveText.color = Color.green;
            else AliveText.color = Color.red;
            
            
            isPatrolingText.text = "isPatroling " + aiActionData.isPatroling.ToString();
            if (aiActionData.isPatroling) isPatrolingText.color = Color.green;
            else isPatrolingText.color = Color.red;


            readyPatrolText.text = "readyPatrol " + aiActionData.readyToPatrol;
            if (aiActionData.readyToPatrol) readyPatrolText.color = Color.green;
            else readyPatrolText.color = Color.red;
            
            
            currentStateText.text = "current " + CurrentState.gameObject.name;
            if(NextState != null) nextStateText.text = "next " + NextState.gameObject.name;
            
            
            targetSpottedText.text = "target spotted " + aiActionData.TargetSpotted.ToString();
            if (aiActionData.TargetSpotted) targetSpottedText.color = Color.green;
            else targetSpottedText.color = Color.red;
            
            
            FirstTimeSpottedText.text = "first " +aiActionData.firstTimeSpotted.ToString();
            if (aiActionData.firstTimeSpotted) FirstTimeSpottedText.color = Color.green;
            else FirstTimeSpottedText.color = Color.red;
            
            
            canmove.text = "canmove " +aiActionData.canMove.ToString();
            if (aiActionData.canMove) canmove.color = Color.green;
            else canmove.color = Color.red;
            
            
            ArrivedText.text = "arrived " +aiActionData.Arrived.ToString();
            if (aiActionData.Arrived) ArrivedText.color = Color.green;
            else ArrivedText.color = Color.red;
            
            
            interestingText.text = "interest " + aiActionData.interestChasing.ToString();
    }
    
    private void Update()
    {
        try
        {
            allPossibleTargets = GameObject.FindGameObjectsWithTag("Plaayer");
            target = GetClosestEnemy(allPossibleTargets);
        }
        catch (Exception ex)
        {
            
        }
        if (target != null) CurrentState.UpdateState();
        
        //if(displayInfo) showInfo();
        
    }
    
    public void Attack()
    {
        attacking?.Invoke();
    }
    
    public void ChangeToState(AIState state)
    {

        if(CurrentState != null) CurrentState.FinishState();
        
        CurrentState = state;
        
        CurrentState.StartState();
        
        NextState = null;
        
    }
    
    

  
    
    
}
