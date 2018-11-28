using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionSlot
{
    public enum SlotStatus { Normal, Blinking, Removed };

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

    public int GetMissionID()
    {
        return missionID;
    }

    public int GetStatus()
    {
        return status;
    }

    public void UpdateStatus(int newstatus)
    {
        status = newstatus;
    }
}

public class MissionSlotController : MonoBehaviour {

    public Transform Spawnpoint;
    public GameObject MissionSlotPrefab;

    List<MissionSlot> MissionSlotList = new List<MissionSlot>();

    public void SpawnMissionSlot(int motion)
    {
        // Instantiate(MissionSlotPrefab, Spawnpoint.position, Spawnpoint.rotation); // Done??
        MissionSlot ms = new MissionSlot(motion);

        MissionSlotList.Add(ms);
        int i;
        for(i = 0; i < MissionSlotList.Count; i++)
        {
            MissionSlot m = MissionSlotList[i];
            Debug.Log(string.Format("Mission {0}: {1} {2}", i, m.GetMissionID(), (MissionSlot.SlotStatus)m.GetStatus()));
        }
        Debug.Log("\n");
    }
    
    public void RemoveMissionSlot(int motion)
    {
        int index = MissionSlotList.FindIndex(x => (x.GetMissionID() == motion && x.GetStatus() != (int)MissionSlot.SlotStatus.Removed));
        MissionSlot ms = MissionSlotList[index];
        ms.UpdateStatus((int)MissionSlot.SlotStatus.Removed);
        MissionSlotList[index] = ms;
        
    }
}
