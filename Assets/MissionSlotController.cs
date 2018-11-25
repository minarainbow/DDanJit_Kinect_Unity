using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionSlotController : MonoBehaviour {

    public Transform Spawnpoint;
    public GameObject MissionSlotPrefab;

    void SpawnMissionSlot()
    {
        Instantiate(MissionSlotPrefab, Spawnpoint.position, Spawnpoint.rotation);
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
