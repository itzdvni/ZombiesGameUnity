using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealMovement : MonoBehaviour
{
    //Asignables
    public Transform playerCam; // Cámara
    public Transform orientation; // Componente Orientación

    //Other
    private Rigidbody rb;

    //Rotación de cámara 
    private float xRotation; // Para rotar en Y
    private float sensitivity = 200f;
    private float sensMultiplier = 1f;

    //Movimiento
    public float moveSpeed = 4500;
    public float maxSpeed = 20;
    public bool grounded;
    public LayerMask isground;

    //contramovimieto HAY QUE ENTENDERLO
    
    //Agacharse y deslizarse
    private Vector3 crouchScale = new Vector3(1, 0.5f, 1); // Reescala el personaje a este tamaño
    private Vector3 playerScale; // Se inicializa como el tamaño del personaje
    public float slideForce = 400; //Fuerza que se aplica al agacharse corriendo
    public float slideCounterMovement = 0.2f; // Fuerza que se aplica en contra al deslizarse

    //Salto
    private bool readyToJump = true; // Cuando saltes no podras volver a saltar hasta que se vuelva true
    private float jumpCooldown = 0.25f; // Tiempo antes de volver a activar el salto
    public float jumpForce = 550f;







    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
