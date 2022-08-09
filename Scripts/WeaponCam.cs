using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCam : MonoBehaviour
{
    public Transform player;
    private float xRotation; // Para rotar en Y
    private float sensitivity = 200f;
    private float sensMultiplier = 1f;
    public Transform orientation;
    private float desiredX;
    void Update()
    {
        Vector3 playpos = new Vector3(player.position.x, player.position.y + 0.4f, player.position.z);
        transform.position = playpos;

        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime * sensMultiplier;

        //Find current look rotation
        Vector3 rot = transform.localRotation.eulerAngles;
        desiredX = rot.y + mouseX;

        //Rotate, and also make sure we dont over- or under-rotate.
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Perform the rotations
        transform.localRotation = Quaternion.Euler(xRotation, desiredX, 0);
        orientation.transform.localRotation = Quaternion.Euler(0, desiredX, 0);
    }
    
}
