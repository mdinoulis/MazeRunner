using MLAgents;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mazeAcademy : Academy
{


    public mazeAgent agentPrefab;
    public MazeSpawner mazePrefab;
    private bool initializing = true;

    /// <summary>
    /// Reset the academy
    /// </summary>
    public override void AcademyReset()
    {
        MazeSpawner mazeInstance;

        if (!initializing)
        {
            mazePrefab.ResetArea();

            Debug.Log("destroying game");
        }

        initializing = false;

        Debug.Log("Academy Reset");

        mazeInstance = Instantiate(mazePrefab, transform);

        Debug.Log("New Instances Created");



    }

}