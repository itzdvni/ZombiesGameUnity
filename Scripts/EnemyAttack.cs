using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class EnemyAttack : MonoBehaviour
{
    protected EnemyAIBrain enemybrain;

    [field: SerializeField] 
    public float AttackDelay { get; private set; } = 4;

    
    [field: SerializeField] public UnityEvent attackStarted { get; set; }
    [field: SerializeField] public UnityEvent attackFinished { get; set; }

    
    protected bool waitBeforeNextAttack;
    
    private void Awake()
    {
        enemybrain = GetComponent<EnemyAIBrain>();
    }
    
    public abstract void Attack(int damage);

    protected IEnumerator WaitBeforeAttackCoroutine()
    {
        attackStarted?.Invoke();
        waitBeforeNextAttack = true;
        
        yield return new WaitForSeconds(AttackDelay);
        attackFinished?.Invoke();
        waitBeforeNextAttack = false;
        
    }

    protected GameObject GetTarget()
    {
        return enemybrain.target;
    }
    
}
