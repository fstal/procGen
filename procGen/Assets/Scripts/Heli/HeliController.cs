using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeliController : MonoBehaviour
{

    //Controller for helicopter. It's a bit fucked atm.

    public float speed = 200.0f;
    public float maxSpeed = 25f;
    public float revMaxSpeed = -25f;
    
    private float currentSpeed = 0;

    private Vector3 currentTilt = new Vector3(0, 0, 0);
    private Vector3 maxTilt = new Vector3(-20, 0, 0);
    private Vector3 maxBackTilt = new Vector3(20, 0, 0);
    private Vector3 movementSpeed = new Vector3(0, 0, 0);
    Vector3  accelerate = new Vector3(0, 0, 7);
    Vector3  deAccelerate = new Vector3(0, 0, -3);
    public Rigidbody heliBody;
    public Rigidbody tiltBody;
    
    // Start is called before the first frame update
    void Start(){
        heliBody = GetComponent<Rigidbody>();
    }


    private void FixedUpdate()
    {
        moveForward();
    }

    private void moveForward()
    {
        heliBody.velocity = transform.forward * currentSpeed;

        if(!Input.anyKey)
        {
            heliBody.velocity = heliBody.velocity - (transform.forward * 0.1f);
            currentSpeed = heliBody.velocity.magnitude;
        }

        if (Input.GetKey(KeyCode.W)) {
            //For some reason, it tilts first and goes forward later. Dunno why. Orkar inte lösa detta just nu.
            float speed = heliBody.velocity.magnitude;
            if(speed < maxSpeed){
                heliBody.velocity = heliBody.velocity + (transform.forward * 0.1f);
                currentSpeed = heliBody.velocity.magnitude;
            }
            
            if(currentTilt.x > maxTilt.x){
                Vector3 corrRotation = new Vector3(0, 180, 0);
                Vector3 tempVect = new Vector3(-30, 0, 0);
                Vector3 deltaRotation = ( tempVect * Time.deltaTime);
                currentTilt = currentTilt + deltaRotation;
                Quaternion quatCurrentTilt = Quaternion.Euler(currentTilt + corrRotation);
                tiltBody.transform.rotation = quatCurrentTilt;
            }

        }

        if (Input.GetKey(KeyCode.S)) {
            float speed = heliBody.velocity.magnitude;
            if(speed > revMaxSpeed){
                heliBody.velocity = heliBody.velocity - (transform.forward * 0.1f);
                currentSpeed = heliBody.velocity.magnitude;
            }
        }
        
        
        if (Input.GetKey(KeyCode.A)) {
            Vector3 tempVect = new Vector3(0, -50, 0);
            Quaternion deltaRotation = Quaternion.Euler(tempVect * Time.deltaTime);
            heliBody.MoveRotation(heliBody.rotation * deltaRotation);
        }

        if (Input.GetKey(KeyCode.D)) {
            Vector3 tempVect = new Vector3(0, 50, 0);
            Quaternion deltaRotation = Quaternion.Euler(tempVect * Time.deltaTime);
            heliBody.MoveRotation(heliBody.rotation * deltaRotation);
        }


        if (Input.GetKey(KeyCode.E)) {
            Vector3 movement = transform.up * 20 * Time.deltaTime;
            heliBody.MovePosition(heliBody.position + movement);
        }

        if (Input.GetKey(KeyCode.Q)) {
            Vector3 movement = -transform.up * 20 * Time.deltaTime;
            heliBody.MovePosition(heliBody.position + movement);
        }
    }
}
