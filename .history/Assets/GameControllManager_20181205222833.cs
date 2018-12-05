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
    public AudioSource speaker;
    public GameObject gameOverSound;
    public static float gameTime; // for total game time.
    public static float gameTotalThreshold; // get timeThreshold & send it to Clock class.
    public Slider angerBarSlider;
    
    public Text motionText;

    public float timeThreshold = 500;
    public Text scoreText;
    public Text gameOverText;
    public Text finalScoreText;
    public Text professorAnnoyedText;
    public Text professorWarnText;
    public Text professorGetOutText;
    public float professorTextTimer = 0.0f;
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

    MissionSlotController msc = new MissionSlotController();

    // Use this for initialization
    void Start () {
        // speaker.Play();
        hasTurned = false;
        gameOver = false;
        punished = false;
        gameOverText.enabled = false;
        finalScoreText.enabled = false;
        nameInput.enabled = false;
        professorAnnoyedText.enabled = false;
        professorWarnText.enabled = false;
        professorGetOutText.enabled = false;
        panel.SetActive(false);
        score = 0;
        motions = new Dictionary<int, string>();
        motions.Add(0, "a");
        motions.Add(1, "b");
        motions.Add(2, "c");
        motions.Add(3, "d");

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

        speaker = GetComponent<AudioSource>();

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
        if(professorTextTimer > 0.00f){
            Debug.Log("here!\n");
            professorTextTimer -= 0.01f;
            if(professorTextTimer < 0.00f)
                hideProfessorText();
        }
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
            scoreText.text = "Score: " + score;
            motionText.text = "Motion : " + motion;
            // professor turned around
            if(Input.anyKeyDown && hasTurned){
                OnSpotted();
            }
            else if(Input.anyKeyDown && !hasTurned){
                if (motions.ContainsKey(motion))
                {
                    if (Input.GetKeyDown(motions[motion]))
                        OnCorrectMotion();
                    else if (Input.anyKeyDown)
                        OnWrongMotion();
                }

                motion = generateMotion();
                motionText.text = "Motion : " + motion;
            }
            if (score < 0) {
                score = 0;
            }
            msc.CheckMissionTimer();
        } else {
            if (speaker.isPlaying){
                speaker.Pause();
                AudioClip clip = (AudioClip) Resources.Load("gameOverSound", typeof(AudioClip));
                speaker.PlayOneShot(clip);
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
        double multiplier = punished ? 1.5 : 1; //should be removed?
        score += (int)(msc.OnCorrectAnswer(motion) * multiplier);
    }

    public void OnWrongMotion()
    {
        return;
        if (punished){
            showProfessorText(professorGetOutText);
        }
        else
            score += msc.OnWrongAnswer(motion);
    }

    public void OnSpotted()
    {
        // Spotted twice, game over
        if (punished){
            showProfessorText(professorGetOutText);
        }
        // Change to punish mode
        else
            SetPunish();
    }

    public void SetPunish()
    {
        Debug.Log("now punished mode\n");
        punished = true;
        showProfessorText(professorWarnText);
        angerBarSlider.value = 0.5f;
        // Set color and position
        lightComp.color = Color.red;
        lightGameObject.transform.position = new Vector3(0, 5, 0);
    }

    public void showProfessorText(Text professorText)
    {
        professorText.enabled = true;
        professorTextTimer = 0.50f;
    }

    public void hideProfessorText()
    {
        Debug.Log("here\n");
        professorAnnoyedText.enabled = false;
        professorWarnText.enabled = false;
        professorGetOutText.enabled = false;
    }
}
