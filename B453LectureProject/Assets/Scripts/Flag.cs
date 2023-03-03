using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    public LineRenderer lineToMouse;
    // Start is called before the first frame update
    void Awake()
    {
        
        lineToMouse = this.gameObject.GetComponent<LineRenderer>();

        lineToMouse.SetPosition(0, this.gameObject.transform.position);

    }

    // Update is called once per frame
    void Update()
    {

        setLineEndpointPositions();

    }

    void setLineEndpointPositions() 
    {

        lineToMouse.SetPosition(0, this.gameObject.transform.position);
        
        lineToMouse.SetPosition(1, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            
    }
}
