# PenguinRestaurant
Animal Restaurant Tycoon Game

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
06/17 Added flyer image variations in the gacha scene, filled empty areas around the map, and made gacha cost increase with each new collection entry
06/18 Increase furniture placement cost based on the number placed, and add growth formulas for cooking tools
06/19 Display the purchase cost above the customer's head using UI, now showing up to two decimal places
06/20 Added shop reputation system that influences customer spawn intervals, along with position adjustments and bug fixes
06/21 Improved an issue where blocked paths weren't updated during furniture relocation, and fixed a bug where relocated cooking appliances were excluded from staff behavior routines
06/22 Adjusted object placement to match actual size, fixed staff spawning inside buildings on game load, and resolved water appearing white on mobile devices
06/23 Enter the clicker mini-game when the trash bin is full and fix several bugs
06/24 Added sound to the reward box and applied a highlight when the reward is available
06/25 Convert the emoji and currency gain UI to world space and add particle effects when animals eat food
06/26 Display cooking equipment shortage icon only when fish is lacking, update the icon graphic, add animal rank names, and include animal personality traits
06/27 Fixed irregular animal feeding behavior, added currency rewards based on last login time when loading the vending machine, and corrected visual glitches
06/28 Fixed character freezing caused by invalid key after meal reward, and modified enemies to be reusable for continuous spawning during gameplay
06/29 Made fishing minigame reusable, fixed XP bar occlusion by airborne penguins, and added highlight to active minigame button
06/30 Add a button to proceed to the next stage when the required count is reached, and adjust the grid size of the counter
07/01 Allow table furniture to be placed adjacent in the grid-based placement system, and ensure that only unobstructed seats are available for use
07/03 Restrict characters interacting with furniture during rearrangement and move them to the updated positions after placement changes
07/04 If a character is at a table and the seat becomes unavailable due to attachment to another table, make the character move to another available seat
07/05 Fix the issue where previous values are not properly cleared when reverting in the placement system
07/06 Fixed bugs occurring during path recalculation caused by furniture rearrangement
07/08 Fixed issues occurring during food competition between customers and enemies at tables
07/09 Fixed an issue where characters at the edge of the screen were culled incorrectly in a 9:16 aspect ratio environment
07/10 Adjusted the UI transparency and height for order status, and added installed doors to the furniture list to allow repositioning
07/13 Added controller to allow doors to be moved to different walls, and updated guest interactions to work correctly after door relocation
07/14 Improved zoom in/out controls for mobile and changed shop access from swipe to click
07/15 Fixed camera clipping into walls and added English localization script
07/16 Added language change functionality to the UI, and fixed an issue where trash bins could be placed touching each other
07/17 Fixed incorrect placement of the trash can and resolved errors occurring during door repositioning
07/30 Bug fixes, graphics settings added, post-processing applied
07/31 Fixed an issue where the batching of walls was broken when repositioning doors
08/01 Add a separate scene for the initial asset download on the game's first launch, and allow the player to manually start the download
08/01 Add a separate scene for the initial asset download on the game's first launch, and allow the player to manually start the download
08/03 Implement download scene with hash-based file updates and server reconnection retry
08/04 Fixed removal of cached old files on update, proper resource release on game exit, and error handling when quitting during loading
08/06 Improve resource unloading for smoother transitions and add Android toast messages
08/07 Control Android toast messages and disable back button functionality during initial game loading
08/08 Update settings UI for sound and haptic feedback
08/09 Added BGM toggle, haptic feedback for in-game actions, and saving of game settings data
08/10 Adjusted employee experience values and recruitment costs, and fixed an issue where fish in the river below the fishing spot were not visible
08/13 Added game guide script and guide character image
08/17 Added tutorial steps 1 through 7
08/18 Added tutorial steps 8 through 12

