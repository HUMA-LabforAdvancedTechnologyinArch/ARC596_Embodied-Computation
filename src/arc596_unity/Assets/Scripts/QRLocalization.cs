using System.Collections;
using System.Collections.Generic;
using JSON;
using Unity.Mathematics;
using UnityEngine;
using Vuforia;
using Instantiate;

public class QRLocalization : MonoBehaviour
{
    
    //Public GameObjects
    private GameObject Elements;
    private GameObject UserObjects;
    private GameObject ObjectLengthsTags;

    //Public Scripts
    public InstantiateObjects instantiateObjects;

    //Public Dictionaries
    public Dictionary<string, Node> QRCodeDataDict = new Dictionary<string, Node>();

    //In script use variables
    public Vector3 pos;

    private string lastQrName = "random";
    
    // Start is called before the first frame update
    void Start()
    {   
        //Find the Instantiate Objects game object to call methods from inside the script
        instantiateObjects = GameObject.Find("Instantiate").GetComponent<InstantiateObjects>();
        
        //Find GameObjects that need to be transformed
        Elements = GameObject.Find("Elements");
        UserObjects = GameObject.Find("UserObjects");
        ObjectLengthsTags = GameObject.Find("ObjectLengthsTags");

    }

    void Update()
    {

        if (QRCodeDataDict.Count > 0 && Elements != null)
        {
            pos = Vector3.zero;

            foreach (string key in QRCodeDataDict.Keys)
            {
                GameObject qrObject = GameObject.Find("Marker_" + key);              

                if (qrObject != null && qrObject.transform.position != Vector3.zero)
                {
                    if(qrObject.name != lastQrName)
                    {
                        GameObject lastQrObject = GameObject.Find(lastQrName);

                        if (lastQrObject != null)
                        {
                            lastQrObject.transform.position = Vector3.zero;
                            lastQrObject.transform.rotation = Quaternion.identity;
                        }

                        lastQrName = qrObject.name;
                    }
                    
                    //Fetch position data from the dictionary
                    Vector3 position_data = instantiateObjects.getPosition(QRCodeDataDict[key].part.frame.point);

                    //Fetch rotation data from the dictionary
                    InstantiateObjects.Rotation rotationData = instantiateObjects.getRotation(QRCodeDataDict[key].part.frame.xaxis, QRCodeDataDict[key].part.frame.yaxis);
                    
                    //Convert Firebase rotation data to Quaternion rotation. Additionally
                    Quaternion rotationQuaternion = instantiateObjects.FromUnityRotation(rotationData);

                    //Set Design Objects rotation to the rotation based on Observed rotation and Inverse rotation of physical QR
                    Quaternion rot = qrObject.transform.rotation * Quaternion.Inverse(rotationQuaternion);
                    
                    //Transform the rotation of game objects that need to be transformed
                    Elements.transform.rotation = rot;
                    UserObjects.transform.rotation = rot;
                    ObjectLengthsTags.transform.rotation = rot;


                    //Translate the position of the object based on the observed position and the inverse rotation of the physical QR
                    pos = TranslatedPosition(qrObject, position_data, rotationQuaternion);

                    //Set the position of the gameobjects object to the translated position                    
                    Elements.transform.position = pos;
                    UserObjects.transform.position = pos;
                    ObjectLengthsTags.transform.position = pos;

                    Debug.Log($"QR: Translation from QR object: {qrObject.name}");
                }
            }
        }

    }

    private Vector3 TranslatedPosition(GameObject gobject, Vector3 position, Quaternion Individualrotation)
    {
        //Position determined by positioning the QR codes observed position and rotation and translating by position vector and the inverse of the QR rotation
        Vector3 pos = gobject.transform.position + (gobject.transform.rotation * Quaternion.Inverse(Individualrotation) * -position);
        return pos;
    }
    public void OnTrackingInformationReceived(object source, TrackingDataDictEventArgs e)
    {
        Debug.Log("Database is loaded." + " " + "Number of QR codes stored as a dict= " + e.QRCodeDataDict.Count);
        QRCodeDataDict = e.QRCodeDataDict;
    }
                    

}
