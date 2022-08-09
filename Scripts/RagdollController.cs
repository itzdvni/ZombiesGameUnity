using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RagdollController : MonoBehaviour
{

    [SerializeField] private Collider MainCollider;
    [SerializeField] private Collider[] AllColliders;
    
    [SerializeField] private Rigidbody[] AllRigidBodies;


    private void Awake()
    {
        MainCollider = GetComponent<Collider>();
        AllColliders = GetComponentsInChildren<Collider>(true);
        AllRigidBodies = GetComponentsInChildren<Rigidbody>();
        DoRagdol(false);
    }

    
    public void DoRagdol(bool isRagdoll)
    {

        //GetComponent<Rigidbody>().isKinematic = isRagdoll;
        try
        {
            GetComponent<Animator>().enabled = !isRagdoll;
        }
        catch (Exception e)
        {
        }
        
        try
        {
            GetComponent<NavMeshAgent>().enabled = !isRagdoll;
        }
        catch (Exception e)
        {
        }

        
        foreach (var col in AllColliders)
        {
            col.enabled = isRagdoll;
        }

        foreach (var rb in AllRigidBodies)
        {
            rb.isKinematic = !isRagdoll;
        }

        // fijamos estos valores al opuesto de si es ragdoll.
        // si es ragdoll DESACTIVAMOS estos componentes.
        try
        {
            MainCollider.enabled = !isRagdoll;
            GetComponent<SurvivorRandomMovement>().enabled = !isRagdoll;

        }
        catch (Exception e)
        {
        }

        GetComponent<Rigidbody>().useGravity = !isRagdoll;
    }
    
    
}
