using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cámara : MonoBehaviour
{
    public float sensibilidad = 100f;
    public Transform PlayerBody;
    float RotationX = 0f;
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;  // Se utiliza el modo Locked para poder mover el cursor infinitamente sin llegar al límite de pantalla
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensibilidad * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidad * Time.deltaTime;

        RotationX -= mouseY; // Si se sumase se invierte la vista
        RotationX = Mathf.Clamp(RotationX, -50f, 50f); // Limita el campo de vision verticalmente a 50 grados hacia arriba o abajo

        transform.localRotation = Quaternion.Euler(RotationX, 0f, 0f); // Rota el eje Y
        PlayerBody.Rotate(Vector3.up * mouseX); // Rota el eje X
    }
}
