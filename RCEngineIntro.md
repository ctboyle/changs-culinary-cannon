1. Intro

The Ragde’s Cube (RC) Engine is a game engine to be made for the Ragde’s cube game. Though it was made for the specifically for the Ragade’s Cube game, it is intedned to be used for all gaming applications. The game engine is based on the Managed Microsoft XNA framework. Since the XNA technology is relatively new, there is a lack of full fledged engines available. The RC engine will demonstrate the minimum requirements to fulfill a game.

1.1 Purpose

This document describes the detailed design of the RC Engine. The purpose of this design document is to provide a detailed design of all game components for the Ragade’s Cube project.

1.2 Scope

This document is intended to show the partitioning between the various entities in the game engine and their hierarchical dependency relationships.  This document also defines the interfaces that the various designers, programmers, and testers will need to understand in order to use the system.  Lastly, this document shall describe the detailed design of each entity within the game engine.

The entities of the engine that will be documented are:
  1. SceneGraph
  1. Rendering
  1. Picking
  1. GUI
  1. StateManagement
  1. Input

1.3 Definitions & Acronyms

  * Camera – Viewpoint for rendering a scene that is a scene node.
  * Engine – Provides prepackaged game functionality to make game development simpler.
  * GUI – Graphical User Interface. For the engine, it is a system that manages object that can be interacted with
  * Picking – A term used to refer to the process of selecting a 3D object from 2D screen coordinates (from a mouse click for example)
  * Rendering – The process of taking 3D dimensional data and transforming it to 2D screen space. (Rasterization)
  * State – A phase of a game that is exclusive to other phases.
  * Homogeneous Matrix Transformation – Process of applying a translation, rotation and scaling to a vector or other Homogeneous Matrices.
  * XNA – Microsoft’s framework for Game Development.