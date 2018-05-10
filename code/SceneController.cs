
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneController : MonoBehaviour, Observer
{
    private int level = 0;
    private bool first_round = true;
    // show the score
    public Text scoreText;
    // show the lose message
    public Text centerText;
    // record the current score
    private ScoreRecorder record;
    // ui controller
    private UIController UI;
    // the obeject factory patrols
    private ObjectFactory fac;
    private GameObject actor;

    // Initialize the patrols & actor
    private void LoadResources()
    {
        // the original position
        float[] posx = { -5, 7, -5, 5 };
        float[] posz = { -5, -7, 5, 5 };
        if(first_round == true)
        {
            // the actor
            actor = Instantiate(Resources.Load("prefabs/Ami"), new Vector3(2, 0, -2), Quaternion.Euler(new Vector3(0, 180, 0))) as GameObject;
            ObjectFactory fac = Singleton<ObjectFactory>.Instance;
            // the patrols
            for (int i = 0; i < posx.Length; i++)
            {
                GameObject patrol = fac.setObjectOnPos(new Vector3(posx[i], 0, posz[i]), Quaternion.Euler(new Vector3(0, 180, 0)));
                patrol.name = "Patrol" + (i + 1);
            }
        }
        // higher level
        else
        {
            level++;
            for (int i = level-1; i < level; i++)
            {
                GameObject patrol = fac.setObjectOnPos(new Vector3(posx[i], 0, posz[i]), Quaternion.Euler(new Vector3(0, 180, 0)));
                patrol.name = "Patrol" + (i + 1);
            }
        }
        first_round = false;
    }

    private void reset()
    {
        record = new ScoreRecorder();
        record.scoreText = scoreText;
        UI = new UIController();
        UI.centerText = centerText;
        fac = Singleton<ObjectFactory>.Instance;
        Publish publisher = Publisher.getInstance();
        publisher.add(this);
        LoadResources();
    }

    public void levelUp()
    {
        LoadResources();
    }

    void Start()
    {
        reset();
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(290, 20, 80, 30), "Level Up"))
        {
            levelUp();
        }
        if (GUI.Button(new Rect(200, 20, 80, 30), "Reset"))
        {
            UI.resetGame();
            Destroy(actor);
            actor = Instantiate(Resources.Load("prefabs/Ami"), new Vector3(2, 0, -2), Quaternion.Euler(new Vector3(0, 180, 0))) as GameObject;
            fac.clear_all();
            scoreText.GetComponent<Text>().text = "Score: 0";
            UI = new UIController();
            UI.centerText = centerText;
            fac = Singleton<ObjectFactory>.Instance;
            Publish publisher = Publisher.getInstance();
            publisher.add(this);
            record.resetScore();
            record.setActive();
        }
    }
    
    // if it receives the notification
    public void notified(ActorState state, int pos, GameObject actor)
    {
        if (state == ActorState.ENTER_AREA)
            record.addScore(1);
        else
        {
            UI.loseGame();
        }
    }
}
