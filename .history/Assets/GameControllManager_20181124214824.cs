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
    
    public Text scoreText;
    public Text gameOverText;
    public Text finalScoreText;
    public InputField nameInput;
    public GameObject panel;
    public GameObject lightGameObject;
    public Light lightComp;
    Color color0 = Color.red;
    Color color1 = Color.blue;
    float duration = 1.0f;

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
        pos.y -= 1.0f;
        scoreText.transform.position = pos;
	}
	
	// Update is called once per frame
	void Update () {
        if (!gameOver) {
            // general mode
            if(!punished){
                scoreText.text = "Score: " + score;
            
                if (Input.GetKeyDown("x") && !hasTurned) {
                    score ++;
                } else if (Input.GetKeyDown("x") && hasTurned) {
                    score -= 3;
                }
                
                if (score < 0) {
                    punished = true;
                    Debug.Log("punished mode\n");
                    // Set color and position
                    lightComp.color = Color.red;
                    lightGameObject.transform.position = new Vector3(0, 5, 0);
                }
            }
            // punished mode
            else{
                scoreText.text = "Score: " + score;
            
                if (Input.GetKeyDown("x") && !hasTurned) {
                    score ++;
                } else if (Input.GetKeyDown("x") && hasTurned) {
                    gameOver = true;
                }
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
