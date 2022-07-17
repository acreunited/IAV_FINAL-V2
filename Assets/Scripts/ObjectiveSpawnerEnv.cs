using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectiveSpawnerEnv : MonoBehaviour
{ 
    // Start is called before the first frame update
    ObjectiveSpawner spawner;
    HallwayAgent agent;
    private void Awake()
    {
        spawner = GetComponentInChildren<ObjectiveSpawner>();
        agent = GetComponentInChildren<HallwayAgent>();
    }
    void Start()
    {
        agent.OnEpisodeBeginEvent += HandleOnEpsiodeBegin;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void HandleOnEpsiodeBegin(object sender, EventArgs e)
    {
        if (spawner.GetObjective() != null)
        {
            Destroy(spawner.GetObjective());
        }
        spawner.GenerateObjective();
    }
}
