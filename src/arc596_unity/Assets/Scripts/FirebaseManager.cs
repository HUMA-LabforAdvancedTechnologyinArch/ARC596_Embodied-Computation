using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public sealed class FirebaseManager
{
    private static FirebaseManager instance = null;
    private static readonly object padlock = new object();

    public string appId;
    public string apiKey;
    public string databaseUrl;
    public string storageBucket;
    public string projectId;

    FirebaseManager() {
      appId = "1:116159730378:android:32163738e6393f7a0b5a33";
      apiKey = "AIzaSyBg2ES85_rL6Aeu76MXKsI4b6RYWW5V2hg";
      databaseUrl = "https://test-project-94f41-default-rtdb.europe-west1.firebasedatabase.app";
      storageBucket = "test-project-94f41.appspot.com";
      projectId = "test-project-94f41";
    }

    public static FirebaseManager Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new FirebaseManager();
                }
                return instance;
            }
        }
    }
}
