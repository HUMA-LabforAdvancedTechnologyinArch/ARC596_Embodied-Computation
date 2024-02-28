using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARCore;
using UnityEngine.EventSystems;

public class Instantiator_MultipleFeatures : MonoBehaviour
{

    /// <summary>
    /// -----------------------------------------------
    /// The Instantiator Script generates all the objects according to the taps.
    /// It obtains the information from the buttons touched and switches to different modes.
    /// -----------------------------------------------
    /// Mode 0 = Add one object with each tap -- single tap 
    /// Mode 1 = Add multiple objects with each tap -- single tap 
    /// Mode 2 = Edit existing objects by scaling and rotating  -- double tap 
    /// Mode 3 = Delete existing objects by tapping on them  -- single tap
    /// </summary>
 
    //1. Variables
    public GameObject selectedPrefab; //we declare it as public to have a default prefab
    private GameObject instantiatedObject;
    public GameObject singleObjectParent; //house prefab parent 
    public GameObject multipleObjectParent; //all other objects prefab parent
    public Material BoundingBoxMaterial; //material for selected objects 
    public Material BoundingBoxMaterial_02; //material for yellow bounding box
    public int mode = 0; //placing a default value as 0 
    private GameObject lastUsedPrefab; // Add this to track the last used prefab
    private GameObject ARObject_new;  // this will be the Instantiated Object name
    private GameObject activeGameObject = null; // this will be our selected gameObject in the edit mode
    private GameObject temporaryObject;

    //raycast related variables here
    private ARRaycastManager rayManager;
    private ARSession arSession;
    List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private float timePressed = 0.0f;
    private float timeDelayThreshold = 1.0f;
    private Vector3 newPosition;


    // Start is called before the first frame update
    void Start()
    {
        //find the Raycast manager in the script in order to use it to acquire data
        rayManager = FindObjectOfType<ARRaycastManager>();
        arSession = FindObjectOfType<ARSession>();
    }

    // Update is called once per frame
    void Update()
    {
        print(Input.touchCount);
        if (Input.touchCount > 0 && !IsPointerOverUIObject(Input.GetTouch(0).position))
        {
            HandleTouch(Input.GetTouch(0));
        }
    }

    private void HandleTouch(Touch touch)
    {
            // if it is mode 0 or mode 1, we can place objects
            if (mode == 0 || mode == 1) 
            {
                print("***MODE 0 or 1***");
                print("TOUCH PHASE IS" + touch.phase);

                // Handle finger movements based on TouchPhase
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        InstantiateOnTouch(touch);
                        Debug.Log("Touch Phase began.");
                        break; //break: If this case is true, it will not check the other ones. More computational efficiency, 
                    
                    case TouchPhase.Moved:
                        if (instantiatedObject != null)
                        {
                            if (Input.touchCount == 1)
                            {
                                Rotate(instantiatedObject, touch);
                            }
                            
                            else if (Input.touchCount == 2)
                            {
                                PinchtoZoom(instantiatedObject);
                            }
                        }
                        break;

                    case TouchPhase.Ended:
                        Debug.Log("Touch Phase Ended.");
                        break;
                }
            }

            else if (mode == 2) //EDIT MODE
            {
                Debug.Log("***MODE 2***");
                EditMode();                   
                
            }

            else if (mode == 3) //DELETE MODE
            {
                Debug.Log("***MODE 3***");
                activeGameObject = SelectedObject();
                DestroySelected(activeGameObject);
            }
            else
            {
                Debug.Log("Press a button to initialize a mode");
            }
    }
    
    private void InstantiateOnTouch(Touch touch)
    {
        print("Single Touch");

        // Check if the raycast hit any trackables.
        if (rayManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
        {
            // Raycast hits are sorted by distance, so the first hit means the closest.
            var hitPose = hits[0].pose;

            //mode 0: single placement of objects, like the 3D printed house hologram
            //mode 1: multiple placement of objects, like multiple trees or characters
            bool shouldInstantiateNewObject = mode == 1 || (mode == 0 && instantiatedObject == null);
            print("shouldInstantiateNewObject" + shouldInstantiateNewObject);
            bool prefabChanged = lastUsedPrefab != selectedPrefab && mode == 0;
            print("prefabChanged" + prefabChanged);

            if (shouldInstantiateNewObject || prefabChanged)
            {
                print("we should instantiate a new object or the prefab changed");
                if (prefabChanged && instantiatedObject != null)
                {
                    Destroy(instantiatedObject); // Optionally destroy the old object if a new prefab is selected
                    Debug.Log("Prefab changed, instantiating new prefab.");
                }

                instantiatedObject = Instantiate(selectedPrefab, hitPose.position, hitPose.rotation, GetParentTransform());
                lastUsedPrefab = selectedPrefab;
            }
            else
            {
                print("we move an instantiated object" + instantiatedObject);
                // Move the existing instantiated object
                instantiatedObject.transform.position = hitPose.position;
                instantiatedObject.transform.rotation = hitPose.rotation;
            }
            AdjustRotationToCamera(instantiatedObject);
        }
    }

    private Transform GetParentTransform()
    {
        return mode == 0 ? singleObjectParent.transform : multipleObjectParent.transform;
    }

    private void AdjustRotationToCamera(GameObject obj)
    {
        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 direction = new Vector3(cameraPosition.x, obj.transform.position.y, cameraPosition.z) - obj.transform.position;
        obj.transform.rotation = Quaternion.LookRotation(direction);
    }

    private void PinchtoZoom(GameObject objectToZoom)

    {
        if (Input.touchCount == 2)
        {
            Debug.Log("Double Touch");
            var touchZero = Input.GetTouch(0);
            var touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
            float pinchAmount = deltaMagnitudeDiff * -0.02f * Time.deltaTime;

            Debug.Log("Scaling Initialized");
            Debug.Log(objectToZoom);
            //scale according to pinch amount
            objectToZoom.transform.localScale += new Vector3(pinchAmount, pinchAmount, pinchAmount);
        }
    }
 
    private void Rotate(GameObject objectToRotate, Touch touch) 
    {
        float rotationSpeed = 0.1f; // Adjust rotation speed as needed
        objectToRotate.transform.Rotate(Vector3.up, touch.deltaPosition.x * rotationSpeed, Space.World);
        
    }

    private void Move(GameObject objectToMove)
    {
        Touch touch;
        touch = Input.GetTouch(0);
        if (Input.touchCount == 1 && touch.phase == TouchPhase.Began)
        {
            timePressed = 0;

        }

        if (Input.touchCount == 1 && touch.phase == TouchPhase.Stationary)
        {
            timePressed += Time.deltaTime;
            Debug.Log("Time Pressed " + timePressed);

        }
        if (timePressed >= timeDelayThreshold)
        {
            ChangeBoundingBoxColor(BoundingBoxMaterial_02);
        }
        if (timePressed >= timeDelayThreshold && touch.phase == TouchPhase.Moved)
        {
            
            Debug.Log("Move touch");
            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            rayManager.Raycast(touch.position, hits);

            if (hits.Count > 0)
            {
                if ((hits[0].hitType & TrackableType.Planes) != 0)
                {
                    newPosition = hits[0].pose.position;
                }
            }
            objectToMove.transform.position = newPosition;
        }
        else
            Rotate(objectToMove, touch);
    }

    private void EditMode()
    {
        if (Input.touchCount == 1) //try and locate the selected object only when we click, not on update
        {
            activeGameObject = SelectedObject();
        }
        if (activeGameObject != null) //change the pinch and zoom place holder only when we locate a new object
        {           
            temporaryObject = activeGameObject;
            addBoundingBox(temporaryObject); //add bounding box around selected game object
        }

        Move(temporaryObject);
        PinchtoZoom(temporaryObject);
        
    }

    private GameObject SelectedObject(GameObject activeGameObject = null)
    {
        Touch touch;
        touch = Input.GetTouch(0);

        //delete the previous selection boundary, will be replaced with a new one

        if (Input.touchCount == 1 && touch.phase == TouchPhase.Ended)
        {
            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            rayManager.Raycast(touch.position, hits);

            if (hits.Count > 0)
            {

                Debug.Log("Ray shooting from camera");
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hitObject;              

                //if our touch hits an existing object, we find that object
                if (Physics.Raycast(ray, out hitObject))  
                {
                    //we make sure we didn't tap a plane
                    if (hitObject.collider.tag != "plane")
                    {
                        //setting the variable
                        Debug.Log("Selected object located");
                        activeGameObject = hitObject.collider.gameObject; //assign GameObject as the active
                        Debug.Log(activeGameObject);

                    }
                }
            }
        }
       
        return activeGameObject; //return the selected GameObject
    }

    private void DestroyBoundingBox()
    {
        //destroy the previous bounding box

        if (GameObject.Find("BoundingArea") != null)
        {
            Destroy(GameObject.Find("BoundingArea"));
        }

    }

    private void ChangeBoundingBoxColor(Material new_material)
    {
        //destroy the previous bounding box

        if (GameObject.Find("BoundingArea") != null)
        {
            GameObject.Find("BoundingArea").GetComponent<Renderer>().material = new_material;
        }

    }

    private void addBoundingBox(GameObject gameObj)
    {

        DestroyBoundingBox(); //destroy the bounding box

        //create a primitive cube
        GameObject boundingArea = GameObject.CreatePrimitive(PrimitiveType.Cube);

        //add name
        boundingArea.name = "BoundingArea";

        //add material
        boundingArea.GetComponent<Renderer>().material = BoundingBoxMaterial;


        //use collider to find object bounds
        Collider collider = gameObj.GetComponent<Collider>();
        Vector3 center = collider.bounds.center;
        float radius = collider.bounds.extents.magnitude;
        Debug.Log("RADIUS BOUNDING BOX = " + radius);

        //destroy any Collider component
        if (boundingArea.GetComponent<Rigidbody>() != null)
        {
            Destroy(boundingArea.GetComponent<BoxCollider>());
        }
        if (boundingArea.GetComponent<Collider>() != null)
        {
            Destroy(boundingArea.GetComponent<BoxCollider>());
        }

        //scale the bounding box according to the bounds values
        boundingArea.transform.localScale = new Vector3(radius * 1.3f, radius * 1.3f, radius * 1.3f);
        boundingArea.transform.localPosition = center;

        boundingArea.transform.SetParent(gameObj.transform);
    }

    private void DestroySelected(GameObject gameObjectToDestroy)
    {
        Destroy(gameObjectToDestroy);
    }


    //   UI Functions
    public void SetMode_A()
    {
        mode = 0; // for single placement of objects, like the 3D printed house hologram
    }
    public void SetMode_B()
    {
        mode = 1; // for multiple placement of objects, like multiple trees or characters
    }
    public void SetMode_C()
    {
        mode = 2; // for editing existing objects
    }
    public void SetMode_D()
    {
        mode = 3; // for deleting objects
    }

    public static bool IsPointerOverUIObject(Vector2 touchPosition)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current) { position = touchPosition };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    public void ResetApp()
    {
        //destroy all created objects
        if (singleObjectParent.transform.childCount > 0 || multipleObjectParent.transform.childCount > 0)
        {
            foreach (Transform child in singleObjectParent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            foreach (Transform child in multipleObjectParent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
       
        //reset AR session : resets all trackable objects and planes. 
        arSession = FindObjectOfType<ARSession>();
        arSession.Reset();
    }
}
