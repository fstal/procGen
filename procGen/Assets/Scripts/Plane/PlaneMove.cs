using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneMove : MonoBehaviour
{

    public float speed = 20.0f;
    public Rigidbody planeBody;


    //var horTurnSpeed : float = 10.0;
    //var verTurnSpeed : float = 10.0;
    // Start is called before the first frame update

    // Update is called once per frame
    void Start(){
        planeBody = GetComponent<Rigidbody>();        // Gets the players Rigidbody.
    }


    private void FixedUpdate()
    {
        moveForward();
    }

    private void moveForward()
    {
        //planeBody.AddForce(Vector3.forward*2);
        planeBody.velocity = transform.forward * speed;
        
        if (Input.GetKey(KeyCode.A)) {
            Vector3 tempVect = new Vector3(0, 0, 100);
            Quaternion deltaRotation = Quaternion.Euler(tempVect * Time.deltaTime);
            planeBody.MoveRotation(planeBody.rotation * deltaRotation);
        }

        if (Input.GetKey(KeyCode.D)) {
            Vector3 tempVect = new Vector3(0, 0, -100);
            Quaternion deltaRotation = Quaternion.Euler(tempVect * Time.deltaTime);
            planeBody.MoveRotation(planeBody.rotation * deltaRotation);
        }

        if (Input.GetKey(KeyCode.W)) {
            Vector3 tempVect = new Vector3(100, 0, 0);
            Quaternion deltaRotation = Quaternion.Euler(tempVect * Time.deltaTime);
            planeBody.MoveRotation(planeBody.rotation * deltaRotation);
        }
        if (Input.GetKey(KeyCode.S)) {
            Vector3 tempVect = new Vector3(-100, 0, 0);
            Quaternion deltaRotation = Quaternion.Euler(tempVect * Time.deltaTime);
            planeBody.MoveRotation(planeBody.rotation * deltaRotation);
        }

    }
}
