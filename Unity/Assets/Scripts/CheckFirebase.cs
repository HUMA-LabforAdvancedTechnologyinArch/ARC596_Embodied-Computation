using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Firebase;
using Firebase.Extensions;
using Firebase.Database;

public class CheckFirebase : MonoBehaviour
{
    //1. Define a delegate
    public delegate void FirebaseInitializedEventHandler(object source, EventArgs args);

    //2. Define an event based on that delegate
    public event FirebaseInitializedEventHandler FirebaseInitialized;

    public void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
            {
                Debug.LogError(message: $"Failed to initialize Firebase with {task.Exception}");
                return;
            }

            OnFirebaseInitialized();
            Debug.Log("Invoked");
        });
    }

    protected virtual void OnFirebaseInitialized()
    {
        if(FirebaseInitialized != null)
        {
            FirebaseInitialized(this, EventArgs.Empty);
        }
    } 

}
