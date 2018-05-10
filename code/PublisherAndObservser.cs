

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// the state of the actor(dead or alive)
public enum ActorState { ENTER_AREA, DEATH }

public interface Publish
{
    void notify(ActorState state, int pos, GameObject actor);
    void add(Observer observer);
    void delete(Observer observer);
}

// implemented in SceneController.cs
public interface Observer
{
    void notified(ActorState state, int pos, GameObject actor);
    // receiver
}

public class Publisher : Publish {

    private delegate void ActionUpdate(ActorState state, int pos, GameObject actor);
    private ActionUpdate updatelist;

    // instance
    private static Publish _instance;
    public static Publish getInstance()
    {
        if (_instance == null)
            _instance = new Publisher();
        return _instance;
    }

    public void notify(ActorState state, int pos, GameObject actor)
    {
        // publish the notification
        if (updatelist != null)
            updatelist(state, pos, actor);
    }

    public void add(Observer observer)
    {
        updatelist += observer.notified;
    }

    public void delete(Observer observer)
    {
        updatelist -= observer.notified;
    }
}
