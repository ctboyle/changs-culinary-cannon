5. Interface Descriptions

The publicly exposed items will be discussed and explained in this section. It is assumed that since the items are public, they are the things that will be used to interact with the modules. Using the classes discussed below is key to utilizing the engine.

5.1 Module Interface

The classes and their functions will be discussed for each engine component.

5.1.1 SceneGraph

Interfaces

  * ISpatial – Outlines the core funtions of a scene graph object.
  * INode – Outlines functionsof a scene graph object with children.

Classes

Spatial

Spatial is the base of a scene graph object. It is abstract and must be overridden. For an object to exist in a scene graph, it must derive from this class.

Spatial also realizes the ISpatial interface. Spatial exists at the leaves of the scene graph. It is natural to derive items that have geometric data from spatial.

SceneNode

SceneNode is derived from Spatial and adds the ability for a scene graph object to have Spatial-derived children. Add children with the AddChild method. Node will also update its world bound to contain all of its children’s world bounds.

AnimationController

Every Spatial object has a list of AnimationControllers. Animation controllers modify the objects that they are attached. This allows for updating of time varying values, such as the animation of objects. The controller will often modify the location of the object, so it is to be updated with the objects world data.

BoundingVolume

BoundingVolume is an abstract class defining a general bounding volume for applying limiting bounds to geometric data. As bounding volumes can take on many different shapes, this class represents the commonalties to all of them. All Bounding volumes are expected to be able to convert themselves into a bounding sphere. In addition, they all must be able to perform intersection tests, with a Ray and a Plane object.  Also a bounding volume should be able to be transformed by a matrix transformation.

AxisAlignedBoundingBox

AxisAlignedBoundingBox defines a bounding volume that is a rectangular prism that has all of its edges aligned with the axis. This ensures collision tests are compute quickly.

BoundingShpere

The BoundingShpere class represents a spherical bounding volume. Collision tests with other spheres are very simple and transformations of BoundingSpheres are too. They require no computation towards the rotation of the sphere.

BoundingRect

The BoundingRect class defines a bounding volume in the form of a flat rectangle. BoundingRects should be used when the geometric contents of a scene object is flat.

5.1.2	Rendering

Rendering supports the drawing of scene graph objects.

RenderManager

The render manager exposes many functions relating to the drawing of a scene graph object. The RenderManager provides a function for setting the current drawing coordinates,  function for setting the current material, functions for enabling the directional lighting, a function for setting the current lighting material, and a function for rendering a scene graph.

LightNode

The LightNode class is derived form the SceneNode class. The light node provides the ability to illuminate objects contained within a scene graph. Use this object by adding it to a scene graph as a parent of all things you would like to be illuminated by the light. There can be up to four directional lights in the scene, and each has to refer to a different RenderManager directional light. If the same light is referenced while it the light is enabled, an exception will be thrown.

Camera

A camera object defines a particular viewpoint, viewport, and projection for rendering a scene. The camera can be position through out the world space and therefore is a scene graph object. Additionally, the camera calls derives from SceneNode. This means that the node can be attached to scene graph nodes and scene graph objects can be attached to the camera.

The camera class is abstract and must be overridden to define the kind of projection the camera will use.

OrthographicCamera

The orthographic camera derives from camera and defines an orthographic projection. The orthographic camera also exposes public properties for modifying the orthographic projection to be updated on the next update pass.

PerspectiveCamera

The perspective camera derives from camera and defines a perspective projection. The camera also exposes public properties for modifying the perspective projection to be updated on the next update pass.

CameraManager

The camera manager aids the RenderManager in determining what view and projection matrices to use for rendering scene graph objects. The CameraManager keeps track of a list of cameras that can be accessed though a string label. Any camera can be set as the currently active camera. The active camera will be the one used by the render manager for drawing.

5.1.3 	Picking

PickingManager

The picking manager exposes two main functions for determining selected objects. One function determines the lists of objects that are selected by screen coordinates. The other determines all of the objects that intersect with a given ray.

The first function is useful for determining what objects were clicked on by the mouse. The second is used in the first application and for others such as laser beam shots, determining the height above terrain, etc.

PickRecord

A pick record is the class that contains the list of objects returned by the picking manager. Objects are sorted form closest to furthest. The list of items can be retrieved from the pick record. Also, the pick record can filter on unwanted or wanted types. This helps to narrow the candidates down when trying to figure out what was selected by the user.

5.1.4 GUI

GUIManager

The GUIManger manages a scene with IGUIElements in it. The GUIManager is responsible for informing the IGUIElements those user events has occurred in its bounds. The GUI manager will need to be informed of user input. Therefore, the GUIManager defines its own control scheme type: GUIInputScheme.

GUIInputScheme

The GUIInputScheme is an abstract class that derives from InputScheme

&lt;GUIMANAGER&gt;

. This defines an InputScheme to have  GUIManager as its generic argument

GUIStandardInputScheme

This class defines all the intuitive user input for controlling a GUI scene. Use it when no unconventional GUI input is required.

GUIObject

Is the base object for the GUI System. It is a fat rectangular scene graph object that derives from Spatial. It is used for all things 2D and can be oriented in any direction in 2d space. It also can be attached to non GUI scenes as it still is a scene graph object.

Also, GUIObjects expose properties defining their size in pixels. Usually pixel size corresponds to the screen but in some cases it does not have to.

The GUIobject also realizes the IGUIElement interface.


Pane

The pane class is a GUIObject that has GUIObject children. Children GUIObjects can be added thought the AddChild function. As GUIObjects are added, the parent pane must resize the child to match the Pane’s pixel resolution; this ensures that GUIObject children are the correct size in terms of the parent’s pixel width and height.

GUI2DScene

This class provides easy creation of 2d scenes. Given a viewport on the screen, the class will automatically create a Orthographic camera and a Pane scene root that represents the screen. Users must access the screen pane to add GUIObjects to the scene graph.

Quad

The quad class is a drawing primitive defined just for the GUI system. A quad is a flat reactngle postionable in any orientation in space. It too drives from GUIObject. Quads are the visible components of an object using th GUI system. A GUIObject will use a quad to display image and text data.

ImageQuad

An ImageQuad is a quad with a texture mapped image. This defiens  arectange with a picture on it.


TextQuad

A TextQuad is quad with text pasted to it. It is used for displaying text in the GUI system.


5.1.5 StateManagement

StateManager

The state manager class manages the different game states in the form of a stack. The Currently visible state is on top of the stack. When ever a stat needs to transition, there are a few options. The old state can call into the StateManager and push on the new state, it can call and replace all the states with a new state, or it can call and pop itself of the stack. When a state is popped of the stack, the state directly underneath is preserved and it is returned to as if no timer ever had passed.

Thie is useful for managing game flow.


GameState

The GameState class defines certain exclusive phases of a game. A state can have multiple scenes to render, its very own input manager, and specific game. The state makes available all of these services for the game. Override the update and draw functions of the state to add state specific behavior. Also any input events that are being watched only affect the current state and none others.


SceneManager

Every state has a scene manager for maintaining multiple scene graphs at any time. The Scene class keeps a list of Scene classes. New Scenes can be added to the scene system by using the AddScene function. The SceneManger will allow the list of scenes to be accessed though a property.

Scene

The Scene class keeps an active camera and a root node of a scene graph for managing one scene graph tree. The scene exposes functions for Drawing, Loading, and Updating the scene tree. When it comes tiem to draw, the scene class switches the CameraMangers, active camera to the one refrenced by the scene. This ensures each scene is drawn with the correct camera.


5.1.6 Input

The input system allows for event an event based input system. Watchers are to expose events and are updated by the InputManager class. When the status of input devices is desired, the appropriate watcher is created and added to the input manager. The manager will invoke the update function on the watcher. The when certain input events occur, publicly visible events in the watcher are raised to invoke any attached delegates.

The watcher class is abstract and has abstract methods for updating.

The engine also includes default specifications of the watcher class for peripheral inputs such as the mouse and keyboard.  The watchers expose keyboard and mouse related events and are invoked when key events or mouse events occur.

The input system also defines a templated InputScheme class. The class’s responsibility is to map a watcher’s events to a given object’s functions. The Generic InputScheme class takes a generic type argument. This generic type is the type of the object that will have its functions mapped to watcher events.

The Apply(T object) function of the InputScheme will keep a reference to the object and then call MapWatcherEvents(). This abstract function will be overridden to explicitly assign the object’s functions to a created watcher’s events