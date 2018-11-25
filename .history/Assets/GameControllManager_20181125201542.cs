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
    
    public Text motionText;
    public Text scoreText;
    public Text gameOverText;
    public Text finalScoreText;
    public InputField nameInput;
    public GameObject panel;
    public GameObject lightGameObject;
    public Light lightComp;
    public MotionGenerator mg;

    DatabaseReference mDatabaseRef;

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

        Vector3 pos = scoreText.transform.position;
        pos.x += 0.2f;
        pos.y -= 0.1f;
        scoreText.transform.position = pos;
        scoreText.color = Color.red;

        Vector3 pos2 = motionText.transform.position;
        pos2.x += 0.4f;
        pos2.y -= 0.4f;
        scoreText.transform.position = pos2;

        mg = GetComponent<MotionGenerator>();
        motion = mg.generateMotion();
        scoreText.text = "Motion : " + motion;
	}
	
	// Update is called once per frame
	void Update () {
        if (!gameOver) {
            // general mode
            scoreText.text = "Score: " + score;
            // professor turned around
            if(Input.anyKey && hasTurned){
                score -= 3;
            }
            else if(!hasTurned){
                switch (motion){
                    case 0:
                        if(Input.GetKeyDown("a"))
                            score ++;
                        else if(Input.anyKey)
                            if(punished)
                                gameOver = true;
                            else
                                score --;
                        break;
                    case 1:
                        if(Input.GetKeyDown("b"))
                            score ++;
                        else if(Input.anyKey)
                            if(punished)
                                gameOver = true;
                            else
                                score --;
                        break;
                    case 2:
                        if(Input.GetKeyDown("c"))
                            score ++;
                        else if(Input.anyKey)
                            if(punished)
                                gameOver = true;
                            else
                                score --;
                        break;
                    case 3:
                        if(Input.GetKeyDown("d"))
                            score ++;
                        else if(Input.anyKey)
                            if(punished)
                                gameOver = true;
                            else
                                score --;
                        break;
                }
                motion = mg.generateMotion();
            }
            if (score < 0 && !punished) {
                punished = true;
                Debug.Log("punished mode\n");
                // Set color and position
                lightComp.color = Color.red;
                lightGameObject.transform.position = new Vector3(0, 5, 0);
            }
        } else {
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


}
