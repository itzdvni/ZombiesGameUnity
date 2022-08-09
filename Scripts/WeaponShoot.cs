using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponShoot : MonoBehaviour
{
    public GameObject bullethole;
    public LayerMask Disparable;
    public Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) Shoot();
    }
    void Shoot()
    {
        Transform t_spawn = cam.transform;

        RaycastHit t_hit = new RaycastHit();
        if(Physics.Raycast(t_spawn.position, t_spawn.forward, out t_hit, 1000f, Disparable))
        {
            GameObject newBulletHole = Instantiate(bullethole, t_hit.point + t_hit.normal * 0.001f, Quaternion.identity) as GameObject;
            newBulletHole.transform.LookAt(t_hit.point + t_hit.normal);
            Destroy(newBulletHole, 5f);            
        }
    }
}
