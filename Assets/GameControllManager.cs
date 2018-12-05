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
    public static float gameTime; // for total game time.
    public static float gameTotalThreshold; // get timeThreshold & send it to Clock class.

    public static Player player1;
    public static Player player2;
    
    public Text motionText;

    public float timeThreshold = 50;

    // for multiplayer
    public Text score1Text;
    public Text score2Text;
    public Text motion1Text;
    public Text motion2Text;

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
    Dictionary<int, string> motions;

    DatabaseReference mDatabaseRef;

    List<MissionSlot> missionSlots = new List<MissionSlot>(4);
    MissionSlotController msc = new MissionSlotController(2);

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
        motions = new Dictionary<int, string>();
        motions.Add(0, "a");
        motions.Add(1, "b");
        motions.Add(2, "c");
        motions.Add(3, "d");

        player1 = new Player(1);
        player2 = new Player(2);

        //        scoreText = GetComponent <Text> ();
        //        gameOverText = GetComponent <Text> ();

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

        //Vector3 pos = score1Text.transform.position;
        //pos.x += 0.2f;
        //pos.y -= 0.1f;
        //score1Text.transform.position = pos;
        //score1Text.color = Color.red;
        
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

        // For clock actions
        gameTotalThreshold = timeThreshold;
        gameTime = 0.0f;
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
            gameTime += Time.deltaTime;
            if (gameTime > timeThreshold) {
                 gameOver = true;
             }

            // general mode
            score1Text.text = "Score: " + player1.getScore();
            score2Text.text = "Score: " + player2.getScore();

            motionText.text = "Motion : " + motion;
            // professor turned around
            if(Input.anyKeyDown && hasTurned){
                OnSpotted();
            }

            else if(Input.anyKeyDown && !hasTurned){
                // 우선은 이렇게 그지같이 짜 놓고 나중에 바꾸기
                if (Input.GetKeyDown("w"))
                {
                    Debug.Log("ASDF");

                    player1.addMotion(1);
                }
                else if (Input.GetKeyDown("s"))
                {
                    player1.addMotion(2);
                }
                else if (Input.GetKeyDown("a"))
                {
                    player1.addMotion(3);
                }
                else if (Input.GetKeyDown("d"))
                {
                    player1.addMotion(4);
                }
                else if (Input.GetKeyDown("up"))
                {
                    player2.addMotion(1);
                }
                else if (Input.GetKeyDown("down"))
                {
                    player2.addMotion(2);
                }
                else if (Input.GetKeyDown("left"))
                {
                    player2.addMotion(3);
                }
                else if (Input.GetKeyDown("right"))
                {
                    player2.addMotion(4);
                }

                if (motions.ContainsKey(motion))
                {
                    if (Input.GetKeyDown(motions[motion]))
                        OnCorrectMotion();
                    else if (Input.anyKeyDown)
                        OnWrongMotion();
                }

                motion1Text.text = player1.getMotionString();
                motion2Text.text = player2.getMotionString();
                motion = generateMotion();
                motionText.text = "Motion : " + motion;
            }
            if (score < 0) {
                score = 0;
            }

            missionSlots = msc.GetCurrentMissionSlots();
        } else {
            score1Text.text = ":(";
            score2Text.text = ":(";

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
        double multiplier = punished ? 1.5 : 1;
        score += (int)(msc.OnCorrectAnswer(motion) * multiplier);
    }

    public void OnWrongMotion()
    {
        return;
        if (punished)
            gameOver = true;
        else
            score += msc.OnWrongAnswer(motion);
    }

    public void OnSpotted()
    {
        // Spotted twice, game over
        if (punished)
            gameOver = true;
        // Change to punish mode
        else
            SetPunish();
    }

    public void SetPunish()
    {
        Debug.Log("now punished mode\n");
        punished = true;

        // Set color and position
        lightComp.color = Color.red;
        lightGameObject.transform.position = new Vector3(0, 5, 0);
    }
}
