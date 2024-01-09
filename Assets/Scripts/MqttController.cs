using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using TMPro;
using UnityEngine.Events;


public class MqttController : MonoBehaviour
{
    public string nameController = "Controller 1";
    //public string tagOfTheMQTTReceiver="";
    private MqttReceiver _eventSender;

    public SaveAppSettings saveAppSettings;

  void Start()
  {
    _eventSender = GetComponent<MqttReceiver>();
    _eventSender.OnMessageArrived += OnMessageArrivedHandler;
  }

  public void OnMessageArrivedHandler(string newMsg)
  {
    Debug.Log("Event Fired. The message, from Object " + nameController +" is = " + newMsg);

    Dictionary<string, string> resultDataDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(newMsg);

    FirebaseManager.Instance.appId = resultDataDict["appId"];
    FirebaseManager.Instance.apiKey = resultDataDict["apiKey"];
    FirebaseManager.Instance.databaseUrl = resultDataDict["databaseUrl"];
    FirebaseManager.Instance.storageBucket = resultDataDict["storageBucket"];
    FirebaseManager.Instance.projectId = resultDataDict["projectId"];

    saveAppSettings.UpdateInputFields();

  }
  
}

  


