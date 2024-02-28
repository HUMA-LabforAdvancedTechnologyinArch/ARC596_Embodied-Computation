using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectManager_T5 : MonoBehaviour
{
    //public variables
    public GameObject prefabA;
    public GameObject prefabB;
    public GameObject prefabC;
    public GameObject Instantiator;
    public Instantiator_MultipleFeatures Object_Spawner;
    

    //private variables
    private GameObject buttonA;
    private GameObject buttonB;
    private GameObject buttonC;

    
    // Start is called before the first frame update
    void Start()
    {
        //find the ObjectSpawner script
        Object_Spawner = Instantiator.GetComponent<Instantiator_MultipleFeatures>();

        //For each button, define OnClick Action and prefab
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(Menu_Toggle);

        buttonA = transform.GetChild(1).gameObject;
        buttonA.GetComponent<Button>().onClick.AddListener(() => OnClick_ChangePrefab(prefabA));

        buttonB = transform.GetChild(2).gameObject; 
        buttonB.GetComponent<Button>().onClick.AddListener(() => OnClick_ChangePrefab(prefabB));

        buttonC = transform.GetChild(3).gameObject; 
        buttonC.GetComponent<Button>().onClick.AddListener(() => OnClick_ChangePrefab(prefabC));
    }
   
    //Toggle ON and OFF the dropdown submenu options
    void Menu_Toggle()
    {
        //deactivate the buttons if they are on
        if (buttonA.activeSelf == true)
        {
            buttonA.SetActive(false);
            buttonB.SetActive(false);
            buttonC.SetActive(false);
        }
        //activate the buttons only if prefabs are set
        else 
        {
            if (prefabA!=null)
                buttonA.SetActive(true);
            if (prefabB != null)
                buttonB.SetActive(true);
            if (prefabC != null)
                buttonC.SetActive(true);
        }
    }
    public void OnClick_ChangePrefab(GameObject prefab)
    {
        if(prefab!=null)
            Object_Spawner.selectedPrefab = prefab;
            Debug.Log(Object_Spawner);
            Debug.Log("Prefab changed to: " + Object_Spawner.selectedPrefab);
    }
    
}
