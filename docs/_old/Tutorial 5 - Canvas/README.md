# ARC596 - Tutorial 5 - Canvas


### Useful Links <a href="_toc86161484" id="_toc86161484"></a>

[→ Unity Manual](https://docs.unity3d.com/Manual/index.html)

[→ More Information on execution order of events in unity](https://docs.unity3d.com/Manual/ExecutionOrder.html)

[→ More information about AR Foundation](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.2/manual/index.html)

[→ Unity Scripting API](https://docs.unity3d.com/ScriptReference/)

[→ Important Classes in Unity Scripting](https://docs.unity3d.com/Manual/ScriptingImportantClasses.html)

[→ User Interface in Unity](https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual/UIVisualComponents.html)

_**Scenes:** _[_https://docs.unity3d.com/Manual/CreatingScenes.html_](https://docs.unity3d.com/Manual/CreatingScenes.html)

_**GameObjects:** _[_https://docs.unity3d.com/ScriptReference/GameObject.html_](https://docs.unity3d.com/ScriptReference/GameObject.html)

_**Prefabs**: _[_https://docs.unity3d.com/Manual/Prefabs.html_](https://docs.unity3d.com/Manual/Prefabs.html)

_**Packages**: _[_https://docs.unity3d.com/Manual/PackagesList.html_](https://docs.unity3d.com/Manual/PackagesList.html)

_**Button:** _[_https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual/script-Button.html_](https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual/script-Button.html)

_**AR Foundation Basics:** _[_https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.0/manual/index.html#samples_](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.0/manual/index.html#samples)

_**AR Raycast Manager: **_[_https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.0/manual/raycast-manager.html_](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.0/manual/raycast-manager.html)

_**AR Plane Manager:**_

[_https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.0/manual/plane-manager.html_](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.0/manual/plane-manager.html)

### &#x20;<a href="_43ofsqpjwh83" id="_43ofsqpjwh83"></a>

### User Interface in Unity <a href="_toc86161485" id="_toc86161485"></a>

#### Button <a href="_toc86161486" id="_toc86161486"></a>

A Button has an **OnClick** UnityEvent to define what it will do when clicked.

![](<../.gitbook/assets/5 (3)>)

#### Slider <a href="_toc86161487" id="_toc86161487"></a>

A Slider has a decimal number **Value** that the user can drag between a minimum and maximum value. It can be either horizontal or vertical. It also has a **OnValueChanged** UnityEvent to define what it will do when the value is changed.

![](<../.gitbook/assets/6 (1)>)

### Raycasters <a href="_toc86161488" id="_toc86161488"></a>

Raycasters are used for figuring out what the pointer is over

#### AR Raycast Manager

Also known as** hit testing,** ray casting allows you to determine where a** **[**ray**](https://docs.unity3d.com/2020.2/Documentation/ScriptReference/Ray.html) (defined by an origin and direction) intersects with a [trackable](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.0/api/UnityEngine.XR.ARFoundation.ARTrackable-2.html). A** "trackable"** is a feature in the physical environment that can be detected and tracked by an** XR device.**

**Example:**

\[SerializeField]\
ARRaycastManager m\_RaycastManager;

List\<ARRaycastHit> m\_Hits = new List\<ARRaycastHit>();

void Update()\
{\
if (Input.touchCount == 0)\
return;

if (m\_RaycastManager.Raycast(Input.GetTouch(0).position, m\_Hits))\
{\
// Only returns true if there is at least one hit\
}\
}

#### Physics Raycaster

Used for 3D physics elements. Casts a ray against all** colliders** in the Scene and returns detailed information on what was hit. This example reports the distance between the current object and the reported Collider:

**Example:**

public class RaycastExample : MonoBehaviour\
{\
void FixedUpdate()\
{\
RaycastHit hit;

if (Physics.Raycast(transform.position, -Vector3.up, out hit))\
print("Found an object - distance: " + hit.distance);\
}\
}

### C# in Unity <a href="_toc86161493" id="_toc86161493"></a>

**Variables** hold values and references to objects. Variables start with a lowercase letter. When Unity compiles the script, it makes **public** variables **visible in the editor**.

**Functions** are collections of code that compare and manipulate these variables. Functions start with an uppercase letter.

**Start Function**– Start will be called if a GameObject is active, but only if the component is enabled.

**Update Function **is called once per frame. This is where you put code to define the logic that runs continuously, like animations, AI, and other parts of the game that have to be constantly updated.

### Overview of the code of day 2 <a href="_toc86161494" id="_toc86161494"></a>

// UI Functions\
**public void SetMode\_A()**\
{\
mode = 0; // for single placement of objects, like the 3D printed house hologram\
}\
**public void SetMode\_B()**\
{\
mode = 1; // for multiple placement of objects, like multiple trees or characters\
}

**These modes are linked with if statements such as- place one instant or multiple**

| <p>else if (mode == 1) //ADD MULTIPLE : create multiple instances of object<br>{<br>Debug.Log("***MODE 1***");<br>Touch touch = Input.GetTouch(0);</p><p>// Handle finger movements based on TouchPhase<br>switch (touch.phase)<br>{<br>case TouchPhase.Began:<br>if (Input.touchCount == 1)<br>{<br>_PlaceInstant(objectParent);<br>}<br>break;<br></p><p>In the unity file you need to link the modes with the buttons:</p><p><img src="../.gitbook/assets/7 (3)" alt=""></p><p><strong>(Drag and drop a script > Select a Public Function to execute)</strong></p><p>This works because the void is set to public and the on click is linked with the <em>instantiator</em> script. The button has therefore access to the public void <strong>SetMode_B()</strong> which sets the mode <strong>to 1.</strong></p> |
| --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| The _Instantiator_ script looks every frame for touch input (as it is in input) and looks for the mode. Depending on the mode and the touch input a different action is activated.                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    |

### Overview - New features of Day 3 <a href="_toc86161495" id="_toc86161495"></a>

#### ![](<../.gitbook/assets/8 (2)>)Edit Mode: <a href="_toc86161496" id="_toc86161496"></a>

* **Tap once** on the object to select it ( you will see a white bounding box appear ).
* **Tap and hold for 1 second. **The bounding box will become yellow. Without lifting your finger, try to move the object around.

#### ![](<../.gitbook/assets/9 (2)>)Delete Mode: <a href="_toc86161497" id="_toc86161497"></a>

* **Tap once **on an object to delete it immediately

#### ![](<../.gitbook/assets/10 (3)>)Menu <a href="_toc86161498" id="_toc86161498"></a>

* When pressed, new buttons_ pop up._

![](<../.gitbook/assets/11 (3)>)**Visibility Button: **Turns on and off the visibility of the 3D printed object

![](../.gitbook/assets/12)**Reset Button :** Resets all scanned planes and deletes all objects in Scene.

![](<../.gitbook/assets/13 (1)>)**Play Button :** Starts animating the characters.

#### ![](<../.gitbook/assets/14 (1)>)Interactive Sunlight <a href="_toc86161499" id="_toc86161499"></a>

![](<../.gitbook/assets/15 (1)>)

In short, we will import 2 interactive sliders to manipulate the Sunlight, by changing the Altitude and the Azimuth. By changing these values, we are in fact iterating through seasons and hours of the day.

### Import UnityPackage for Day 3 <a href="_toc86161500" id="_toc86161500"></a>

* Go to **Assets>Import Package>Custom Package**

![](<../.gitbook/assets/16 (2)>)

* Select the** .unitypackage** file you downloaded for Day 03

![](<../.gitbook/assets/17 (3)>)

* **Select all Assets** and clic**k Import**

![](<../.gitbook/assets/18 (2)>) ![](../.gitbook/assets/19)

* You should be able to see a new **Day 03 Assets folder **in your Assets.

### Configure new GameObjects <a href="_toc86161501" id="_toc86161501"></a>

_Since we imported new Prefabs and we want to incorporate them in our scene, we have to do some necessary steps to Relink some dependencies for Scripts and Buttons._

### Instantiator <a href="_toc86161502" id="_toc86161502"></a>

* Since we have a **new** Instantiator for Day 03, we will **delete** the previous Instantiator _GameObject._

![](<../.gitbook/assets/20 (2)>)

* **Import as GameObject**

Drag and Drop the Instantiator Prefab from the Day03 Materials to the Hierarchy.

* **Unpack GameObject**

Respectively, we will** Unpack the Prefab Completely**

* **Relink GameObject**

Since we imported Instantiator again, we have to **re-link the previous GameObject, where needed.**

### AR Canvas - New Buttons <a href="_toc86161503" id="_toc86161503"></a>

* **Import as GameObject**

Drag and drop the **AR Canvas **Prefab of **Day 03 Asset Folde**r to the Hierarchy

**Note: **Notice that it is renamed to _AR\_Canvas(1)_. This happens because the _Prefab_ has the same name as the_ Menu._ We will take the_ Children_ we need and import them in our existing AR\_Canvas.

* **Unpack Prefab**

Respectively,** unpack completely** the AR\_Canvas(1).

* **Format AR Canvas**

These are the 4 buttons we will import in our existing AR\_Canvas.\[1] Drag them into the existing AR\_Canvas, **as children. **The structure should look like this \[2]

\[1]\[2]

Delete the existing Reset button (there is already one included in the new Menu).

* **Delete the AR\_Canvas(1) **(Should be now an empty GameObject).

### Relink Menu Buttons <a href="_toc86161504" id="_toc86161504"></a>

We will relink the buttons on their **OnClick** Modes.

* Go to your **Menu Buttons** from Day 2, go to the **Inspector, **and Drag & Drop the** Instantiator **on the **On Click ()** component on the Inspector.
* Re-configure the Function for the button you need (**Mode A **= Instantiate one (for the 3d object) , **Mode B **= Instantiate multiple).

### Link new modes to the new Instantiator Script <a href="_toc86161505" id="_toc86161505"></a>

#### Link Edit Button (mode==2) <a href="_toc86161506" id="_toc86161506"></a>

In this new mode, we will be able to select existing objects we created, scale, rotate and move them**. ( AR Canvas > Edit\_Button)**

_Drag and drop _the Instantiator GameObject and choose: **Instantiator> Instantiator.SetMode\_C()**

#### Link Delete Button (mode ==3) <a href="_toc86161507" id="_toc86161507"></a>

In this mode, we will be able to delete existing objects by tapping on them.

_Drag and drop_ the Instantiator GameObject and choose: **Instantiator> Instantiator.SetMode\_D()**

**. ( AR Canvas >Delete\_Button)**

#### Link Visibility Button <a href="_toc86161508" id="_toc86161508"></a>

**. ( AR Canvas >Menu> Shadow\_Button) ///Directional\_Light > Sun.VisibilityToggle()**

#### Link Reset Button <a href="_toc86161509" id="_toc86161509"></a>

**. ( AR Canvas >Menu> Reset\_Button) /// Instantiator > Instatiator.ResetApp()**

&#x20;

### Light <a href="_toc86161510" id="_toc86161510"></a>

* **Delete Previous Light**

We will import a new Light in the Scene, so let’s **delete **the existing Directional Light in the _Hierarchy._

* **Import as GameObject**

We will now **drag and drop **the Directional Light _Prefab_ from the** Day 03 Asset Folder** to the **Hierarchy.**

* **Unpack Prefab Completely**

Right Click on the** Directional Light > Prefab> Unpack Completely.** This will “detach” the link between the GameObject and the Prefab.

### Sun Sliders <a href="_toc86161511" id="_toc86161511"></a>

#### Link Sliders to the Script <a href="_toc86161512" id="_toc86161512"></a>

Click on the **Directional Light** we imported, go to the _Inspecto_r on the** Sun Script** and_ drag and drop _the 2 **sliders** we imported from the _Day 3 Assets package._

_Drag and drop t_he** Azimuth **and **Altitude slider** (children in AR Canvas) to the** Sun Script** in the _Inspector. _Drag and drop the parent of the 3d object (houseParent).

**You should be able to see the newly added UI Menu on the Right.**

### Build the App <a href="_toc86161513" id="_toc86161513"></a>

**Test the new features!**

### Menu Script <a href="_toc86161514" id="_toc86161514"></a>

Go to the new** Menu** GameObject that was imported. On the_ Inspector,_ find the **MenuScript** (C#** **Script component), double click and open the code.

### Script Overview <a href="_toc86161515" id="_toc86161515"></a>

public class Menuscript : MonoBehaviour\
{\
//variables\
public GameObject Shadow\_Button;\
public GameObject Reset\_Button;\
public GameObject Animation\_Button;

// Start is called before the first frame update\
void Start()\
{\
//For each button, define OnClick Action and prefab\
**Button btn = GetComponent\<Button>();**\
**btn.onClick.AddListener(Menu\_Toggle);**

}

//Toggle ON and OFF the dropdown submenu options\
private void Menu\_Toggle()\
{\
//deactivate the buttons if they are on\
**if (Shadow\_Button.activeSelf == true)**\
{\
Shadow\_Button.SetActive(false);\
Reset\_Button.SetActive(false);\
Animation\_Button.SetActive(false);\
}\
**else**\
{\
Shadow\_Button.SetActive(true);\
Reset\_Button.SetActive(true);\
Animation\_Button.SetActive(true);\
}\
}\
}

_→ Here the** “AddListener”** redirects to the Menu\_Toggle() function. This is a computationally efficient way of checking if a button is clicked to execute a function._

_→ This Menu Script turns on and off different buttons. This allows us to create a “pop up” Menu that includes multiple buttons, and make the UI Experience more compact._

### New Instantiator Script - Overview <a href="_toc86161516" id="_toc86161516"></a>

&#x20;→ Go to the _Instantiator C# _Script.

### How we set a mode <a href="_toc86161517" id="_toc86161517"></a>

\[Line 402]

public void SetMode\_A()\
{\
mode = 0; // for single placement of objects, like the 3D printed house hologram\
}\
public void SetMode\_B()\
{\
mode = 1; // for multiple placement of objects, like multiple trees or characters\
}\
public void SetMode\_C()\
{\
mode = 2; // for editing existing objects\
}\
public void SetMode\_D()\
{\
mode = 3; // for deleting objects\
}

### How these modes are used <a href="_toc86161518" id="_toc86161518"></a>

\[Line 61]

private void \_InstantiateOnTouch()\
{\
**if (Input.touchCount > 0)** //if there is an input..\
{\
if (PhysicRayCastBlockedByUi(Input.GetTouch(0).position))\
{\
**if (mode == 0) **// ADD ONE : destroy previous object with every tap\
{\
Debug.Log("\*\*\*MODE 0\*\*\*");\
**Touch touch = Input.GetTouch(0);**

// Handle finger movements based on TouchPhase\
switch (touch.phase)\
{\
case TouchPhase.Began:\
if (Input.touchCount == 1)\
{\
**\_PlaceInstant(houseParent);**\
}\
break; //break: If this case is true, it will not check the other ones. More computational efficiency,

case TouchPhase.Moved:\
// Record initial touch position.\
if (Input.touchCount == 1)\
{\
**\_Rotate(ARObject\_new);**\
}

if (Input.touchCount == 2)\
{\
\_**PinchtoZoom(ARObject\_new);**\
}\
break;

case TouchPhase.Ended:\
Debug.Log("Touch Phase Ended.");\
break;\
}\
}\
**else if (mode == 1) **//ADD MULTIPLE : create multiple instances of object\
{

Debug.Log("\*\*\*MODE 1\*\*\*");\
Touch touch = Input.GetTouch(0);

// Handle finger movements based on TouchPhase\
switch (touch.phase)\
{\
case TouchPhase.Began:\
if (Input.touchCount == 1)\
{\
**\_PlaceInstant(objectParent);**\
}\
break;

case TouchPhase.Moved:\
// Record initial touch position.\
if (Input.touchCount == 1)\
{\
**\_Rotate(ARObject\_new);**\
}

if (Input.touchCount == 2)\
{\
**\_PinchtoZoom(ARObject\_new);**\
}\
break;

case TouchPhase.Ended:\
Debug.Log("Touch Phase Ended.");\
break;\
}\
}

**else if (mode == 2)** **//EDIT MODE**\
{\
Debug.Log("\*\*\*MODE 2\*\*\*");\
\_EditMode();\
}

**else if (mode == 3)** **//DELETE MODE**\
**{**\
Debug.Log("\*\*\*MODE 3\*\*\*");\
activeGameObject = SelectedObject();\
**\_DestroySelected(activeGameObject);**\
}\
**else**\
{\
Debug.Log("Press a button to initialize a mode");\
}\
}\
}\
}

### EditMode() Function <a href="_toc86161519" id="_toc86161519"></a>

Line 278

**mode==2**

**private void \_EditMode()**\
{\
**if (Input.touchCount == 1) **//try and locate the selected object only when we click, not on update\
{\
**activeGameObject = SelectedObject();**\
}\
**if (activeGameObject != null) **//change the pinch and zoom **place holder **only when we locate a new object\
{\
temporaryObject = activeGameObject;\
\_**addBoundingBox(temporaryObject); **//add bounding box around selected game object\
}

\_**Move(temporaryObject);**\
**\_PinchtoZoom(temporaryObject);**

}

### Find Selected Object by RayCasting <a href="_toc86161520" id="_toc86161520"></a>

Note: This This function **Returns **an object (the activeGameObject), when the** Raycast** hits the** Physics collider** of that object.

**private GameObject SelectedObject(GameObject activeGameObject = null)**\
{\
Touch touch;\
touch = Input.GetTouch(0);

//delete the previous selection boundary, will be replaced with a new one

if (Input.touchCount == 1 && touch.phase == TouchPhase.Ended)\
{\
Debug.Log("Single Touch");\
**List\<ARRaycastHit> hits = new List\<ARRaycastHit>();**\
**rayManager.Raycast(touch.position, hits);**

if (hits.Count > 0)\
{

Debug.Log("Ray shooting from camera");\
**Ray ray = arCamera.ScreenPointToRay(touch.position);**\
RaycastHit hitObject;

//if our touch hits an existing object, we find that object\
**if (Physics.Raycast(ray, out hitObject))**\
{\
//we make sure we didn't tap a plane\
if (hitObject.collider.tag != "plane")\
{\
//setting the variable\
Debug.Log("Selected object located");\
activeGameObject = hitObject.collider.gameObject; //assign GameObject as the active\
Debug.Log(activeGameObject);

}\
}\
}\
}

### Delete mode <a href="_toc86161521" id="_toc86161521"></a>

**mode==3**

In delete mode, we use the same script to locate the Raycast hit Object, and then instead of Moving, Rotating or Scaling, we just use the **Destroy() **function.

\[line 141]

else if (mode == 3) //DELETE MODE\
{\
Debug.Log("\*\*\*MODE 3\*\*\*");\
activeGameObject = SelectedObject();\
**\_DestroySelected(activeGameObject)**;\
}

\------------------------------------------------------------------------

\[line 394]

private void \_DestroySelected(GameObject gameObjectToDestroy)\
{\
**Destroy(gameObjectToDestroy);**\
}

_**→ Pro Tip**: If you want to go directly to a function you see in the code, you can **CTRL+ click **on the name. (e.g. here we would CTRL+click on the \_DestroySelected(activeGameObject))_

### Interactive Sunlight <a href="_toc86161522" id="_toc86161522"></a>

In the new Directional Light we imported, there is a custom _C# script_ attached named **“Sun”.**

Here, we link the position and rotation of our Sunlight according to the slider values we have on our UI Canvas, which we manipulate on the fly.

Also, we use this script to turn **ON/OFF **the **visibility o**f our 3D printed object (that’s why we use the House Parent GameObject).

Let’s take a look at the scripts.

### Azimuth - Altitude <a href="_toc86161523" id="_toc86161523"></a>

#### AddListener <a href="_toc86161524" id="_toc86161524"></a>

**On line 40,** we add listeners for everytime we change the slider for each parameter.

azimuth\_slider**.onValueChanged.**AddListener(AdjustLatitude);\
altitude\_slider**.onValueChanged.**AddListener(AdjustLongitude);

#### Adjust Values <a href="_toc86161525" id="_toc86161525"></a>

**On line 45**, we assign these new values in the** AdjustTime() **function.

public void AdjustAzimuth(float value)\
{\
New\_Azimuth = value;\
AdjustTime(New\_Azimuth, New\_Altitude);\
}

public void AdjustAltitude(float value)\
{\
New\_Altitude = value;\
AdjustTime(New\_Azimuth, New\_Altitude);\
}

#### Adjust Sun transform <a href="_toc86161526" id="_toc86161526"></a>

**On line 69, **we adjust the position of our 3D Sphere object, according to the Azimuth and Altitude values.

The centerpoint of this sphere, is the House (the 3D printed object)

if (house!=null)\
{\
**coordPosition.x** = radius\*Mathf.Cos(New\_Azimuth\*Mathf.Deg2Rad)\*Mathf.Cos(New\_Altitude\*Mathf.Deg2Rad);\
**coordPosition.z** = radius\*Mathf.Cos(New\_Azimuth\*Mathf.Deg2Rad)\*Mathf.Sin(New\_Altitude\*Mathf.Deg2Rad);\
coordPosition.y = radius\*Mathf.Sin(New\_Azimuth\*Mathf.Deg2Rad);

**coordPosition** += centerpoint;\
sun.transform.position = new Vector3(coordPosition.x, coordPosition.y, coordPosition.z);\
sun.transform.LookAt(house.transform);\
}

_**Note: **We use the LookAt command, to rotate the sunlight, by making it “look” at our object each time it is moving._

### Visibility Toggle <a href="_toc86161527" id="_toc86161527"></a>

In the same C# Script (Sun), we use the function **VisibilityToggle() **to be able to turn ON/OFF the visibility of the 3d model, while still keeping the shadows of it.

public void VisibilityToggle()

{\
**//\*\*Preview ON/Off the house 3dmodel\*\***\
\
//Check if the house is instantiated\
**if (houseParent.transform.childCount != 0)**\
house = houseParent.transform.GetChild(0).gameObject;

**if(house != null)**\
{\
//Get access to the model obj and adjust the MeshRenderer parameters\
GameObject **obj** = house.transform.GetChild(0).gameObject;\
Debug.Log(obj);

**if (shadowMode == 0)**\
{\
**obj**.GetComponent\<MeshRenderer>().shadowCastingMode = **UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;**\
**shadowMode = 1;**\
}\
else\
{\
obj.GetComponent\<MeshRenderer>().shadowCastingMode = **UnityEngine.Rendering.ShadowCastingMode.On;**\
**shadowMode = 0;**\
}\
}

### Animation <a href="_toc86161528" id="_toc86161528"></a>

### “Animanimals” Script <a href="_toc86161529" id="_toc86161529"></a>

Go to the **Prefabs**, on the Animated Characters from day 2.

These downloaded_ Assets come_ with different **Animations** embedded in the prefab. This means that by assigning different functions, they can “switch” their animation to the desired preset.

Our goal is to activate the** “Walk” animation,** so that our characters can walk around in the _Augmented Reality environment._

#### Add Animanimals Script <a href="_toc86161530" id="_toc86161530"></a>

1. Select the animated character Prefab.
2. Go to the_ Inspecto_r, click on _“Add Component”_, type in **“ Animanimals ”** and import the Script.

#### Overview of Animanimals Script <a href="_toc86161531" id="_toc86161531"></a>

Open the code by double clicking on the **Animanimals **Script.

First, we locate the Button we need (Which is named “Animation\_Button, in our AR Canvas)

And then** Add a Listener **to it. When it is clicked, the_ ControllPLayer() v_oid is called. This sets a value of either **1** or **0 **(1 is for moving = “Walk”).

_When the “Animation\_Button” button is clicked, the animation “Walk” is activated, and our characters start to move._



void Start()\
{\
**anim = gameObject.GetComponent\<Animator>();**\
rb = GetComponent\<Rigidbody>();\
//Button btn = Anim\_Button.GetComponent\<Button>();\
**btn = GameObject.Find("/AR\_Canvas/Menu/Animation\_Button").GetComponent\<Button>(); **//.GetComponent\<Button>();\
btn.onClick.AddListener(**ControllPlayer**);\
}

void Update()\
{\
Debug.Log(move);\
if (move)\
{\
**Vector3 movement = transform.forward;**\
**transform.Translate(movement \* movementSpeed \* Time.deltaTime, Space.World);**\
}\
}

\
public void **ControllPlayer()**\
{\
Debug.Log("walk");\
**anim.SetInteger("Walk", 1);**\
if (move)\
{\
move = false;\
}\
else{\
move = true;\
}\
Debug.Log("move");\
}

### Colliders <a href="_toc86161532" id="_toc86161532"></a>

In order for our animations and placement of the objects to work, we have to make sure our imported Prefabs have Colliders and are Rigid Bodies.

**Collider components** define the shape of a GameObject for the purposes of **physical collisions**. A collider, which is **invisible**, does not need to be the exact same shape as the GameObject’s mesh.

_→ Our cat has a Box Collider attached to it._

A rough approximation of the mesh is often more efficient and indistinguishable in gameplay.The simplest (and least processor-intensive) colliders are primitive collider types. In 3D, these are the [Box Collider](https://docs.unity3d.com/Manual/class-BoxCollider.html), [Sphere Collider](https://docs.unity3d.com/Manual/class-SphereCollider.html) and [Capsule Collider](https://docs.unity3d.com/Manual/class-CapsuleCollider.html).

#### Mesh Collider (Component) <a href="_toc86161533" id="_toc86161533"></a>

[→ Documentation Here](https://docs.unity3d.com/560/Documentation/Manual/class-MeshCollider.html#:\~:text=The%20Mesh%20Collider%20takes%20a,collide%20with%20other%20Mesh%20Colliders.)

The Mesh Collider builds its collision representation from the [Mesh](https://docs.unity3d.com/560/Documentation/Manual/class-Mesh.html) attached to the GameObject, and reads the properties of the attached [Transform](https://docs.unity3d.com/560/Documentation/Manual/class-Transform.html) to set its position and scale correctly. The benefit of this is that you can make the shape of the Collider exactly the same as the shape of the visible Mesh for the GameObject, resulting in more precise and authentic collisions. However, this precision comes with a higher processing overhead than collisions involving primitive colliders (such as Sphere, Box, and Capsule) and so it is best to use Mesh Colliders sparingly.

_**→ Check if all of your prefabs have some sort of Collider.**_

### Rigid Body (Component) <a href="_toc86161534" id="_toc86161534"></a>

**Rigidbodies **enable your _GameObjects_ to act under the control of **physics.** The Rigidbody can receive _forces_ to make your objects move in a realistic way. Any _GameObject _must contain a** Rigidbody **to be influenced by **gravity,** act under added forces via scripting, or interact with other objects.

#### Rigid Body Properties <a href="_toc86161535" id="_toc86161535"></a>

| **Property:**    | **Function:**                                                                                                                                                                                                            |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| **Mass**         | The mass of the object (in kilograms by default).                                                                                                                                                                        |
| **Drag**         | How much air resistance affects the object when moving from forces. 0 means no air resistance, and infinity makes the object stop moving immediately.                                                                    |
| **Angular Drag** | How much air resistance affects the object when rotating from torque. 0 means no air resistance. Note that you cannot make the object stop rotating just by setting its Angular Drag to infinity.                        |
| **Use Gravity**  | If enabled, the object is affected by gravity.                                                                                                                                                                           |
| **Is Kinematic** | If enabled, the object will not be driven by the physics engine, and can only be manipulated by its Transform. This is useful for moving platforms or if you want to animate a Rigidbody that has a HingeJoint attached. |

_→ The Rigidbody component is what makes our animals “fall”, or “move around”._

### Develop your App further <a href="_toc86161536" id="_toc86161536"></a>

**Now, it is time to continue developing your Augmented Reality environment setup!**

**Good luck!**
