using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {
    Rigidbody phys;
	bool jumping = false;
	bool doJump = false;

	bool allowTurn = true;
	bool enableGravity = true;
	bool orientPlayer = true;
	bool allowMovement = true;

	bool inAir = false;

	float turnSpeed = 100;
	float jumpForce = 10;
	float moveSpeed = 100;
	float gravity = 100;
	float drag = 10;

	Vector3 normal = Vector3.up;

    void Start() {
        phys = GetComponent<Rigidbody>();
		Transform head = transform.Find("Spacer_character_rigged").Find("Head");
		Vector3 centerOfMass = head.position - transform.position;
		phys.centerOfMass = centerOfMass;
    }

    void FixedUpdate() {
        // player turn
        float spin = Input.GetAxis("Horizontal");
		
		if (Mathf.Abs(spin)>0 && allowTurn) {
			phys.AddTorque(transform.up * spin * turnSpeed * Time.deltaTime, ForceMode.VelocityChange);
		}

		Debug.DrawLine(transform.position, transform.position + phys.angularVelocity, Color.red);

		// player stay up right
        Vector3 right = Vector3.Cross(transform.forward, normal);
        Vector3 forward = Vector3.Cross(normal, right);
        Quaternion newRotation = Quaternion.LookRotation(forward, normal);
        Quaternion fromTo = newRotation * Quaternion.Inverse(transform.rotation);

		if (orientPlayer) {
			float speed = 1000000;

			if (Quaternion.Dot(transform.rotation, newRotation) > 0) {
				phys.AddTorque(new Vector3(fromTo.x, fromTo.y, fromTo.z) * speed * Time.deltaTime);
			} else {
				phys.AddTorque(new Vector3(fromTo.x, fromTo.y, fromTo.z) * -speed * Time.deltaTime);
			}
		}

		// move forward
		float forwardMove = Input.GetAxis("Vertical");
		if (allowMovement) phys.AddForce(forward*moveSpeed*forwardMove,ForceMode.Force);

		// gravity
		RaycastHit hit;
		if (Physics.Raycast( transform.position, -transform.up, out hit)) {
			if (hit.distance>3.55 && enableGravity) {
				phys.AddForce(-normal*gravity,ForceMode.Force);
			}
		}

		// jump
		if (doJump) {
			phys.AddForce(transform.up*jumpForce,ForceMode.Impulse);
			doJump = false;
			phys.drag = 0;
			enableGravity = false;
			inAir = true;
		}

		Debug.DrawLine(hit.point, hit.point + hit.normal*10, Color.blue);

		// in air flip check
		if (inAir) {
			if (Physics.Raycast( transform.position, phys.velocity, out hit)) {
				if (hit.distance < 20*(phys.velocity.magnitude/10.0f)) {
					normal = hit.normal;
					enableGravity = true;
					inAir = false;
				}
			}
		} else { // stay normal with walking check
			if (hit.distance<=3.55 && phys.drag != 10) {
				phys.drag = 10;
			}

			//normal = hit.normal;

			if (Quaternion.Angle(transform.rotation, newRotation) < 0.01) {
				//normal = hit.normal;
			}
		}
    }
	
    void Update() {
		if (Input.GetAxis("Jump") != 0 && !jumping) {
			doJump = true;
			jumping = true;
		} else if (Input.GetAxis("Jump") == 0 && jumping) {
			jumping = false;
		}
    }
}

/*
public class PlayerMovement : MonoBehaviour {

	float moveSpeed = 6f; // move speed
	float turnSpeed = 30f; // turning speed (degrees/second)
	float lerpSpeed = 10f; // smoothing speed
	float gravity = 10f; // gravity acceleration
	bool isGrounded;
	float deltaGround = 0.2f; // character is grounded up to this distance
	float jumpSpeed = 20f; // vertical jump initial speed
	float jumpRange = 100f;
	float flipRange = 20;

	private Vector3 surfaceNormal;
	private Vector3 myNormal;
	private float distGround;
	private bool jumping  = false;
	private float vertSpeed = 0f;
	private bool feetGravity = true;
    bool flipping = false;

	Rigidbody myRigidBody;
	Vector3 cubePos;
	Vector3 dstPOSCube;
	void Start()
	{
		myNormal = transform.up;
		myRigidBody = GetComponent<Rigidbody> ();
		//myRigidBody.freezeRotation = true;
		distGround = GetComponent<Collider> ().bounds.max.y - GetComponent<Collider> ().bounds.min.y;
		Debug.Log (GetComponent<Collider> ().bounds.extents.y + "GetComponent<Collider> ().bounds.extents.y");
		Debug.Log (GetComponent<Collider> ().bounds.center.y + "GetComponent<Collider> ().bounds.center.y");
		Debug.Log (distGround + "disground");

        Transform head = transform.Find("Spacer_character_rigged").Find("Head");
        Vector3 centerOfMass = head.position - transform.position;
        myRigidBody.centerOfMass = centerOfMass;
    }

	void FixedUpdate()
	{
        if (feetGravity) {
            if (!flipping) {
                myRigidBody.AddForce(-gravity * myRigidBody.mass * Vector3.up);
                
            }
		}

        Transform head = transform.Find("Spacer_character_rigged").Find("Head");
        //myRigidBody.AddTorque(head.right*100, ForceMode.VelocityChange);
        //myRigidBody.AddForceAtPosition(-head.up*10, transform.position - transform.up * 3.5f);

        float turn = Input.GetAxis("Horizontal");

        if (Mathf.Abs(turn) > 0) {
            myRigidBody.AddTorque(transform.up * turn * turnSpeed * Time.deltaTime, ForceMode.VelocityChange);
        } else {
        }


        // rot
        Vector3 right = Vector3.Cross(transform.forward, Vector3.up);
        Vector3 forward = Vector3.Cross(Vector3.up, right);
        Quaternion newRotation = Quaternion.LookRotation(forward, Vector3.up);
        Quaternion fromTo = newRotation * Quaternion.Inverse(transform.rotation);

        if (Quaternion.Dot(transform.rotation, newRotation) > 0)
        {
            //myRigidBody.AddTorque(new Vector3(fromTo.x, fromTo.y, fromTo.z) * 10000 * Time.deltaTime);
        }
        else
        {
            //myRigidBody.AddTorque(new Vector3(fromTo.x, fromTo.y, fromTo.z) * -10000 * Time.deltaTime);
        }

        float x = myRigidBody.angularVelocity.x;
        float y = myRigidBody.angularVelocity.y;
        float z = myRigidBody.angularVelocity.z;

        myRigidBody.AddTorque(new Vector3(-x*10, -y*10, -z*10));
        //transform.Rotate(0, Input.GetAxis("Horizontal") * turnSpeed * Time.deltaTime, 0)
    }

	void Update()
	{
        //Debug.DrawLine(transform.position, head.position, Color.red);

        Debug.Log(isGrounded);

        if (jumping) {
			Ray ray1;
			RaycastHit hit1;
			ray1 = new Ray(transform.position, transform.up);
			if (Physics.Raycast (ray1, out hit1, flipRange)) {
                //GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
				StartCoroutine (flipCharacter (hit1.point, hit1.normal));
				feetGravity = true;
				jumping = false;
                flipping = true;

            }
		}
			
			
		Ray ray;
		RaycastHit hit;
		if(Input.GetButtonDown("Jump")){
				Debug.Log ("Jump");
				ray = new Ray(transform.position, transform.up);
			if (Physics.Raycast (ray, out hit, jumpRange)) {
				cubePos = hit.point;
				Debug.DrawRay (transform.position, transform.up * hit.distance,Color.green,10);
				StartCoroutine(JumpToWall (hit.point, hit.normal));
			} else if (isGrounded) {
				myRigidBody.velocity += jumpSpeed * myNormal;
			}
		}


		//transform.Rotate (0, Input.GetAxis ("Horizontal") * turnSpeed * Time.deltaTime, 0);

		ray = new Ray (transform.position, -myNormal);
		if (Physics.Raycast (ray, out hit)) {
			isGrounded = hit.distance <= distGround + deltaGround;
			surfaceNormal = hit.normal;
		} else {
			isGrounded = false;
			surfaceNormal = transform.up;
		}
		myNormal = Vector3.Lerp (myNormal, surfaceNormal, lerpSpeed * Time.deltaTime);
		Vector3 myForward = Vector3.Cross (transform.right, myNormal);
		Quaternion targetRot = Quaternion.LookRotation (myForward, myNormal);
		//transform.rotation = Quaternion.Lerp (transform.rotation, targetRot, lerpSpeed * Time.deltaTime);
		//transform.Translate (0, 0, Input.GetAxis ("Vertical") * moveSpeed * Time.deltaTime);
	}

    Vector3 oldPos = new Vector3(0, 0, 0);
    Vector3 oldRight = new Vector3(0, 0, 0);

	IEnumerator flipCharacter(Vector3 point, Vector3 normal)
	{
		Vector3 orgPos = transform.position;
		Quaternion orgRot = transform.rotation;
		Debug.Log (point);
		Debug.Log (normal);
		Vector3 dstPos = point + normal * (distGround + .05f);
		dstPOSCube = dstPos;
		Vector3 myForward = Vector3.Cross (transform.right, normal);
		Quaternion dstRot = Quaternion.LookRotation (myForward, normal);



        //transform.RotateAround(transform.GetComponentInChildren<Dummy>().transform.parent.transform.position, Vector3.up, 20 * Time.deltaTime );

        Transform head = transform.Find("Spacer_character_rigged").Find("Head");
        oldPos = head.position;
        oldRight = head.right;

        float sofar = 0;
        float change = 0 ;
        while (Vector3.Dot(normal,transform.up) < .9) {
            oldPos = head.position;
            oldRight = head.right;

            change = 50 * Time.deltaTime;
            sofar += change;
            //transform.position = Vector3.Lerp(orgPos, dstPos, t);
            ///transform.rotation = Quaternion.Slerp(orgRot, dstRot, t);
            //transform.RotateAround(oldPos, oldRight, change);
            myRigidBody.AddTorque(head.right/3, ForceMode.VelocityChange);
            yield return null; // return here next frame
		}
        myNormal = normal;
        flipping = false;
        yield return null;

	}


	/*
	IEnumerator JumpToWall(Vector3 point, Vector3 normal)
	{	
		Debug.Log ("Start");
		jumping = true;
		myRigidBody.isKinematic = true;
		Vector3 orgPos = transform.position;
		Quaternion orgRot = transform.rotation;
		Debug.Log (point);
		Debug.Log (normal);
		Vector3 dstPos = point + normal * (distGround + .05f);
		dstPOSCube = dstPos;
		Vector3 myForward = Vector3.Cross (transform.right, normal);
		Quaternion dstRot = Quaternion.LookRotation (myForward, normal);
		for (float t = 0.0f; t < 1.0f;) {
			t += Time.deltaTime;
			transform.position = Vector3.Lerp(orgPos, dstPos, t);
			transform.rotation = Quaternion.Slerp(orgRot, dstRot, t);
			yield return null; // return here next frame
		}

		myRigidBody.isKinematic = false;
		jumping = false;
			




	}*/

/*

IEnumerator JumpToWall(Vector3 point, Vector3 normal)
{
    Debug.Log ("Start");
    jumping = true;
    feetGravity = false;
    myRigidBody.AddForce (gravity * myRigidBody.mass * myNormal,ForceMode.Impulse);
    yield return null;
    //now that we are floating, check above our head to see if we are approaching an object
    /*
    Ray ray;
    RaycastHit hit;
    ray = new Ray(transform.position, transform.up);
    while (!Physics.Raycast (ray, out hit, flipRange)) 
        Debug.DrawRay (transform.position, transform.up * hit.distance,Color.green,10);
        dstPOSCube = hit.point;
        yield return null;
    }*/
/*
Vector3 orgPos = transform.position;
Quaternion orgRot = transform.rotation;
Debug.Log (point);
Debug.Log (normal);
Vector3 dstPos = hit.point + hit.normal * (distGround + .05f);
dstPOSCube = dstPos;
Vector3 myForward = Vector3.Cross (transform.right, hit.normal);
Quaternion dstRot = Quaternion.LookRotation (myForward, hit.normal);*/
/*
for (float t = 0.0f; t < 1.0f;) {
    t += .05f*Time.deltaTime;
    transform.position = Vector3.Lerp(orgPos, dstPos, t);
    transform.rotation = Quaternion.Slerp(orgRot, dstRot, t);
    yield return null; // return here next frame
}*/



/*
	}



	void OnDrawGizmos()
	{	Gizmos.color = Color.blue;
		Gizmos.DrawCube(cubePos, new Vector3(1, 1, 1));
		Gizmos.DrawCube(dstPOSCube, new Vector3(1, 1, 1));
	}

}

    */
      