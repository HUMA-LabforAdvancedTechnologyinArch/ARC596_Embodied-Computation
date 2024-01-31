using System;
using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Firebase.Database;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;
using ApplicationInfo;
using JSON;
using Extentions;
using Dummiesman;
using TMPro;
using ApplicationModeControler;
using UnityEngine.Events;
using UnityEngine.Analytics;
using UnityEngine.InputSystem;
using System.Xml.Linq;


//scripts to initiate all geometries in the scene
namespace Instantiate
{
    public class InstantiateObjects : MonoBehaviour
    {

        //DATA STRUCTURE ITEMS
        public Dictionary<string, Node> DataItemDict;
        
        //OTHER Sript Objects
        public DatabaseManager databaseManager;
        public UIFunctionalities UIFunctionalities;

        //INPUT MATERIALS AND OBJECTS
        public Material BuiltMaterial;
        public Material UnbuiltMaterial;
        public Material HumanBuiltMaterial;
        public Material HumanUnbuiltMaterial;
        public Material RobotBuiltMaterial;
        public Material RobotUnbuiltMaterial;
        public Material LockedObjectMaterial;
        public Material SearchedObjectMaterial;

        //Parent Objects
        public GameObject QRMarkers; 
        public GameObject Elements;
        public GameObject UserObjects;

        //Events
        public delegate void InitialElementsPlaced(object source, EventArgs e);
        public event InitialElementsPlaced PlacedInitialElements;

        //Make Initial Visulization controler
        public ModeControler visulizationController = new ModeControler();

        //Private IN SCRIPT USE OBJECTS
        private GameObject geometry_object;
        private GameObject IdxImage;
        private GameObject SelectionArrow;
        private GameObject NewUserArrow;
        private GameObject ObjectLengthsTags;

        public struct Rotation
        {
            public Vector3 x;
            public Vector3 y;
            public Vector3 z;
        }

        //Private in script use objects
        private ARRaycastManager rayManager;

        public void Awake()
        {
            //Initilization Method for finding objects and materials
            OnAwakeInitilization();
        }
        
    /////////////////////////////// INSTANTIATE OBJECTS //////////////////////////////////////////
        private void OnAwakeInitilization()
        {
            //Find Additional Scripts.
            databaseManager = GameObject.Find("DatabaseManager").GetComponent<DatabaseManager>();
            UIFunctionalities = GameObject.Find("UIFunctionalities").GetComponent<UIFunctionalities>();

            //Find Parent Object to Store Our Items in.
            Elements = GameObject.Find("Elements");
            QRMarkers = GameObject.Find("QRMarkers");
            UserObjects = GameObject.Find("UserObjects");

            //Find Initial Materials
            BuiltMaterial = GameObject.Find("Materials").FindObject("Built").GetComponentInChildren<Renderer>().material;
            UnbuiltMaterial = GameObject.Find("Materials").FindObject("Unbuilt").GetComponentInChildren<Renderer>().material;
            HumanBuiltMaterial = GameObject.Find("Materials").FindObject("HumanBuilt").GetComponentInChildren<Renderer>().material;
            HumanUnbuiltMaterial = GameObject.Find("Materials").FindObject("HumanUnbuilt").GetComponentInChildren<Renderer>().material;
            RobotBuiltMaterial = GameObject.Find("Materials").FindObject("RobotBuilt").GetComponentInChildren<Renderer>().material;
            RobotUnbuiltMaterial = GameObject.Find("Materials").FindObject("RobotUnbuilt").GetComponentInChildren<Renderer>().material;
            LockedObjectMaterial = GameObject.Find("Materials").FindObject("LockedObjects").GetComponentInChildren<Renderer>().material;
            SearchedObjectMaterial = GameObject.Find("Materials").FindObject("SearchedObjects").GetComponentInChildren<Renderer>().material;
            
            //Find GameObjects fo internal use
            IdxImage = GameObject.Find("IdxTagsTemplates").FindObject("Circle");
            SelectionArrow = GameObject.Find("SelectionArrows").FindObject("Arrow");
            NewUserArrow = GameObject.Find("SelectionArrows").FindObject("NewUserArrow");
            ObjectLengthsTags = GameObject.Find("ObjectLengthsTags");

            //Set Initial Visulization Modes
            visulizationController.VisulizationMode = VisulizationMode.BuiltUnbuilt;
            visulizationController.TouchMode = TouchMode.None;
        }
        public void placeElements(List<Step> DataItems) 
        {
            int i = 0;
            foreach (Step step in DataItems)
                {
                    placeElement(i.ToString(), step);
                    i++;
                }

        }
        public void placeElement(string Key, Step step)
        {
            Debug.Log($"Placing Element: {step.data.element_ids[0]} from Step: {Key}");

            //get position
            Vector3 positionData = getPosition(step.data.location.point);
            
            //get rotation
            Rotation rotationData = getRotation(step.data.location.xaxis, step.data.location.yaxis);
            
            //Define Object Rotation
            Quaternion rotationQuaternion = FromRhinotoUnityRotation(rotationData, databaseManager.objectOrientation);

            //instantiate a geometry at this position and rotation
            GameObject geometry_object = gameobjectTypeSelector(step);

            if (geometry_object == null)
            {
                Debug.Log($"This key:{step.data.element_ids[0]} from Step: {Key} is null");
                return;
            }

            //Instantiate new gameObject from the existing selected gameobjects.
            GameObject elementPrefab = Instantiate(geometry_object, positionData, rotationQuaternion);
            
            // Destroy Initial gameobject that is made.
            if (geometry_object != null)
            {
                Destroy(geometry_object);
            }

            //Set parent and name
            elementPrefab.transform.SetParent(Elements.transform, false);
            
            //Name the object afte the step number... might be better to get the step_id in the building plan from Chen.
            elementPrefab.name = Key;

            //Get the nested gameobject from the .Obj so we can adapt colors only the first object
            GameObject geometryObject = elementPrefab.FindObject(step.data.element_ids[0] + " Geometry");

            // Create and attach text label to the GameObject
            CreateIndexTextForGameObject(elementPrefab, step.data.element_ids[0]);
            CreateCircleImageForTag(elementPrefab);

            //Case Switches to evaluate color and touch modes.
            ObjectColorandTouchEvaluater(visulizationController.VisulizationMode, visulizationController.TouchMode, step, geometryObject);
            
            //Check if the visulization tags mode is on
            if (UIFunctionalities.IDToggleObject.GetComponent<Toggle>().isOn)
            {
                //Set tag and Image visibility if the mode is on
                elementPrefab.FindObject(elementPrefab.name + " Text").gameObject.SetActive(true);
                elementPrefab.FindObject(elementPrefab.name + "IdxImage").gameObject.SetActive(true);
            }

            //If Priority Viewer toggle is on then color the add additional color based on priority: //TODO: IF I CHANGE PV then it checks text.
            if (UIFunctionalities.PriorityViewerToggleObject.GetComponent<Toggle>().isOn)
            {
                ColorObjectByPriority(databaseManager.CurrentPriority, step.data.priority.ToString(), Key, geometryObject);
            }

            //If the object is equal to the current step also color it human or robot and instantiate an arrow again.
            if (Key == UIFunctionalities.CurrentStep)
            {
                ColorHumanOrRobot(step.data.actor, step.data.is_built, geometryObject);
                ArrowInstantiator(elementPrefab, Key);
            }
        }
        public void placeElementAssembly(string Key, Node node)
        {
            Debug.Log($"Placing element {node.type_id}");

            //get position
            Vector3 positionData = getPosition(node.part.frame.point);
            
            //get rotation
            Rotation rotationData = getRotation(node.part.frame.xaxis, node.part.frame.yaxis);
            
            //Define Object Rotation
            Quaternion rotationQuaternion = FromRhinotoUnityRotation(rotationData, databaseManager.objectOrientation);

            //instantiate a geometry at this position and rotation
            GameObject geometry_object = gameobjectTypeSelectorAssembly(node);

            if (geometry_object == null)
            {
                Debug.Log($"This key is null {node.type_id}");
                return;
            }

            //Instantiate new gameObject from the existing selected gameobjects.
            GameObject elementPrefab = Instantiate(geometry_object, positionData, rotationQuaternion);
            
            // Destroy Initial gameobject that is made.
            if (geometry_object != null)
            {
                Destroy(geometry_object);
            }

            //Set parent and name
            elementPrefab.transform.SetParent(Elements.transform, false);
            
            //Name the object after the node number
            elementPrefab.name = node.type_id;

            //Get the nested Object from the .Obj so we can adapt colors only the first object
            GameObject child_object = elementPrefab.transform.GetChild(0).gameObject;

            //Color it Built or Unbuilt
            ColorBuiltOrUnbuilt(node.attributes.is_built, child_object);
        }
        public void placeElementsDict(Dictionary<string, Step> BuildingPlanDataDict)
        {
            if (BuildingPlanDataDict != null)
            {
                Debug.Log($"Number of key-value pairs in the dictionary = {BuildingPlanDataDict.Count}");
                
                //loop through the dictionary and print out the key
                foreach (KeyValuePair<string, Step> entry in BuildingPlanDataDict)
                {
                    if (entry.Value != null)
                    {
                        placeElement(entry.Key, entry.Value);
                    }

                }
                //Trigger event that all initial objects have been placed
                OnInitialObjectsPlaced();
            }
            else
            {
                Debug.LogWarning("The dictionary is null");
            }
        }   
        public GameObject gameobjectTypeSelector(Step step)
        {

            if (step == null)
            {
                Debug.LogWarning("Step is null. Cannot determine GameObject type.");
                return null;
            }

            GameObject element;

            switch (step.data.geometry)
                {
                    //TODO:REVIEW THE SIZE AND SCALE OF THESE
                    case "0.Cylinder":
                        //Create Empty gameObject to store the cylinder (Named by Step Number)
                        element = new GameObject();
                        element.transform.position = Vector3.zero;
                        element.transform.rotation = Quaternion.identity;
                        
                        //Define the Size of the Cylinder from the data values
                        float cylinderRadius = databaseManager.AssemblyDataDict[step.data.element_ids[0].ToString()].attributes.width;
                        float cylinderHeight = databaseManager.AssemblyDataDict[step.data.element_ids[0].ToString()].attributes.height;
                        Vector3 cylindersize = new Vector3(cylinderRadius*2, cylinderHeight, cylinderRadius*2);
                        
                        //Create, Scale, & name child object (Named by Assembly ID)
                        GameObject cylinderObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                        cylinderObject.transform.localScale = cylindersize;
                        cylinderObject.name = step.data.element_ids[0].ToString() + " Geometry";

                        //Add a collider to the gameobject
                        BoxCollider cylinderCollider = cylinderObject.AddComponent<BoxCollider>();
                        Vector3 cylinderSize = cylinderObject.GetComponent<MeshRenderer>().bounds.size;
                        Vector3 cylinderColliderSize = new Vector3(cylinderSize.x*1.1f, cylinderSize.y*1.2f, cylinderSize.z*1.2f);
                        cylinderCollider.size = cylinderColliderSize;

                        //Set the cylinder as a child of the empty gameObject
                        cylinderObject.transform.SetParent(element.transform);

                        break;

                    case "1.Box":                    
                        //Create Empty gameObject to store the cylinder (Named by step number)
                        element = new GameObject();
                        element.transform.position = Vector3.zero;
                        element.transform.rotation = Quaternion.identity;
                        
                        //Define the Size of the Cube from the data values
                        Vector3 cubesize = new Vector3(databaseManager.AssemblyDataDict[step.data.element_ids[0].ToString()].attributes.width, databaseManager.AssemblyDataDict[step.data.element_ids[0].ToString()].attributes.height, databaseManager.AssemblyDataDict[step.data.element_ids[0].ToString()].attributes.length);
                        
                        //Create, Scale, & name Box object (Named by Assembly ID)
                        GameObject boxObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        boxObject.transform.localScale = cubesize;
                        boxObject.name = step.data.element_ids[0].ToString() + " Geometry";

                        //Add a collider to the gameobject
                        BoxCollider boxCollider = boxObject.AddComponent<BoxCollider>();
                        Vector3 boxSize = boxObject.GetComponent<MeshRenderer>().bounds.size;
                        Vector3 boxColliderSize = new Vector3(boxSize.x*1.1f, boxSize.y*1.2f, boxSize.z*1.2f);
                        boxCollider.size = boxColliderSize;

                        //Set the cylinder as a child of the empty gameObject
                        boxObject.transform.SetParent(element.transform);

                        break;

                    case "2.ObjFile":

                        string basepath = Application.persistentDataPath;
                        string folderpath = Path.Combine(basepath, "Object_Storage");
                        string filepath = Path.Combine(folderpath, step.data.element_ids[0]+".obj");

                        if (File.Exists(filepath))
                        {
                            element =  new OBJLoader().Load(filepath);
                        }
                        else
                        {
                            element = null;
                            Debug.Log ("ObjPrefab is null");
                        }

                        //Change Objects Name to the name of the key from the assembly and Add collider
                        if (element!=null && element.transform.childCount > 0)
                        {
                            //Set name of the child to the Element ID name.
                            GameObject child_object = element.transform.GetChild(0).gameObject;
                            child_object.name = step.data.element_ids[0].ToString() + " Geometry";

                            //Add a collider to the object
                            BoxCollider collider = child_object.AddComponent<BoxCollider>();
                            Vector3 MeshSize = child_object.GetComponent<MeshRenderer>().bounds.size;
                            Vector3 colliderSize = new Vector3(MeshSize.x*1.1f, MeshSize.y*1.2f, MeshSize.z*1.2f);
                            collider.size = colliderSize;

                            Debug.Log($"Atempting to add colider to object {element.name}");

                        }
                        
                        break;

                    case "3.Mesh":
                        //TODO: CONFIRM FETCH OBJECT AS OBJ OR CREATE OBJECT FROM PROVIDED DATA.
                        element = null;
                        break;

                    default:
                        Debug.LogWarning($"No element type found for type {step.data.geometry}");
                        return null;
                }

                Debug.Log($"Element type {step.data.geometry}");
                return element;
            
        }
        public GameObject gameobjectTypeSelectorAssembly(Node node)
        {

            if (node == null)
            {
                Debug.LogWarning("Node is null. Cannot determine GameObject type.");
                return null;
            }

            GameObject element;

            switch (node.type_data)
                {
                    //TODO:REVIEW THE SIZE AND SCALE OF THESE 
                    case "0.Cylinder":
                        //Define the Size of the Cylinder from the data values
                        float cylinderRadius = node.attributes.width;
                        float cylinderHeight = node.attributes.height;
                        Vector3 cylindersize = new Vector3(cylinderRadius*2, cylinderHeight, cylinderRadius*2);
                        
                        //Create and Scale Element
                        element = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                        element.transform.localScale = cylindersize;
                        break;

                    case "1.Box":                    
                        //Define the Size of the Cube from the data values
                        Vector3 cubesize = new Vector3(node.attributes.width, node.attributes.height, node.attributes.length);
                        
                        //Create and Scale Element
                        element = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        element.transform.localScale = cubesize;
                        break;

                    case "2.ObjFile":

                        string basepath = Application.persistentDataPath;
                        string folderpath = Path.Combine(basepath, "Object_Storage");
                        string filepath = Path.Combine(folderpath, node.type_id+".obj");

                        if (File.Exists(filepath))
                        {
                            element =  new OBJLoader().Load(filepath);
                            
                        }
                        else
                        {
                            element = null;
                            Debug.Log ("ObjPrefab is null");
                        }
                        
                        break;

                    case "3.Mesh":
                        //TODO: CONFIRM FETCH OBJECT AS OBJ OR CREATE OBJECT FROM PROVIDED DATA.
                        element = null;
                        break;

                    default:
                        Debug.LogWarning($"No element type found for type node: {node.type_id} of type: {node.type_data}");
                        return null;
                }

                Debug.Log($"Element: {node.type_id} type: {node.type_data}");
                return element;
            
        }
        private void CreateIndexTextForGameObject(GameObject gameObject, string assemblyID)
        {
            // Create a new GameObject for the text
            GameObject IndexTextContainer = new GameObject(gameObject.name + " Text");
            TextMeshPro IndexText = IndexTextContainer.AddComponent<TextMeshPro>();
            IndexText.text = assemblyID;
            IndexText.fontSize = 1f;
            IndexText.alignment = TextAlignmentOptions.Center;

            // Calculate the center of the GameObject
            GameObject childobject = gameObject.FindObject(assemblyID + " Geometry");
            Renderer renderer = childobject.GetComponent<Renderer>();
            if (renderer == null)
            {
                Debug.Log("Renderer not found in the parent object.");
            }
            Vector3 center = Vector3.zero;
            center = renderer.bounds.center;

            // Offset the position slightly above the GameObject
            float verticalOffset = 0.13f;
            Vector3 textPosition = new Vector3(center.x, center.y + verticalOffset, center.z);
            IndexTextContainer.transform.position = textPosition;
            IndexTextContainer.transform.rotation = Quaternion.identity;
            IndexTextContainer.transform.SetParent(gameObject.transform);

            // Add billboard effect(object rotating with camera)
            GameObjectExtensions.Billboard billboard = IndexTextContainer.AddComponent<GameObjectExtensions.Billboard>();

            // Initially set the text as inactive
            IndexTextContainer.SetActive(false);

        }
        private void CreateCircleImageForTag(GameObject parentObject)
        {            
            if (IdxImage == null)
            {
                Debug.LogError("CircleImage template is not found or not assigned.");
                return;
            }

            //Find the element ID from the step associated with this geometry
            string elementID = databaseManager.BuildingPlanDataItem.steps[parentObject.name].data.element_ids[0];

            // Find the center of the parent object's renderer
            Renderer renderer = parentObject.FindObject(elementID + " Geometry").GetComponentInChildren<Renderer>();
            if (renderer == null)
            {
                Debug.LogError("Renderer not found in the parent object.");
                return;
            }

            Vector3 centerPosition = renderer.bounds.center;

            // Define the vertical offset 
            float verticalOffset = 0.13f;
            Vector3 offsetPosition = new Vector3(centerPosition.x, centerPosition.y + verticalOffset, centerPosition.z);

            // Instantiate the image object at the offset position
            GameObject circleImage = Instantiate(IdxImage, offsetPosition, Quaternion.identity, parentObject.transform);

            //Set name and parent
            circleImage.transform.SetParent(parentObject.transform);
            circleImage.name = $"{parentObject.name}IdxImage";

            // Add billboard effect
            GameObjectExtensions.Billboard billboard = circleImage.AddComponent<GameObjectExtensions.Billboard>();

            //Set Initial Visivility to false
            circleImage.SetActive(false);
        }
        public void ArrowInstantiator(GameObject parentObject, string itemKey, bool newUserArrow = false)
        {            
            if (SelectionArrow == null)
            {
                Debug.LogError("Could Not find SelectionArrow.");
                return;
            }

            //Find the center of the Item key object
            GameObject itemObject = Elements.FindObject(itemKey);
            string elementID = databaseManager.BuildingPlanDataItem.steps[itemKey].data.element_ids[0];

            Renderer renderer = itemObject.FindObject(elementID + " Geometry").GetComponentInChildren<Renderer>();
            if (renderer == null)
            {
                Debug.LogError("Renderer not found in the parent object.");
                return;
            }

            Vector3 centerPosition = renderer.bounds.center;

            // Define the vertical offset 
            float verticalOffset = 0.13f;
            Vector3 offsetPosition = new Vector3(centerPosition.x, centerPosition.y + verticalOffset, centerPosition.z);

            //Define rotation for the gameObject.
            Rotation arrowRotation = getRotation(databaseManager.BuildingPlanDataItem.steps[itemKey].data.location.xaxis, databaseManager.BuildingPlanDataItem.steps[itemKey].data.location.yaxis); 
            Rotation rotationlh = rhToLh(arrowRotation.x , arrowRotation.y);
            Quaternion rotationQuaternion = GetQuaternion(rotationlh.y, rotationlh.z);

            //Set new arrow item
            GameObject newArrow = null;

            // Instantiate arrow at the offset position
            if (newUserArrow)
            {
                newArrow = Instantiate(NewUserArrow, offsetPosition, rotationQuaternion, parentObject.transform);
            }
            else
            {
                newArrow = Instantiate(SelectionArrow, offsetPosition, rotationQuaternion, parentObject.transform);
            }
            //Set name and parent
            newArrow.transform.SetParent(parentObject.transform);
            newArrow.name = $"{parentObject.name} Arrow";

            //Set Active
            newArrow.SetActive(true);
        }
        public void CreateNewUserObject(string UserInfoname, string itemKey)
        {
            GameObject userObject = new GameObject(UserInfoname);

            //Set parent
            userObject.transform.SetParent(UserObjects.transform);

            //Set position and rotation
            userObject.transform.position = Vector3.zero;
            userObject.transform.rotation = Quaternion.identity;

            //Instantiate Arrow
            ArrowInstantiator(userObject, itemKey, true);
        }
        public void CalculateandSetLengthPositions(string key)
        {
            //Find Gameobject Associated with that step
            GameObject element = Elements.FindObject(key);
            Step step = databaseManager.BuildingPlanDataItem.steps[key];

            //Find gameobject center
            Vector3 center = element.FindObject(step.data.element_ids[0] + " Geometry").GetComponent<Renderer>().bounds.center;

            //Find length from assembly dictionary
            float length = databaseManager.AssemblyDataDict[step.data.element_ids[0]].attributes.length;

            //Calculate position of P1 and P2 
            Vector3 P1Position = center + element.transform.right * (length / 2)* -1;
            Vector3 P2Position = center + element.transform.right * (length / 2);

            //Set Positions of P1 and P2
            ObjectLengthsTags.FindObject("P1Tag").transform.position = P1Position;
            ObjectLengthsTags.FindObject("P2Tag").transform.position = P2Position;

            //Check if the component has a billboard component and if it doesn't add it.
            if (ObjectLengthsTags.FindObject("P1Tag").GetComponent<GameObjectExtensions.Billboard>() == null)
            {
                ObjectLengthsTags.FindObject("P1Tag").AddComponent<GameObjectExtensions.Billboard>();
            }
            if (ObjectLengthsTags.FindObject("P2Tag").GetComponent<GameObjectExtensions.Billboard>() == null)
            {
                ObjectLengthsTags.FindObject("P2Tag").AddComponent<GameObjectExtensions.Billboard>();
            }

            //Adjust P1 and P2 to be the same xz position as the elements for distance calculation
            Vector3 ElementsPosition = Elements.transform.position;
            Vector3 P1Adjusted = new Vector3(ElementsPosition.x, P1Position.y, ElementsPosition.z);
            Vector3 P2Adjusted = new Vector3(ElementsPosition.x, P2Position.y, ElementsPosition.z);

            //Get distance between position of P1, P2 and position of elements
            float P1distance = Vector3.Distance(P1Adjusted, ElementsPosition);
            float P2distance = Vector3.Distance(P2Adjusted, ElementsPosition);

            //Update Distance Text
            UIFunctionalities.SetObjectLengthsText(P1distance, P2distance);
        }
    
    /////////////////////////////// POSITION AND ROTATION ////////////////////////////////////////
        public Quaternion FromRhinotoUnityRotation(Rotation rotation, bool objZ_up)
        {   
            //Set Unity Rotation
            Rotation rotationLh = rhToLh(rotation.x , rotation.y);

            Rotation Zrotation = ZRotation(rotationLh);

            Rotation ObjectRotation;

            if (objZ_up == true)
            {
                ObjectRotation = XRotation(Zrotation);
            }
            else
            {
                ObjectRotation = Zrotation;
            }

            //Rotate Instance
            Quaternion rotationQuaternion = GetQuaternion(ObjectRotation.y, ObjectRotation.z);

            return rotationQuaternion;
        } 
        public Quaternion FromUnityRotation(Rotation rotation)
        {   
            //Right hand to left hand conversion
            Rotation rotationLh = rhToLh(rotation.x , rotation.y);

            //Set Unity Rotation
            Quaternion rotationQuaternion = GetQuaternion(rotationLh.y, rotationLh.z);

            return rotationQuaternion;
        } 
        public Vector3 getPosition(float[] pointlist)
        {
            Vector3 position = new Vector3(pointlist[0], pointlist[2], pointlist[1]);
            return position;
        }
        public Rotation getRotation(float[] x_vecdata, float [] y_vecdata)
        {
            Vector3 x_vec_right = new Vector3(x_vecdata[0], x_vecdata[1], x_vecdata[2]);
            Vector3 y_vec_right  = new Vector3(y_vecdata[0], y_vecdata[1], y_vecdata[2]);
            
            Rotation rotationRH;
            
            rotationRH.x = x_vec_right;
            rotationRH.y = y_vec_right;
            //This is never used just needed to satisfy struct code structure.
            rotationRH.z = Vector3.zero;
            
            return rotationRH;
        } 
        public Rotation rhToLh(Vector3 x_vec_right, Vector3 y_vec_right)
        {        
            Vector3 x_vec = new Vector3(x_vec_right[0], x_vec_right[2], x_vec_right[1]);
            Vector3 z_vec = new Vector3(y_vec_right[0], y_vec_right[2], y_vec_right[1]);
            Vector3 y_vec = Vector3.Cross(z_vec, x_vec);

            Rotation rotationLh;
            rotationLh.x = x_vec;
            rotationLh.z = z_vec;
            rotationLh.y = y_vec;


            return rotationLh;
        } 
        public Quaternion GetQuaternion(Vector3 y_vec, Vector3 z_vec)
        {
            Quaternion rotation = Quaternion.LookRotation(z_vec, y_vec);
            return rotation;
        }

        //Methods for obj imort correction.
        public Rotation ZRotation(Rotation ObjectRotation)
        {
            //Deconstruct Rotation Struct into Vector3
            Vector3 x_vec = ObjectRotation.x;
            Vector3 z_vec = ObjectRotation.z;
            Vector3 y_vec = ObjectRotation.y;
            
            //FIRST ROTATE 180 DEGREES AROUND Z AXIS
            Quaternion z_rotation = Quaternion.AngleAxis(180, z_vec);
            x_vec = z_rotation * x_vec;
            y_vec = z_rotation * y_vec;
            z_vec = z_rotation * z_vec;

            //Reconstruct new rotation struct from manipulated vectors
            Rotation ZXrotation;
            ZXrotation.x = x_vec;
            ZXrotation.y = y_vec;
            ZXrotation.z = z_vec;

            return ZXrotation;
        }
        public Rotation XRotation(Rotation ObjectRotation)
        {
            //Deconstruct Rotation Struct into Vector3
            Vector3 x_vec = ObjectRotation.x;
            Vector3 z_vec = ObjectRotation.z;
            Vector3 y_vec = ObjectRotation.y;

            //THEN ROTATE 90 DEGREES AROUND X AXIS
            Quaternion rotation_x = Quaternion.AngleAxis(90f, x_vec);
            x_vec = rotation_x * x_vec;
            y_vec = rotation_x * y_vec;
            z_vec = rotation_x * z_vec;

            //Reconstruct new rotation struct from manipulated vectors
            Rotation ZXrotation;
            ZXrotation.x = x_vec;
            ZXrotation.y = y_vec;
            ZXrotation.z = z_vec;

            return ZXrotation;
        }

    /////////////////////////////// Material and colors ////////////////////////////////////////
        public void ObjectColorandTouchEvaluater(VisulizationMode visualizationMode, TouchMode touchMode, Step step, GameObject geometryObject)
        {
            //Set Color Based on Visulization Mode
            switch (visulizationController.VisulizationMode)
            {
                case VisulizationMode.BuiltUnbuilt:
                    ColorBuiltOrUnbuilt(step.data.is_built, geometryObject);
                    break;
                case VisulizationMode.ActorView:
                    ColorHumanOrRobot(step.data.actor, step.data.is_built, geometryObject);
                    break;
            }

            //Set Touch mode based on Touch Mode
            switch (visulizationController.TouchMode)
            {
                case TouchMode.None:
                    //Do nothing
                    break;
                case TouchMode.ElementEditSelection:
                    //Disable collider if the element edit selection is on and the element priority is not the same as my current element.
                    if (step.data.priority != databaseManager.BuildingPlanDataItem.steps[UIFunctionalities.CurrentStep].data.priority)
                    {    
                        geometryObject.GetComponent<Collider>().enabled = false;
                        geometryObject.GetComponent<Renderer>().material = LockedObjectMaterial;
                    }
                    break;
            }
        }
        public void ColorBuiltOrUnbuilt (bool built, GameObject gamobj)
        {
            //Get Object Renderer
            Renderer m_renderer= gamobj.GetComponentInChildren<MeshRenderer>();
            
            if (built)
            {          
                //Color For built Objects
                m_renderer.material = BuiltMaterial; 
            }
        
            else
            {
                //Color For Unbuilt Objects
                m_renderer.material = UnbuiltMaterial;
            }
        }
        public void ColorHumanOrRobot (string placed_by, bool Built, GameObject gamobj)
        {
            
            //Get Object Renderer
            Renderer m_renderer= gamobj.GetComponentInChildren<Renderer>();
            
            if (placed_by == "HUMAN")
            {
                if(Built)
                {
                    //Color For Built Human Objects
                    m_renderer.material = HumanBuiltMaterial;
                }
                else
                {
                    //Color For Unbuilt Human Objects
                    m_renderer.material = HumanUnbuiltMaterial; 
                }
            }
            else
            {
                if(Built)
                {
                    //Color For Built Robot Objects
                    m_renderer.material = RobotBuiltMaterial;
                }
                else
                {
                    //Color For Unbuilt Robot Objects
                    m_renderer.material = RobotUnbuiltMaterial;
                }
            }
        }
        public void ColorObjectByPriority(string SelectedPriority, string StepPriority,string Key, GameObject gamobj)
        {
            //Get Object Renderer
            Renderer m_renderer= gamobj.GetComponentInChildren<Renderer>();

            //If the steps priority is not the same as the selected priority then color it grey
            if (StepPriority != SelectedPriority)
            {
                Debug.Log($"Coloring object {gamobj.name} grey");

                //Create a new color for the object based on its current color, and add a greyscale blend factor
                Color objectAdjustedColor = AdjustColorByGreyscale(m_renderer.material.color, 0.45f);

                //Set the object to the new color
                m_renderer.material.color = objectAdjustedColor;
            }
            else
            {
                //Find the item in the dictionary
                Step step = databaseManager.BuildingPlanDataItem.steps[Key];
                string elementID = step.data.element_ids[0];

                //Color based on visulization mode
                ObjectColorandTouchEvaluater(visulizationController.VisulizationMode, visulizationController.TouchMode, step, gamobj.FindObject(elementID + " Geometry"));
            }
        }
        public void ApplyColorBasedOnBuildState()
        {
            if (databaseManager.BuildingPlanDataItem.steps != null)
            {
                foreach (KeyValuePair<string, Step> entry in databaseManager.BuildingPlanDataItem.steps)
                {
                    GameObject gameObject = GameObject.Find(entry.Key);

                    if (gameObject != null && gameObject.name != UIFunctionalities.CurrentStep)
                    {
                        ColorBuiltOrUnbuilt(entry.Value.data.is_built, gameObject.FindObject(entry.Value.data.element_ids[0]));

                        //Check if Priority Viewer is on and color based on priority also if it is.
                        if (UIFunctionalities.PriorityViewerToggleObject.GetComponent<Toggle>().isOn)
                        {
                            //Color based on Priority
                            ColorObjectByPriority(databaseManager.CurrentPriority, entry.Value.data.priority.ToString(), entry.Key, gameObject.FindObject(entry.Value.data.element_ids[0]));
                        }
                    }
                }
            }
        }
        public void ApplyColorBasedOnActor()
        {
            if (databaseManager.BuildingPlanDataItem.steps != null)
            {
                foreach (var entry in databaseManager.BuildingPlanDataItem.steps)
                {
                    GameObject gameObject = GameObject.Find(entry.Key);
                    
                    if (gameObject != null && gameObject.name != UIFunctionalities.CurrentStep)
                    {
                        ColorHumanOrRobot(entry.Value.data.actor, entry.Value.data.is_built, gameObject.FindObject(entry.Value.data.element_ids[0]));

                        //Check if Priority Viewer is on and color based on priority if it is.
                        if (UIFunctionalities.PriorityViewerToggleObject.GetComponent<Toggle>().isOn)
                        {
                            //Color based on priority
                            ColorObjectByPriority(databaseManager.CurrentPriority, entry.Value.data.priority.ToString(), entry.Key, gameObject.FindObject(entry.Value.data.element_ids[0]));
                        }
                    }
                }
            }
        }
        public void ApplyColorBasedOnPriority(string SelectedPriority)
        {
            Debug.Log($"Applying color based on priority: {SelectedPriority}.");
            if (databaseManager.BuildingPlanDataItem.steps != null)
            {
                foreach (var entry in databaseManager.BuildingPlanDataItem.steps)
                {
                    GameObject gameObject = GameObject.Find(entry.Key);
                    
                    //If the objects are not null color by priority function.
                    if (gameObject != null && entry.Key != UIFunctionalities.CurrentStep)
                    {
                        //Color based on priority
                        ColorObjectByPriority(SelectedPriority, entry.Value.data.priority.ToString(), entry.Key, gameObject.FindObject(entry.Value.data.element_ids[0]));
                    }
                    else
                    {
                        Debug.LogWarning($"Could not find object with key: {entry.Key}");
                    }
                }
            }
        }
        public Color AdjustColorByGreyscale(Color originalColor, float factor)
        {
            //If Color is not white (Built and Unbuilt Colors)
            if (originalColor.r != 1.000f && originalColor.g != 1.000f && originalColor.b != 1.000f)
            {
                // Factor should be between 0 and 1, where 0 is unchanged and 1 is fully gray
                factor = Mathf.Clamp01(factor);

                // Convert the original color to grayscale
                float grayscaleValue = originalColor.r * 0.3f + originalColor.g * 0.59f + originalColor.b * 0.11f;

                // Blend the grayscale color with the original color based on the factor
                float blendedR = originalColor.r * (1 - factor) + grayscaleValue * factor;
                float blendedG = originalColor.g * (1 - factor) + grayscaleValue * factor;
                float blendedB = originalColor.b * (1 - factor) + grayscaleValue * factor;

                // Ensure color values are in the valid range (0 to 1)
                blendedR = Mathf.Clamp01(blendedR);
                blendedG = Mathf.Clamp01(blendedG);
                blendedB = Mathf.Clamp01(blendedB);

                // Create and return the new color
                Color newColor = new Color(blendedR, blendedG, blendedB, originalColor.a);
                
                Debug.Log($"Original Color: {originalColor} New Color: {newColor}");
                
                return newColor;
            }
            else
            {
                Color newColor = new Color(originalColor.r * factor, originalColor.g * factor, originalColor.b * factor, originalColor.a);
                return newColor;
            }
        }
    
    /////////////////////////////// EVENT HANDLING ////////////////////////////////////////
        public void OnDatabaseInitializedDict(object source, BuildingPlanDataDictEventArgs e)
        {
            Debug.Log("Database is loaded." + " " + "Number of nodes stored as a dict= " + e.BuildingPlanDataItem.steps.Count);
            placeElementsDict(e.BuildingPlanDataItem.steps);
        }
        public void OnDatabaseUpdate(object source, UpdateDataItemsDictEventArgs eventArgs)
        {
            Debug.Log("Database is loaded." + " " + "Key of node updated= " + eventArgs.Key);
            if (eventArgs.NewValue == null)
            {
                Debug.Log("Object will be removed");
                RemoveObjects(eventArgs.Key);
            }
            else
            {
                Debug.Log("Object will be instantiated");
                InstantiateChangedKeys(eventArgs.NewValue, eventArgs.Key);
            }

        }
        public void OnUserInfoUpdate(object source, UserInfoDataItemsDictEventArgs eventArgs)
        {
            Debug.Log("User Info is loaded." + " " + "Key of node updated= " + eventArgs.Key);
            if (eventArgs.UserInfo == null)
            {
                Debug.Log($"user {eventArgs.Key} will be removed");
                RemoveObjects(eventArgs.Key);
            }
            else
            {
                if (GameObject.Find(eventArgs.Key) != null)
                {
                    //Remove existing Arrow
                    RemoveObjects(eventArgs.Key + " Arrow");

                    //Instantiate new Arrow
                    ArrowInstantiator(GameObject.Find(eventArgs.Key), eventArgs.UserInfo.currentStep, true);
                }
                else
                {
                    Debug.Log($"Creating a new user object for {eventArgs.Key}");
                    CreateNewUserObject(eventArgs.Key, eventArgs.UserInfo.currentStep);
                }
            }
        }
        private void InstantiateChangedKeys(Step newValue, string key)
        {
            if (GameObject.Find(key) != null)
            {
                Debug.Log("Deleting old object with key" + key);
                GameObject oldObject = GameObject.Find(key);
                Destroy(oldObject);
            }
            else
            {
                Debug.Log( $"Could Not find Object with key: {key}");
            }
            placeElement(key, newValue);
        }
        public void RemoveObjects(string key)
        {
            //Delete old object if it already exists
            if (GameObject.Find(key) != null)
            {
                Debug.Log("Deleting old object");
                GameObject oldObject = GameObject.Find(key);
                Destroy(oldObject);
            }
            else
            {
                Debug.Log( $"Could Not find Object with key: {key}");
            }
        }
        protected virtual void OnInitialObjectsPlaced()
        {
            PlacedInitialElements(this, EventArgs.Empty);

            //Find the first unbuilt element in the database
            databaseManager.FindInitialElement();
        }
    }
}
