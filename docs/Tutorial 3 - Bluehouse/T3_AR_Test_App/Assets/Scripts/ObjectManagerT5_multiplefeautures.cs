using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectManagerT5_multiplefeautures : MonoBehaviour
{
    //public variables
    public GameObject prefabA;
    public GameObject prefabB;
    public GameObject prefabC;
    public GameObject prefabD;
    public List<GameObject> prefabs;

    //private variables
    public Instantiator_MultipleFeatures Object_Spawner;	
    public GameObject Instantiator;
    private GameObject buttonA;
    private GameObject buttonB;
    private GameObject buttonC;
    private GameObject buttonD;

    public List<Button> buttons;
    public List<GameObject> children;
    private bool menue_active = false;

    void Start()
    {
        //find the ObjectSpawner script
        Object_Spawner = Instantiator.GetComponent<Instantiator_MultipleFeatures>();

        GameObject[] btns = GameObject.FindGameObjectsWithTag("button");
        // Iterate through the array of 'btn' and add them to the 'buttons' list
        for (int i = 0; i < btns.Length; i++)
        {
            // Adding the current 'btn' to the 'buttons' list
            buttons.Add(btns[i].GetComponent<Button>());
        }

        //put prefab 
        prefabs.InsertRange(prefabs.Count, new List<GameObject> { prefabA, prefabB, prefabC, prefabD });

        //put all children button in a list

        for (int i = 1; i < transform.childCount; i++)
        {
            children.Add(transform.GetChild(i).gameObject);
        }

        //For each button, define OnClick Action and prefab
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(Turn_off_menu);
        btn.onClick.AddListener(Menu_Toggle);

        for (int i = 0; i < children.Count; i++)
        {
            if (children[i] != null)
                children[i].GetComponent<Button>().onClick.AddListener(() => OnClick_ChangePrefab(prefabs[i]));
            children[i].GetComponent<Button>().onClick.AddListener(() => Hide_buttons());
        }
    }


    void Turn_off_menu()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            int nr_c = buttons[i].transform.childCount;
            for (int j = 1; j < nr_c; j++)
            {
                GameObject child = buttons[i].transform.GetChild(j).gameObject;
                if (child != null)
                    child.SetActive(false);
            }

        }

    }

    //Toggle ON and OFF the dropdown submenu options
    void Menu_Toggle()
    {
        //deactivate the buttons if they are on
        if (menue_active == true)
        {
            for (int i = 0; i < children.Count; i++)
            {
                children[i].SetActive(false);
            }
            menue_active = false;
        }

        //activate the buttons only if prefabs are set
        else
        {
            menue_active = true;
            for (int i = 0; i < children.Count; i++)
            {
                if (prefabs[i] != null)
                {
                    Debug.Log(prefabs[i]);
                    children[i].SetActive(true);
                }
                else
                {
                    Debug.Log("empty");
                }
            }
        }
    }
    void Hide_buttons()
    {
        for (int i = 0; i < children.Count; i++)
        {
            if (children[i] != null)
                children[i].SetActive(false);
        }
    }

    public void OnClick_ChangePrefab(GameObject prefab)
    {
        if (prefab != null)
            Object_Spawner.selectedPrefab = prefab;
            Debug.Log("Prefab changed to: " + Object_Spawner.selectedPrefab);
    }

}


