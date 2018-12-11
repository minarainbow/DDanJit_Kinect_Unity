using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player {

    bool punished;
    bool dead;
    int score;
    int claps;
    public AudioSource playerSpeaker;
    public AudioClip motionSound;
    public AudioClip clapSound;

    string motions; // 1 = up 2 = down 3 = left 4 = right
    int motion_count;

    int type;
    public Dictionary<string, string> keyMap;

    public Player(int type) {
        this.dead = false;
        this.punished = false;
        this.score = 0;
        this.claps = 1; // 지금 현재는 1개만 주는 걸로.

        this.type = type;
        this.motion_count = 0;
        this.motions = null;
        this.keyMap = new Dictionary<string, string>();
    }

    public void addKeyMap(string[] keys)
    {
        int n = 0;
        foreach (var key in keys)
        {
            keyMap.Add(key, n.ToString());
            n++;
        }
    }

    public void addMotion(string motion) {
        motions += motion;
    }

    public string getMotion() {
        return motions;
    }

    public void clearMotion()
    {
        motions = null;
    }

    public bool isRightTotal(int[] answer) {
        return isRight(answer);
    }

    public bool isRight(int[] answer) {
        for (int i = 0; i < motion_count; i++) {
            if (motions[i] != answer[i]) {
                motion_count = 0;
                return false;
            }
        }
        return true;
    }

    public bool isPunished() {
        return punished;
    }

    public bool isDead() {
        return dead;
    }

    public void setPunished() {
        this.punished = true;
    }

    public void setDead() {
        this.dead = true;
    }

    public int getScore() {
        return score;
    }

    public int getClaps() {
        return claps;
    }

    public void useClap()
    {
        playerSpeaker.clip = clapSound;
        playerSpeaker.play();
        claps--;
    }

    public int getType() {
        return type;
    }

    public void addScore(int score) {
        this.score += score;
    }
}
