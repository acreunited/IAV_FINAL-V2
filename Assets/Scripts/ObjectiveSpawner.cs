using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveSpawner : MonoBehaviour
{
    public Material x;
    public Material o;
    GameObject objective;
    void Start()
    {
        //GenerateObjective();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject GetObjective()
    {
        return objective;
    }

    public void GenerateObjective()
    {
        if (Random.value > 0.5f)
        {
            objective = GameObject.CreatePrimitive(PrimitiveType.Quad);
            objective.transform.parent = transform;
            objective.transform.localPosition = new Vector3(0, 0.5f, 0);
            objective.transform.localScale=new Vector3(2, 2, 2);
            objective.transform.Rotate(0, 90f, 0);
            objective.tag = "X";
            objective.GetComponent<MeshRenderer>().material = x;
            
        }
        else 
        {
            objective = GameObject.CreatePrimitive(PrimitiveType.Quad);
            objective.transform.parent = transform;
            objective.transform.localPosition = new Vector3(0, 0.5f, 0);
            objective.transform.localScale = new Vector3(2, 2, 2);
            objective.transform.Rotate(0, 90f, 0);
            objective.tag = "O";
            objective.GetComponent<MeshRenderer>().material = o;
           
        }
    }
}
