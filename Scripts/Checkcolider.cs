using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkcolider : MonoBehaviour
{
    public Transform player;
    [SerializeField] public bool stcrouch = true;
    private void Update()
    {
        Vector3 playpos = new Vector3(player.position.x, player.position.y + 1f, player.position.z);
        transform.position = playpos;
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.layer == 8)
        {
            stcrouch = false;
        }
    }
    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.layer == 8)
        {
            stcrouch = true;
        }
    }
}
