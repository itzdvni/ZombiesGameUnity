using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Movimiento : MonoBehaviour
{
    #region variables publicas
    //Asignables
    public Transform playerCam; // C�mara
    public Transform orientation; // Componente Orientaci�n
    public Transform player;
    public Transform weaponCam;
    public AudioClip WalkAudio;
    public AudioClip CrouchAudio;
    private AudioSource audioPlayer = null;

    //Collisions and collectibles
    public AmmoBox[] boxes;

    //Other
    private Rigidbody rb;

    //Rotaci�n de c�mara 
    private float xRotation; // Para rotar en Y
    private float sensitivity = 200f;
    private float sensMultiplier = 1f;

    //Movimiento
    public float moveSpeed = 4500;
    public float maxSpeed;
    public bool grounded;
    public LayerMask whatIsGround;
    private int speedadd;

    //Contramovimiento
    public float counterMovement = 0.175f;
    private float threshold = 0.01f;
    public float maxSlopeAngle;

    //Crouch & Slide
    private Vector3 crouchScale = new Vector3(1.5f, 0.75f, 1.5f);
    private Vector3 playerScale;
    public float slideForce = 400;
    public float slideCounterMovement = 0.2f;
    private Vector3 playerorigin;

    //Jumping
    private bool readyToJump = true;
    private float jumpCooldown = 0.25f;
    public float jumpForce = 550f;

    //Input
    float x, y;
    public bool jumping, sprinting, crouching, sliding, forward, crouchstarted, moving, spaudio, waudio, craudio;

    //Sliding
    private Vector3 normalVector = Vector3.up;
    private Vector3 wallNormalVector;

    [SerializeField]
    GameObject capsuleobjeto;
    List<GameObject> objs = new List<GameObject>();
    #endregion

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        audioPlayer = GetComponent<AudioSource>();
        playerScale = transform.localScale;
        playerorigin = transform.position;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        crouchstarted = false;
        spaudio = false;
        waudio = false;
        craudio = false;
    }


    private void FixedUpdate()
    {
        Movement();
    }

    private void Update()
    {
        
        MyInput();
        //Look();
        if (sprinting && !crouching && !spaudio && moving)
        {
            StopAllCoroutines();
            audioPlayer.clip = WalkAudio;
            StartCoroutine("SprintFS");
        }
        else if (moving && !crouching && !waudio && !sprinting)
        {
            StopAllCoroutines();
            audioPlayer.clip = WalkAudio;
            StartCoroutine("WalkFS");
        }
        else if (crouching && !craudio && moving)
        {
            StopAllCoroutines();
            audioPlayer.clip = WalkAudio;
            StartCoroutine("CrouchFS");
        }
        if (!crouching)
        {
            craudio = false;
        }
        if (!sprinting)
        {
            spaudio = false;
        }
        if (!moving)
        {
            waudio = false;
        }
        Debug.Log(sprinting);
    }

    private void MyInput()
    {
        x = Input.GetAxisRaw("Horizontal"); //Devuelve 1 o -1 dependiendo del bot�n utilizado.
        y = Input.GetAxisRaw("Vertical");
        if (x != 0 || y != 0){
            moving = true;
        }
        else
        {
            moving = false;
        }
        jumping = Input.GetButton("Jump");
        crouching = Input.GetKey(KeyCode.LeftControl);
        sprinting = Input.GetKey(KeyCode.LeftShift);
        forward = Input.GetKey(KeyCode.W);

        //Crouching
        if (Input.GetKeyDown(KeyCode.LeftControl))
            StartCrouch();
        if (!crouching && crouchstarted)
        {
            if (CanStadUp())
            {
                crouchstarted = false;
                StopCrouch();
            }
        }
    }

    private void StartCrouch()
    {
        crouchstarted = true;
        player.transform.localScale = crouchScale;
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.3f, transform.position.z);
        if (sprinting && forward)
        {
            if (grounded)
            {
                sliding = true;
                if(maxSpeed == 20f) maxSpeed = 30f;
                rb.AddForce(orientation.transform.forward * slideForce);
                Invoke(nameof(StopSliding), Time.deltaTime * 1.5f);
            }
        }
    }
    private void StopSliding()
    {
        sliding = false;
    }
    private void StopCrouch()
    {
        transform.localScale = playerScale;
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z);
    }
    private void Movement()
    {
        Debug.Log(maxSpeed);
        //Gravedad extra
        rb.AddForce(Vector3.down * Time.deltaTime * 10);

        //Find actual velocity relative to where player is looking
        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x, yMag = mag.y;

        //If holding jump && ready to jump, then jump
        if (readyToJump && jumping) Jump();

        //Set max speed
        if (sprinting && forward && crouching)
        {
            maxSpeed = Mathf.MoveTowards(maxSpeed, 5f, Time.deltaTime * 30);
        }
        else if (sprinting && forward && !sliding && !crouching && !crouchstarted)
        {
            maxSpeed = Mathf.MoveTowards(maxSpeed, 13f + speedadd, Time.deltaTime * 30);
        }
        else if(crouching || crouchstarted && !sliding)
        {
            maxSpeed = 5f;
        }
        else
        {
            maxSpeed = 10f + speedadd;
        }

        //Counteract sliding and sloppy movement
        CounterMovement(x, y, mag);

        //If sliding down a ramp, add force down so player stays grounded and also builds speed
        if (sliding && grounded && readyToJump)
        {
            rb.AddForce(Vector3.down * Time.deltaTime * 3000);
            return; 
        }

        //If speed is larger than maxspeed, cancel out the input so you don't go over max speed
        if (x > 0 && xMag > maxSpeed) x = 0;
        if (x < 0 && xMag < -maxSpeed) x = 0;
        if (y > 0 && yMag > maxSpeed) y = 0;
        if (y < 0 && yMag < -maxSpeed) y = 0;

        //Some multipliers
        float multiplier = 1f, multiplierV = 1f;

        // Movement in air
        if (!grounded)
        {
            multiplier = 0.5f;
            multiplierV = 0.5f;
        }

        // Movement while sliding
        if (grounded && sliding) multiplierV = 0f;

        //Apply forces to move player
        rb.AddForce(orientation.transform.forward * y * moveSpeed * Time.deltaTime * multiplier * multiplierV);
        rb.AddForce(orientation.transform.right * x * moveSpeed * Time.deltaTime * multiplier);
    }
    private bool CanStadUp()
    {
        bool stcrouch = GameObject.Find("Sphere").GetComponent<Checkcolider>().stcrouch;
        if (stcrouch)
        {
            return true;
        }
        if (!stcrouch)
        {
            return false;
        }
        return true;
    }
    private void Jump()
    {
        if (grounded && readyToJump && !crouchstarted)
        {
            readyToJump = false;

            //Add jump forces
            rb.AddForce(Vector2.up * jumpForce * 1.5f);
            rb.AddForce(normalVector * jumpForce * 0.5f);

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private float desiredX;
    /*private void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime * sensMultiplier;

        //Find current look rotation
        Vector3 rot = playerCam.transform.localRotation.eulerAngles;
        desiredX = rot.y + mouseX;

        //Rotate, and also make sure we dont over- or under-rotate.
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Perform the rotations
        playerCam.transform.localRotation = Quaternion.Euler(xRotation, desiredX, 0);
        weaponCam.transform.localRotation = Quaternion.Euler(xRotation, desiredX, 0);
        orientation.transform.localRotation = Quaternion.Euler(0, desiredX, 0);
    }*/
    private void CounterMovement(float x, float y, Vector2 mag)
    {
        if (!grounded || jumping) return;

        //Slow down sliding
        if (sliding)
        {
            rb.AddForce(moveSpeed * Time.deltaTime * -rb.velocity.normalized * slideCounterMovement);
            return;
        }

        //Counter movement
        if (Mathf.Abs(mag.x) > threshold && Mathf.Abs(x) < 0.05f || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0))
        {
            rb.AddForce(moveSpeed * orientation.transform.right * Time.deltaTime * -mag.x * counterMovement);
        }
        if (Mathf.Abs(mag.y) > threshold && Mathf.Abs(y) < 0.05f || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0))
        {
            rb.AddForce(moveSpeed * orientation.transform.forward * Time.deltaTime * -mag.y * counterMovement);
        }
        if (Mathf.Sqrt((Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.z, 2))) > maxSpeed)
        {
            float fallspeed = rb.velocity.y;
            Vector3 n = rb.velocity.normalized * maxSpeed;
            rb.velocity = new Vector3(n.x, fallspeed, n.z);
        }
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.layer == 22)
        {
            GameObject.Find("Personaje").GetComponent<WeaponManager>().GetAmmoBox(collision.gameObject.GetComponent<playAnimation>().type);
            try
            {
                collision.gameObject.GetComponent<playAnimation>().play();
            }
            catch (Exception e)
            {
                
            }
            Destroy(collision.gameObject);
            
        }
        if (collision.gameObject.layer == 24)
        {
            GameObject.Find("Player").GetComponent<Player>().Health += 50;
            Destroy(collision.gameObject);
        }
    }
    public Vector2 FindVelRelativeToLook()
    {
        float lookAngle = orientation.transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitue = rb.velocity.magnitude;
        float yMag = magnitue * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitue * Mathf.Cos(v * Mathf.Deg2Rad);

        return new Vector2(xMag, yMag);
    }

    private bool IsFloor(Vector3 v)
    {
        float angle = Vector3.Angle(Vector3.up, v);
        return angle < maxSlopeAngle;
    }

    private bool cancellingGrounded;

    private void OnCollisionStay(Collision other)
    {
        //Make sure we are only checking for walkable layers
        int layer = other.gameObject.layer;
        if (whatIsGround != (whatIsGround | (1 << layer))) return;

        //Iterate through every collision in a physics update
        for (int i = 0; i < other.contactCount; i++)
        {
            Vector3 normal = other.contacts[i].normal;
            //FLOOR
            if (IsFloor(normal))
            {
                grounded = true;
                cancellingGrounded = false;
                normalVector = normal;
                CancelInvoke(nameof(StopGrounded));
            }
        }

        //Invoke ground/wall cancel, since we can't check normals with CollisionExit
        float delay = 3f;
        if (!cancellingGrounded)
        {
            cancellingGrounded = true;
            Invoke(nameof(StopGrounded), Time.deltaTime * delay);
        }
    }

    private void StopGrounded()
    {
        grounded = false;
    }
    IEnumerator SprintFS()
    {
        while (sprinting)
        {
            spaudio = true;
            waudio = false;
            craudio = false;
            audioPlayer.volume = 1f;
            audioPlayer.pitch = Random.Range(0.80f,1.20f) ;
            audioPlayer.Play();
            yield return new WaitForSeconds(0.35f);
        }
    }
    IEnumerator WalkFS()
    {
        while (moving && !sprinting)
        {
            waudio = true;
            craudio = false;
            spaudio = false;
            audioPlayer.volume = 1f;
            audioPlayer.pitch = Random.Range(0.80f, 1.20f);
            audioPlayer.Play();
            yield return new WaitForSeconds(0.45f);
        }
    }
    IEnumerator CrouchFS()
    {
        while (crouching)
        {
            craudio = true;
            spaudio = false;
            waudio = false;
            audioPlayer.volume = 1f;
            audioPlayer.pitch = Random.Range(0.80f, 1.20f);
            audioPlayer.Play();
            yield return new WaitForSeconds(0.6f);
        }
    }
    public void stamin()
    {
        speedadd = 5;
    }
}
