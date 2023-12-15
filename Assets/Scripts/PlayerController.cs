using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public Rigidbody2D rb;

    public float moveSpeed = 5f;

    private Vector2 moveInput;
    private Vector2 mouseInput;

    private float maxAngle = 160;
    private float minAngle = 10;
    public float mouseSensisitivity = 3f;

    public Camera viewCam;

    public GameObject bulletImpact;
    public int currentAmmo;

    public Animator gunAnim;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Player movement
        moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        Vector3 moveHorizontal = transform.up * -moveInput.x;

        Vector3 moveVertical = transform.right * moveInput.y;

        rb.velocity = (moveHorizontal + moveVertical) * moveSpeed;

        //Player view
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensisitivity;

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z - mouseInput.x);

        viewCam.transform.localRotation = Quaternion.Euler(viewCam.transform.localRotation.eulerAngles + new Vector3(0f, mouseInput.y, 0f));

        Vector3 rotAmount = viewCam.transform.localRotation.eulerAngles + new Vector3(0f, mouseInput.y, 0f);

        viewCam.transform.localRotation = Quaternion.Euler(rotAmount.x, Math.Clamp(rotAmount.y, minAngle, maxAngle), rotAmount.z);

        //Shooting
        if (Input.GetMouseButtonDown(0))
        {
            if (currentAmmo > 0)
            {
                Ray ray = viewCam.ViewportPointToRay(new Vector3(.5f, .5f, 0f));
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    //Debug.Log("I'm looking at " + hit.transform.name);
                    Instantiate(bulletImpact, hit.point, transform.rotation);

                    if (hit.transform.tag == "Enemy")
                    {
                        hit.transform.parent.GetComponent<EnemyController>().TakeDamage();
                    }
                }
                else
                {
                    Debug.Log("I'm looking at nothing");
                }
                currentAmmo--;
                gunAnim.SetTrigger("Shoot");
            }
        }
    }
}
