using UnityEngine;
using UnityEngine.UI;
using Firebase;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using TMPro;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class SaveAppSettings : MonoBehaviour

 {
    private TMP_InputField applicationIdInput;
    private TMP_InputField apiKeyInput;
    private TMP_InputField databaseUrlInput;
    private TMP_InputField storageBucketInput;
    private TMP_InputField projectIdInput;
    private List<TMP_InputField> inputFields = new List<TMP_InputField>();

    [HideInInspector]
    public TMP_InputField topicSubscribeInput;
    
    void Awake()
    {
        applicationIdInput = GameObject.Find("appId").GetComponent<TMP_InputField>();
        apiKeyInput = GameObject.Find("apiKey").GetComponent<TMP_InputField>();
        databaseUrlInput = GameObject.Find("databaseUrl").GetComponent<TMP_InputField>();
        storageBucketInput = GameObject.Find("storageBucket").GetComponent<TMP_InputField>();
        projectIdInput = GameObject.Find("projectId").GetComponent<TMP_InputField>();
        topicSubscribeInput = GameObject.Find("topicSubscribe").GetComponent<TMP_InputField>();
       
        inputFields.Add(applicationIdInput);
        inputFields.Add(apiKeyInput);
        inputFields.Add(databaseUrlInput);
        inputFields.Add(storageBucketInput);
        inputFields.Add(projectIdInput);
        inputFields.Add(topicSubscribeInput);
    }

    void Start()
    { 
        UpdateInputFields();
        LoadInputs();
        UpdateFirebaseManagerInputs();
    }


    public void SaveInputs()
    {
        foreach (TMP_InputField inputField in inputFields)
        {
            string inputText = inputField.text;
            PlayerPrefs.SetString(inputField.name, inputText);
            Debug.Log("Input Saved: " + inputText);
        }

        PlayerPrefs.Save();
        UpdateFirebaseManagerInputs();

    }

    private void LoadInputs()
{
    foreach (TMP_InputField inputField in inputFields)
    {
        string key = inputField.name;
        if (PlayerPrefs.HasKey(key))
        {
            string savedInput = PlayerPrefs.GetString(key);
            inputField.text = savedInput;
            Debug.Log("Input Loaded: " + savedInput);
        }
    }
}

    private void UpdateFirebaseManagerInputs()
    {
        FirebaseManager.Instance.appId = applicationIdInput.text;
        FirebaseManager.Instance.apiKey = apiKeyInput.text;
        FirebaseManager.Instance.databaseUrl = databaseUrlInput.text;
        FirebaseManager.Instance.storageBucket = storageBucketInput.text;
        FirebaseManager.Instance.projectId = projectIdInput.text;
    }

    public void UpdateInputFields()
    {
        applicationIdInput.text = FirebaseManager.Instance.appId;
        apiKeyInput.text = FirebaseManager.Instance.apiKey;
        databaseUrlInput.text = FirebaseManager.Instance.databaseUrl;
        storageBucketInput.text = FirebaseManager.Instance.storageBucket;
        projectIdInput.text = FirebaseManager.Instance.projectId;
    }
}
