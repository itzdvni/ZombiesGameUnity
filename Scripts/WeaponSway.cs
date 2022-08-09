using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    // Estas son para el sway normal
    public float intensity;
    public float smooth;

    private Quaternion origin_rotation;

    // Estas son para el sway rotacional
    public float rAmount;
    public float rMaxAmount;
    public float rSmooth;

    public bool rotationX = true;
    public bool rotationY = true;
    public bool rotationZ = true;

    private Quaternion initialRPos;

    //Inputs
    private float inputX;
    private float inputY;


    
    private void Start()
    {
        origin_rotation = transform.localRotation;
        initialRPos = transform.localRotation;
    }

    private void Update()
    {
        CalcSway();
        UpdateSway();
        UpdateTiltSway();
    }
    private void CalcSway()
    {
        float inputX = Input.GetAxis("Mouse X");
        float inputY = Input.GetAxis("Mouse Y");
    }
    private void UpdateSway()
    {
        
        Quaternion t_x_adj = Quaternion.AngleAxis(-intensity * inputX, Vector3.up);
        Quaternion t_y_adj = Quaternion.AngleAxis(intensity * inputY, Vector3.right);
        Quaternion target_rotation = origin_rotation * t_x_adj * t_y_adj;

        //rotate towards target rotation
        transform.localRotation = Quaternion.Lerp(transform.localRotation, target_rotation, Time.deltaTime * smooth);
    }
    private void UpdateTiltSway()
    {
        float rotX = Mathf.Clamp(inputX * rAmount, -rMaxAmount, rMaxAmount);
        float rotY = Mathf.Clamp(inputY * rAmount, -rMaxAmount, rMaxAmount);

        Quaternion finalRot = Quaternion.Euler(new Vector3(
            rotationX ? -rotX : 0f,
            rotationY ? rotY : 0f,
            rotationZ ? rotY : 0f
            ));
        transform.localRotation = Quaternion.Slerp(transform.localRotation, finalRot * initialRPos, Time.deltaTime * rSmooth);


    } 
}
