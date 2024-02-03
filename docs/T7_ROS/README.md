# ARC596 - Tutorial 7 - ROS

- ARC596: Embodied Computation
- Professor: Daniela Mitterberger - mitterberger@princeton.edu
- Assistant Instructor: Kirill Volchinskiy - kvolchinskiy@princeton.edu
  
### Requirements

1. [Rhinoceros 7](https://www.rhino3d.com/en/7/)
2. [Github Desktop](https://desktop.github.com/) 
3. [Anaconda](https://www.anaconda.com/)
4. [Unity 2022.3.3f1](https://unity.com/) 
>	Note: When developing for Android, the Android SDK and Java JDK have to be ticked in the installation modules when installing Unity.

### Dependencies

1. [COMPAS](https://compas.dev)
2. [COMPAS Fab - Fabrication Library for Robots](https://gramaziokohler.github.io/compas_fab/latest/)
3. [COMPAS Eve - Communication](https://compas.dev/compas_eve/latest/index.html)
4. [Vuforia](https://developer.vuforia.com/downloads/sdk)
5. [ROS#](https://www.ros.org/)

## Installing Basic Requirements 

- Install Docker [https://www.docker.com/products/docker-desktop/](https://www.docker.com/products/docker-desktop/)

> Note: Docker is a virtualization platform that lets you run linux on windows. You need linux to run a ROS server. ROS ias a communication protocol that allows you to communicate with the robots. 

> Important! Make sure that you enable virtualization support in your BIOS. If you have a Mac (even if it is running Windows), scroll down to the end of this page to enable CPU virtualization. Go to the end of this guide for information on how to enable it. 

When Docker asks you whether to use WSL 2, use it:

  <img width="600" alt="" src="https://i.imgur.com/5r8F3Gx.jpeg">

Allow Docker through the firewall when it asks: 

  <img width="450" alt="" src="https://i.imgur.com/xNJTqXY.jpeg">

> Note: Docker works in the following way: when containers are ```UP```, the operating system within the container is running. When the container has exited, it means that the operating system has shut off. 

  <img width="600" alt="" src="https://i.imgur.com/YTrXAaE.png">

## Run a test ROS image on Docker 

- Use this guide for more information on ROS and docker [https://gramaziokohler.github.io/compas_fab/0.11.0/backends/ros.html](https://gramaziokohler.github.io/compas_fab/0.11.0/backends/ros.html)

- Open Docker. 

- To try a minimally functional ROS image, open the command line, and run:

```
docker run -p 9090:9090 -t gramaziokohler/ros-base roslaunch rosbridge_server rosbridge_websocket.launch
```

- If the command executes, you are running Docker and ROS successfully, specifically if it states ```[-] [INFO] [1707004071.416390]: Rosbridge WebSocket server started at ws://0.0.0.0:9090``` at the end or similar:

  <img width="600" alt="" src="https://i.imgur.com/EtnD7P7.jpeg">


## Troubleshooting

### Virtualization 

- If you receive the ```Docker Desktop - Unexepcted WSL Error```, or the Hypervisor error you need to enable virtualization on your computer. 

  <img width="400" alt="" src="https://i.imgur.com/eELy2gM.png">
  <img width="400" alt="" src="https://i.imgur.com/O9rPBaY.png">

- Personal Computer users, user the following links to enable it in your BIOS:

  - [https://docs.docker.com/desktop/troubleshoot/topics/#virtualization](https://docs.docker.com/desktop/troubleshoot/topics/#virtualization)

  - [https://www.virtualmetric.com/blog/how-to-enable-hardware-virtualization](https://docs.docker.com/desktop/troubleshoot/topics/#virtualization)

- Mac or Apple computers, enable virualization in the following way. See here for the guide [https://apple.stackexchange.com/questions/120361/how-to-turn-on-hardware-virtualization-on-late-2013-macbook-pro-for-windows-8-1](https://apple.stackexchange.com/questions/120361/how-to-turn-on-hardware-virtualization-on-late-2013-macbook-pro-for-windows-8-1):

- Install the rEFInd boot manager for mac: [https://sourceforge.net/projects/refind/](https://sourceforge.net/projects/refind/) 

  <img width="800" alt="" src="https://i.imgur.com/WLSNHDZ.jpeg">