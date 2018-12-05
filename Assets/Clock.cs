using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clock : MonoBehaviour {

    public Image hand;
    public Image clockBack;

    private float timeThreshold;
    private float timeHalf;
    private float timeThirdQ;
	// Use this for initialization
	void Start () {
        timeThreshold = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
        if (!GameControllManager.gameOver)
        {
            timeThreshold = GameControllManager.gameTotalThreshold;
            timeHalf = timeThreshold / 2;
            timeThirdQ = timeThreshold * 3 / 4;

            float timeDur = GameControllManager.gameTime;
            float rot = -Time.deltaTime * 360 / timeThreshold; // 반시계방향

            Vector3 rotVec = new Vector3(0, 0, rot);
            hand.transform.Rotate(rotVec);

            if (timeDur > timeHalf)
            {
                clockBack.color = Color.yellow;
            }

            if (timeDur > timeThirdQ)
            {
                clockBack.color = Color.red;
            }
        }
	}
}
