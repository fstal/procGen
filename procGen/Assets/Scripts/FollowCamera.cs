using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    //public GameObject plane;
    public Transform target;
    public Vector3 offset;
    public float speed = 0.2f;
    public float lookAheadDst = 10;
    public float rotSmoothSpeed = 3;
    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 desPos = target.position + target.forward * offset.z + target.up * offset.y + target.right * offset.x;
        Vector3 smoothPos = Vector3.Lerp(transform.position, desPos, speed);
        transform.position = smoothPos;


        Quaternion rot = transform.rotation;
        transform.LookAt(target.position + target.forward * lookAheadDst);
        Quaternion targetRot = transform.rotation;

        transform.rotation = Quaternion.Slerp(rot,targetRot,Time.deltaTime * rotSmoothSpeed);

    }
}
