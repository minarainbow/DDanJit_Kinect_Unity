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
        DatabaseReference mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;

        // light = GetComponent<Light>();

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
                    gameOver = true;
                    Debug.Log("punished mode\n");
                    RenderSettings.ambientSkyColor = Color.red;
                    GetComponent<Light>().color -= (Color.white / 2.0f) * Time.deltaTime;
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
            RenderSettings.ambientSkyColor = Color.red;
            GetComponent<Light>().color -= (Color.white / 2.0f) * Time.deltaTime;
            float t = Mathf.PingPong(Time.time, duration) / duration;
            light.color = Color.Lerp(color0, color1, t);
            scoreText.text = ":(";
            gameOverText.text = ">>> 엫힝 끝남 <<<";
            gameOverText.enabled = true;

            // TODO: get new user
            panel.SetActive(true);
            finalScoreText.enabled = true;
            nameInput.enabled = true;

            finalScoreText.text = score.ToString();
            nameInput.onEndEdit.AddListener(delegate
            {
                string newId = mDatabaseRef.Child("users").Push().Key;
                writeNewUser(newId, nameInput.text, score);
            });
        }
	}
    
    private void writeNewUser(string userId, string name, int score) {
        User user = new User(name, score);
        string json = JsonUtility.ToJson(user);
        
        mDatabaseRef.Child("users").Child(userId).SetRawJsonValueAsync(json);
    }
}
