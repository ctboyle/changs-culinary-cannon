3. Decomposition

The engine is broken down into separate components that define the entire engine functionality; the game engine includes the components that are being pre-packaged in a manner that could later be used for general game creation.

These components include:
  1. Picking –GUI –
  1. StateManagement –Input – Abstracts input. Also contains support for local input devices such as mouse, keyboard, and gamepad.

Most modules are integrated into the same distributed library but live in different namespaces.

3.1 Module Decomposition

The modules will be divided further into its constituent classes and namepaces

3.1.1 SceneGraph

This module is about representing graphical elements in a hierarchical fashion to manage their Drawing, Updating, and Loading. This module allows for the creation of hierarchical object trees called scene graphs. Scene graphs represent a scene to be rendered. Many places though out the engine are scene graphs referenced. The entire graph can be referenced by referencing its root scene object. Most operations on the scene graph are recursive and affect the entire tree.

The following classes define the SceneGraph module.

Class Division
  * Spatial
  * SceneNode
  * Animation
    * AnimationController
  * BoundingVolumes
    * BoundingVolume
    * BoundingShpere
    * BoundingBox
    * BoundingRect

Interface Division
  * ISpatial
  * INode

3.1.2 Rendering

Rendering is responsible for correctly displaying graphical elements on the screen. Specifically, the rendering system hosts to drawing scene graphs. The rendering system has its own scene graph objects for the aid of creating lighting effects and managing the viewpoint of a scene. The Rendering component can apply lighting, texture mapping, lighting materials, and rendering coordinates to scene objects. The scene objects themselves will make use of the rendering module to draw their visible representations.

Class Division
  * RenderManager
  * LightNode
  * CameraSystem
    * CameraManager
    * Camera
      * OrthographicCamera
      * PerspectiveCamera

3.1.3	Picking

The Picking system allows scene graph objects to be selected using a ray. The system also can determine what was selected by a mouse event. A couple of components use the picking system. The GUIManager will use it to determine what GUIObject was selected, while specific game states will use it to see what objects were selected by the user.

Class Division
  * PickingManager
  * PickRecord

3.1.4	GUI

The GUIModule is responsible for managing user interaction with the graphical elements of the game engine. The GUI System defines its own scene graph system using the same base scene graph object as the SceneGraph module. The GUI System mainly supports flat 2D rectangular objects, however, any object can be notified of user interaction through the IGUIElement interface.

Class Division
  * GUIManager
  * GUISceneObjects
    * GUIObject
    * Pane
    * Control (and derivitives)
  * Scenes
    * GUI2DScene
  * Primitives
    * Quad
    * TextQuad
    * ImageQuad

Interface Division
  * IGUIElement

3.1.5  	StateManagement

This module defines and manages game states that define the game experience throughout different phases of the game. The GameState class represents a single part of a game; usually a state represents the different screens of a game. The StateManager manages all the created states in the form of a stack.

Scene management is placed in state management because a scene manager is an object made for the GameState class. As a state will have many different scenes, GUI scenes, 3d scenes, etc, the scene manager will keep track of the loading, updating and drawing of the scenes, a tedious task to do manually.

Class Division
  * StateManager
  * GameState
  * SceneManagemet
    * SceneManager
    * Scene

3.1.6  	Input

The input system abstacts input of all forms. The input system supports standard periphial inputs, such as the mouse, keyboard, and gamepads, but also can support any kind of input. The specification of the watcher class defines what kinds of events to watch for.


The input module is dividedn into abstract input and periohial input. These prorived input functionality for the RC  Engine.

The detailed design documents the various classes and their functions.

3.2 Concurrent Process Description

The game engine runs in a single-process loop, and thus contains no concurrent processes. Additionally, the game engine has no other processes.