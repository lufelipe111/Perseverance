using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoverController : MonoBehaviour
{
    Rigidbody rb;

    [SerializeField]
    public float moveSpeed = 4f;
    Vector3 forward, right, heading, heading2;
    bool isCrouched;
    public int magnitude;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        magnitude = 10;

        isCrouched = false;

        forward = Camera.main.transform.forward;
        forward.y = 0; // planify the move direction
        forward = Vector3.Normalize(forward);
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward; // trasnposes the horizontal direction to be 90 degrees side from the vertical
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (Input.anyKey || isCrouched)
            Control();

        
    }

    void Control()
    {
        // movement control
        if (!isCrouched && (Input.GetAxis("HorizontalKey") != 0 || Input.GetAxis("VerticalKey") != 0))
        {
            Vector3 rightMovement = Input.GetAxis("HorizontalKey") * moveSpeed * Time.deltaTime * right;   // horizontal movement
            Vector3 upMovement = Input.GetAxis("VerticalKey") * moveSpeed * Time.deltaTime * forward;      // horizontal movement
            Vector3 rightMovement2 = Input.GetAxis("HorizontalKey") * Time.deltaTime * right;  // horizontal movement
            Vector3 upMovement2 = Input.GetAxis("VerticalKey") * Time.deltaTime * forward;      // vertical movement

            heading = Vector3.Normalize(rightMovement + upMovement); // direction used to turn around
            heading2 = Vector3.Normalize(rightMovement2 + upMovement2); // direction used to turn around

            transform.forward = heading; // turn around
            rb.velocity = magnitude * moveSpeed * heading2;
            //rb.AddVelocity(rightMovement + upMovement);

            //transform.position += rightMovement;
            //transform.position += upMovement;
        }

        // Interaction control
        if (Input.GetAxis("InteractionKey") > 0)
            Debug.Log("Interação");

        // Crouch control
        if (Input.GetAxis("AditionalKey") > 0 && !isCrouched)
        {
            isCrouched = true;
            transform.position -= new Vector3(0, 0.5f, 0);
            Debug.Log("Crouch");
        }
        else if (Input.GetAxis("AditionalKey") == 0 && isCrouched)
        {
            isCrouched = false;
            transform.position += new Vector3(0, 0.5f, 0);
            Debug.Log("Stand up");
        }

    }
}
