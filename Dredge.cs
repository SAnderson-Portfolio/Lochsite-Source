using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dredge : MonoBehaviour
{
   
    // Start is called before the first frame update
    void Start()
    {
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.name = "Paint Canvas";
        plane.AddComponent<PaintBrush>();
        plane.transform.parent = gameObject.transform;
        plane.transform.Rotate(new Vector3(90f, 90f, -90f));
        plane.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Scripts/Shaders/Unlit_Paint");
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
