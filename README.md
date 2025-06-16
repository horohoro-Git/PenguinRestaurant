# PenguinRestaurant
Animal Restaurant Tycoon Game
"plus DJ303"

04/29 Overall code refactoring and adding asset loading system
04/30 Improved input system usage and camera dragging
05/01 Camera movement, UI input and interaction overhaul, and various bug fixes
05/02 Several fixes and a loading scene added, check locations where staff can be hired
05/05 Penguin employment and animal movement no longer get stuck in structures
05/07 Adding emotional expressions to animals
05/08 Adding animals lod system
05/09 Applied the newly added features to all animals, updated the illustrations of the animals
05/10 Added settings window and optimized memory usage
05/11 Fixed the issue where multiple baked characters were displayed as the same character, fixed the issue where animals would misalign when lining up, updated to allow penguins to be picked up by hand
05/12 Optimized pathfinding and fixed customer behavior on controller reuse
05/13 Improved pathfinding with animal size and fixed penguin position bug
05/14 Modified the pathfinding system to pre-process neighbor checks, and applied character UI batching
05/15 Grid-Based Building System (In Progress)
05/16 Added a grid-based placement system and shop UI to improve user interaction
05/17 Integrate the placement feature with the shop system
05/18 Improve loading speed, add placeable furniture items, save grid-based placement data and add an “undo placement” function, keep furniture objects draggable until they’re fully placed
05/19 Added repositioning, improved UI-placement sync, and tray animation on appliance activation
05/20 Polish cooking animations, clean up scripts, and add door opposite first counter
05/21 Furniture layout improves pathfinding efficiency, Guests begin to gather around the generated doorway once the conditions are met
05/22 Minor bug fixes and changed the background map's ground to a texture
05/23 Background buildings converted to textures. Fixed staff path deviation on spawn
05/24 Added support for placing multiple furniture, shop-based building expansion, and auto-door placement on expansion overlap
05/26 Fixing bugs in the mobile environment
05/27 Enhanced resource value abbreviation system
05/28 Bug fixes, added animation to currency abbreviation
05/29 Refactored data table structure, implemented new fuel UI
05/30 Change the grid selection from a Line Renderer to a sprite, enable fuel consumption for the cooking device and Display a UI warning when fuel is low
05/31 Add a vending machine that generates income even while the game is not running, update the UI to display upgrade costs in abbreviated format
06/01 The UI button now plays a sound when clicked
06/02 Add sounds for food preparation, food movement, currency, and furniture placement
06/03 Add sound licensing information, eating sounds, and emotional expression sounds
06/04 Limited purchase scale animation to model only, added shop open/close sounds
06/05 Add BGM (loading/in-game), Revise seating: pick between 2 zones, align toward nearest of 4 seat points, sit in a straight line
06/06 Add a black consumer character who steals customers' food and interacts with guests eating at the table
06/07 Allow intruders to be defeated by touch, add a defeat sound effect, and play a firework sound when the map expands
06/08 Added sound to the gacha map and adjusted patty orientation on the burger grill
06/09 Conducting tests for migrating the rendering pipeline from Built-In to URP
06/10 Completed migration to URP and restored broken batching caused by the pipeline change
06/11 Added a custom time scale for the restaurant and made all non-UI asynchronous operations in the restaurant follow it; also added a minigame entry feature
06/12 Add the ability to pollute the river water and include living, moving fish in the mini-game
06/13 Fixed water ripple visibility issue based on distance; fish can now be tapped to receive rewards
06/14 Changed restaurant sound to 3D, added sound pooling, and adjusted render queue of afterimage particles hidden behind water
06/15 Added dying fish sound and effect, and adjusted overlapping sound volumes
06/16 Added a cutscene to the draw sequence and made rewards generated at the start of the draw