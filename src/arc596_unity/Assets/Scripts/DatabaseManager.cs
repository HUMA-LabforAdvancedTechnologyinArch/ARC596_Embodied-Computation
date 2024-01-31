using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using JSON;
using ApplicationInfo;
using Firebase;
using Firebase.Storage;
using Firebase.Auth;
using System.IO;
using UnityEngine.Networking;
using System.Linq;
using System.Linq.Expressions;
using Unity.VisualScripting;
using Instantiate;
using Google.MiniJSON;
using Extentions;
using UnityEngine.InputSystem;


public class BuildingPlanDataDictEventArgs : EventArgs
{
    public BuildingPlanData BuildingPlanDataItem { get; set; }
}

public class TrackingDataDictEventArgs : EventArgs
{
    public Dictionary<string, Node> QRCodeDataDict { get; set; }
}

public class UpdateDataItemsDictEventArgs : EventArgs
{
    public Step NewValue { get; set; }
    public string Key { get; set; }
}
public class UserInfoDataItemsDictEventArgs : EventArgs
{
    public UserCurrentInfo UserInfo { get; set; }
    public string Key { get; set; }
}

public class ApplicationSettingsEventArgs : EventArgs
{
    public ApplicationSettings Settings { get; set; }
}

public class UpdateDatabaseReferenceEventArgs: EventArgs
{
    public DatabaseReference Reference { get; set; }
}
    
public class DatabaseManager : MonoBehaviour
{
    // Firebase database references
    public DatabaseReference dbreference_assembly;
    public DatabaseReference dbreference_buildingplan;

    public DatabaseReference dbreference_steps;
    public DatabaseReference dbreference_LastBuiltIndex;

    public DatabaseReference dbreference_qrcodes;
    public DatabaseReference dbrefernece_usersCurrentSteps;
    public StorageReference storageReference;
    public DatabaseReference dbreference_project;


    // Data structures to store nodes and steps
    public Dictionary<string, Node> AssemblyDataDict { get; private set; } = new Dictionary<string, Node>();
    public BuildingPlanData BuildingPlanDataItem { get; private set; } = new BuildingPlanData();
    public Dictionary<string, Node> QRCodeDataDict { get; private set; } = new Dictionary<string, Node>();
    public Dictionary<string, UserCurrentInfo> UserCurrentStepDict { get; private set; } = new Dictionary<string, UserCurrentInfo>();
    public Dictionary<string, List<string>> PriorityTreeDict { get; private set; } = new Dictionary<string, List<string>>();

    //Data Structure to Store Application Settings
    public ApplicationSettings applicationSettings;


    // Define event delegates and events
    public delegate void StoreDataDictEventHandler(object source, BuildingPlanDataDictEventArgs e); 
    public event StoreDataDictEventHandler DatabaseInitializedDict;

    public delegate void TrackingDataDictEventHandler(object source, TrackingDataDictEventArgs e); 
    public event TrackingDataDictEventHandler TrackingDictReceived;

    public delegate void UpdateDataDictEventHandler(object source, UpdateDataItemsDictEventArgs e); 
    public event UpdateDataDictEventHandler DatabaseUpdate;

    public delegate void StoreApplicationSettings(object source, ApplicationSettingsEventArgs e);
    public event StoreApplicationSettings ApplicationSettingUpdate;
    
    public delegate void UpdateUserInfoEventHandler(object source, UserInfoDataItemsDictEventArgs e);
    public event UpdateUserInfoEventHandler UserInfoUpdate;


    //Define HTTP request response classes
    class ListFilesResponse
    {
        public List<object> prefix { get; set; }
        public List<FileMetadata> items { get; set; }
    }

    class FileMetadata
    {
        public string name { get; set; }
        public string bucket { get; set; }
    }

    //Other Scripts
    public UIFunctionalities UIFunctionalities;

    //In script use objects.
    public bool objectOrientation;
    public string TempDatabaseLastBuiltStep;

    public string CurrentPriority = null;

    void Awake()
    {
        OnAwakeInitilization();
    }

/////////////////////// FETCH AND PUSH DATA /////////////////////////////////
    private void OnAwakeInitilization()
    {
        //Set Persistence: Disables storing information on the device for when there is no internet connection.
        FirebaseDatabase.DefaultInstance.SetPersistenceEnabled(false);

        //Find UI Functionalities
        UIFunctionalities = GameObject.Find("UIFunctionalities").GetComponent<UIFunctionalities>();

    }
    public async void FetchSettingsData(DatabaseReference settings_reference)
    {
        await settings_reference.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error fetching data from Firebase");
                print("Error Fetching Settings Data");
                return;
            }

            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                DeserializeSettingsData(snapshot);
            }
        });

    }    
    public async void FetchData(object source, ApplicationSettingsEventArgs e)
    {
        //Create DB References
        dbreference_project = FirebaseDatabase.DefaultInstance.GetReference(e.Settings.parentname);
        dbreference_assembly = FirebaseDatabase.DefaultInstance.GetReference(e.Settings.parentname).Child("assembly").Child("graph").Child("node");
        dbreference_buildingplan = FirebaseDatabase.DefaultInstance.GetReference(e.Settings.parentname).Child("building_plan").Child("data");
        dbreference_steps = FirebaseDatabase.DefaultInstance.GetReference(e.Settings.parentname).Child("building_plan").Child("data").Child("steps");
        dbreference_LastBuiltIndex = FirebaseDatabase.DefaultInstance.GetReference(e.Settings.parentname).Child("building_plan").Child("data").Child("LastBuiltIndex");
        dbreference_qrcodes = FirebaseDatabase.DefaultInstance.GetReference(e.Settings.parentname).Child("QRFrames").Child("graph").Child("node");
        dbrefernece_usersCurrentSteps = FirebaseDatabase.DefaultInstance.GetReference(e.Settings.parentname).Child("UsersCurrenStep");

        //If there is nothing to download Storage=="None" then trigger Objects Secured event
        if (e.Settings.storagename == "None")
        {
            //Fetch QR Data no event trigger
            FetchRTDData(dbreference_qrcodes, snapshot => DeserializeDataSnapshot(snapshot, QRCodeDataDict), "TrackingDict");
            
            //Fetch Assembly Data no event trigger
            FetchRTDData(dbreference_assembly, snapshot => DeserializeDataSnapshot(snapshot, AssemblyDataDict));

            //Fetch Building plan data with event trigger
            FetchRTDData(dbreference_buildingplan, snapshot => DesearializeBuildingPlan(snapshot), "BuildingPlanDataDict");

        }
        
        //Else trigger download.
        else
        {
            //Set Obj Orientation bool
            objectOrientation = e.Settings.objorientation;
            
            //Storage Reference from data fetched
            storageReference = FirebaseStorage.DefaultInstance.GetReference("obj_storage").Child(e.Settings.storagename);
            string basepath = storageReference.Path;
            string path = basepath.Substring(1);
            Debug.Log($"Path for download on FB Storage: {path}");

            //Get a list of files from the storage location
            List<FileMetadata> files = await GetFilesInFolder(path);

            //Fetch Data from both storage and Realtime Database.
            FetchAllData(files);
        }
    }
    private async void FetchAllData(List<FileMetadata> files)
    {
        //Fetch Storage Data
        await FetchStorageData(files);

        //Fetch QR Data no event trigger
        FetchRTDData(dbreference_qrcodes, snapshot => DeserializeDataSnapshot(snapshot, QRCodeDataDict), "TrackingDict");
        
        //Fetch Assembly Data no event trigger
        FetchRTDData(dbreference_assembly, snapshot => DeserializeDataSnapshot(snapshot, AssemblyDataDict));
        
        //Fetch Building plan data with event trigger
        FetchRTDData(dbreference_buildingplan, snapshot => DesearializeBuildingPlan(snapshot), "BuildingPlanDataDict");
    }
    async Task<List<FileMetadata>> GetFilesInFolder(string path)
    {
        //Building the storage url dynamically
        string storageBucket = FirebaseManager.Instance.storageBucket;
        string baseUrl = $"https://firebasestorage.googleapis.com/v0/b/{storageBucket}/o?prefix={path}/&delimiter=/";

        // const string baseUrl = "https://firebasestorage.googleapis.com/v0/b/test-project-94f41.appspot.com/o?prefix=obj_storage/buildingplan_test/&delimiter=/"; //Hardcoded value for example
 
        Debug.Log($"BaseUrl: {baseUrl}");

        // Send a GET request to the URL
        using (HttpClient client = new HttpClient())
        using (HttpResponseMessage response = await client.GetAsync(baseUrl))
        using (HttpContent content = response.Content)
        {
            
            // Read the response body
            string responseText = await content.ReadAsStringAsync();
            Debug.Log($"HTTP Client Response: {responseText}");

            // Deserialize the JSON response
            ListFilesResponse responseData = JsonConvert.DeserializeObject<ListFilesResponse>(responseText);

            // Return the list of files
            return responseData.items;
        }
    }
    async Task FetchStorageData(List<FileMetadata> files) 
    {
        List<Task> downloadTasks = new List<Task>();
        
        foreach (FileMetadata file in files)
        {    
            string baseurl = file.name;

            //Construct FirebaseStorage Reference from 
            StorageReference FileReference = FirebaseStorage.DefaultInstance.GetReference(baseurl);
            string basepath = Application.persistentDataPath;
            string filename = Path.GetFileName(baseurl);
            string folderpath = Path.Combine(basepath, "Object_Storage");
            string savefilepath = Path.Combine(folderpath, filename);
                                
            // Replace backslashes with forward slashes
            savefilepath = savefilepath.Replace('\\', '/');

            //Run all async tasks and add them to a task list we can wait for all of them to complete.            
            downloadTasks.Add(FileReference.GetFileAsync(savefilepath).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    foreach (var exception in task.Exception.InnerExceptions)
                    {
                        Debug.LogError("Error fetching data from Firebase: " + exception.Message);
                    }
                    return;
                }

                if (task.IsCompleted)
                {
                    Debug.Log($"Downloaded file to path '{savefilepath}'");
                    CheckPathExistance(savefilepath);
                }
            }));
        }
        
        //Await all download tasks are done before refreshing.
        await Task.WhenAll(downloadTasks);
    }        
    public async Task FetchRTDData(DatabaseReference dbreference, Action<DataSnapshot> customAction, string eventname = null)
    {
        await dbreference.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error fetching data from Firebase");
                return;
            }

            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (customAction != null)
                {
                    customAction(snapshot);
                }
            }
        });

        if (eventname != null && eventname == "BuildingPlanDataDict")
        {
            OnDatabaseInitializedDict(BuildingPlanDataItem); 
        }

        if (eventname != null && eventname == "TrackingDict")
        {
            OnTrackingDataReceived(QRCodeDataDict);
        }
    }      
    public void PushAllDataBuildingPlan(string key)
    {        
        //Find step that I changed in the building plan and add my custom device id.
        Step specificstep = BuildingPlanDataItem.steps[key];
        specificstep.data.device_id = SystemInfo.deviceUniqueIdentifier;

        //Searilize the data for push to firebase
        string data = JsonConvert.SerializeObject(BuildingPlanDataItem);
        
        //Push the data to firebase
        dbreference_buildingplan.SetRawJsonValueAsync(data);
    }
    public void PushAllDataAssembly(string key)
    {
        //TODO: Add custom device_id to the assembly data structure.
        //Find step that I changed in the building plan and add my custom device id.
        // Node specificnode = AssemblyDataDict[key];
        // specificnode.attributes.device_id = SystemInfo.deviceUniqueIdentifier;

        //Searilize the data for push to firebase
        string data = JsonConvert.SerializeObject(AssemblyDataDict);
        
        //Push the data to firebase
        dbreference_buildingplan.SetRawJsonValueAsync(data);
    }
    public void PushStringData(DatabaseReference db_ref, string data)
    {
        db_ref.SetRawJsonValueAsync(data);
    }

/////////////////////////// DATA DESERIALIZATION ///////////////////////////////////////
    private void DeserializeSettingsData(DataSnapshot snapshot)
    {
        CleanObjectStorageFolder();
        string AppData = snapshot.GetRawJsonValue();

        if (!string.IsNullOrEmpty(AppData))
        {
            Debug.Log("Application Settings:" + AppData);
            applicationSettings = JsonConvert.DeserializeObject<ApplicationSettings>(AppData);
        }
        else
        {
            Debug.LogWarning("You did not set your settings data properly");
        }
    
        OnSettingsUpdate(applicationSettings);
    } 
    private void DeserializeDataSnapshot(DataSnapshot snapshot, Dictionary<string, Node> dataDict)
    {
        //Clear current Dictionary if it contains information
        dataDict.Clear();

        //Desearialize individual data items from the snapshots
        foreach (DataSnapshot childSnapshot in snapshot.Children)
        {
            string key = childSnapshot.Key;
            var json_data = childSnapshot.GetValue(true);
            Node node_data = NodeDeserializer(key, json_data);
            
            if (IsValidNode(node_data))
            {
                dataDict[key] = node_data;
                dataDict[key].type_id = key;
            }
            else
            {
                Debug.LogWarning($"Invalid Node structure for key '{key}'. Not added to the dictionary.");
            }
        }
        
        Debug.Log("Number of nodes stored as a dictionary = " + dataDict.Count);

    }
    private void DesearializeLastBuiltIndex(DataSnapshot snapshot)
    {  
        string jsondatastring = snapshot.GetRawJsonValue();
        Debug.Log("Last Bulit Index Data:" + jsondatastring);
        
        if (!string.IsNullOrEmpty(jsondatastring))
        {
            TempDatabaseLastBuiltStep = JsonConvert.DeserializeObject<string>(jsondatastring);
        }
        else
        {
            Debug.LogWarning("Last Built Index Did not produce a value");
            TempDatabaseLastBuiltStep = null;
        }
    }
    private void DesearializeBuildingPlan(DataSnapshot snapshot)
    {
        //Set Buliding plan to a null value
        if (BuildingPlanDataItem.steps != null && BuildingPlanDataItem.LastBuiltIndex != null)
        {
            BuildingPlanDataItem.LastBuiltIndex = null;
            BuildingPlanDataItem.steps.Clear();
        }

        var jsondata = snapshot.GetValue(true);

        BuildingPlanData buildingPlanData = BuildingPlanDeserializer(jsondata);

        if (buildingPlanData != null && buildingPlanData.steps != null)
        {
            BuildingPlanDataItem = buildingPlanData;
        }
        else
        {
            Debug.LogWarning("You did not set your building plan data properly");
        }
        
    }

/////////////////////////// INTERNAL DATA MANAGERS //////////////////////////////////////
    private void CleanObjectStorageFolder()
    {
        //Construct storage folder path
        string path = Application.persistentDataPath;
        string folderpath = Path.Combine(path, "Object_Storage");
        folderpath = folderpath.Replace('\\', '/');

        //If the folder exists delete all files in the folder. If not create the folder.
        if (Directory.Exists(folderpath))
        {
            System.IO.DirectoryInfo di = new DirectoryInfo(folderpath);
            foreach (FileInfo file in di.GetFiles())
            {
                Debug.Log("Deleted Files: " + file);
                file.Delete();
            }
        }
        else
        {
            Directory.CreateDirectory(folderpath);
            print($"Created Directory for Object Storage @ {folderpath}");
        }

    }
    private bool IsValidNode(Node node)
    {   
        // Basic validation: Check if the required properties are present or have valid values
        if (node != null &&
            !string.IsNullOrEmpty(node.type_id) &&
            !string.IsNullOrEmpty(node.type_data) &&
            node.part != null &&
            node.part.frame != null)
        {
            if (node.type_data == "4.Joint")
            {
                Debug.Log("This is a timbers Joint and should be ignored");
                return false;
            }
            else if (node.type_data != "3.Frame")
            {
                // Check if the required properties are present or have valid values
                if (node.attributes != null &&
                    node.attributes.length != null &&
                    node.attributes.width != null &&
                    node.attributes.height != null)
                {
                    // Set default values for properties that may be null
                    return true;
                }
                else
                {
                    // If it is not a frame assembly and does not have geometric description.
                    return false;
                }
            }
            else
            {
                // Set default values for properties that may be null
                return true;
            }
        }
        Debug.Log($"node.type_id is: '{node.type_id}'");
        return false;
    }
    private bool IsValidStep(Step step)
    {
        // Basic validation: Check if the required properties are present or have valid values
        if (step != null &&
            step.data.element_ids != null &&
            !string.IsNullOrEmpty(step.data.actor) &&
            step.data.location != null &&
            step.data.geometry != null &&
            step.data.instructions != null &&
            step.data.is_built != null &&
            step.data.is_planned != null &&
            step.data.elements_held != null &&
            step.data.priority != null)
        {
            // Set default values for properties that may be null
            return true;
        }
        Debug.Log($"node.key is: '{step.data.element_ids[0]}'");
        return false;
    }
    private bool AreEqualSteps(Step step ,Step NewStep)
    {
        // Basic validation: Check if two steps are equal
        if (step != null &&
            NewStep != null &&
            step.data.device_id == NewStep.data.device_id &&
            step.data.element_ids == step.data.element_ids &&
            step.data.actor == NewStep.data.actor &&
            step.data.location.point.SequenceEqual(NewStep.data.location.point) &&
            step.data.location.xaxis.SequenceEqual(NewStep.data.location.xaxis) &&
            step.data.location.yaxis.SequenceEqual(NewStep.data.location.yaxis) &&
            step.data.geometry == NewStep.data.geometry &&
            step.data.instructions.SequenceEqual(NewStep.data.instructions) &&
            step.data.is_built == NewStep.data.is_built &&
            step.data.is_planned == NewStep.data.is_planned &&
            step.data.elements_held.SequenceEqual(NewStep.data.elements_held) &&
            step.data.priority == NewStep.data.priority)
        {
            // Set default values for properties that may be null
            return true;
        }
        Debug.Log($"Steps with elementID : {step.data.element_ids[0]} and {NewStep.data.element_ids[0]} are not equal");
        return false;
    }
    public string print_out_data(DatabaseReference dbreference_assembly)
    {
        string jsondata = "";
        dbreference_assembly.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("error");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot dataSnapshot = task.Result;

                // Raw JSON string of everything inside node
                jsondata = dataSnapshot.GetRawJsonValue(); 
                Debug.Log("all nodes" + jsondata);
            }
        });
        return jsondata;
    }
    public void CheckPathExistance(string path)
    {       
        // Replace backslashes with forward slashes
        path = path.Replace('\\', '/');

        if (File.Exists(path))
        {
            Debug.Log($"File Exists @ {path}");
        }
        else
        {
            Debug.Log($"File does not exist @ {path}");
        }

    }
    public void FindInitialElement()
    {
        //ITERATE THROUGH THE BUILDING PLAN DATA DICT IN ORDER.
        for (int i =0 ; i < BuildingPlanDataItem.steps.Count; i++)
        {
            //Set data items
            Step step = BuildingPlanDataItem.steps[i.ToString()];

            //Find the first unbuilt element
            if(step.data.is_built == false)
            {
                //Set Current Priority as the priority of the first this current step. This needs to be done before setting the current step.
                UIFunctionalities.SetCurrentPriority(step.data.priority.ToString());

                //Set Current Element
                UIFunctionalities.SetCurrentStep(i.ToString());

                break;
            }
        }
    }
    public int OtherUserPriorityChecker(Step step, string stepKey)
    {        
        //If the priority of the current priority then check if it is complete.
        if (CurrentPriority == step.data.priority.ToString())
        {
            //New empty list to store unbuilt elements
            List<string> UnbuiltElements = new List<string>();
            
            //Find the current priority in the dictionary for iteration
            List<string> PriorityDataItem = PriorityTreeDict[CurrentPriority];

            //Iterate through the Priority tree dictionary to check the elements and if the priority is complete
            foreach(string element in PriorityDataItem)
            {
                //Find the step in the dictoinary
                Step stepToCheck = BuildingPlanDataItem.steps[element];

                //Check if the element is built
                if(!stepToCheck.data.is_built)
                {
                    UnbuiltElements.Add(element);
                }
            }

            //If the list is empty return 2 because all elements of that priority are built, and we want to move on to the next priority.
            if(UnbuiltElements.Count == 0)
            {
                Debug.Log($"OtherUser Priority Check: Current Priority is complete. Unlocking Next Priority.");
                
                //Return 2, to move on to the next priority.
                return 2;
            }
            
            //If the list is not empty return 0 so do nothing.
            else
            {
                Debug.Log($"OtherUser Priority Check: Current Priority is not complete. Incomplete Priority");
                
                //Return 0 to not push data.
                return 0;
            }
        }
        else
        {
            //If the priority is not the same as the current priority (meaning it is a lower priority) then we should set it.
            Debug.Log($"OtherUser Priority Check: Current Priority is not the same as the priority of the step. Set this priority.");
            
            //Return set this item to the current priority.
            return 1;
        }

    }

/////////////////////////////// Input Data Handlers //////////////////////////////////  
    //This also creates the priority tree dictionary, but this is temporary to limit searching.
    private BuildingPlanData BuildingPlanDeserializer(object jsondata)
    {
        Dictionary<string, object> jsonDataDict = jsondata as Dictionary<string, object>;
        
        //Create new building plan instance
        BuildingPlanData buidingPlanData = new BuildingPlanData();
        buidingPlanData.steps = new Dictionary<string, Step>();
        
        //Attempt to get last built index and if it doesn't exist set it to null
        if (jsonDataDict.TryGetValue("LastBuiltIndex", out object last_built_index))
        {
            Debug.Log($"Last Built Index Fetched From database: {last_built_index.ToString()}");
            buidingPlanData.LastBuiltIndex = last_built_index.ToString();
        }
        else
        {
            buidingPlanData.LastBuiltIndex = null;
        }

        //Try to access steps as dictionary... might need to be a list
        List<object> stepsList = jsonDataDict["steps"] as List<object>;

        //Loop through steps desearialize and check if they are valid
        for(int i =0 ; i < stepsList.Count; i++)
        {
            string key = i.ToString();
            var json_data = stepsList[i];

            //Create step instance from the information
            Step step_data = StepDeserializer(json_data);
            
            //Check if step is valid and add it to building plan dictionary
            if (IsValidStep(step_data))
            {
                //Add step to building plan dictionary
                buidingPlanData.steps[key] = step_data;
                Debug.Log($"Step {key} successfully added to the building plan dictionary");

                //Add step to priority tree dictionary
                if (PriorityTreeDict.ContainsKey(step_data.data.priority.ToString()))
                {
                    //If the priority already exists add the key to the list
                    PriorityTreeDict[step_data.data.priority.ToString()].Add(key);
                    Debug.Log($"Step {key} successfully added to the priority tree dictionary item {step_data.data.priority.ToString()}");
                }
                else
                {
                    //If not create a new list and add the key to the list
                    PriorityTreeDict[step_data.data.priority.ToString()] = new List<string>();
                    PriorityTreeDict[step_data.data.priority.ToString()].Add(key);
                    Debug.Log($"Step {key} added a new priority {step_data.data.priority.ToString()} to the priority tree dictionary");
                }
            }
            else
            {
                Debug.LogWarning($"Invalid Step structure for key '{key}'. Not added to the dictionary.");
            }
        }

        Debug.Log("THIS IS THE PRIORITY TREE DICTIONARY: " + JsonConvert.SerializeObject(PriorityTreeDict));
        return buidingPlanData;
    }
    public Node NodeDeserializer(string key, object jsondata, bool additionalAttributes = false)
    {
        //Generic Dictionary for deserialization     
        Dictionary<string, object> jsonDataDict = jsondata as Dictionary<string, object>;

        // Access nested values 
        Dictionary<string, object> partDict = jsonDataDict["part"] as Dictionary<string, object>;
        Dictionary<string, object> dataDict = partDict["data"] as Dictionary<string, object>;
        Dictionary<string, object> frameDataDict = dataDict["frame"] as Dictionary<string, object>;

        //Create class instances of node elements
        Node node = new Node();
        node.part = new Part();
        node.attributes = new Attributes();
        node.part.frame = new Frame();

        //Set node type_id
        node.type_id = key;

        //Get dtype from partDict
        string dtype = (string)partDict["dtype"];
        
        //Check dtype, and determine how they should be deserilized.
        if (dtype != "compas.datastructures/Part")
        {
            DtypeGeometryDesctiptionSelector(node, dtype, dataDict);
        }
        else
        {
            PartDesctiptionSelector(node, dataDict);
        }

        //Convert System.double items to float for use in instantiation
        List<object> pointslist = frameDataDict["point"] as List<object>;
        List<object> xaxislist = frameDataDict["xaxis"] as List<object>;
        List<object> yaxislist = frameDataDict["yaxis"] as List<object>;

        if (pointslist != null && xaxislist != null && yaxislist != null)
        {
            node.part.frame.point = pointslist.Select(Convert.ToSingle).ToArray();
            node.part.frame.xaxis = xaxislist.Select(Convert.ToSingle).ToArray();
            node.part.frame.yaxis = yaxislist.Select(Convert.ToSingle).ToArray();
        }
        else
        {
            Debug.Log("One of the Frame lists is null");
        }

        //If additional Attributes bool then get additional attributes
        if (additionalAttributes)
        {
            GetAdditionalNodeAttributes(node, jsonDataDict);
        }

        return node;
    }
    private void GetAdditionalNodeAttributes(Node node, Dictionary<string, object> jsonDataDict)
    {
        //Set Attributes Class Values for extra assembly processes
        node.attributes.is_built = (bool)jsonDataDict["is_built"];
        node.attributes.is_planned =  (bool)jsonDataDict["is_planned"];
        node.attributes.placed_by = (string)jsonDataDict["placed_by"];
       
    }
    private void DtypeGeometryDesctiptionSelector(Node node, string dtype, Dictionary<string, object> jsonDataDict)
    {
        //Set node part dtype
        node.part.dtype = dtype;

        switch (dtype)
        {
            case "compas.geometry/Cylinder":
                
                //Set node type_data (THIS IS FOR INTERNAL USE... IN any scenario where I am pushing data it should contain this information)
                node.type_data = "0.Cylinder";
                
                // Accessing different parts of json data to make common attributes dictionary
                float height = Convert.ToSingle(jsonDataDict["height"]);
                float radius = Convert.ToSingle(jsonDataDict["radius"]);

                //Add Items to the attributes dictionary remapping name to length, width, height
                node.attributes.length = radius;
                node.attributes.width = radius;
                node.attributes.height = height;
                break;

            case "compas.geometry/Box":
                
                //Set node type_data (THIS IS FOR INTERNAL USE... IN any scenario where I am pushing data it should contain this information)
                node.type_data = "1.Box";

                // Accessing different parts of json data to make common attributes dictionary
                float xsize = Convert.ToSingle(jsonDataDict["xsize"]);
                float ysize = Convert.ToSingle(jsonDataDict["ysize"]);
                float zsize = Convert.ToSingle(jsonDataDict["zsize"]);

                //Add Items to the attributes dictionary remapping name to length, width, height
                node.attributes.length = xsize;
                node.attributes.width = ysize;
                node.attributes.height = zsize;
                break;
            
            case "compas.datastructures/Mesh":

                //Set node type_data (THIS IS FOR INTERNAL USE... IN any scenario where I am pushing data it should contain this information)
                node.type_data = "2.ObjFile";

                /* TODO: OBJ FILE OPTIONS
                 I think we need a clearer plan for handling OBJ files.
                 I think I should make a generic component for them on the python side that takes a list of frames & a list of (breps or meshs)
                 This componenet should either converts it to a mesh assembly or a frame assembly and Export the proper obj files.
                */
                Debug.Log("This is a Mesh assembly");
                break;

            case "compas.geometry/Frame":
                
                //Set node type_data (THIS IS FOR INTERNAL USE... IN any scenario where I am pushing data it should contain this information)
                node.type_data = "3.Frame";

                /* TODO: OBJ FILE OPTIONS
                 I think we need a clearer plan for handling OBJ files.
                 I think I should make a generic component for them on the python side that takes a list of frames & a list of (breps or meshs)
                 This componenet should either converts it to a mesh assembly or a frame assembly and Export the proper obj files.
                */
                Debug.Log("This is a frame assembly");
                break;

            case "compas_timber.parts/Beam":

                //Set node type_data (THIS IS FOR INTERNAL USE... IN any scenario where I am pushing data it should contain this information)
                node.type_data = "2.ObjFile";

                // Accessing different parts of json data to make common attributes dictionary
                float objLength = Convert.ToSingle(jsonDataDict["length"]);
                float objWidth = Convert.ToSingle(jsonDataDict["width"]);
                float objHeight = Convert.ToSingle(jsonDataDict["height"]);

                //Add Items to the attributes dictionary remapping name to length, width, height
                node.attributes.length = objLength;
                node.attributes.width = objWidth;
                node.attributes.height = objHeight;

                break;

            case string connectionType when connectionType.StartsWith("compas_timber.connections"):
        
                //Set node type_data (THIS IS FOR INTERNAL USE... IN any scenario where I am pushing data it should contain this information)
                node.type_data = "4.Joint";

                // This actually serves as a great oppertunity to split off for joints and use the information.
                //......

                Debug.Log("This is a timbers connection");
                break;


            default:
                Debug.Log("Default");
                break;
        }
    }
    private void PartDesctiptionSelector(Node node, Dictionary<string, object> jsonDataDict)
    {
        Debug.Log("Assembly is constructed of Parts");

        //Access nested Part information.
        Dictionary<string, object> attributesDict = jsonDataDict["attributes"] as Dictionary<string, object>;
        Dictionary<string, object> nameDict = attributesDict["name"] as Dictionary<string, object>;
        Dictionary<string, object> partdataDict = nameDict["data"] as Dictionary<string, object>;

        //Get dtype from name dictionary
        string dtype = (string)nameDict["dtype"];

        //Call dtype description selector.
        DtypeGeometryDesctiptionSelector(node, dtype, partdataDict);

    }
    public Step StepDeserializer(object jsondata)
    {
        Dictionary<string, object> jsonDataDict = jsondata as Dictionary<string, object>;

        //Create class instances of node elements
        Step step = new Step();
        step.data = new Data();
        step.data.location = new Frame();

        //Set values for base node class to keep data structure consistent
        step.dtype = (string)jsonDataDict["dtype"];
        step.guid = (string)jsonDataDict["guid"];

        //Access nested information
        Dictionary<string, object> dataDict = jsonDataDict["data"] as Dictionary<string, object>;
        Dictionary<string, object> locationDataDict = dataDict["location"] as Dictionary<string, object>;

        //Try to get device_id for the step if it does not exist set it to null.
        if (dataDict.TryGetValue("device_id", out object device_id))
        {
            step.data.device_id = device_id.ToString();
        }
        else
        {
            step.data.device_id = null;
        }
        
        //Set values for step
        step.data.actor = (string)dataDict["actor"];
        step.data.geometry = (string)dataDict["geometry"];
        step.data.is_built = (bool)dataDict["is_built"];
        step.data.is_planned = (bool)dataDict["is_planned"];
        step.data.priority = (int)(long)dataDict["priority"];


        //List Conversions System.double items to float for use in instantiation & Int64 to int & Object to string
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
            step.data.location.point = pointslist.Select(Convert.ToSingle).ToArray();
            step.data.location.xaxis = xaxislist.Select(Convert.ToSingle).ToArray();
            step.data.location.yaxis = yaxislist.Select(Convert.ToSingle).ToArray();
            step.data.elements_held = elements_held.Select(Convert.ToInt32).ToArray();
            step.data.element_ids = element_ids.Select(x => x.ToString()).ToArray();
            step.data.instructions = instructions.Select(x => x.ToString()).ToArray();
        }
        else
        {
            Debug.Log("One of the Location lists is null");
        }

        return step;
    }
    public UserCurrentInfo UserInfoDeserilizer(object jsondata)
    {
        Dictionary<string, object> jsonDataDict = jsondata as Dictionary<string, object>;

        //Create class instances of node elements
        UserCurrentInfo userCurrentInfo = new UserCurrentInfo();

        //Set values for base node class to keep data structure consistent
        userCurrentInfo.currentStep = (string)jsonDataDict["currentStep"];
        userCurrentInfo.timeStamp = (string)jsonDataDict["timeStamp"];

        return userCurrentInfo;
    }

/////////////////////////////// EVENT HANDLING ////////////////////////////////////////

    // Add listeners and remove them for firebase child events
    public void AddListeners(object source, EventArgs args)
    {        
        Debug.Log("Adding Listners");
        
        //Add listners for building plan steps
        dbreference_steps.ChildAdded += OnStepsChildAdded;
        dbreference_steps.ChildChanged += OnStepsChildChanged;
        dbreference_steps.ChildRemoved += OnStepsChildRemoved;
        
        //Add Listners for Users Current Step
        dbrefernece_usersCurrentSteps.ChildAdded += OnUserChildAdded; 
        dbrefernece_usersCurrentSteps.ChildChanged += OnUserChildChanged;
        dbrefernece_usersCurrentSteps.ChildRemoved += OnUserChildRemoved;

        //Add Listner for building plan last built index
        dbreference_LastBuiltIndex.ValueChanged += OnLastBuiltIndexChanged;

        //Add Listners to the Overall project to list for data changes in assembly, qrcodes, and additional Info.
        dbreference_project.ChildAdded += OnProjectInfoChangedUpdate;
        dbreference_project.ChildChanged += OnProjectInfoChangedUpdate;
        dbreference_project.ChildRemoved += OnProjectInfoChangedUpdate;
    }
    public void RemoveListners()
    {        
        Debug.Log("Removing the listeners");

        //Remove listners for building plan steps
        dbreference_steps.ChildAdded += OnStepsChildAdded;
        dbreference_steps.ChildChanged += OnStepsChildChanged;
        dbreference_steps.ChildRemoved += OnStepsChildRemoved;
        
        //Remove Listners for Users Current Step
        dbrefernece_usersCurrentSteps.ChildAdded += OnUserChildAdded; 
        dbrefernece_usersCurrentSteps.ChildChanged += OnUserChildChanged;
        dbrefernece_usersCurrentSteps.ChildRemoved += OnUserChildRemoved;

        //Remove Listner for building plan last built index
        dbreference_LastBuiltIndex.ValueChanged += OnLastBuiltIndexChanged;

        //Remove Listners to the Overall project to list for data changes in assembly, qrcodes, and additional Info.
        dbreference_project.ChildAdded += OnProjectInfoChangedUpdate;
        dbreference_project.ChildChanged += OnProjectInfoChangedUpdate;
        dbreference_project.ChildRemoved += OnProjectInfoChangedUpdate;
    }

    // Event handler for BuildingPlan child changes
    public void OnStepsChildAdded(object sender, Firebase.Database.ChildChangedEventArgs args) 
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        var key = args.Snapshot.Key;
        var childSnapshot = args.Snapshot.GetValue(true);
        Debug.Log($"ON CHILD ADDED {key}");

        if (childSnapshot != null)
        {
            Step newValue = StepDeserializer(childSnapshot);
           
            //make a new entry in the dictionary if it doesnt already exist
            if (IsValidStep(newValue))
            {
                if (BuildingPlanDataItem.steps.ContainsKey(key))
                {
                    Debug.Log("The key already exists in the dictionary");
                }
                else
                {
                    Debug.Log($"The key '{key}' does not exist in the dictionary");
                    BuildingPlanDataItem.steps.Add(key, newValue);

                    //TODO: Remove Temporary Building Plan Priority Tree Dictionary
                    //Check if the steps priority is one that I already have in the priority tree dictionary
                    if (PriorityTreeDict.ContainsKey(newValue.data.priority.ToString()))
                    {
                        //If the priority already exists add the key to the list
                        PriorityTreeDict[newValue.data.priority.ToString()].Add(key);
                        Debug.Log($"Step {key} successfully added to the priority tree dictionary item {newValue.data.priority.ToString()}");
                    }
                    else
                    {
                        //If not create a new list and add the key to the list
                        PriorityTreeDict[newValue.data.priority.ToString()] = new List<string>();
                        PriorityTreeDict[newValue.data.priority.ToString()].Add(key);
                        Debug.Log($"Step {key} added a new priority {newValue.data.priority.ToString()} to the priority tree dictionary");
                    }
                    //TODO: Remove Temporary Building Plan Priority Tree Dictionary

                    //Instantiate new object
                    OnDatabaseUpdate(newValue, key);
                    
                }
            }
            else
            {
                Debug.LogWarning($"Invalid Step structure for key '{key}'. Not added to the dictionary.");
            }

            //Print out the priority tree as a check
            Debug.Log("THIS IS THE PRIORITY TREE DICTIONARY: " + JsonConvert.SerializeObject(PriorityTreeDict));
        }
    } 
    public void OnStepsChildChanged(object sender, Firebase.Database.ChildChangedEventArgs args) 
    {
        if (args.DatabaseError != null) {
        Debug.LogError($"Database error: {args.DatabaseError}");
        return;
        }

        if (args.Snapshot == null) {
            Debug.LogWarning("Snapshot is null. Ignoring the child change.");
            return;
        }

        string key = args.Snapshot.Key;
        if (key == null) {
            Debug.LogWarning("Snapshot key is null. Ignoring the child change.");
        }

        var childSnapshot = args.Snapshot.GetValue(true);

        if (childSnapshot != null)
        {
            Step newValue = StepDeserializer(childSnapshot);
            
            //Check: if the step is equal to the one that I have in the dictionary
            if (!AreEqualSteps(newValue, BuildingPlanDataItem.steps[key]))
            {    
                Debug.Log($"On Child Changed: This key actually changed {key}");

                if (newValue.data.device_id != null)
                {
                    //Check: if the change is from me or from someone else could possibly get rid of this because we check every step, but still safety check for now.
                    if (newValue.data.device_id == SystemInfo.deviceUniqueIdentifier && AreEqualSteps(newValue, BuildingPlanDataItem.steps[key]))
                    {
                        Debug.Log($"I changed element {key}");
                        return;
                    }
                    //This means that the change was specifically from another device.
                    else
                    {    
                        if(IsValidStep(newValue))
                        {
                            //TODO: Remove Temporary Building Plan Priority Tree Dictionary
                            //First check if the new steps priorty is different from the old one then remove the old one and add a new one.
                            if (newValue.data.priority != BuildingPlanDataItem.steps[key].data.priority)
                            {
                                //Remove the old key from the priority tree dictionary
                                PriorityTreeDict[BuildingPlanDataItem.steps[key].data.priority.ToString()].Remove(key);

                                //Check if the priority tree value item is null and if so remove it from the dictionary
                                if (PriorityTreeDict[BuildingPlanDataItem.steps[key].data.priority.ToString()].Count == 0)
                                {
                                    PriorityTreeDict.Remove(BuildingPlanDataItem.steps[key].data.priority.ToString());
                                }

                                //Check if the steps priority is one that I already have in the priority tree dictionary
                                if (PriorityTreeDict.ContainsKey(newValue.data.priority.ToString()))
                                {
                                    //If the priority already exists add the key to the list
                                    PriorityTreeDict[newValue.data.priority.ToString()].Add(key);
                                    Debug.Log($"Step {key} successfully added to the priority tree dictionary item {newValue.data.priority.ToString()}");
                                }
                                else
                                {
                                    //If not create a new list and add the key to the list
                                    PriorityTreeDict[newValue.data.priority.ToString()] = new List<string>();
                                    PriorityTreeDict[newValue.data.priority.ToString()].Add(key);
                                    Debug.Log($"Step {key} added a new priority {newValue.data.priority.ToString()} to the priority tree dictionary");
                                }
                            }
                            else
                            {
                                Debug.Log($"The priority of the step {key} did not change");
                            }
                            //TODO: Remove Temporary Building Plan Priority Tree Dictionary

                            //Add the new value to the Building plan dictionary
                            BuildingPlanDataItem.steps[key] = newValue;
                        }
                        else
                        {
                            Debug.LogWarning($"Invalid Node structure for key '{key}'. Not added to the dictionary.");
                        }

                        Debug.Log($"HandleChildChanged - The key of the changed Step is {key}");
                        Debug.Log("newData[key] = " + BuildingPlanDataItem.steps[key]);
                        
                        //Instantiate new object
                        OnDatabaseUpdate(newValue, key);
                    }
                }
                //Check: This change happened either manually or from grasshopper. To an object that doesn't have a device id.
                else
                {
                    Debug.LogWarning($"Device ID is null: the change for key {key} happened from gh or manually.");

                    if(IsValidStep(newValue))
                    {
                        //TODO: Remove Temporary Building Plan Priority Tree Dictionary
                        //First check if the new steps priorty is different from the old one then remove the old one and add a new one.
                        if (newValue.data.priority != BuildingPlanDataItem.steps[key].data.priority)
                        {                            
                            //Remove the old key from the priority tree dictionary
                            PriorityTreeDict[BuildingPlanDataItem.steps[key].data.priority.ToString()].Remove(key);

                            //Check if the priority tree value item is null and if so remove it from the dictionary
                            if (PriorityTreeDict[BuildingPlanDataItem.steps[key].data.priority.ToString()].Count == 0)
                            {
                                PriorityTreeDict.Remove(BuildingPlanDataItem.steps[key].data.priority.ToString());
                            }

                            //Check if the steps priority is one that I already have in the priority tree dictionary
                            if (PriorityTreeDict.ContainsKey(newValue.data.priority.ToString()))
                            {
                                //If the priority already exists add the key to the list
                                PriorityTreeDict[newValue.data.priority.ToString()].Add(key);
                                Debug.Log($"Step {key} successfully added to the priority tree dictionary item {newValue.data.priority.ToString()}");
                            }
                            else
                            {
                                //If not create a new list and add the key to the list
                                PriorityTreeDict[newValue.data.priority.ToString()] = new List<string>();
                                PriorityTreeDict[newValue.data.priority.ToString()].Add(key);
                                Debug.Log($"Step {key} added a new priority {newValue.data.priority.ToString()} to the priority tree dictionary");
                            }
                            
                            BuildingPlanDataItem.steps[key] = newValue;
                        }
                        else
                        {
                            Debug.Log($"The priority of the step {key} did not change");
                        }
                        //TODO: Remove Temporary Building Plan Priority Tree Dictionary

                    }
                    else
                    {
                        Debug.LogWarning($"Invalid Node structure for key '{key}'. Not added to the dictionary.");
                    }

                    Debug.Log($"HandleChildChanged - The key of the changed Step is {key}");
                    Debug.Log("newData[key] = " + BuildingPlanDataItem.steps[key]);
                    
                    //Instantiate new object
                    OnDatabaseUpdate(newValue, key);

                }

                //Print out the priority tree as a check
                Debug.Log("THIS IS THE PRIORITY TREE DICTIONARY: " + JsonConvert.SerializeObject(PriorityTreeDict));
            }

        }
    }
    public void OnStepsChildRemoved(object sender, Firebase.Database.ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        string key = args.Snapshot.Key;
        string childSnapshot = args.Snapshot.GetRawJsonValue();

        if (!string.IsNullOrEmpty(childSnapshot))
        {
            //remove an entry in the dictionary
            if (BuildingPlanDataItem.steps.ContainsKey(key))
            {
                Step newValue = null;
                Debug.Log("The key exists in the dictionary and is going to be removed");
                
                //TODO: Remove Temporary Building Plan Priority Tree Dictionary
                //First check thee steps priorty Remove the steps key from the priority tree dictionary
                PriorityTreeDict[BuildingPlanDataItem.steps[key].data.priority.ToString()].Remove(key);

                //Check if the priority tree value item is null and if so remove it from the dictionary
                if (PriorityTreeDict[BuildingPlanDataItem.steps[key].data.priority.ToString()].Count == 0)
                {
                    PriorityTreeDict.Remove(BuildingPlanDataItem.steps[key].data.priority.ToString());
                }
                //TODO: Remove Temporary Building Plan Priority Tree Dictionary
                
                //Remove the step from the building plan dictionary
                BuildingPlanDataItem.steps.Remove(key);

                //Trigger database event
                OnDatabaseUpdate(newValue, key);
            }
            else
            {
                Debug.Log("The key does not exist in the dictionary");
            }
        }
    }  
    public async void OnLastBuiltIndexChanged(object sender, Firebase.Database.ValueChangedEventArgs args)
    {

        if (args.DatabaseError != null) {
        Debug.LogError($"Database error: {args.DatabaseError}");
        return;
        }

        if (args.Snapshot == null) {
            Debug.LogWarning("Snapshot is null. Ignoring the child change.");
            return;
        }
        
        Debug.Log("Last Built Index Changed");
        
        //Set Temp Current Element to null so that everytime an event is triggered it becomes null again and doesnt keep old data.
        TempDatabaseLastBuiltStep = null;
        
        await FetchRTDData(dbreference_LastBuiltIndex, snapshot => DesearializeLastBuiltIndex(snapshot));
    
        if (TempDatabaseLastBuiltStep != null)
        {
            if(TempDatabaseLastBuiltStep != BuildingPlanDataItem.LastBuiltIndex)
            {
                // Update Last Built Index
                BuildingPlanDataItem.LastBuiltIndex = TempDatabaseLastBuiltStep;
                Debug.Log($"Last Built Index is now {BuildingPlanDataItem.LastBuiltIndex}");

                // Update On Screen Text
                UIFunctionalities.SetLastBuiltText(BuildingPlanDataItem.LastBuiltIndex);
            
                // Check Priority to see if its complete.
                Step step = BuildingPlanDataItem.steps[BuildingPlanDataItem.LastBuiltIndex];
                int priorityCheck = OtherUserPriorityChecker(step, BuildingPlanDataItem.LastBuiltIndex);

                // If the priority checker == 1 then set priority from this step. If it == 2 then set priority from the next step. (THIS COULD BE PRIORITY + 1, but safest to use the next step because it avoids errors from if a priority is missing)
                if (priorityCheck == 1)
                {
                    UIFunctionalities.SetCurrentPriority(step.data.priority.ToString());
                }
                else if (priorityCheck == 2 && Convert.ToInt32(BuildingPlanDataItem.LastBuiltIndex) < BuildingPlanDataItem.steps.Count - 1)
                {
                    //Get my current priority
                    string localCurrentPriority = CurrentPriority;

                    //convert to int and add 1
                    int nextPriority = Convert.ToInt32(localCurrentPriority) + 1;

                    //Set this as my new current priority
                    UIFunctionalities.SetCurrentPriority(nextPriority.ToString());

                    //If my CurrentStep Priority is the same as New Current Priority then update UI graphics
                    Step localCurrentStep = BuildingPlanDataItem.steps[UIFunctionalities.CurrentStep];

                    //If my CurrentStep Priority is the same as New Current Priority then update UI graphics
                    if(localCurrentStep.data.priority.ToString() == CurrentPriority)
                    {    
                        UIFunctionalities.IsBuiltButtonGraphicsControler(localCurrentStep.data.is_built, localCurrentStep.data.priority);
                    }

                }
                else
                {
                    Debug.Log($"OtherUser Priority Check: returned {priorityCheck} and is not 1 or 2 and shouldn't do anything");
                }
            }
            else
            {
                Debug.Log("Last Built Index is the same your current Last Built Index");
            }

        }
    }    
    
    // Event handlers for User Current Step
    public void OnUserChildAdded(object sender, Firebase.Database.ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null) {
        Debug.LogError($"Database error: {args.DatabaseError}");
        return;
        }

        if (args.Snapshot == null) {
            Debug.LogWarning("Snapshot is null. Ignoring the child change.");
            return;
        }

        string key = args.Snapshot.Key;
        var childSnapshot = args.Snapshot.GetValue(true);

        if (childSnapshot != null)
        {
            UserCurrentInfo newValue = UserInfoDeserilizer(childSnapshot);
            
            //make a new entry in the dictionary if it doesnt already exist
            if (newValue != null)
            {
                if (UserCurrentStepDict.ContainsKey(key))
                {
                    Debug.Log($"This User {key} already exists in the dictionary");
                }
                else
                {
                    Debug.Log($"The key '{key}' does not exist in the dictionary");
                    UserCurrentStepDict.Add(key, newValue);

                    //Instantiate new object
                    OnUserInfoUpdated(newValue, key);
                }
            }
            else
            {
                Debug.LogWarning($"Invalid Step structure for key '{key}'. Not added to the dictionary.");
            }
        }
    }
    public void OnUserChildChanged(object sender, Firebase.Database.ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null) {
        Debug.LogError($"Database error: {args.DatabaseError}");
        return;
        }

        if (args.Snapshot == null) {
            Debug.LogWarning("Snapshot is null. Ignoring the child change.");
            return;
        }

        string key = args.Snapshot.Key;
        var childSnapshot = args.Snapshot.GetValue(true);

        if (childSnapshot != null)
        {
            UserCurrentInfo newValue = UserInfoDeserilizer(childSnapshot);
            
            //Check: if the current step update was from me or not.
            if (key != SystemInfo.deviceUniqueIdentifier)
            {    
                Debug.Log($"User {key} updated their current step");


                Debug.Log($"I entered here for key {key}");
                if(newValue != null)
                {
                    UserCurrentStepDict[key] = newValue;
                }
                else
                {
                    Debug.LogWarning($"Invalid Node structure for key '{key}'. Not added to the dictionary.");
                }

                Debug.Log($"Handle Changed User INfo - User {key} updated their current step to {newValue.currentStep}");
                
                //Instantiate new object
                OnUserInfoUpdated(newValue, key);
            }
            else
            {
                Debug.Log("I updated my current key");
            }

        }
    }
    public void OnUserChildRemoved(object sender, Firebase.Database.ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null) {
        Debug.LogError($"Database error: {args.DatabaseError}");
        return;
        }
        
        string key = args.Snapshot.Key;
        string childSnapshot = args.Snapshot.GetRawJsonValue();
        
        if (!string.IsNullOrEmpty(childSnapshot))
        {
            //remove an entry in the dictionary
            if (UserCurrentStepDict.ContainsKey(key))
            {
                UserCurrentInfo newValue = null;
                Debug.Log("The key exists in the dictionary and is going to be removed");
                UserCurrentStepDict.Remove(key);
                OnUserInfoUpdated(newValue, key);
            }
            else
            {
                Debug.Log("The key does not exist in the dictionary");
            }
        }

    }

    // Event Handlers for Additional Project Information, Assembly, Parts, QRFrames, & Joints
    public async void OnProjectInfoChangedUpdate(object sender, Firebase.Database.ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null) {
        Debug.LogError($"Database error: {args.DatabaseError}");
        return;
        }

        if (args.Snapshot == null) {
            Debug.LogWarning("Snapshot is null. Ignoring the child change.");
            return;
        }

        //Get the child changed key and value
        string key = args.Snapshot.Key;
        var childSnapshot = args.Snapshot.GetValue(true);

        //If Snapshot and Key are not null check for where the change needs to happen.
        if (childSnapshot != null && key != null)
        {
            if(key == "assembly")
            {
                Debug.Log("Project Changed: Assembly Changed");
                
                //If the assembly changed then fetch new assembly data
                await FetchRTDData(dbreference_assembly, snapshot => DeserializeDataSnapshot(snapshot, AssemblyDataDict));
            }
            else if(key == "QRFrames")
            {
                Debug.Log("Project Changed: QRFrames Changed");

                //If the qrcodes changed then fetch new qrcode data
                await FetchRTDData(dbreference_qrcodes, snapshot => DeserializeDataSnapshot(snapshot, QRCodeDataDict), "TrackingDict");
            }
            else if(key == "beams")
            {
                Debug.Log("Project Changed: Beams");
            }
            else if(key == "joints")
            {
                Debug.Log("Project Changed: Joints");
            }
            else if(key == "building_plan")
            {
                Debug.Log("Project Changed: BuildingPlan and should be handled by other listners");
            }
            else if(key == "UserCurrentStep")
            {
                Debug.Log("Project Changed: User Current Step Changed this should be handled by other listners");
            }
            else
            {
                Debug.LogWarning($"Project Changed: The key: {key} did not match the expected project keys");
            }
        }
    }
    
    // Event handling for database initialization
    protected virtual void OnDatabaseInitializedDict(BuildingPlanData BuildingPlanDataItem)
    {
        UnityEngine.Assertions.Assert.IsNotNull(DatabaseInitializedDict, "Database dict is null!");
        Debug.Log("Building Plan Data Received");
        DatabaseInitializedDict(this, new BuildingPlanDataDictEventArgs() {BuildingPlanDataItem = BuildingPlanDataItem});
    }
    protected virtual void OnTrackingDataReceived(Dictionary<string, Node> QRCodeDataDict)
    {
        UnityEngine.Assertions.Assert.IsNotNull(TrackingDictReceived, "Tracking Dict is null!");
        Debug.Log("Tracking Data Received");
        TrackingDictReceived(this, new TrackingDataDictEventArgs() {QRCodeDataDict = QRCodeDataDict});
    }
    protected virtual void OnDatabaseUpdate(Step newValue, string key)
    {
        UnityEngine.Assertions.Assert.IsNotNull(DatabaseInitializedDict, "new dict is null!");
        DatabaseUpdate(this, new UpdateDataItemsDictEventArgs() {NewValue = newValue, Key = key });
    }
    protected virtual void OnUserInfoUpdated(UserCurrentInfo newValue, string key)
    {
        UnityEngine.Assertions.Assert.IsNotNull(UserInfoUpdate, "new dict is null!");
        UserInfoUpdate(this, new UserInfoDataItemsDictEventArgs() {UserInfo = newValue, Key = key });
    }
    protected virtual void OnSettingsUpdate(ApplicationSettings settings)
    {
        ApplicationSettingUpdate(this, new ApplicationSettingsEventArgs(){Settings = settings});
    }

}