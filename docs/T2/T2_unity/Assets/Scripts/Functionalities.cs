using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Firebase.Database;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;
using JSON;
using UnityEngine.Events;
using System.Linq;
using UnityEngine.InputSystem;

//find a child by a specific name
namespace Extentions
{
    public static class GameObjectExtensions
    { 
        public static GameObject FindObject(this GameObject parent, string name)
        {
            Transform[] trs= parent.GetComponentsInChildren<Transform>(true);
            foreach(Transform t in trs){
                if(t.name == name){
                    return t.gameObject;
                }
            }
            return null;
        }

        public static float Remap(float from, float fromMin, float fromMax, float toMin,  float toMax)
        {
            var fromAbs  =  from - fromMin;
            var fromMaxAbs = fromMax - fromMin;      
        
            var normal = fromAbs / fromMaxAbs;

            var toMaxAbs = toMax - toMin;
            var toAbs = toMaxAbs * normal;

            var to = toAbs + toMin;
        
            return to;
        }

        public static bool IsPointerOverUIObject(Vector2 touchPosition)
        {
            //checking if we are touching a button
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = touchPosition;
            List<RaycastResult> raycastResults = new List<RaycastResult>();

            EventSystem.current.RaycastAll(pointerEventData, raycastResults);

            if (raycastResults.Count > 0)
            {
                Debug.Log($"Touch: Pointer over a UI element > 0 {raycastResults.Count}");
            }
            else
            {
                Debug.Log($"Touch: Pointer over a UI element NOT > 0 {raycastResults.Count}");
            }

            return raycastResults.Count > 0;
        }

        public static void PrintStepDataTypes(Step step, string key)
        {
            Debug.Log($"Value Type: {step.GetType()}");
            Debug.Log($"Value types {key} " +
                        "Data Type: " + step.data.GetType() +
                        "dtype Type: " + step.dtype.GetType() +
                        "guid Type: " + step.guid.GetType() + 
                        "Device_id Type: " + step.data.device_id.GetType() +
                        "Element_ids Type: " +step.data.element_ids[0].GetType() +
                        "Actor Type: " + step.data.actor.GetType() +
                        "Location Type: " + step.data.location.GetType() +
                        "Geometry Type: " +step.data.geometry.GetType() +
                        "Instructions Type: " + step.data.instructions.GetType() +
                        "Is_Bulit type: " + step.data.is_built.GetType() +
                        "Is_Planned type: " + step.data.is_planned.GetType() +
                        "Elements_held Type: " + step.data.elements_held[0].GetType() + 
                        step.data.priority.GetType());
            
            Debug.Log("Nested Value Types for " + key + " " + step.GetType() + 
                        "Point Types: " + step.data.location.point.GetType() + " " + step.data.location.point[1].GetType() + " " + step.data.location.point[2].GetType() +
                        "xaxis Types: " + step.data.location.xaxis[0].GetType() + " " + step.data.location.xaxis[1].GetType() + " " + step.data.location.xaxis[2].GetType() +
                        "yaxis Types: " + step.data.location.yaxis[0].GetType() + " " + step.data.location.yaxis[1].GetType() + " " + step.data.location.yaxis[2].GetType());

        }

        public static void PrintTypesJsonStep(string key, object jsonStep)
        {
            Dictionary<string, object> jsonDataDict = jsonStep as Dictionary<string, object>;
            
            //Access nested information
            Dictionary<string, object> dataDict = jsonDataDict["data"] as Dictionary<string, object>;
            Dictionary<string, object> locationDataDict = dataDict["location"] as Dictionary<string, object>;
            
            Debug.Log(  "Value Types for " + key + " " + jsonStep.GetType() +
                        "Actor Type: " + dataDict["actor"].GetType() + 
                        "Geometry Type: " + dataDict["geometry"].GetType() +
                        "Is_Bulit type: " + dataDict["is_built"].GetType() + 
                        "Is_planned" + dataDict["is_planned"].GetType() + 
                        "Priority Type:" + dataDict["priority"].GetType() +
                        "Device_id Type: " + dataDict["device_id"].GetType() + 
                        "Element_ids Type: " + dataDict["element_ids"].GetType() + 
                        "Instructions Type: " + dataDict["instructions"].GetType() +
                        "Elements_held Type: " + dataDict["elements_held"].GetType() +
                        "Location Type: " + dataDict["location"].GetType());

            //Nested value types
            List<object> pointslist = locationDataDict["point"] as List<object>;
            List<object> xaxislist = locationDataDict["xaxis"] as List<object>;
            List<object> yaxislist = locationDataDict["yaxis"] as List<object>;
            List<object> element_ids = dataDict["element_ids"] as List<object>;
            List<object> instructions = dataDict["instructions"] as List<object>;
            List<object> elements_held = dataDict["elements_held"] as List<object>;
            
            if (pointslist != null &&
                xaxislist != null &&
                yaxislist != null &&
                element_ids != null &&
                instructions != null &&
                elements_held != null)
            {
                Debug.Log("Nested Value Types for " + key + " " + jsonStep.GetType() + 
                            "Point Types: " + pointslist[0].GetType() + " " + pointslist[1].GetType() + " " + pointslist[2].GetType() +
                            "xaxis Types: " + xaxislist[0].GetType() + " " + xaxislist[1].GetType() + " " + xaxislist[2].GetType() +
                            "yaxis Types: " + yaxislist[0].GetType() + " " + yaxislist[1].GetType() + " " + yaxislist[2].GetType() +
                            "Element_ids Types: " + element_ids[0].GetType() +
                            "Instructions Types: " + instructions[0].GetType() +
                            "Elements_held Types: " + elements_held[0].GetType());
            }            

        }
    }   

}

