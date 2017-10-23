using UnityEngine;
using UnityStandardAssets.Utility;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class SphereMovement : MonoBehaviour {

    private Rigidbody rb;
    private bool isGrounded;
    private Camera m_Camera;
    private Vector3 m_OriginalCameraPosition;
    private Vector3 currentNormal;
    [SerializeField] private MouseLook m_MouseLook;
    public int groundSpeed;
    public int launchSpeed;
    

    // Use this for initialization
    void Start ()
    {
        isGrounded = false;
        m_Camera = GetComponentInChildren<Camera>();
        m_OriginalCameraPosition = m_Camera.transform.localPosition;
        m_MouseLook.Init(transform, m_Camera.transform);
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        m_MouseLook.LookRotation(transform, m_Camera.transform);
    }
    void FixedUpdate()
    {
        if (isGrounded)
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical);
            Debug.DrawRay(transform.position, m_Camera.transform.forward * 30, Color.red);

            rb.AddForce(movement * groundSpeed);
            if (Input.GetKeyDown("space"))
            {
                rb.AddForce(m_Camera.transform.forward * launchSpeed);
                RaycastHit hit;
                Physics.Raycast(transform.position, m_Camera.transform.forward, out hit);
                currentNormal = hit.normal;
                Debug.Log(currentNormal);
                isGrounded = false;
            }
        }

        if (Input.GetKeyDown("x"))
        {
            rb.AddForce(transform.forward * launchSpeed);
        }
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Arena")
        {
            isGrounded = true;
            rb.velocity = Vector3.zero;
        }
    }
}
