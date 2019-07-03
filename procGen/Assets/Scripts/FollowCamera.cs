using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float speed = 0.8f;

    public float lookConst = 10;
    public float rotSpeed = 5;

    void FixedUpdate()
    {
        //target.forward, target.up, target.right used to make the offset in the correct direction as plane moves.
        Vector3 desPos = target.position + target.forward * offset.z + target.up * offset.y + target.right * offset.x;
        Vector3 smoothPos = Vector3.Lerp(transform.position, desPos, speed);
        transform.position = smoothPos;


        Quaternion rotation = transform.rotation;
        //LookAt looks in the direction of the plane. Const used to look a bit in front of the plane to speed it up and make we don't loose sight of plane. 
        transform.LookAt(target.position + target.forward * lookConst);
        Quaternion targetRotation = transform.rotation;

        transform.rotation = Quaternion.Slerp(rotation ,targetRotation,Time.deltaTime * rotSpeed);

    }
}
