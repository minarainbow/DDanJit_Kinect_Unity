using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionSlot
{
    public enum SlotStatus { Normal, Blinking, Removed };

    string missionKey;
    public int score;
    DateTime TimeCreated;
    int status;

    float threshold;
    float time;
    public bool IsCorrect;
    
    public MissionSlot(string mKey, float threshold)
    {
        missionKey = mKey;
        score = mKey.Length * (mKey.Length + 1) / 2;
        TimeCreated = DateTime.Now;
        status = (int)SlotStatus.Normal;
        IsCorrect = false;
        this.threshold = threshold;
    }

    public bool IsBlinking()
    {
        return false;
    }

    public bool IsTimeout()
    {
        //double SecondsElapsed = (DateTime.Now - TimeCreated).TotalSeconds;

        return time > threshold;
    }

    public int setTimeAndCheck() {
        time += Time.deltaTime;
        if (IsTimeout()) {
            this.status = (int)SlotStatus.Removed;
            return -1;
        }
        return 0;
    }

    public string GetMissionKey()
    {
        return missionKey;
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

public class MissionSlotController {

    public Transform Spawnpoint;
    public GameObject MissionSlotPrefab;

    List<MissionSlot> MissionSlotList = new List<MissionSlot>();

    float timeThreshold;

    public MissionSlotController(float threshold) {
        this.timeThreshold = threshold;
    }

    // Add a mission slot with MissionID motion to list
    public void SpawnMissionSlot(string motion)
    {
        // Instantiate(MissionSlotPrefab, Spawnpoint.position, Spawnpoint.rotation); // Done??
        MissionSlot ms = new MissionSlot(motion, timeThreshold);

        MissionSlotList.Add(ms);
        /*
        for (int i = 0; i < MissionSlotList.Count; i++)
        {
            MissionSlot m = MissionSlotList[i];
            Debug.Log(string.Format("Mission {0}: {1} {2}", i, m.GetMissionID(), (MissionSlot.SlotStatus)m.GetStatus()));
        }
        Debug.Log("\n");
        */
    }

    // Get four opened slot
    public List<MissionSlot> GetCurrentMissionSlots() {
        List<MissionSlot> currentMissionSlots = new List<MissionSlot>(4);
        int cnt = 0;
        // TODO: 처음부터 네 개 받아오. removed말고.
        for (int i = 0; i < MissionSlotList.Count; i++)
        {
            if (!MissionSlotList[i].GetStatus().Equals(
                MissionSlot.SlotStatus.Removed))
            {
                // remove가 아닌 경우
                MissionSlotList[cnt] = MissionSlotList[i];
                cnt++;
            }

            if (cnt > 4) {
                break;
            }
        }

        return currentMissionSlots;
    }

    /* This function does not actually remove a mission slot,
     * but only sets mission status to 'Removed' */
    int RemoveMissionSlot(string motion, bool IsCorrect)
    {
        int index = MissionSlotList.FindIndex(x => (x.GetMissionKey() == motion && x.GetStatus() != (int)MissionSlot.SlotStatus.Removed));
        MissionSlot ms = MissionSlotList[index];
        ms.UpdateStatus((int)MissionSlot.SlotStatus.Removed);
        ms.IsCorrect = IsCorrect;
        MissionSlotList[index] = ms;
        return ms.score;

    }

    public int OnCorrectAnswer(string motion)
    {
        return RemoveMissionSlot(motion, true);
    }

    public int OnWrongAnswer(string motion)
    {
        return -1;
    }

    public void OnTimeout(string motion)
    {
        RemoveMissionSlot(motion, false);
    }

    /* Check for any time-out missions, and remove them
     * Also change visibility features in this function */
    //NOW UNUSED
    public void CheckMissionTimer()
    {
        for (int i = 0; i < MissionSlotList.Count; i++)
        {
            MissionSlot m = MissionSlotList[i];

            // Check for time-out mission
            if (m.IsTimeout())
            {
                OnTimeout(m.GetMissionKey());
            }

            // TODO: Change visibility features, if any
        }
    }

    public void Update()
    {
        for (int i = 0; i < MissionSlotList.Count; i++) {
            if (!MissionSlotList[i].GetStatus().Equals(
                MissionSlot.SlotStatus.Removed)) {
                // remove가 아닌 경우
                if (MissionSlotList[i].setTimeAndCheck().Equals(-1)) {
                    //SpawnMissionSlot()
                    // TODO: Generate new motion
                }
            }
        }
    }
}
