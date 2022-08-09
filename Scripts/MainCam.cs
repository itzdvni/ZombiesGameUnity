using UnityEngine;
using System.Collections;

public class MainCam : MonoBehaviour{
    public Transform player;
    public Transform cam;
    public Camera camara;
    public bool sprinting;
    public bool moving;
    public bool crouching;
    public bool scoping;
    [SerializeField] private float destfov = 90f;
    [SerializeField] private float initfov = 80f;
    [SerializeField] private float speed = 100;
    private float xRotation; // Para rotar en Y
    public float sensitivity = 200f;
    public float regSens = 200f;
    public float scopeSens = 150f;
    private float sensMultiplier = 1f;
    public Transform orientation;
    private float desiredX;
    private void Start()
    {
        
    }
    void Update()
    {
        MyInput();
        Vector3 playpos = new Vector3(player.position.x, player.position.y+0.60f, player.position.z);
        transform.position = playpos;
        cam.localScale = player.localScale;
        if (sprinting && moving && !crouching && !scoping)
        {
            camara.fieldOfView = Mathf.MoveTowards(camara.fieldOfView, destfov, Time.deltaTime * speed);
        }
        else if (GameObject.Find("Personaje").GetComponent<WeaponManager>().currWeapon != null)
        {
            if (GameObject.Find("Personaje").GetComponent<WeaponManager>().currWeapon.shootType == "Sniper")
            {
                if (!GameObject.Find("Personaje").GetComponent<WeaponManager>().aiming)
                {
                    camara.fieldOfView = Mathf.MoveTowards(camara.fieldOfView, initfov, Time.deltaTime * speed);
                    scoping = true;
                    sensitivity = regSens; 
                }
                else
                {
                    scoping = false;
                    sensitivity = scopeSens;
                }
            }
            else if(GameObject.Find("Personaje").GetComponent<WeaponManager>().currWeapon.shootType != "Sniper")
            {
                camara.fieldOfView = Mathf.MoveTowards(camara.fieldOfView, initfov, Time.deltaTime * speed);
            }
        }
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
        if (GameObject.Find("Personaje").GetComponent<WeaponManager>().currWeapon != null)
        {
            if (GameObject.Find("Personaje").GetComponent<WeaponManager>().aiming)
            {
                if (GameObject.Find("Personaje").GetComponent<WeaponManager>().currWeapon.shootType == "Sniper" && !scoping && !GameObject.Find("Personaje").GetComponent<WeaponManager>().reloading)
                {
                    StartCoroutine("ScopeActive");
                }
                else
                {
                    camara.fieldOfView = initfov;
                    scoping = false;
                    StopCoroutine("ScopeActive");
                }
            }
            else
            {
                if (scoping == true)
                {
                    camara.fieldOfView = initfov;
                }
                scoping = false;
                StopCoroutine("ScopeActive");
            }
        }
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
    private void MyInput()
    {
        sprinting = Input.GetKey(KeyCode.LeftShift);
        moving = Input.GetKey(KeyCode.W);
        crouching = Input.GetKey(KeyCode.LeftControl);
    }
    IEnumerator ScopeActive()
    {
        Debug.Log("sdaa");
        scoping = true;
        yield return new WaitForSeconds(0.2f);
        camara.fieldOfView = 20;
    }
}
