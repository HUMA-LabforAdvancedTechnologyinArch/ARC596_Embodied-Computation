using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARCore;
using UnityEngine.EventSystems;

public class Instantiator : MonoBehaviour
{

    /// <summary>
    /// -----------------------------------------------
    /// The Instantiator Script generates all the objects according to the taps.
    /// It obtains the information from the buttons touched and switches to different modes.
    /// -----------------------------------------------
    /// Mode 0 = Add one object with each tap 
    /// Mode 1 = Add multiple objects with each tap 
    /// Mode 2 ----- to be Added on Day 3 
    /// Mode 3 ----- to be Added Day 3
    /// </summary>
 
    //1. Variables
    public GameObject selected_prefab; //we declare it as public to have a default prefab
    public Transform arCameraTransform; //we use the position to make the objects look at the camera
    public GameObject houseParent; //house prefab parent 
    public GameObject objectParent; //all other objects prefab parent

    private int mode = 0; //placing a default value as 0 
    private GameObject ARObject_new;  // this will be the Instantiated Object name

    //raycast related variables here
    private ARRaycastManager rayManager;
    private ARSession arSession;
    public Camera arCamera;


    // Start is called before the first frame update
    void Start()
    {
        //find the Raycast manager in the script in order to use it to acquire data
        rayManager = FindObjectOfType<ARRaycastManager>();
    }

    // Update is called once per frame
    void Update()
    {
        _InstantiateOnTouch();
    }

    private void _InstantiateOnTouch()
    {

        if (Input.touchCount > 0) //if there is an input..           
        {
            if (PhysicRayCastBlockedByUi(Input.GetTouch(0).position))
            {
                if (mode == 0) // ADD ONE : destroy previous object with every tap
                {
                    Debug.Log("***MODE 0***");
                    Touch touch = Input.GetTouch(0);

                    // Handle finger movements based on TouchPhase
                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            if (Input.touchCount == 1)
                            {
                                _PlaceInstant(houseParent);
                            }
                            break; //break: If this case is true, it will not check the other ones. More computational efficiency, 

                        case TouchPhase.Moved:
                        // Record initial touch position.
                            if (Input.touchCount == 1)
                                {
                                _Rotate(ARObject_new);
                                }
                            
                            if (Input.touchCount == 2)
                            {
                                _PinchtoZoom(ARObject_new);
                            }
                            break;

                        case TouchPhase.Ended:
                            Debug.Log("Touch Phase Ended.");
                            break;
                    }
                }
                else if (mode == 1) //ADD MULTIPLE : create multiple instances of object
                {
                    Debug.Log("***MODE 1***");
                    Touch touch = Input.GetTouch(0);

                    // Handle finger movements based on TouchPhase
                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            if (Input.touchCount == 1)
                            {
                                _PlaceInstant(objectParent);
                            }
                            break;

                        case TouchPhase.Moved:
                        // Record initial touch position.
                            if (Input.touchCount == 1)
                                {
                                _Rotate(ARObject_new);
                                }
                            
                            if (Input.touchCount == 2)
                            {
                                _PinchtoZoom(ARObject_new);
                            }
                            break;

                        case TouchPhase.Ended:
                            Debug.Log("Touch Phase Ended.");
                            break;
                    }
                }

                //else if (mode == 2) //EDIT MODE
                //{
                //    Debug.Log("***MODE 2***");  // to be added on day 3                  
                    
                //}

                //else if (mode == 3) //DELETE MODE
                //{
                //    Debug.Log("***MODE 3***"); //to be added on day 3

                //}
                //else
                //{
                //    Debug.Log("Press a button to initialize a mode");
                //}
            }
        }
    }
    private void _PlaceInstant(GameObject parentGameObject)
    {
        Touch touch;
        touch = Input.GetTouch(0);
        Debug.Log("Single Touch");

        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        rayManager.Raycast(touch.position, hits);
        if (hits.Count > 0)
        {
            if ((hits[0].hitType & TrackableType.Planes) != 0) //if our touch hits a scanned plane, it instantiates an object
            {
                //Debug.Log("HIT TYPE = " + hits[0].hitType);
                if (mode == 0)
                {                          
                    Debug.Log("We are in the deleting area");
                    if (houseParent.transform.childCount > 0)
                    { Destroy(houseParent.transform.GetChild(0).gameObject); } //destroys the previous object
                    
                }

                // You can instantiate a 3D object here if you haven't set Raycast Prefab in the scene.
                ARObject_new = Instantiate(selected_prefab, hits[0].pose.position, hits[0].pose.rotation);
                ARObject_new.transform.SetParent(parentGameObject.transform);  //set the correct parent of the GameObject 

                //transform object to Look at our phone camera
                Vector3 cameraPosition = arCameraTransform.position;
                cameraPosition.y = hits[0].pose.position.y;
                ARObject_new.transform.LookAt(cameraPosition, ARObject_new.transform.up);

                //create AR Anchor for each instantiated object
                if (ARObject_new.GetComponent<ARAnchor>() == null)
                {
                    ARObject_new.AddComponent<ARAnchor>();
                }
            }
        }
             
    }
    private void _PinchtoZoom(GameObject objectToZoom)

    //scale using pinch involves 2 touches
    // we count both the touches, store them and measure the distance between pinch
    // and scale depending on the pinch distance
    {
        Touch touch;
        touch = Input.GetTouch(0);

        
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
        float pinchAmount = deltaMagnitudeDiff * -0.05f * Time.deltaTime;
    
        Debug.Log("Scaling Initialized");
        Debug.Log(objectToZoom);
        //scale according to pinch amount
        objectToZoom.transform.localScale += new Vector3(pinchAmount, pinchAmount, pinchAmount);
        
    }
    private void _Rotate(GameObject objectToRotate) 
    {
        Touch touch;
        touch = Input.GetTouch(0);
        
        Debug.Log("Rotate touch");
        objectToRotate.transform.Rotate(Vector3.up * 40f * Time.deltaTime * touch.deltaPosition.x, Space.Self);
        Debug.Log("Delta Touch is " + touch.deltaPosition);
        
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

    //public void SetMode_C()
    //{
    //    mode = 2;
    //}

    //public void SetMode_D()
    //{
    //    mode = 3;
    //}

    public class PointerOverUI
    {
        public static bool IsPointerOverUIObject(Vector2 touchPosition)
        {
            //checking if we are touching a button
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = touchPosition;
            List<RaycastResult> raycastResults = new List<RaycastResult>();

            EventSystem.current.RaycastAll(pointerEventData, raycastResults);

            return raycastResults.Count > 0;
        }
    }
    private bool PhysicRayCastBlockedByUi(Vector2 touchPosition)
    {
        //creating a Boolean value if we are touching a button
        if (PointerOverUI.IsPointerOverUIObject(touchPosition))
        {
            return false;
        }

        return true;
    }

    public void ResetApp()
    {
        //destroy all created objects
        if (houseParent.transform.childCount > 0)
        {
            foreach (Transform child in houseParent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
        if (objectParent.transform.childCount > 0)
        {
            foreach (Transform child in objectParent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
       
        //reset AR session : resets all trackable objects and planes. 
        arSession = FindObjectOfType<ARSession>();
        arSession.Reset();
    }
}
