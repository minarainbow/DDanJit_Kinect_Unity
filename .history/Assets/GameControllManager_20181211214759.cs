using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

public class GameControllManager : MonoBehaviour {

    public static bool gameOver;
    public static bool turnTrigger;
    public static bool isPunishedMode;
    public static string mission;
    public static float timer;
    public AudioSource speaker;
    public AudioSource playerSpeaker;
    public AudioClip gameOverSound;
    public AudioClip clapSound;
    public AudioClip passSound;
    public static float gameTime; // for total game time.
    public static float gameTotalThreshold; // get timeThreshold & send it to Clock class.
    public Slider angerBarSlider;


    public float timeThreshold = 50;

    // for multiplayer
    public Text score1Text;
    public Text score2Text;
    public Text motionText;
    public Text motion1Text;
    public Text motion2Text;

    int playerNum = 2;
    public static Player[] players;

    int missionMinLength = 4;
    int missionMaxLength = 8;

    public Text gameOverText;
    public Text professorAnnoyedText;
    public Text professorWarnText;
    public Text professorGetOutText;
    public float professorTextTimer = 0.0f;
    
    public GameObject panel;
    public Text finalScore1Text;
    public Text finalScore2Text;
    public InputField nameInput1;
    public InputField nameInput2;
    public GameObject lightGameObject;
    public Light lightComp;

    Color color0 = Color.red;
    Color color1 = Color.blue;
    float duration = 1.0f;
    float time;


    DatabaseReference mDatabaseRef;

    List<MissionSlot> missionSlots = new List<MissionSlot>(4);
    MissionSlotController msc = new MissionSlotController(2);

    // Use this for initialization
    void Start () {
        // speaker.Play();
        gameOver = false;
        turnTrigger = false;
        isPunishedMode = false;
        gameOverText.enabled = false;
        finalScore1Text.enabled = false;
        nameInput1.enabled = false;
        finalScore2Text.enabled = false;
        nameInput2.enabled = false;
        professorAnnoyedText.enabled = false;
        professorWarnText.enabled = false;
        professorGetOutText.enabled = false;
        panel.SetActive(false);

        speaker = GetComponents<AudioSource>()[0];
        playerSpeaker = GetComponents<AudioSource>()[1];

        // Initialize players.
        players = new Player[playerNum + 1];
        for (var i = 1; i <= playerNum; i++)
        {
            players[i] = new Player(i);
        }

        // Attach keymap to each player.
        // Note: z and / are temp. clap keys.
        string[] player1Keys = { "z", "w", "a", "s", "d" };
        string[] player2Keys = { "/", "up", "left", "down", "right" };
        players[1].addKeyMap(player1Keys);
        players[2].addKeyMap(player2Keys);

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
        // Vector3 pos2 = motionText.transform.position;
        // pos2.x += 0.2f;
        // pos2.y -= 0.15f;
        // motionText.transform.position = pos2;
        motionText.color = Color.blue;

        mission = generateMission();
        motionText.text = "Mission : " + mission;
        timer = 20.0f;

        // For clock actions
        gameTotalThreshold = timeThreshold;
        gameTime = 0.0f;
	}

	// Update is called once per frame
	void Update () {
        // For debugging
        if (Input.GetKeyDown("q"))
            gameOver = true;

        timer -= 0.01f;
        if(professorTextTimer > 0.00f){
            professorTextTimer -= 0.01f;
            if(professorTextTimer < 0.00f)
                hideProfessorText();
        }
        if (timer < 0){
            mission = generateMission();
            motionText.text = "Mission : " + mission;
        }

        if (gameOver)
        {
            if (nameInput1.enabled == false)
            {
                speaker.Pause();
                speaker.clip = gameOverSound;
                speaker.PlayOneShot(gameOverSound);
            }
            score1Text.text = ":(";
            score2Text.text = ":(";

            gameOverText.text = ">>> 엫힝 끝남 <<<";
            gameOverText.enabled = true;

            // TODO: get new user
            panel.SetActive(true);
            finalScore1Text.enabled = true;
            finalScore2Text.enabled = true;
            finalScore1Text.text = players[1].getScore().ToString();
            finalScore2Text.text = players[2].getScore().ToString();
            nameInput1.enabled = true;
            nameInput2.enabled = true;
        }
        else
        {
            time += Time.deltaTime;
            gameTime += Time.deltaTime;
            if (gameTime > timeThreshold)
                gameOver = true;

            // general mode
            score1Text.text = "Score: " + players[1].getScore();
            score2Text.text = "Score: " + players[2].getScore();

            motionText.text = "Mission : " + mission;
            // professor turned around

            if (Input.anyKeyDown)
            {
                // Compare key input to players' keymap.
                int playerID;
                bool clapped = false;
                for (playerID = 1; playerID <= playerNum; playerID++)
                {
                    bool keyFound = false;
                    foreach (string key in players[playerID].keyMap.Keys)
                    {
                        if (Input.GetKeyDown(key))
                        {
                            keyFound = true;
                            // Player clapped.
                            if (players[playerID].keyMap[key].Equals("0"))
                            {
                                clapped = true;
                                // Do not add clap into player's motion list.
                                break;
                            }
                            // Add pressed key to corresponding player's motion list.
                            players[playerID].addMotion(players[playerID].keyMap[key]);
                            break;
                        }
                    }
                    // key is in playerID's keymap.
                    if (keyFound)
                        break;
                }

                // key input is matched to a player.
                if (playerID <= playerNum)
                {
                    // Spotted by professor. Change game mode.
                    if (ProfessorController.isTurned)
                        OnSpotted(playerID);
                    else
                    {
                        if (clapped)
                        {
                            OnClap(playerID);
                        }
                        else
                        {
                            string motion = players[playerID].getMotion();
                            if (mission.Equals(motion))
                                OnCompletedMotion(playerID);
                            else if (mission.StartsWith(motion))
                                OnCorrectMotion(playerID);
                            else
                                OnWrongMotion(playerID);

                            motion1Text.text = players[1].getMotion();
                            motion2Text.text = players[2].getMotion();
                        }
                    }
                }
            }

            missionSlots = msc.GetCurrentMissionSlots();
        }
	}

    private void writeNewUser(string userId, string name, int score) {
        User user = new User(name, score);
        string json = JsonUtility.ToJson(user);

        mDatabaseRef.Child("users").Child(userId).SetRawJsonValueAsync(json);
    }

    public void InputCallBack() {
        string userName1 = nameInput1.text;
        string newId1 = mDatabaseRef.Child("users").Push().Key;
        writeNewUser(newId1, userName1, players[1].getScore());

        string userName2 = nameInput2.text;
        string newId2 = mDatabaseRef.Child("users").Push().Key;
        writeNewUser(newId2, userName2, players[2].getScore());
    }

    /* Generates new mission.
     * Motion length is randomly chosen in range 4~8.
     * Keys: 1 - up, 2 - left, 3 - down, 4 - right */
    public string generateMission()
    {
        string motion = null;
        int motionLength = Random.Range(missionMinLength, missionMaxLength + 1);
        for (int i = 0; i < motionLength; i++)
            motion += Random.Range(1, 5).ToString();

        msc.SpawnMissionSlot(motion);
        return motion;
    }

    public void OnCompletedMotion(int playerID)
    {
        Debug.Log("Player " + playerID.ToString() + " completed mission.");
        double multiplier = isPunishedMode ? 1.5 : 1;
        players[playerID].addScore((int)(msc.OnCorrectAnswer(mission) * multiplier));
        mission = generateMission();
        motionText.text = "Mission : " + mission;

        for (var i = 1; i <= playerNum; i++)
            players[i].clearMotion();
    }

    public void OnCorrectMotion(int playerID)
    {
        Debug.Log("Player " + playerID.ToString() + " correct motion.");
    }

    public void OnWrongMotion(int playerID)
    {
        Debug.Log("Player " + playerID.ToString() + " wrong motion.");

        // Clear player's motion.
        players[playerID].clearMotion();
    }

    public void OnSpotted(int playerID)
    {
        // Spotted twice. Game over.
        if (isPunishedMode)
        {
            players[playerID].setDead();
            showProfessorText(professorGetOutText);
            gameOver = true;
        }
        else
        {
            showProfessorText(professorWarnText);
            SetPunishMode();
        }
    }

    public void OnClap (int playerID)
    {
        if (players[playerID].getClaps() <= 0)
            return;
        playerSpeaker.clip = clapSound;
        AudioSource.PlayOneShot(clapSound);
        players[playerID].useClap();
        turnTrigger = true;
    }


    public void SetPunishMode()
    {
        Debug.Log("now punished mode\n");
        isPunishedMode = true;
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
        if(professorGetOutText.enabled)
            gameOver = true;

        professorAnnoyedText.enabled = false;
        professorWarnText.enabled = false;
        professorGetOutText.enabled = false;
    }
}