using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] public NavMeshAgent navMeshAgent;

    [SerializeField] float navSpeed = 0;
    
    
    [SerializeField] EnemyAIBrain enemyAIBrain;

    private void Awake()
    {
        enemyAIBrain = GetComponent<EnemyAIBrain>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navSpeed = navMeshAgent.speed;
    }
    
    public void Move(Vector3 movePositionTransform)
    {

        /*if (enemyAIBrain.aiActionData.isAlive == false)
        {
            movePositionTransform = new Vector3(movePositionTransform.x + 1, movePositionTransform.y + 1,
                movePositionTransform.z);
        }*/
        
        if(enemyAIBrain.aiActionData.canMove) navMeshAgent.destination = movePositionTransform;
        else
        {
            navMeshAgent.destination = transform.position;
        }
    }

    private void Update()
    {
        SetArrived();
    }
    
    public void SetArrived()
    {
        if (navMeshAgent.transform.position != navMeshAgent.destination)
        {
            enemyAIBrain.aiActionData.Arrived = false;
        }
        else
        {
            enemyAIBrain.aiActionData.Arrived = true;
        }
    }
    
    public void removeSpeed()
    {
        navMeshAgent.speed = 0;
    }

    public void AddOriginalSpeed()
    {
        navMeshAgent.speed = navSpeed;
    }
    
    
    
    
    
}
