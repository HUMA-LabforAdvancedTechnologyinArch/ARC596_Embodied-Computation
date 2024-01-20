using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using TMPro;
using UnityEngine.Events;


public class MqttController : MonoBehaviour
{
    public string nameController = "Controller 1";
    public string topic;
    private MqttReceiver _eventSender;

    public TMP_InputField topicSubscribeInput;
    public TMP_InputField topicPublishInput;
    public TMP_InputField yourMessageSubscriced;
    public TMP_InputField yourMessagePublished;
    




    void Awake()
    {
        yourMessageSubscriced = GameObject.Find("receivedMessage").GetComponent<TMP_InputField>();
        yourMessagePublished = GameObject.Find("topicPublish").GetComponent<TMP_InputField>();
        topicSubscribeInput = GameObject.Find("topicSubscribe").GetComponent<TMP_InputField>();
        topicPublishInput = GameObject.Find("messagePublish").GetComponent<TMP_InputField>();
    }


  void Start()
  {
    _eventSender = GetComponent<MqttReceiver>();
    _eventSender.OnMessageArrived += OnMessageArrivedHandler;
    
  }

  public void OnMessageArrivedHandler(string newMsg)
  {
    Debug.Log("Event Fired. The message, from Object " + nameController +" is = " + newMsg);
    Debug.Log(newMsg);
    string topic = newMsg;
  }


  
}

  


