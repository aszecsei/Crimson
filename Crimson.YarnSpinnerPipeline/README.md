# YarnSpinner Content Pipeline

> Import YarnSpinner files into MonoGame

## How To Use

### 1. Add a reference to your MonoGame content file

1. Click on `Content` at the root of the tree
2. Click on `References` in the properties pane
3. Click on the ellipses [`...`] button
4. Type in the path to the DLL built from this library, relative to the `Content.mgcb` file

Alternately, you can edit the `Content.mgcb` file directly.

### 2. Add the YarnSpinner program as content

It should automatically select the YarnSpinner importer, assuming your files use a `.yarn` extension.

Build your content.

### 3. Load the content into your game

You should now be able to use the following to load the yarn program:

```c#
var _yarnProgram = Content.Load<YarnProgram>("my_program");
```