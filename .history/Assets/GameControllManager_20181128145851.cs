using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

public class GameControllManager : MonoBehaviour {
    
    public static bool gameOver;
    public static bool punished;
    public static bool hasTurned;
    public static int score;
    public static int motion;
    public static float timer;
    public AudioSource[] speaker; 
    public AudioClip bgm; 
    public GameObject gameOverSound;
    
    public Text motionText;

    public float timeThreshold = 50;
    public Text scoreText;
    public Text gameOverText;
    public Text finalScoreText;
    public InputField nameInput;
    public GameObject panel;
    public GameObject lightGameObject;
    public Light lightComp;
    public MotionGenerator mg;
    Color color0 = Color.red;
    Color color1 = Color.blue;
    float duration = 1.0f;
    float time;

    DatabaseReference mDatabaseRef;

    MissionSlotController msc = new MissionSlotController();

    // Use this for initialization
    void Start () {
        hasTurned = false;
        gameOver = false;
        punished = false;
        gameOverText.enabled = false;
        finalScoreText.enabled = false;
        nameInput.enabled = false;
        panel.SetActive(false);
        score = 0;

        // Setting Firebase instance.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://ddanjit-f2f5d.firebaseio.com/");
        
        // Getting root reference from firebase.
        mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;

        // Make a game object
        lightGameObject = new GameObject("The Light");

         // Add the light component
        lightComp = lightGameObject.AddComponent<Light>();

        // Set initial time
        time = 0.0f;

        Vector3 pos = scoreText.transform.position;
        pos.x += 0.2f;
        pos.y -= 0.1f;
        scoreText.transform.position = pos;
        scoreText.color = Color.red;
        
        // motionText.transform.position = new Vector3(-210, 80, 0);
        Vector3 pos2 = motionText.transform.position;
        pos2.x += 0.2f;
        pos2.y -= 0.15f;
        motionText.transform.position = pos2;
        motionText.color = Color.blue;

        mg = GetComponent<MotionGenerator>();
        motion = generateMotion();
        motionText.text = "Motion : " + motion;
        timer = 20.0f;

        speaker = GetComponents<AudioSource>();
        Debug.Log(speaker.Length);
	}
	
	// Update is called once per frame
	void Update () {
        // For debugging
        if (Input.GetKeyDown("q"))
        {
            gameOver = true;
        }

        timer -= 0.01f;
        if (timer < 0){
            motion = generateMotion();
            motionText.text = "Motion : " + motion;
        }

        if (!gameOver) {
            time += Time.deltaTime;
            // if (time > timeThreshold) {
            //     gameOver = true;
            // }

            // general mode
            scoreText.text = "Score: " + score;
            motionText.text = "Motion : " + motion;
            // professor turned around
            if(Input.anyKeyDown && hasTurned){
                score -= 3;
            }
            else if(Input.anyKeyDown && !hasTurned){
                switch (motion){
                    case 0:
                        if(Input.GetKeyDown("a"))
                        {
                            OnCorrectMotion();
                        }
                        else if(Input.anyKeyDown){
                            if(punished){
                                gameOver = true;
                            }
                            else
                                score --;
                        }
                            
                        break;
                    case 1:
                        if(Input.GetKeyDown("b"))
                            OnCorrectMotion();
                        else if(Input.anyKeyDown)
                            if(punished){
                                gameOver = true;
                            }
                            else
                                score --;
                        break;
                    case 2:
                        if(Input.GetKeyDown("c"))
                            OnCorrectMotion();
                        else if(Input.anyKeyDown)
                            if(punished){
                                gameOver = true;
                            }
                            else
                                score --;
                        break;
                    case 3:
                        if (Input.GetKeyDown("d"))
                            OnCorrectMotion();
                        else if (Input.anyKeyDown)
                            if (punished){
                                gameOver = true;
                            }
                            else
                                score--;
                        break;
                }
                motion = generateMotion();
                motionText.text = "Motion : " + motion;
            }
            if (score < 0 && !punished) {
                punished = true;
                Debug.Log("punished mode\n");
                score = 0;
                // Set color and position
                lightComp.color = Color.red;
                lightGameObject.transform.position = new Vector3(0, 5, 0);
            }
        } else {
            if (speaker[0].isPlaying){
                speaker[0].Pause();
                speaker[1].Play();
            }
            scoreText.text = ":(";
            gameOverText.text = ">>> 엫힝 끝남 <<<";
            gameOverText.enabled = true;

            // TODO: get new user
            panel.SetActive(true);
            finalScoreText.enabled = true;
            nameInput.enabled = true;

            finalScoreText.text = score.ToString();
        }
	}

    private void writeNewUser(string userId, string name, int score) {
        User user = new User(name, score);
        string json = JsonUtility.ToJson(user);
        
        mDatabaseRef.Child("users").Child(userId).SetRawJsonValueAsync(json);
    }

    public void InputCallBack() {
        string userName = nameInput.text;
        string newId = mDatabaseRef.Child("users").Push().Key;

        if (score < 0) {
            score = 0;
        }

        writeNewUser(newId, userName, score);
    }

    public int generateMotion(){
        int motion;
        motion = Random.Range(0,4);
        msc.SpawnMissionSlot(motion);
        return motion;
    }

    public void OnCorrectMotion()
    {
        score++;
        msc.RemoveMissionSlot(motion);
    }
}
