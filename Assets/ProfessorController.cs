using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfessorController : MonoBehaviour {

    public float randLeftRange = 3.0f;
    public float randRightRange = 10.0f;

    public static bool isTurned;
    
    float time;
    float threshold = 1.0f;
    float randThreshold = 3.0f;
    public static int stressGauge; // Indicator of profess turning probability.
    
    bool toggleRot;
    
	// Use this for initialization
	void Start () {
        isTurned = false;
        toggleRot = false;
        stressGauge = 0;
        time = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
        if (!GameControllManager.gameOver) {
            time += Time.deltaTime;
            if (!toggleRot) {
                if (time >= randThreshold) {
                    time = 0.0f;
                    transform.rotation = Quaternion.Euler(0, 0, 0); // 똑바로 봐라.
                    
                    // TODO: set randThreshold again.
                    randThreshold = Random.Range(randLeftRange, randRightRange);
                    
                    // TODO: set variables to turn around again.
                    toggleRot = true;
                    
                    // TODO: set GameControllManager variable.
                    isTurned = true;
                }
            } else {
                // TODO: turn around again.
                if (time >= threshold) {
                    time = 0.0f;
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                    
                    toggleRot = false;
                    
                    // TODO: set GameControllManager variable.
                    isTurned = false;
                }
            }
        }
	}
}
