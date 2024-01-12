# ARC596 - Tutorial 4 - Basic Unity AR App

- ARC596: Embodied Computation
- Professor: Daniela Mitterberger - mitterberger@princeton.edu
- Assistant Instructor: Kirill Volchinskiy - kvolchinskiy@princeton.edu
- Tutorial 4 - Basic Unity AR App

### Requirements

1. [Rhinoceros 7](https://www.rhino3d.com/en/7/)
2. [Github Desktop](https://desktop.github.com/) 
3. [Anaconda](https://www.anaconda.com/)
4. [Unity 2022.3.3f1](https://unity.com/) 
>	*Note: Android SDK and Java JDK (when developing for Android) - have to be ticked in the installation modules when installing Unity.*

### Dependencies

1. [COMPAS](https://compas.dev)
2. [COMPAS Fab - Fabrication Library for Robots](https://gramaziokohler.github.io/compas_fab/latest/)
3. [COMPAS Eve - Communication](https://compas.dev/compas_eve/latest/index.html)
4. [Vuforia](https://developer.vuforia.com/downloads/sdk)
5. [ROS#](https://www.ros.org/)

## Getting Started with This Project

1. Install the correct Unity Version using Unity Hub ```2022.3.3f1``` https://unity.com/releases/editor/whats-new/2022.3.3
      - *Note: Do not click the blue download link. If you download directly from this website it is cubersome to install the dependencies. Unity is version-sensitive, and it needs to be exactly this version*
	  <img width="400" alt="" src=" https://i.imgur.com/cqnSaTm.jpg">
	 
	- Be sure to select the appropriate dependencies below.
      - *Note: this installs roughly 20gb of data, make sure you have enough free space on your computer.*
		<img width="600" alt="" src="https://i.imgur.com/D9o3zho.jpg">
		
		- Microsoft Visual Studio 
		- Android Build Support (Both Android SDK and OpenJDK)
		- iOS Build Support
		- Universal Windows Platform Build Support
		- Windows Build Support (IL2CPP)
		- Documentation
	
2. Run Unity Hub. Create a new Unity Project. Be sure to select the correct Unity Version as basis for the project ```2022.3.3f1```

	<img width="650" alt="" src="https://i.imgur.com/I77ndmz.jpg">
	
	
3. Install Unity Packages for XR (Mixed Reality)

	- Access the Package Manager
	
		<img width="650" alt="" src="https://i.imgur.com/iQD4avo.png">
	
	- Switch to the Unity Registry 		
		
		<img width="400" alt="" src="https://i.imgur.com/73kTqaN.png">
	
	- Search & Install ```AR Foundation```, repeat for ```ARCore XR plugin```
		
		<img width="650" alt="" src="https://i.imgur.com/x0k6B1T.png">
	
	- Add ARCore Unity Extensions. Select ```Add package from git URL```, https://github.com/google-ar/arcore-unity-extensions.git
		
		<img width="400" alt="" src="https://i.imgur.com/JDOpw0S.png">

	- Verify ```AR Foundation```, ```ARCore XR plugin```, ```ARCore Extensions``` are installed
		
		<img width="650" alt="" src="https://i.imgur.com/4LUeQrF.png">
	

# BEGIN ETH #
<!---------------------- END ETH   ----->


### Unity Interface 

<img width="650" alt="" src="https://i.imgur.com/fctlEa8.png">

### The Scene View 

A Scene contains the environments and menus of your game. Think of each unique Scene file as a unique level. In each Scene, you place your environments, obstacles, and decorations, essentially designing and building your game in pieces. Scene view is your interactive view into the world you are creating. You can use the Scene view to select and position scenery, characters, Cameras , lights, and all other types of Objects. Selecting, manipulating, and modifying GameObjects in the Scene view are some of the first skills you must learn to begin working in Unity.

→ [More info](https://docs.unity3d.com/Manual/CreatingScenes.html)

### Scene View Navigation 

<img alt="" src="https://i.imgur.com/RaSGErB.png">

The Gizmo allows you to quickly modify the viewing angle and the projection mode.

#### Arrow movement

You can use the** Arrow Keys** to move around the Scene as though “walking” through it. The up and down arrows move the _Camera_ forward and backward in the direction it is facing. The** left **and **right** arrows pan the view **sideways**.

#### Basic Tools

You can use the following Buttons to select, move, rotate, scale. This toolbar is located on the top left corner.

<img width="300" alt="" src="https://i.imgur.com/TYhoU7d.png">

1. **Hand** Tool (Keyboard Shortcut: **Q**)
2. **Move** Tool (Keyboard Shortcut: **W**)
3. **Rotate** Tool (Keyboard Shortcut: **E**)
4. **Scale** tool (Keyboard Shortcut: **R**)

### The Game View 

The Game view is rendered from the Camera(s) in your application. It represents your final, published application. You need to use one or more Cameras to control what the player sees when they are using your application. In AR, we will see a mixed environment between real and digital objects, as the camera orientation is constantly changing by our movement in space.

**Play mode**

<img width="100" alt="" src="https://i.imgur.com/Np9EDMS.png">

Use the buttons in the _Toolbar _to control the Editor Play mode and see how your published application plays. An important fact is that, in** Play mode**, any changes you make are **temporary,** and are **reset **when you exit Play mode.

[→ More info](https://docs.unity3d.com/Manual/GameView.html)

### The Hierarchy Window

The Hierarchy window displays every [GameObject](https://docs.unity3d.com/Manual/GameObjects.html) in a Scene, such as models, Cameras, or [Prefabs](https://docs.unity3d.com/Manual/Prefabs.html). You can use the Hierarchy window to sort and group the GameObjects you use in a Scene.

![The default Hierarchy window view when you open a new Unity project]

<img alt="" src="https://i.imgur.com/0Xs2azM.png">

The default Hierarchy window view when you open a new Unity project

### The Inspector Window 

The** Inspector Window** is being used to [view and edit **properties**](https://docs.unity3d.com/Manual/EditingValueProperties.html) and** settings** for almost everything in the_ Unity Editor_. In the _Inspector Window_, one can add or remove _“Components”_ , which enable different features.. We will see what these are soon. [→ More Info](https://docs.unity3d.com/Manual/UsingTheInspector.html)![](<../.gitbook/assets/10 (1)>)

### The Project Window 

<img width="650" alt="" src="https://i.imgur.com/YtMU8b9.png">

The_ Project window_ displays all of the** files **related to your_ Project _and is the main way you can navigate and find_ Assets _and other _Project files_ in your application. When you start a new _Project _by default this window is open. However, if you cannot find it, or it is closed, you can open it via **Window > General > Project** or use the keyboard command **Ctrl + 9** (Command + 9 on macOS).

### GameObjects, Components, Prefabs 

### Game Objects 

<img width="650" alt="" src="https://i.imgur.com/yjd1wvG.png">

**GameObjects **are the fundamental objects in _Unity _that represent _3d objects, props and scenery_. They do not accomplish much in themselves but they act as containers for **Components**, which implement the real **functionality**.

A _GameObject _always has a [Transform](https://docs.unity3d.com/Manual/class-Transform.html) component attached (to represent position and orientation) and it is not possible to remove this. The other _components_ that give the object its functionality can be added from the editor’s Component menu or from a script. There are also many useful _pre-constructed o_bjects (_primitive shapes, Cameras,_ etc) available on the **GameObject > 3D Object menu** (more info: [Primitive Objects](https://docs.unity3d.com/Manual/PrimitiveObjects.html) )

### Components 

**Components** implement** functionalities** on the _GameObjects_. For example, a_ Ligh_t object is created by attaching a_ Light component _to a_ GameObject. _**Components** are contained by GameObjects. _Unity_ has many** built-in** components, and you can create your own by writing scripts that inherit from _MonoBehaviour_. _(scripting language in C#)_

_→ _[_More Info about Scripting in Unity_](https://docs.unity3d.com/Manual/CreatingComponents.html)

<img width="650" alt="" src="https://i.imgur.com/cqqIjt9.png">

A simple Cube GameObject with several Components

### Prefabs 

**Prefabs **allow you to create, configure, and store a _GameObject _complete with all its _components, property values, and child GameObjects._The **Prefab Asset **acts as a **template **from which you can create new Prefab instances in the Scene.

[→ More Info](https://docs.unity3d.com/Manual/Prefabs.html)

### Create Game Objects and Basic Transformations: Quick Tutorial

### Parenting

Unity uses the concept of parent-child hierarchies, or parenting, to group GameObjects. An object can contain other GameObjects that inherit its properties. You can link GameObjects together to help move, scale, or transform a collection of GameObjects. When you move the top-level object, or parent GameObject, you also move all child GameObjects. You can also create nested parent-child GameObjects. All nested objects are still descendants of the original parent GameObject, or root GameObject.Child GameObjects inherit the movement and rotation of the parent GameObject. To learn more about this, see documentation on the 
>   [Transform component](https://docs.unity3d.com/Manual/class-Transform.html)![Child 1 and Child 2 are the child GameObjects of Parent. Child 3 is a child GameObject of Child 2, and a descendant GameObject of Parent.]


<img alt="" src="https://i.imgur.com/9xteFSv.png">


_**Child 1** and **Child 2** are the child GameObjects of **Parent**. **Child 3** is a child GameObject of **Child 2**, and a descendant GameObject of **Parent**._

### GameObjects 

### Creating new GameObjects 

To create a new GameObject in the Hierarchy window:

1. Right-click on empty space in the selected Scene.
2. Select the GameObject you want to create. Let’s make a **cube**_GameObject _ by selecting **3D Object > Cube.**
3. Now let’s make another _**empty** GameObject._

→ You can also press **Ctrl+Shift+N** (Windows) or **Command+Shift+N** (macOS) to create a new empty GameObject.

<img height="400" alt="" src="https://i.imgur.com/zsk1bg1.png"> <img height="400" alt="" src="https://i.imgur.com/pKyxJiL.png">
 

### Creating child GameObjects 

To create a _child GameObject_:

1. Drag the GameObject onto the parent GameObject in the Hierarchy.
2. Drag _Object 4_ (selected) onto the parent _GameObject_, _Object 1 _(highlighted in a blue) to create a child GameObject.

![Drag Object 4 (selected) onto the parent GameObject, Object 1 (highlighted in a blue) to create a child GameObject.](<img width="250" alt="" src="https://i.imgur.com/JXuUcVX.png">)

### Creating parent GameObjects 

You can add a new GameObject into the Hierarchy view as the parent of existing GameObjects.

To create a parent GameObject:

1. Right-click a _GameObject,_ or select multiple _GameObjects _on the same level and right-click.
2. Select** Create Empty Parent.**

You can also press **Ctrl+Shift+G **(Windows) or **Command+Shift+G **(macOS) to create a parent GameObject.

→ You can also **click and drag **GameObjects inside, or outside parent GameObjects.

![Drag Object 4 (selected) between Object 2 and Object 3 (indicated by the blue horizontal line), to create a sibling GameObject under the parent GameObject Object 1 (highlighted in a blue).](<../.gitbook/assets/19 (2)>)

### Duplicating GameObjects <a href="_toc86085559" id="_toc86085559"></a>

To duplicate _GameObjects,_ right-click the target GameObject and select **Duplicate.**

You can also press **Ctrl+D **(Windows) or **Command+D **(macOS) to duplicate the selected GameObject.

### Task: <a href="_toc86085560" id="_toc86085560"></a>

Try inserting multiple _GameObjects _like _cubes_, _spheres _and _Planes _in the Scene. Use the basic tools to **Move, Rotate and Scale** the Objects and go to the I**nspector Window**. Try to transform them Manually by Inserting Values on** Position, Rotation and Scale.**

![](<../.gitbook/assets/20 (1)>)

**Note: **To quickly** reset all the Transform values,** you can right click on the “Transform” title, and click **“Reset”**

![](<../.gitbook/assets/image (9).png>)

### Make a new Material <a href="_toc86085561" id="_toc86085561"></a>

1. In the** Project Window**, go to **Assets**, right click on the empty space and make a new folder by going to **Create> Folder.** Name it “Materials”.
2. Double click on the folder, right click on the empty space and make a new material by going to **Create>Material. **A new Material is created.
3. Click on the color palette on the_ Inspector _and select a color of your choice.\

4. Drag and drop the material from **the Assets Folder** on the desired objects

**Note: **You can explore different Material Properties, such as** Transparency** (in Rendering Mode)**, Emission **(“glowy” effect, especially when combined with Bloom Rendering effects in Gaming), or add a **Texture **as a Map, similarly to other softwares.

![](<../.gitbook/assets/image (14).png>)

1. Move the **camera **to look at the cube.

You see a _small icon_ appearing at the bottom, showing how _the Game Mode_ will look like through this _camera._

_**Tip**: If you don’t see the camera and light icons, try to click on the “**Gizmos”button** on top._

### Events in Unity <a href="_toc86085562" id="_toc86085562"></a>

![](<../.gitbook/assets/image (15).png>)

An **event is a message** sent to an object to signal that an event happened such as the pressing of a button has occurred. An event sender pushes notifications that an event happened and a receiver receives that notification and defines to respond to it. The object that raises the event is called the **event sender**. The event sender doesn’t know which object will receive the events it raises.

### Event Functions <a href="_toc86085563" id="_toc86085563"></a>

A script in Unity is not like the traditional idea of a program where the code runs continuously in a loop until it completes its task. Instead, Unity passes control to a script intermittently by calling certain functions that are declared within it. Once a function has finished executing, control is passed back to Unity. These functions are known as event functions since they are activated by Unity in response to events that occur during gameplay. Unity uses a naming scheme to identify which function to call for a particular event. For example, you will already have seen the** Update** function (called before a frame update occurs) and the **Start **function (called just before the object’s first frame update).

_Many more event functions are available in Unity; the full list can be found in the script reference page for the _[_MonoBehaviour_](https://docs.unity3d.com/ScriptReference/MonoBehaviour.html)_ class\[1] along with details of their usage. The following are some of the most common and important events._

### Create a C# script for keyboard interaction <a href="_toc86085564" id="_toc86085564"></a>

Let’s create our first C# script, to interact with the Cube we made previously.

1. Click on the cube GameObject you created, go to the Inspector and click “Add Component”(1). Go to **New Script , **name it **“Zoom” **and click on **“Create and Add” **(2).

![](<../.gitbook/assets/image (6) (1).png>)

1. **Double click **on the** Scrip**t name, or** right click>Edit Script**



Open the file with **Visual Studio 2019. **Login with _your Microsoft account_, or create a new one.

The Visual studio interface should appear. Let’s take a look at the structure of the script.

```
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zoom : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


```

### MonoBehaviour: <a href="_toc86085565" id="_toc86085565"></a>

Unity’s base class \[2]that all scripts should derive from if they are used as components.

MonoBehaviour is a part of the** UnityEngine **namespace and implements a big list of event functions.

### Start and Update <a href="_toc86085566" id="_toc86085566"></a>

Unity adds the Start and Update methods by default to all new scripts.

**Start:** Runs once, at the first time the component is initialized.

**Update:** Runs on every frame (e.g. if it is 60fps, it runs 60 times per second!)

_→ We will write a script in **Update**, but will create an Event that triggers the command only when we press a button_.

### Declare a variable <a href="_toc86085567" id="_toc86085567"></a>

Insert the **cube GameObject **variable as a public gameObject (public = we can access it outside of the script and change it).

_**Note:** Remember to always put a semicolon (;) at the end of every line_

![](../.gitbook/assets/image.png)

```
public class Zoom : MonoBehaviour
{
public GameObject cube;
// Start is called before the first frame update
void Start()
{
}
// Update is called once per frame
void Update()
{
}
}
```

### Add an event in an “if” statement <a href="_toc86085568" id="_toc86085568"></a>

We put the statement in the update, so that the script is checking constantly for our input.

![](<../.gitbook/assets/image (12).png>)

```
    void Update()
    {
        if(Input.GetKey(KeyCode.Space))
        {
            //do something here
        }
 }
```

### Add an action <a href="_toc86085569" id="_toc86085569"></a>

```
     void Update()
    {
        if(Input.GetKey(KeyCode.Space))
        {
            cube.transform.localScale += (Vector3.one*1.00001f)/100 ;
        }
    }
```

we added a very small value, since it will move very fast

1. **Save** the script, go back to_ Unity_ and** Play** the Scene.

Long Press the **SpaceBar** of your keyboard and watch the cube zoom exponentially.

![](<../.gitbook/assets/image (13).png>)

_→ Task: Change the code from **cube.transform.localScale += (Vector3.one\*1.00001f)/100 ;**_

_To **transform.localScale += (Vector3.one\*1.00001f)/100 ; **. Save it and run the code again. It still works because the script is self-referencing the object that the component is added to._

### Download animated characters <a href="_toc86085570" id="_toc86085570"></a>

Go to the Unity Asset Store:

[https://assetstore.unity.com/](https://assetstore.unity.com)

→ The** Unity Asset Store **is home to a growing library of **free and commercial assets **created both by _Unity Technologies _and also members of the community. A wide variety of assets are available, covering everything from _textures, models and animations_, _to whole project examples, tutorials and Extension Assets_.

Find and download the free asset _“5 animated Voxel animals_” .

[https://assetstore.unity.com/packages/3d/characters/animals/5-animated-voxel-animals-145754](https://assetstore.unity.com/packages/3d/characters/animals/5-animated-voxel-animals-145754)

* **Click “Add to My Assets”**
* **Click “Open in Unity”**

### Import Package in your Unity File <a href="_toc86085571" id="_toc86085571"></a>

After clicking the **“Open in Unity”** button, you will be redirected to the Unity software, on the package manager. Make sure you are in **Packages: ”My Assets”**. Click **Import**

This Window pops up. When importing a package, you can select the parts you want to import. For now, we will import everything. Click **“Import”.**

### Make a Plane <a href="_toc86085572" id="_toc86085572"></a>

In the Hierarchy ,** **right click and go to **3D Object> Plane.**



_**→ Make sure the Transform position values are set to 0,0,0**_

### Import a character <a href="_toc86085573" id="_toc86085573"></a>

Go to** Assets>VoxelAnimals>Assets>Prefabs **and drag and drop a character on the Scene or the Hierarchy. Make sure it is correctly placed on the plane.

** **![](<../.gitbook/assets/image (4).png>)****

**Tip: **To zoom in on an object fast, you can first select it from the Hierarchy or the scene and then click on the “F” button on your keyboard.

You can move the camera to a position where you see everything nicely.

### Click on Play! <a href="_toc86085574" id="_toc86085574"></a>

By clicking on the Keyboard arrows your character will move, by clicking on the Space Button the character will jump! If the cube gets bigger, the dog collides with it and might fall. The character is a** “Rigid Body”** with** “Collision”** properties.

### Regular Update Events <a href="_toc86085575" id="_toc86085575"></a>

A game is rather like an animation where the animation frames are generated on the fly. A key concept in games programming is that of making changes to **position, state and behavior** of objects in the game just before each frame is rendered. The [**Update**](https://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html)** **function is the main place for this kind of code in Unity. _Update_ is called **before the frame is rendered** and also **before animations are calculated**.

```

void Update() 
{
    float distance = speed * Time.deltaTime * Input.GetAxis("Horizontal");
    transform.Translate(Vector3.right * distance);
}
```

[→ More Information on execution order of events in unity](https://docs.unity3d.com/Manual/ExecutionOrder.html)

### ARCore <a href="_toc86085576" id="_toc86085576"></a>

_ARCore_ is _Google’s_ platform for building augmented reality experiences. Using different** APIs\[3], ARCore** enables your phone to sense its environment, understand the world and interact with information. Some of the APIs are available across _Android _and _iOS _to enable shared AR experiences.

ARCore uses_ three key capabilities _to integrate virtual content with the real world as seen through your phone's camera:

* [**Motion tracking**](https://developers.google.com/ar/discover/concepts#motion\_tracking)** **allows the phone to understand and track its position relative to the world.
* [**Environmental understanding**](https://developers.google.com/ar/discover/concepts#environmental\_understanding)** **allows the phone to detect the size and location of all types of surfaces: horizontal, vertical and angled surfaces like the ground, a coffee table or walls.
* [**Light estimation**](https://developers.google.com/ar/discover/concepts#light\_estimation) allows the phone to estimate the environment's current lighting conditions.

### Supported devices <a href="_toc86085577" id="_toc86085577"></a>

_ARCore i_s designed to work on a wide variety of qualified Android phones running_ Android 7.0 (Nougat) _and later. A full list of all supported devices [is available here](https://developers.google.com/ar/discover/supported-devices).

### How does ARCore work? <a href="_3mrgulpdwuf1" id="_3mrgulpdwuf1"></a>

Fundamentally,_ ARCore_ is doing two things:

* tracking the **position **of the mobile device as it moves
* building its own understanding of the real world.

ARCore's motion tracking technology uses the **phone's camera **to identify interesting **points,** called features, and **tracks** how those points move over time. With a combination of the movement of these points and readings from the phone's inertial sensors, _ARCore _determines both the position and orientation of the phone as it moves through space.

In addition to identifying key points, ARCore can detect **flat surfaces**, like a table or the floor, and can also estimate the** average lighting **in the area around it.

_→ For a more detailed breakdown of how ARCore works, check out _[_fundamental concepts_](https://developers.google.com/ar/discover/concepts)_._

_ARCore _provides_** SDKs**_\[4] for many of the most popular development environments. These SDKs provide native APIs for all of the essential AR features like motion tracking, environmental understanding, and light estimation. With these capabilities you can build entirely new AR experiences or enhance existing apps with AR features.

[Android](https://developers.google.com/ar/develop/java/quickstart) // [Android NDK](https://developers.google.com/ar/develop/c/quickstart) // [Unity (AR Foundation)](https://developers.google.com/ar/develop/unity-arf) // [iOS](https://developers.google.com/ar/develop/ios/overview) // [Unreal](https://developers.google.com/ar/develop/unreal/quickstart)

_In our workshop, we will focus on Unity’s AR Foundation Framework, and build the application for Android devices._

### AR Foundation <a href="_toc86085579" id="_toc86085579"></a>

[AR Foundation](https://unity.com/unity/features/arfoundation) is a **cross-platform framework** that allows you to build augmented reality experiences once, then build for either Android or iOS devices. **ARCore Extensions** for AR

AR Foundation allows you to work with augmented reality platforms in a multi-platform way within Unity. This package presents an interface for Unity developers to use, but doesn't implement any AR features itself. To use AR Foundation on a target device, you also need separate packages for the target platforms officially supported by Unity:

* [ARCore XR Plug-in](https://docs.unity3d.com/Packages/com.unity.xr.arcore@4.2/manual/index.html) on Android
* [ARKit XR Plug-in](https://docs.unity3d.com/Packages/com.unity.xr.arkit@4.2/manual/index.html) on iOS
* [Magic Leap XR Plug-in](https://docs.unity3d.com/Packages/com.unity.xr.magicleap@6.0/manual/index.html) on Magic Leap
* [Windows XR Plug-in](https://docs.unity3d.com/Packages/com.unity.xr.windowsmr@4.0/manual/index.html) on HoloLens

AR Foundation is a set _of MonoBehaviours_ and APIs for dealing with devices that support the following concepts. A few of them are:

* **Device tracking:** track the device's position and orientation in physical space.
* **Plane detection: **detect horizontal and vertical surfaces.
* **Anchor**: an arbitrary position and orientation that the device tracks.
* **Light estimation:** estimates for average color temperature and brightness in physical space.
* **Face tracking**: detect and track human faces.
* **2D image tracking:** detect and track 2D images.
* **Meshing: **generate triangle meshes that correspond to the physical space.
* **Collaborative participants:** track the position and orientation of other devices in a shared AR experience.
* **Raycast:** queries physical surroundings for detected planes and feature points.

[→ More information about AR Foundation](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.2/manual/index.html)

### Start working on the AR App <a href="_toc86085580" id="_toc86085580"></a>

Open the file you created when following the _**Installation Guide** _for the Seminar Week.

(By going to Unity Hub and Select your file)

### Configure an AR session and add AR Foundation components <a href="_toc86085581" id="_toc86085581"></a>

A scene needs an AR session to enable [AR processes](https://developers.google.com/ar/discover/concepts), such as motion tracking, environmental understanding, and lighting estimation. You will need the following game objects to support an AR session:

* **AR Session**: Controls the lifecycle of an AR experience.
* **AR Session Origin**: Transforms AR coordinates into Unity world coordinates.

1. Before adding the new game objects, delete the default **Main Camera**. It will be replaced by a new **AR Camera** in the **AR Session Origin**.
2. Add the new AR game objects to your scene: right-click the **Hierarchy** pane and select **XR**. Add a new **AR Session** and a new **AR Session Origin** game object.

![](<../.gitbook/assets/image (5).png>)

**What is a Session?**

All [AR processes](https://developers.google.com/ar/discover/concepts), such as motion tracking, environmental understanding, and lighting estimation, happen inside an ARCore session. [ARSession](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.1/api/UnityEngine.XR.ARFoundation.ARSession.html) is the main entry point to the ARCore API. It manages the AR system state and handles the session **lifecycle**, allowing the app to create, configure, start, or stop a session. Most importantly, it enables the app to receive frames that allow access to the camera image and device pose.

Your_ Hierarchy_ should now look like this:

Expand the **AR Session Origin** you created in the _Hierarchy_, and select the **AR Camera object**. In the inspector, change its _**Tag**_ to **MainCamera.**

### Detect planes in the real world <a href="_toc86085582" id="_toc86085582"></a>

### Add an ARPlane Manager Component and place an AR Plane Prefab <a href="_toc86085583" id="_toc86085583"></a>

An [ARPlaneManager](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.1/api/UnityEngine.XR.ARFoundation.ARPlaneManager.html) detects [ARPlane](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.1/api/UnityEngine.XR.ARFoundation.ARPlane.html)s and creates, updates, and removes game objects when the device's understanding of the environment changes.

1. Go to _Hierarchy _and click on the **AR Session Origin **GameObject. On the_ Inspector _Window, click on** “Add Component”, search** for the **“AR Plane Manager” **and **Add it**.
2. Respectively, click again on the **“Add Component**”, search for **“AR Raycast Manager”** and add it as well.

![](<../.gitbook/assets/image (3).png>)

_We will need this component later. It helps obtain information of the raycasts deriving from the user input on the screen._

1. In the_ Hierarchy Window_, right click , go to XR -> AR Default Plane. This will create an AR-configured plane, that we will customize a bit and use as a Prefab for our application, in order to visualize the AR detected planes.

This _XR GameObject _contains _scripts_ that generate and visualize the planes detected by the _AR device_, using the _AR Plane manager_ script we created in the previous step.

* **AR Plane script summary: **Represents a plane (i.e., a flat surface) detected by an AR device. Generated by the** \<ARPlaneManager> **when an AR device detects a plane in the environment.
* **AR Plane Mesh Visualizer Summary: **Generates a mesh for an \<ARPlane>. It generates a mesh and updates the boundary points with a \<LineRenderer>.

1. Now we will go to the Project Window, create a new empty** folder** in our **Assets,** and name it “Prefabs”, to organize our material. (**Right click > Create > Folder**). Then, double click it and go inside the folder.
2. Drag and drop the_** “AR Default Plane”** _GameObject inside the **Prefabs** folder, in order to save it as a _Prefab._ In general, this is an easy way to _create new Prefabs_, after we have edited them as we like in our _Scene._

Note: In short, Unity’s **Prefab** system allows you to create, configure, and store a GameObject complete with all its components, property values, and child **GameObjects **as a reusable Asset. The Prefab Asset acts as a template from which you can create new Prefab instances in the **Scene.**\
[**→ More info about prefabs**](https://docs.unity3d.com/Manual/Prefabs.html)

→ Notice that the _AR Default Plane_ becomes** blue **in the _Hierarchy. _This is because the process of creating the _Prefab Asset _also turns the original _GameObject_ into a_** Prefab instance**_. Every change happening in the _prefab_, will happen in the _instance _as well.

1. We can now **delete **the _AR Default Plane Object_ in the** Hierarchy,** since we will only use the** Prefab.**
2. On the **AR Session Origin **_GameObject_, go to the **AR Plane Manager Script** we added in Step 1. There is an _empty Plane Prefab._ Drag and Drop there the Prefab we created in the Assets.

You should now see the _AR Default Plane Prefab_ placed as the _Plane Prefab._

We can now visualize the AR detected planes in our device.

_**Let’s build the app and see if it works!**_

### Build the AR App to test the AR Plane detection <a href="_toc86085584" id="_toc86085584"></a>

1. _Plug _your _Android AR device_ on your computer, using the_ USB_ cable.
2. Make Sure that your computer recognizes the device (You can check this in **This PC>Devices and drives**)

If you don’t see it, try having the device unlocked when you plug it, and if USB options popup, select _**Use USB to: “Transfer files” **_on your phone screen.

1. Go to **File>Build Settings**
2. Respectively, you should be able to see your device detected in the _“Run Device” _drop-down Menu. You can click _Refresh_ if you don’t see it right away.
3. Click _**“Build and Run”**_. Select a folder and name your application as desired. This might take a bit of time. Keep the device unlocked, so that the application runs right away.

Move the phone **slowly **up and down, left and right, to start the detection.

After some time, you should be able to see this s_emi-transparent yellow material_ with an outline, indicating the **scanned AR Planes.**

![](<../.gitbook/assets/image (1) (1).png>)

### Import a file from Rhino <a href="_toc86085585" id="_toc86085585"></a>

### Export file from Rhino <a href="_toc86085586" id="_toc86085586"></a>

In _Rhino_, open your desired file (in our case the _**3D printed objec**t_)

**Units:**

_→ If your units are in millimeters, you will need to scale the model by **0.001** when you import it in Unity_

_→ If your units are in meters, you will not need to scale it._

**Move the x,y center** of your geometry to** 0,0,0 **(you can use the_ Move_ command, and snap it) and align it correctly on the 0,0,0 plane. (so that it is above the ground).

_→ This step will help us have our model correctly placed in Unity._

Select the part(s) that you want to export



Go to **File>Export Selected**

1. Save as type **.obj,** in a folder that you can easily find (e.g. another folder on your Desktop called _“Seminar\_Week\_Assets”,_ and click **“OK”** in the _OBJ Export Options_ dialog.

→Note: Check if your file is created correctly in the folder.

### Import file in Unity <a href="_toc86085587" id="_toc86085587"></a>

First, we will learn how to import our model correctly into _Unity_, adjust its scale and position it correctly. In this example, there is _Rhinoceros as a CAD softwar_e, but you can use any of your preferred modelling software. Be sure to structure your model according to the materials you want to apply on it later. Unity understands a variety of file types, you can use **OBJ **and export each layer as a separate OBJ file.

Back in the existing Unity App, go to the Prefab folder we created before in the Assets (Project Window).

Go to the Assets folder of the Unity Project on your computer (in the file explorer) , and copy paste both the .obj and the .mtl. You can make a new folder called “ImportedObjects”.

**Note:** Unity Projects are actually folders that contain other folders. So you can have full access on Prefabs, scripts, objects. When you put something in this folder, it updates in Unity as well.

Inside the** ImportedObjects **folder, _copy and past_e both your **.obj **and the** .mtl** file that you exported from_ Rhino_.

**Note: **Alternatively, you can make the new folder inside Unity and drag and drop both of the files.

Back to_ Unity_, you will see that the folder is also updated in the** Project Window > Assets.**



Inside the folder, you should be able to see an icon of your house in the_ Prefabs._ Click on it once and go to the_** Inspector Window. **_Enable _**Read/Write. Click “Apply”.**_



### Create Prefab with imported Model <a href="_toc86085588" id="_toc86085588"></a>

Drag and drop the house Prefab into the Hierarchy. You will now see the object imported in your Scene.

In the_ Inspector_, make sure that the _Transform, Position and Rotation _are on **0,0,0.**

To check that the _Scale of the model _we imported is correct ( 1:1 in physical space), we can compare it with a **1,1,1 **cube (=1x1x1meters) (**Create>3D Object>Cube**). If we see that the Cube is too big or too small, we have to scale our model. It is better to have an object that is small enough to fit in the phone camera.

To scale our model, we can click on the Prefab once, go to the Inspector, and type in the amount of desired scale (e.g. for this model was 0.001). Click **“Apply”** on the bottom right.



**Note:** Don’t forget to Save your Project from time to time! (**File > Save**) or** Ctrl+S**.

Now that the scale of the object is correct, we can apply a _Material_ of our choice.Let’s go back to **Assets** and make another** empty Folder **called **“Materials”**. Here we will be organizing our materials throughout the development.

Tip: You can rename an existing folder by clicking on it once, and pressing** F2 **on your keyboard.

Inside the Materials folder, create a new Material (**Right Click > Create > Material**) Like the steps before, you can assign a color, or create multiple materials and assign them on your object.

When you are finished, Drag and Drop the object GameObject back to the **Assets>Prefabs,** and name it as _“Building\_Prefab\_01” . _Today, we will use this Prefab to instantiate it in the AR Detected Planes.



### Instantiate object <a href="_toc86085589" id="_toc86085589"></a>

_Now we can start the scripting base ! The goal for today is to **“Tap and Instantiate”** the 3D object in our App, on the detected AR Planes._

### Import the Instantiator Script <a href="_toc86085590" id="_toc86085590"></a>

In the** Assets**, Create a new empty_ Folder _and name it_ “Scripts”._ Here, we will be collecting all of the **C# scripts** we will be using.

Go to the folder with the material of the Seminar Week you downloaded. Go to **Scripts>Day1>Instantiator.cs **Drag and drop the script into the _Script _folder we created in Unity. This is a custom Script , made for the Seminar Week, that we will use to build our application upon.

Make an Empty Game Object and name it_ “Instantiator”. _Here we will assign our Instantiator script, where we manage the instances of the objects we create in the App.

On the** Instantiator **GameObject, go to the_ Inspector _and click on **“Add Component”**. Find and add the** “Instantiator” **script we imported in the previous steps.

![](<../.gitbook/assets/image (2).png>)

**Note:** Always make sure that the Transform Values are set to Zero when creating a new GameObject.

We see some defined Variables that don’t have any object assigned to them. But first, let’s take a look at the **code.**

### Overview of the code <a href="_toc86085591" id="_toc86085591"></a>

To open the script, you can either go to **the Assets** and double click, or on the **component** itself, and **double click** on the name .

![](<../.gitbook/assets/image (8).png>)

![](<../.gitbook/assets/image (10).png>)

Do these variables seem familiar? They are the ones that appear on the Inspector!

![](<../.gitbook/assets/image (11).png>)

We can choose what kind of GameObject we need.

_E.g. For the Camera, we only need the “Transform” component, to get information about its position and rotation in space._

Let’s drag and drop the objects we need.

1. For the** Selected Prefab**, we can put the House Prefab we created , by dragging it from the** Assets>Prefabs** folder we created.
2. For the **Ar Camera Transform**, we will drag and drop the** AR Camera GameObject**, which we can find under _“AR Session Origin”_ in the Hierarchy.
3. As we saw in the _C# _script, the **House Paren**t, is an empty GameObject that will be the **parent** of all the instances we will create with our taps on the screen. So let’s create an **Empty GameObject** and name it “**HouseParent**” respectively. Then, drag and drop it on the Instantiator Script. _(Important: Make sure the Transform is set to 0,0,0 in the Inspector!)_



The Inspector should look like this.

_**\*\*\*We are ready to build the App and test it !\*\*\***_

### Build App <a href="_toc86085592" id="_toc86085592"></a>

### Debugging Session <a href="_toc86085593" id="_toc86085593"></a>

1. MonoBehaviour is the base class from which every Unity script derives. ↑
2. A class enables you to create your custom types by grouping variables of other types, methods, and events. ↑
3.  API is the acronym for** Application Programming Interface**, which is a software intermediary that allows two applications to talk to each other.

    ↑
4.  \= Software Development Kits

    ↑
5.  Tags help you identify GameObjects for scripting purposes.

    ↑
6. Plane = two-dimensional surface. A flat surface with no thickness. ↑

# END ETH #
<!---------------------- END ETH   ----->
	

### Configure Build and Project Settings in Unity

1. Select Android in build settings

	- Go to File - Build Settings
	
	- Switch to the Android Platform - Go to player settings
		<img width="650" alt="" src="https://i.imgur.com/8H0ccdQ.jpg">	
	- If Unity requires you to download Android support in order to switch platforms, follow the link and install the required dependency. 
	
	- Go to Player > Other Settings > Rendering
	- Make sure “Auto Graphics API” is unchecked. Change Color Space to “Linear”
		<img width="650" alt="" src="https://i.imgur.com/KI4UmgW.jpeg">	
	
	- Go to Player > Other Settings > Package Name. Create a unique app ID using a Java package name format. For example, use com.Princeton.AR
	- Go to Player > Other Settings > Minimum API Level. Select Android 7.0 'Nougat' (API Level 24) or higher (For AR Optional apps, the Minimum API level is 14.)
	- Go to Player > Other Settings > Scripting Backend. Select IL2CPP instead of Mono.
	- Go to Player > Other Settings > Target Architectures. To meet the Google Play 64-bit requirement, enable ARM64 (64-bit ARM). Leave ARMv7 (32-bit ARM) enabled to support 32-bit devices
		<img width="650" alt="" src="https://i.imgur.com/nzciMsI.jpeg">		
	
2. Configure Project Settings
	
	- Open Edit > Project Settings... and click on the XR Plug-in Management section. In the Android tab, enable ARCore.
		<img width="650" alt="" src="https://i.imgur.com/ILVPZQS.jpg">	
	
### Android

1. Enable Developer Mode on your Android 

	- Go to the settings on your phone. Settings > About Phone > Build Number (or similar)
	
	- To enable developer options, tap the Build Number option 7 times 
	
	- Enable USB debugging
	
	- Use a USBC Data cable to connect to your computer, as opposed to a power cable. 


<!--
(Unless you wish to test the project with the given credentials, please follow all steps below. Otherwise, skip to 7)
Register your Android app with [Firebase](https://firebase.google.com/docs/unity/setup).
1. Create a Unity project in the Firebase console.

2. Associate your project to an app by clicking the Add app button, and selecting the Unity icon.
    - You should use ```com.ETHZ.cdf``` as the package name while you're testing.
    - If you do not use the prescribed package name you will need to update the bundle identifier as described in the
      - *Note: *Optional: Update the Project Bundle Identifier below.*
    - change the rules in ```Realtime Database``` to :

```
{
  "rules": {
    ".read": true,
    ".write": true
  }
}
```

3. Accessing Firebase config information
    - In your Firebase console, navigate to Project Overview and click the gear icon.
    - In the drop-down window select Project Settings
    - In the project settings window under Your apps select CDF Web App
    - The Required Config Information is listed under the section SDK setup and configuration and an example is shown below

```
// Your web app's Firebase configuration
// For Firebase JS SDK v7.20.0 and later, measurementId is optional
const firebaseConfig = {
  apiKey: "AIzaSyAO_YVROUIc866BqgWgcBpPxUe6SVG5O9g",
  authDomain: "cdf-project-f570f.firebaseapp.com",
  databaseURL: "https://cdf-project-f570f-default-rtdb.europe-west1.firebasedatabase.app",
  projectId: "cdf-project-f570f",
  storageBucket: "cdf-project-f570f.appspot.com",
  messagingSenderId: "641027065982",
  appId: "1:641027065982:web:20ca92f0a2326bc3dab02f",
  measurementId: "G-RZ5BVHNGK8"
};
```

4. Android apps must be signed by a SHA1 key, and the key's signature must be registered to your project in the Firebase Console. To generate a SHA1, first you will need to set the keystore in the Unity project.
    - Go to ```Publishing Settings``` under ```Player Settings``` in the Unity editor.
    - Select an existing keystore, or create a new keystore using the toggle.
    - Select an existing key, or create a new key using ```Create a new key```.
    - Build an apk to be able to generate the SHA1 key
    - After setting the keystore and key, as well as building the app once, you can generate a SHA1 by running this command in CMD (admin):
      
    ```
    keytool -list -v -keystore <path_to_keystore> -alias <key_name>
    ```

    - Copy the SHA1 digest string into your clipboard.
    - Navigate to your Android App in your Firebase console.
    - From the main console view, click on your Android App at the top, and open the settings page.
    - Scroll down to your apps at the bottom of the page and click on Add Fingerprint.
    - Paste the SHA1 digest of your key into the form. The SHA1 box will illuminate if the string is valid. If it's not valid, check that you have copied the entire SHA1 digest string.
      
5. Download the ```google-services.json``` file associated with your Firebase project from the console. This file contains the information mentioned above that, you need to connect your Android app to the Firebase backend, and will need to be included either in the FirebaseInitialize script in the Unity project or at the start of the app, before initializing Firebase. You will need to look for the following parameters:
App id, api key, database url, storage bucket, and project id


6. Optional: Update the Project Bundle Identifier.
    - If you did not use ```com.ETHZ.cdf``` as the project package name you will need to update the sample's Bundle Identifier.
    - Select the File > Build Settings menu option.
    - Select Android in the Platform list.
    - Click Player Settings.
    - In the Player Settings panel scroll down to Bundle Identifier and update the value to the package name you provided when you registered your app with Firebase.
      
7. Build for Android.
    - Select the File > Build Settings menu option.
    - Select Android in the Platform list.
    - Click Switch Platform to select Android as the target platform.
    - Wait for the spinner (compiling) icon to stop in the bottom right corner of the Unity status bar.
    - Click Build and Run.




# cdf_unity

Firebase Installations Quickstart



## Getting started with this project

### Installing Dependencies


### Android

Register your Android app with [Firebase](https://firebase.google.com/docs/unity/setup).
1. Create a Unity project in the Firebase console.

2. Associate your project to an app by clicking the Add app button, and selecting the Unity icon.
    - You should use ```com.ETHZ.cdf``` as the package name while you're testing.
    - If you do not use the prescribed package name you will need to update the bundle identifier as described in the
      - *Note: *Optional: Update the Project Bundle Identifier below.*
    - change the rules in ```Realtime Database``` to :

```
{
  "rules": {
    ".read": true,
    ".write": true
  }
}
```

3. Accessing Firebase config information
    - In your Firebase console, navigate to Project Overview and click the gear icon.
    - In the drop-down window select Project Settings
    - In the project settings window under Your apps select CDF Web App
    - The Required Config Information is listed under the section SDK setup and configuration and an example is shown below

```
// Your web app's Firebase configuration
// For Firebase JS SDK v7.20.0 and later, measurementId is optional
const firebaseConfig = {
  apiKey: "AIzaSyAO_YVROUIc866BqgWgcBpPxUe6SVG5O9g",
  authDomain: "cdf-project-f570f.firebaseapp.com",
  databaseURL: "https://cdf-project-f570f-default-rtdb.europe-west1.firebasedatabase.app",
  projectId: "cdf-project-f570f",
  storageBucket: "cdf-project-f570f.appspot.com",
  messagingSenderId: "641027065982",
  appId: "1:641027065982:web:20ca92f0a2326bc3dab02f",
  measurementId: "G-RZ5BVHNGK8"
};
```

4. Android apps must be signed by a SHA1 key, and the key's signature must be registered to your project in the Firebase Console. To generate a SHA1, first you will need to set the keystore in the Unity project.
    - Go to ```Publishing Settings``` under ```Player Settings``` in the Unity editor.
    - Select an existing keystore, or create a new keystore using the toggle.
    - Select an existing key, or create a new key using ```Create a new key```.
    - After setting the keystore and key, you can generate a SHA1 by running this command in CMD (admin):
      
    ```
    keytool -list -v -keystore <path_to_keystore> -alias <key_name>
    ```

    - Copy the SHA1 digest string into your clipboard.
    - Navigate to your Android App in your Firebase console.
    - From the main console view, click on your Android App at the top, and open the settings page.
    - Scroll down to your apps at the bottom of the page and click on Add Fingerprint.
    - Paste the SHA1 digest of your key into the form. The SHA1 box will illuminate if the string is valid. If it's not valid, check that you have copied the entire SHA1 digest string.
      
5. Download the ```google-services.json``` file associated with your Firebase project from the console. This file identifies your Android app to the Firebase backend, and will need to be included in the Asset folder of your Unity file.
    - For further details please refer to the general instructions page which describes how to configure a Firebase application for Android.

6. Download the ```Firebase Unity SDK``` and unzip it somewhere convenient.

7. Open the project folder in the Unity editor.
    - Select the File > Open Project menu item.
    - Click Open.
    - Navigate to the sample directory ```cdf_unity``` in the file dialog and click Open.
    - Open the scene ```AssemblyApp``` by double click in the Asset folder.
      
8.	Import the Firebase Installations plugin.
    - Select the Assets > Import Package > Custom Package menu item.
    - From the Firebase Unity SDK downloaded previously, import the required Firebase installations (in this case: FirebaseAnalytics, FirebaseAppCheck, FirebaseAuth and FirebaseDatabase)
    - ```IMPORTANT: Do not install different versions of Firebase Assets and do not change the way you install Firebase packages (either via the package manager or via the custom package menu```

9. Add the google-services.json file to the project.
    - Navigate to the Assets/Data folder in the Project window.
    - Drag the google-services.json downloaded from the Firebase console into the folder. Note: google-services.json can be placed anywhere under the Assets folder.

10. Optional: Update the Project Bundle Identifier.
    - If you did not use ```com.ETHZ.cdf``` as the project package name you will need to update the sample's Bundle Identifier.
    - Select the File > Build Settings menu option.
    - Select Android in the Platform list.
    - Click Player Settings.
    - In the Player Settings panel scroll down to Bundle Identifier and update the value to the package name you provided when you registered your app with Firebase.
      
11. Build for Android.
    - Select the File > Build Settings menu option.
    - Select Android in the Platform list.
    - Click Switch Platform to select Android as the target platform.
    - Wait for the spinner (compiling) icon to stop in the bottom right corner of the Unity status bar.
    - Make sure you clicked the scene ```AssemblyApp```
    - Click Build and Run.

-->
