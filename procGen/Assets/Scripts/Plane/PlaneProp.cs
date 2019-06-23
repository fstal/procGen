using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneProp : MonoBehaviour
{

    public float rotationSpeed = 0.1f;
    public GameObject m_Prop;
    // Start is called before the first frame update

    // Update is called once per frame


    private void FixedUpdate()
    {
        RotateProp();
    }

    private void RotateProp()
    {

        m_Prop.transform.Rotate(0, 20, 0, Space.Self);

    }
}
