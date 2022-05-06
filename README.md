## Instructions

Make a simple Match 3 game based on this Unity project.

### Unity version

Make sure the Unity version you use matches the one in the project.
You can see that in Unity Hub, or by looking in the file `ProjectSettings/ProjectVersion.txt`.

### Game mechanics

The game mechanics are as follows:

  * The game consists of m x n tiles (configurable in editor or code).

  * If three or more of the same tile type are in a horizontal row, the game removes them.

  * A tile falls down if there is no tile below it (unless it's on the bottom row).

  * The user can click on a tile. The game then removes the tile.

  * When the game starts, all the positions should be filled with tiles and nothing should fall down.

Do not add more features than this.

### Other guidelines

Make it simple, but write the code to a quality that's good enough for professional developers to continue working on.
As a performance guideline for the code, design it so it runs well on relatively limited devices such as mobiles.

## Requirements

  * Do not use the physics engine to move the tiles.

  * Do not use raycasting for anything except if required to detect what tile the player clicked on.
