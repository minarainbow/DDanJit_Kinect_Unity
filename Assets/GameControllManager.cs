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
    public static string mission;
    public static float timer;
    public AudioSource speaker;
    public GameObject gameOverSound;
    public static float gameTime; // for total game time.
    public static float gameTotalThreshold; // get timeThreshold & send it to Clock class.
    public Slider angerBarSlider;

    public static Player player1;
    public static Player player2;

    public float timeThreshold = 50;

    // for multiplayer
    public Text score1Text;
    public Text score2Text;
    public Text motionText;
    public Text motion1Text;
    public Text motion2Text;

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

    DatabaseReference mDatabaseRef;

    List<MissionSlot> missionSlots = new List<MissionSlot>(4);
    MissionSlotController msc = new MissionSlotController(2);

    Dictionary<string, string> player1KeyMap;
    Dictionary<string, string> player2KeyMap;

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

        player1 = new Player(1);
        player2 = new Player(2);
        player1KeyMap = new Dictionary<string, string>();
        player1KeyMap.Add("w", "1");
        player1KeyMap.Add("a", "2");
        player1KeyMap.Add("s", "3");
        player1KeyMap.Add("d", "4");
        player2KeyMap = new Dictionary<string, string>();
        player2KeyMap.Add("up", "1");
        player2KeyMap.Add("left", "2");
        player2KeyMap.Add("down", "3");
        player2KeyMap.Add("right", "4");

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

        mission = generateMission();
        motionText.text = "Mission : " + mission;
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
            mission = generateMission();
            motionText.text = "Mission : " + mission;
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

            motionText.text = "Mission : " + mission;
            // professor turned around

            string[] player1_key = {
                    "w", "s", "a", "d"
                };

            string[] player2_key = {
                    "up", "down", "left", "right"
                };

            if (Input.anyKeyDown && hasTurned){
                for (int i = 0; i < 4; i++)
                {
                    if (Input.GetKeyDown(player1_key[i]))
                    {
                        OnSpotted(1);
                    }
                }
                for (int i = 0; i < 4; i++)
                {
                    if (Input.GetKeyDown(player2_key[i]))
                    {
                        OnSpotted(2);
                    }
                }
            }

            else if(Input.anyKeyDown && !hasTurned){
                bool isPlayer1, isPlayer2;
                isPlayer1 = isPlayer2 = false;
                foreach (string key in player1KeyMap.Keys)
                {
                    if(Input.GetKeyDown(key))
                    {
                        player1.addMotion(player1KeyMap[key]);
                        isPlayer1 = true;
                    }
                }
                foreach (string key in player2KeyMap.Keys)
                {
                    if (Input.GetKeyDown(key))
                    {
                        player2.addMotion(player2KeyMap[key]);
                        isPlayer2 = true;
                    }
                }

                Debug.Assert(isPlayer1 == false || isPlayer2 == false);

                if (isPlayer1 || isPlayer2)
                {
                    string motion = isPlayer1 ? player1.getMotion() : player2.getMotion();
                    /* Detect key, find corresponding player, and append to 'motion'. */
                    if (mission.Equals(motion))
                    {
                        OnCompletedMotion(isPlayer1);
                    }
                    else if (mission.StartsWith(motion))
                    {
                        OnCorrectMotion(isPlayer1);
                    }
                    else
                    {
                        // TODO: Change it to per-player. Handle restart in below function.
                        OnWrongMotion(isPlayer1);
                    }

                    motion1Text.text = player1.getMotion();
                    motion2Text.text = player2.getMotion();
                    mission = generateMission();
                    motionText.text = "Mission : " + mission;
                }
            }

            missionSlots = msc.GetCurrentMissionSlots();
        } else {
            if (speaker.isPlaying){
                speaker.Pause();
                AudioClip clip = (AudioClip) Resources.Load("gameOverSound", typeof(AudioClip));
                speaker.PlayOneShot(clip);
            }
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

    /* Function that generates new motion.
     * Motion length is 4~8
     * Keys: 1 - up, 2 - left, 3 - down, 4 - right */
    public string generateMission(){
        string motion = null;
        int motionLength = Random.Range(4, 9);
        for (int i = 0; i < motionLength; i++)
        {
            motion = motion + Random.Range(1, 5).ToString();
        }
        msc.SpawnMissionSlot(motion);
        return motion;
    }

    public void OnCompletedMotion(bool isPlayer1)
    {
        double multiplier = punished ? 1.5 : 1;
        if (isPlayer1)
            player1.addScore((int)(msc.OnCorrectAnswer(mission) * multiplier));
    }

    public void OnCorrectMotion(bool isPlayer1)
    {
        // TODO: Update player progress.
    }

    public void OnWrongMotion(bool isPlayer1)
    {

        return;
        if (punished){
            showProfessorText(professorGetOutText);
        }
        else
            score += msc.OnWrongAnswer(mission);
    }

    public void OnSpotted(int type)
    {
        // Spotted twice, game over
        if (type == 1) {
            if (player1.isPunished())
            {
                player1.setDead();
            } else 
            {
                player1.setPunished();
            }
        } else if (type == 2) {
            if (player2.isPunished())
            {
                player2.setDead();
            } else {
                player2.setPunished();
            }
        }
        if (player1.isDead() || player2.isDead())
        {
            gameOver = true;
        }
        // Change to punish mode
        else
        {
            showProfessorText(professorGetOutText);
            SetPunish();
        }
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
        professorTextTimer = 0.80f;
    }

    public void hideProfessorText()
    {
        Debug.Log("here\n");
        if(professorGetOutText.enabled){
            gameOver = true;
        }
        professorAnnoyedText.enabled = false;
        professorWarnText.enabled = false;
        professorGetOutText.enabled = false;
    }
}
