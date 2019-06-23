using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeliProps : MonoBehaviour
{
    public float rotationSpeed = 0.1f;
    public GameObject frontProp;
    public GameObject backProp;
    // Start is called before the first frame update

    // Update is called once per frame


    private void FixedUpdate()
    {
        RotateProp();
    }

    private void RotateProp()
    {

        frontProp.transform.Rotate(0, 0, 20, Space.Self);
        backProp.transform.Rotate(20, 0, 0, Space.Self);

    }
}
