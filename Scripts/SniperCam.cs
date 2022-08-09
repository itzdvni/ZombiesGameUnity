using UnityEngine;

public class SniperCam : MonoBehaviour
{
    public Transform player;
    public Transform cam;
    public Camera camara;
    public bool sprinting;
    public bool moving;
    public bool crouching;
    [SerializeField] private float destfov = 90f;
    [SerializeField] private float initfov = 80f;
    [SerializeField] private float speed = 100;
    private float xRotation; // Para rotar en Y
    private float sensitivity = 200f;
    private float sensMultiplier = 1f;
    public Transform orientation;
    private float desiredX;
    private void Start()
    {

    }
    void Update()
    {
        MyInput();
        Vector3 playpos = new Vector3(player.position.x, player.position.y + 0.40f, player.position.z+8f);
        transform.position = playpos;
        cam.localScale = player.localScale;
        if (sprinting && moving && !crouching)
        {
            camara.fieldOfView = Mathf.MoveTowards(camara.fieldOfView, destfov, Time.deltaTime * speed);
        }
        else
        {
            camara.fieldOfView = Mathf.MoveTowards(camara.fieldOfView, initfov, Time.deltaTime * speed);
        }
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
    private void MyInput()
    {
        sprinting = Input.GetKey(KeyCode.LeftShift);
        moving = Input.GetKey(KeyCode.W);
        crouching = Input.GetKey(KeyCode.LeftControl);
    }
}
