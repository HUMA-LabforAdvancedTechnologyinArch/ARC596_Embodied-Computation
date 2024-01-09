    // Create a class structure that matches the JSON data
// using System;
// using System.Collections;
using System.Collections.Generic;
// using UnityEngine;

namespace JSON
{   
    
   /////////////Classes for Assembly Desearialization./////////////// 
    [System.Serializable]
    public class Node
    {
        public Part part { get; set; }
        public string type_data { get; set; }
        public string type_id { get; set; }
        public Attributes attributes { get; set; }
    }

    [System.Serializable]
    public class Part
    {
        public Frame frame { get; set; }
    }

    [System.Serializable]
    public class Attributes
    {
        // public string device_id { get; set; }
        public bool is_built { get; set;}
        public bool is_planned { get; set;}
        public string placed_by { get; set; }
        public float length { get; set; }
        public float width { get; set; }
        public float height { get; set; }
        public string type { get; set; }
    } 

    [System.Serializable]
    public class Frame
    {
        public float[] point { get; set; }
        public float[] xaxis { get; set; }
        public float[] yaxis { get; set; }
    }

    /////////////// Classes For Step Desearialization///////////////////
    
    [System.Serializable]
    public class BuildingPlanData
    {
        public string LastBuiltIndex { get; set; }
        public Dictionary<string, Step> steps { get; set; }
    }
    
    [System.Serializable]
    public class Step
    {
        public Data data { get; set; }
        public string dtype { get; set; }
        public string guid { get; set; }
    }

    [System.Serializable]
    public class Data
    {
        public string device_id { get; set; }
        public string[] element_ids { get; set; }
        public string actor { get; set; }
        public Frame location { get; set; }
        public string geometry { get; set; }
        public string[] instructions { get; set; }
        public bool is_built { get; set; }
        public bool is_planned { get; set; }
        public int[] elements_held { get; set; }
        public int priority { get; set; }
    }
    

    ///////////////Classes for QR Desearialization/////////////////////
    
    [System.Serializable]
    public class JSONQRData
    {
        public Dictionary<string, QRcode> frame { get; set; } 
    }
    
    [System.Serializable]
    public class QRcode
    {
        public string Key { get; set; }
        public float[] point;
        public float[] quaternion;
        public float[] xaxis;
        public float[] yaxis;

    }

    ////////////////Classes for User Current Informatoin/////////////////////
    
    [System.Serializable]
    public class UserCurrentInfo
    {
        public string currentStep { get; set; }
        public string timeStamp { get; set; }
        
    }
}