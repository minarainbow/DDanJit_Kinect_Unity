using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionGenerator : MonoBehaviour {

    public static int motion = Random.range(0,4);
    
	// Use this for initialization
	void Start () {
        time = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
        if (!GameControllManager.gameOver) {
            time += Time.deltaTime;
            if (toggleRot == 0) {
                if (time >= randThreshold) {
                    time = 0.0f;
                    transform.rotation = Quaternion.Euler(0, 0, 0); // 똑바로 봐라.
                    
                    // TODO: set randThreshold again.
                    randThreshold = Random.Range(randLeftRange, randRightRange);
                    
                    // TODO: set variables to turn around again.
                    toggleRot = 1;
                    
                    // TODO: set GameControllManager variable.
                    GameControllManager.hasTurned = true;
                }
            } else {
                // TODO: turn around again.
                if (time >= threshold) {
                    time = 0.0f;
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                    
                    toggleRot = 0;
                    
                    // TODO: set GameControllManager variable.
                    GameControllManager.hasTurned = false;
                }
            }
        }
	}
}
