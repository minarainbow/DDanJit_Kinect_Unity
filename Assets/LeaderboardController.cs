using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System.Threading.Tasks;

public class LeaderboardController : MonoBehaviour {

    public Text[] textIndexArray;
    public Text[] textNameArray;
    public Text[] textScoreLabelArray;
    public Text[] textScoreArray;

    DatabaseReference mDatabaseRef;

    // Use this for initialization
    void Start () {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(
            "https://ddanjit-f2f5d.firebaseio.com/");

        mDatabaseRef = FirebaseDatabase.DefaultInstance
                                       .RootReference;

        for (int i = 0; i < 9; i++) {
            textIndexArray[i].enabled = false;
            textNameArray[i].enabled = false;
            textScoreLabelArray[i].enabled = false;
            textScoreArray[i].enabled = false;
        }
    }

    // Update is called once per frame
    void Update () {
        FirebaseDatabase.DefaultInstance.GetReference("users").OrderByChild("score").LimitToLast(9).GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted) {
                Debug.Log("Fault task");
            } else if (task.IsCompleted) {
                Firebase.Database.DataSnapshot snapshot = 
                    task.Result;
                int i = 0;
                foreach (var childSnapshot in snapshot.Children) {
                    textIndexArray[i].enabled = true;
                    textScoreLabelArray[i].enabled = true;
                    
                    // set values
                    Text nameText = textNameArray[i];
                    nameText.text = childSnapshot
                        .Child("username").Value.ToString();
                    nameText.enabled = true;
                    textNameArray[i] = nameText;

                    Text scoreText = textScoreArray[i];
                    scoreText.text = childSnapshot
                        .Child("score").Value.ToString();
                    scoreText.enabled = true;
                    textScoreArray[i] = scoreText;

                    i++;
                }
            }
        });
    }
}
