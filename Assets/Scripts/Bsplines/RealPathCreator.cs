using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class RealPathCreator : MonoBehaviour
{

    PathCreator pathCreator;
    public GameObject go;
    float speed = 2f;
    float dist = 0;


    // Start is called before the first frame update
    void Start()
    {
        pathCreator = GetComponent<PathCreator>();
        //go = GetComponentInChildren<MeshRenderer>().gameObject;
       
    }

    // Update is called once per frame
    void Update()
    {
        go.transform.position = pathCreator.path.GetPointAtDistance(dist);
        go.transform.rotation = pathCreator.path.GetRotationAtDistance(dist);
        dist += speed * Time.deltaTime;
    }
}
