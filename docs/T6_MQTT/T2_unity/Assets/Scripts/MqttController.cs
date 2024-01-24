using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using TMPro;
using UnityEngine.Events;
using UnityEngine;

public class MqttController : MonoBehaviour
{
    public string nameController = "Controller 1";
    public string topic;
    private MqttReceiver _eventSender;

    public TMP_InputField topicSubscribeInput;
    public TMP_InputField topicPublishInput;
    public TMP_InputField yourMessageSubscribed;
    public TMP_InputField yourMessagePublished;
    




    void Awake()
    {
        yourMessageSubscribed = GameObject.Find("receivedMessage").GetComponent<TMP_InputField>();
        yourMessagePublished = GameObject.Find("messagePublish").GetComponent<TMP_InputField>();
        topicSubscribeInput = GameObject.Find("topicSubscribe").GetComponent<TMP_InputField>();
        topicPublishInput = GameObject.Find("topicPublish").GetComponent<TMP_InputField>();
    }


  void Start()
  {
    _eventSender = GetComponent<MqttReceiver>();
    _eventSender.OnMessageArrived += OnMessageArrivedHandler;
    
  }

  public void OnMessageArrivedHandler(string msg)
  {
    Dictionary<string, string> resultDataDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(msg);
    Debug.Log("Event Fired. The message, from Object " + nameController +" is = " + msg);
    yourMessageSubscribed.text = resultDataDict["result"];    
    
  }

  
}

  


