using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwayAlt : MonoBehaviour
{
    [Header("Asignables")]
    public Transform weapon;
    public bool sliding;
    public bool crouching;
    public GameObject currentWeapon;
    public GameObject scope;

    [Header("Directional Sway")]
    // Estas son para el sway normal
    public float amount = 0.2f;
    public float maxAmount = 0.06f;
    public float smoothAmount = 6f;


    private Vector3 initialPos;
    private Quaternion origin_rot;

    // Estas son para el sway rotacional
    [Header("Rotational Sway")]
    public float rAmount = 2f;
    public float rMaxAmount = 4f;
    public float rSmooth = 6f;

    [Header("Breathign and Headbob")]
    private float movementCount;
    private float idlecount;
    private Vector3 targetBobPos;

    private Rigidbody rig;
    private Vector3 weaponOrigin;
    private Vector3 ScopeOrigin;

    [HideInInspector] public bool rotationX = true;
    [HideInInspector] public bool rotationY = true;
    [HideInInspector] public bool rotationZ = true;

    private Quaternion initialRPos;

    //Inputs
    private float inputX;
    private float inputY;
    private float hmove;
    private float vmove;
    private bool sprinting;
    private bool aiming;


    private void Start()
    { 
        
        initialPos = transform.localPosition;
        origin_rot = transform.localRotation;
        initialRPos = transform.localRotation;
        weaponOrigin = weapon.localPosition;
        ScopeOrigin = scope.transform.localPosition;
    }

    private void Update()
    {
        if (GameObject.Find("Personaje").GetComponent<WeaponManager>().currentWeapon != null)
        {
            currentWeapon = GameObject.Find("Personaje").GetComponent<WeaponManager>().currentWeapon;
        }
        Inputs();
        UpdateSway();
        UpdateTiltSway();
        if (!GameObject.Find("Personaje").GetComponent<WeaponManager>().aiming || GameObject.Find("Personaje").GetComponent<WeaponManager>().currWeapon.shootType != "Sniper")
        {
            if (hmove == 0 && vmove == 0)
            {
                HeadBob(idlecount, 0.05f, 0.05f);
                idlecount += Time.deltaTime * 2;
                weapon.localPosition = Vector3.Lerp(weapon.localPosition, targetBobPos, Time.deltaTime * 2f);
            }
            else if (sprinting && !crouching)
            {
                HeadBob(movementCount, 0.1f, 0.075f);
                movementCount += Time.deltaTime * 9f;
                weapon.localPosition = Vector3.Lerp(weapon.localPosition, targetBobPos, Time.deltaTime * 8f);
            }
            else if (crouching || sliding)
            {
                HeadBob(movementCount, 0.05f, 0.05f);
                movementCount += Time.deltaTime * 4f;
                weapon.localPosition = Vector3.Lerp(weapon.localPosition, targetBobPos, Time.deltaTime * 3f);
            }
            else
            {
                HeadBob(movementCount, 0.04f, 0.04f);
                movementCount += Time.deltaTime * 6f;
                weapon.localPosition = Vector3.Lerp(weapon.localPosition, targetBobPos, Time.deltaTime * 6f);
            }
        }
        else
        {
            if (hmove == 0 && vmove == 0)
            {
                HeadBob(idlecount, 25f, 25f);
                idlecount += Time.deltaTime * 2;
                scope.transform.localPosition = Vector3.Lerp(scope.transform.localPosition, targetBobPos, Time.deltaTime * 2f);
            }
            else if (sprinting && !crouching)
            {
                HeadBob(movementCount, 50f, 37.5f);
                movementCount += Time.deltaTime * 9f;
                scope.transform.localPosition = Vector3.Lerp(scope.transform.localPosition, targetBobPos, Time.deltaTime * 8f);
            }
            else if (crouching || sliding)
            {
                HeadBob(movementCount, 25f, 25f);
                movementCount += Time.deltaTime * 4f;
                scope.transform.localPosition = Vector3.Lerp(scope.transform.localPosition, targetBobPos, Time.deltaTime * 3f);
            }
            else
            {
                HeadBob(movementCount, 20f, 20f);
                movementCount += Time.deltaTime * 6f;
                scope.transform.localPosition = Vector3.Lerp(scope.transform.localPosition, targetBobPos, Time.deltaTime * 6f);
            }
        }        
    }
    void HeadBob (float p_z, float p_x_intensity, float p_y_intensity)
    {
        if (!GameObject.Find("Personaje").GetComponent<WeaponManager>().aiming || GameObject.Find("Personaje").GetComponent<WeaponManager>().currWeapon.shootType != "Sniper")
        {
            targetBobPos = weaponOrigin + new Vector3(Mathf.Cos(p_z) * p_x_intensity, Mathf.Sin(p_z * 2) * p_y_intensity, 0);
        }
        else
        {
            targetBobPos = ScopeOrigin + new Vector3(Mathf.Cos(p_z) * p_x_intensity, Mathf.Sin(p_z * 2) * p_y_intensity, 0);
        }
    }
    private void Inputs()
    {
        aiming = Input.GetMouseButton(1);
        sliding = GameObject.Find("Player").GetComponent<Movimiento>().sliding;
        crouching = GameObject.Find("Player").GetComponent<Movimiento>().crouching;
        inputX = -Input.GetAxis("Mouse X");
        inputY = -Input.GetAxis("Mouse Y");
        hmove = Input.GetAxisRaw("Horizontal");
        vmove = Input.GetAxisRaw("Vertical");
        if (vmove == 1 && Input.GetKey(KeyCode.LeftShift)) sprinting = true;
        else sprinting = false;
    }
    private void UpdateSway()
    {
        float moveX = Mathf.Clamp(inputX * amount, -maxAmount, maxAmount);
        float moveY = Mathf.Clamp(inputY * amount, -maxAmount, maxAmount);

        Vector3 finalPosition = new Vector3(moveX, moveY, 0);

        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition + initialPos, Time.deltaTime * smoothAmount);
        
    }
    private void UpdateTiltSway()
    {
        float rotX = Mathf.Clamp(inputX * rAmount, -rMaxAmount, rMaxAmount);
        float rotY = Mathf.Clamp(inputY * rAmount, -rMaxAmount, rMaxAmount);

        Quaternion finalRot = Quaternion.Euler(new Vector3(
            rotationX ? rotY : 0f,
            rotationY ? rotX : 0f,
            rotationZ ? rotX : 0
            ));
        transform.localRotation = Quaternion.Slerp(transform.localRotation, finalRot * initialRPos, Time.deltaTime * rSmooth);


    } 
}
