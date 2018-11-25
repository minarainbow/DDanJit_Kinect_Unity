using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionSlot
{
    enum SlotStatus { Normal, Blinking, Removed };

    int missionID; // TODO: need to match with mission data type
    int score;
    DateTime TimeCreated;
    int status;

    public MissionSlot(int mID)
    {
        missionID = mID;
        score = 10;
        TimeCreated = DateTime.Now;
        status = (int)SlotStatus.Normal;
    }

    public bool IsBlinking()
    {
        return false;
    }

    public bool IsTimeout()
    {
        DateTime CurrentTIme = DateTime.Now;
        return false;
    }
}

public class MissionSlotController : MonoBehaviour {

    public Transform Spawnpoint;
    public GameObject MissionSlotPrefab;

    List<MissionSlot> MissionSlotList;

    void SpawnMissionSlot()
    {
        Instantiate(MissionSlotPrefab, Spawnpoint.position, Spawnpoint.rotation); // Done??
        MissionSlot ms = new MissionSlot(0);
        MissionSlotList.Add(ms);
    }


    void DestroyMissionSlot(int i)
    {
        // Archive removed slot?
        MissionSlotList.RemoveAt(i);
    }

    // Use this for initialization
    void Start () {
        MissionSlotList = new List<MissionSlot>();
    }
	
	// Update is called once per frame
	void Update () {
        for (var i = 0; i < MissionSlotList.Count; i++)
        {
            MissionSlot ms = MissionSlotList[i];
            if (ms.IsTimeout())
            {
                DestroyMissionSlot(i);
            }
            else if (ms.IsBlinking())
            {
                // TODO: change status to Blink
            }
            else
            {

            }
        }
	}
}
