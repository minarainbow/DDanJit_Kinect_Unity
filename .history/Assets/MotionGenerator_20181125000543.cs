using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionGenerator : MonoBehaviour {

    
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    public int generateMotion(){
        int motion;
        motion = UnityEngine.Random.range(0,4);
        return motion;
    }
}
