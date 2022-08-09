using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeAtack : EnemyAttack
{
    private bool operando = true;
    [SerializeField] AttackAction attackdistance;
    public override void Attack(int damage)
    {
        if (waitBeforeNextAttack == false) // && enemybrain.aiActionData.AttackFinished
        {
            if (operando)
            {
                StartCoroutine(LittleDelay(0.3f, damage));
                operando = false;
            }
           
        }
    }


    private IEnumerator LittleDelay(float seconds, int damage)
    {
        enemybrain.soundsZombie.playPreAttackSound();
        yield return new WaitForSeconds(seconds);

        if (Vector3.Distance(this.transform.position, enemybrain.target.transform.position) <= attackdistance.Distance)
        {
            var hittable = GetTarget().GetComponent<iHittable>();
            hittable?.GetHit(damage, gameObject);
            enemybrain.soundsZombie.playRealAttackSound();
        }

        StartCoroutine(WaitBeforeAttackCoroutine());
        operando = true;
    }
}
