# ARC596 Installation Guide

- ARC596: Embodied Computation
- Daniela Mitterberger 

### Requirements

1. [Rhinoceros 7](https://www.rhino3d.com/en/7/)
2. [Github Desktop](https://desktop.github.com/) 
3. [Anaconda](https://www.anaconda.com/)
4. [Unity 2022.3.3f1](https://unity.com/) 
5. Android SDK and Java JDK (when developing for Android) - have to be ticked in the installation modules when installing Unity.

### Dependencies

1. [COMPAS](https://compas.dev)
2. [COMPAS Fab - Fabrication Library for Robots](https://gramaziokohler.github.io/compas_fab/latest/)
3. [COMPAS Eve - Communication](https://compas.dev/compas_eve/latest/index.html)
4. [Vuforia](https://developer.vuforia.com/downloads/sdk)
5. [ROS#](https://www.ros.org/)

## Getting started with this project

### Installing Requirements 

Enabling communication between Rhino, the robot, and the Unity App on your phone. 

1. Install Rhino 7 (Grasshopper is installed as part of Rhino)

2. Install Git https://desktop.github.com/

      - *Note: Git is a package manager, and it allows you to publish code onto the cloud or public web. It also tracks your changes.*

		

3. Install Git LFS https://git-lfs.com/ using the Anaconda prompt. Be sure to activate the ARC596 environment. 

	- Open the Windows Command Prompt. Search CMD in the start menu. 
	
		<img width="650" alt="" src="https://i.imgur.com/hJg15Ws.jpg">
	
	- Run ```git lfs install```

      - *Note: This is a Git extension to allow you to post larger files to the cloud folder called a repository*


3. Install Anaconda https://www.anaconda.com/download

      - *Note: Anaconda is a package management system for Python; it manages python and related packages on your computer.*

4. Install compas, compas_fab, and compas_eve

      - *Note: Compas is a framework for working with robotics*
	  
	- Run Anaconda Prompt. Go to start menu and search ```anaconda prompt```
	
		<img width="650" alt="" src="https://i.imgur.com/4OSBP7y.jpg">
		
		- *Note: This is the Anaconda-flavored Python terminal. It is different from the Windows Command Prompt in that it runs python code and commands, not windows commands*
	
	- Type the following commands into the terminal, this creates a new development environment called ARC596, and installs compas, compas_fab, compas_eve, and links them into Rhino 7:

	```
	conda create -n ARC596
	conda activate ARC596
	conda install -c conda-forge compas
	conda update compas
	conda install conda-forge::compas_fab
	conda install conda-forge::compas_eve
	python -m compas_rhino.install -v 7.0
	```

      - *Note: For documentation or help, please see: https://compas.dev/compas/latest/userguide/installation.html*
	  

		<img width="650" alt="" src="https://i.imgur.com/Qs8CP1P.jpg">


5. Unblock the programs using the firewall
		
	- When windows asks whether to allow network access for Python, be sure to allow for both private and public networks. 

		<img width="400" alt="" src="https://i.imgur.com/2lRvYge.jpg">
			
			- *Note: If your host (broker) for messages is not localhost. DISABLE your Firewall or Ensure that the required ports for MQTT (typically 1883 or 8883 for secure connections) are open and accessible.*

	
	
### Installing Unity and Starting a Project

1. Install Unity https://unity.com/download 

      - *Note: We will use Unity 3D to develop the Android – based augmented reality apps.*
	
2. License Unity & Create Unity ID

	- Create your Unity ID: https://id.unity.com/en/conversations/b1516ea8-e6f1-4061-96b5-a060365abe06019f

	- License unity as personal non-commercial license 
	
		<img width="400" alt="" src="https://i.imgur.com/GChEZvL.jpg">
	
	
	
<!--- Download the project from Github
*** Open Unity Hub. In Projects, click on Open(MacOS) or ADD(Windows). Locate the folder you downloaded from GitHub `name of project here` on the drive and add it.
-->

3. Download & Install the correct Unity Version. ```2022.3.3f1``` https://unity.com/releases/editor/whats-new/2022.3.3
      - *Note: Unity is version-sensitive, and it needs to be exactly this version*
	- Be sure to select the appropriate dependencies below.

		<img width="400" alt="" src="https://i.imgur.com/ShxDrED.jpg">
	
	
4. Run Unity Hub. Create a new Unity Project.

	<img width="650" alt="" src="https://i.imgur.com/I77ndmz.jpg">
	
	
5. Install Unity Packages for XR (Mixed Reality)

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
	
	
6. Install Vuforia - SDK for AR 

	- Download the installer and run it https://developer.vuforia.com/downloads/sdk 
		- Use the ```add-vuforia-package-10-19-3.unitypackage``` file
	
	- Either make an account or use ```a7314621@drdrb.net```/```Aa7314621``` as the user/pass in order to download
	
		<img width="300" alt="" src="https://i.imgur.com/GEwKZaN.jpg">

    - In Unity, check that Vuforia is installed by seeing if menus show up: Help - Vuforia Engine - Show Developer Agreement


7. Import Ros# into the project



	- Go to the Asset Store and add Ros# to your asset list: https://assetstore.unity.com/packages/tools/physics/ros-107085
		
		- *Note: Ros# is a communication protocol to pass data back and forth from robots*	
		
		<img width="650" alt="" src="https://i.imgur.com/hduyAYd.jpg">
		
	- In Unity’s Package Manager Window open the Packages drop-down menu and choose My Assets.  Make sure you are loading all your assets in the list.
		
    - Download and import Ros# to the project 



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

1.0 Enable Developer Mode on your Android 

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
