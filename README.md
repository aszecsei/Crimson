# Crimson Engine

> An engine for MonoGame

## How To Use

### Installation

Use this repository as a git submodule. Add the `Crimson` project to your solution, along with the optional `Crimson.UI` project if you want the UI features.

You will need to copy over the `Content\Crimson` folder to your MonoGame project's content folder. Ensure that the folder layout remains the same.

### Design

Concepts in the Crimson engine revolve around a traditional, entity-component based organization, alongside multiple "subsystems." Components still contain logic, making this separate from a true ECS. Instead, any subsystems simply run alongside the component update loop.

Thus, for example, a physics subsystem would be able to iterate over all physics-related components multiple times during collision resolution, something that would be more difficult to do in an entirely EC-based model.

All EC-based logic exists within a `Scene`, which can be loaded in and out during gameplay. Subsystems run during *all* scenes, and so are more useful to implement large, engine-level features. Game-specific logic should likely be confined to entities and components.

Your Game1 class should inherit from the `Crimson.Engine` class. Any values should have sane defaults, but can be changed at will.