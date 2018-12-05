using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player {

    bool punished;
    bool dead;
    int score;
    int claps;

    int[] motions; // 1 = up 2 = down 3 = left 4 = right
    int motion_count;

    int type;

    public Player(int type) {
        this.dead = false;
        this.punished = false;
        this.score = 0;
        this.claps = 1; // 지금 현재는 1개만 주는 걸로.

        this.type = type;
        this.motion_count = 0;
        this.motions = new int[10];
    }

    public void addMotion(int motion) {
        if (motion_count == 10)
        {
            motion_count = 0;
        }
        motions[motion_count] = motion;
        motion_count += 1;
    }

    public int[] getMotion() {
        return motions;
    }

    public string getMotionString() {
        string ret_str = "";
        for (int i = 0; i < motion_count; i++) {
            switch (motions[i])
            {
                case 1:
                    ret_str += "[U]";
                    break;
                case 2:
                    ret_str += "[D]";
                    break;
                case 3:
                    ret_str += "[L]";
                    break;
                case 4:
                    ret_str += "[R]";
                    break;
            }
        }
        //switch (motions[motion_count])
        //{
        //    case 1:
        //        ret_str += "[U]";
        //        break;
        //    case 2:
        //        ret_str += "[D]";
        //        break;
        //    case 3:
        //        ret_str += "[L]";
        //        break;
        //    case 4:
        //        ret_str += "[R]";
        //        break;
        //}

        return ret_str;
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

    public int getType() {
        return type;
    }

    public void addScore(int score) {
        this.score += score;
    }
}
