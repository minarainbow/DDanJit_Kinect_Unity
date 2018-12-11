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
    public static float timer;
    public AudioSource speaker;
    public AudioSource playerSpeaker;
    public AudioClip gameOverSound;
    public AudioClip clapSound;
    public AudioClip passSound;
    public AudioClip pewSound;
    public AudioClip yeahSound;
    public AudioClip punishedSound;
    public static float gameTime; // for total game time.
    public static float gameTotalThreshold; // get timeThreshold & send it to Clock class.
    public Slider angerBarSlider;
    public Button[] player1Motions;
    public Button[] player2Motions;
    public Text ClapNum1;
    public Text ClapNum2;

    public float timeThreshold = 50;

    // for multiplayer
    public Text score1Text;
    public Text score2Text;
    public Text motionText;
    public Text motion1Text;
    public Text motion2Text;

    int playerNum = 2;
    public static Player[] players;

    public static Mission mission;
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
    Dictionary<string, Button>[] motions;


    DatabaseReference mDatabaseRef;

    // List<MissionSlot> missionSlots = new List<MissionSlot>(4);
    // MissionSlotController msc = new MissionSlotController(2);

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
        string[] player1Keys = { "left shift", "w", "a", "s", "d" };
        string[] player2Keys = { "right shift", "up", "left", "down", "right" };
        players[1].addKeyMap(player1Keys);
        players[2].addKeyMap(player2Keys);
        motions = new Dictionary<string, Button>[2];
        setMotions(player1Keys, player2Keys);

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
            ClapNum1.text = "Claps Left: " + players[1].getClaps();
            ClapNum2.text = "Claps Left: " + players[2].getClaps();

            // professor turned around

            if (Input.anyKeyDown)
            {
                setButtonNeutral();
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
                            Debug.Log("key: " + key);
                            Button pressed = 
                                motions[playerID - 1][key];
                            pressed.image.color = color0;
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
                    string motion = players[playerID].getMotion();
                    // Spotted by professor. Change game mode.
                    if (ProfessorController.isTurned)
                        OnSpotted(playerID);
                    else
                    {
                        if (clapped)
                        {
                            string key = new List<string>(motions[playerID - 1].Keys)[0];
                            print("key");
                            motions[playerID - 1][key].image.color = color0;
                            OnClap(playerID);
                        }
                        else
                        {
                            ProfessorController.stressGauge += 0.1f;

                            if (mission.motion.StartsWith(motion))
                                OnCorrectMotion(playerID);
                            else
                                OnWrongMotion(playerID);
                        }
                    }
                    if (mission.motion.Equals(motion))
                        OnCompletedMotion(playerID);

                    motion1Text.text = players[1].getMotion();
                    motion2Text.text = players[2].getMotion();
                }
            }

            angerBarSlider.value = ProfessorController.stressGauge;

            //missionSlots = msc.GetCurrentMissionSlots();
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
    public Mission generateMission()
    {
        string motion = null;
        int motionLength = Random.Range(missionMinLength, missionMaxLength + 1);

        bool hasClap = false;
        int clapLocation = -1;
        if (Random.Range(0, 1) < 0.1f)
        {
            hasClap = true;
            clapLocation = Random.Range(0, motionLength);

        }
        for (int i = 0; i < motionLength; i++)
        {
            motion += Random.Range(1, 5).ToString();
        }

        // msc.SpawnMissionSlot(motion);
        string missiontext = "Mission : ";
        Mission mission = new Mission(motion, hasClap, clapLocation);
        int prefixLength = missiontext.Length;
        missiontext += motion;
        if (hasClap)
        {
            char clapMotion = motion[clapLocation];
            missiontext = missiontext.Remove(clapLocation + prefixLength, 1).Insert(clapLocation + prefixLength, 
                "<color=red>" + clapMotion + "</color>");
        }
        motionText.text = missiontext;
        return mission;
    }

    public void OnCompletedMotion(int playerID)
    {
        playerSpeaker.clip = passSound;
        playerSpeaker.PlayOneShot(passSound);
        Debug.Log("Player " + playerID.ToString() + " completed mission.");
        double multiplier = isPunishedMode ? 1.5 : 1;
        players[playerID].addScore((int)(mission.score * multiplier));
        mission = generateMission();

        for (var i = 1; i <= playerNum; i++)
            players[i].clearMotion();
    }

    public void OnCorrectMotion(int playerID)
    {
        playerSpeaker.clip = yeahSound;
        playerSpeaker.PlayOneShot(yeahSound);
        Debug.Log("Player " + playerID.ToString() + " correct motion.");
        string motion = players[playerID].getMotion();
        if (mission.hasClap && motion.Length - 1 == mission.clapLocation)
        {
            players[playerID].addClap();
            mission.hasClap = false;
            mission.clapLocation = -1;
            motionText.text = motionText.text.Replace("<color=red>", "");
            motionText.text = motionText.text.Replace("</color>", "");
        }
    }

    public void OnWrongMotion(int playerID)
    {
        playerSpeaker.clip = pewSound;
        playerSpeaker.PlayOneShot(pewSound);
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
        playerSpeaker.PlayOneShot(clapSound);
        players[playerID].useClap();
        turnTrigger = true;
    }

    public void SetPunishMode()
    {
        playerSpeaker.clip = punishedSound;
        playerSpeaker.PlayOneShot(punishedSound);
        Debug.Log("now punished mode\n");
        isPunishedMode = true;
        showProfessorText(professorWarnText);
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

    void setMotions(string[] player1, string[] player2) {
        Dictionary<string, Button> playerMotion1 = 
            new Dictionary<string, Button>();

        Dictionary<string, Button> playerMotion2 =
            new Dictionary<string, Button>();
        for (int i = 0; i < 5; i++) {
            playerMotion1.Add(player1[i], player1Motions[i]);
        }

        for (int j = 0; j < 5; j++) {
            playerMotion2.Add(player2[j], player2Motions[j]);
        }

        motions[0] = playerMotion1;
        motions[1] = playerMotion2;
    }

    void setButtonNeutral() {
        List<string> key1 = 
            new List<string>(motions[0].Keys);
        List<string> key2 =
            new List<string>(motions[1].Keys);

        for (int i = 0; i < 5; i++) {
            motions[0][key1[i]].image.color = Color.white;
        }
        for (int i = 0; i < 5; i++)
        {
            motions[1][key2[i]].image.color = Color.white;
        }
    }
}
