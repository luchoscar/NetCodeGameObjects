# NetCodeGameObjects
 ArgusLabs 

## Architecture
### Client Authoritative with Server Validator
- Fast feedback for players on input
- Server validates if player can move to the desired positions.  Failing validation sends a request to force update the object's position
- Low chance of cheating as server reverts an invalid client state
### Single Access 
- Service provider to allow classes to select required services
- Sevices can be injected into their users via interfaces in case they need to be swaped 

## Challenges
### Validating object movements
Servers simulates objects movemet when receving a request.  
Validation time depends on the amount of objects to validate.  Worst case can be of O(n^2) which can beptimized by:
- Implementing spacial partition to reduce checks, or
- Running physics, server side only, to use collision detection to signal invalid potential movements
### Character rewining due to server correction
Server correction can be very noticeable depending on what it is correcting (possition, instantiation, etc)
This can be minimized by:
- Blocking player input for a couple seconds and reset variables involved in muvement
- Using animation transitions
- Only rewining player's actions that do not have a high impact on player experience
- Use a better prediction logic to anticipate collision 5-10 frames ahead 
  
## Client Controlls
WASD to move character on XY-plane

## Command Line
### Windows
- Starting via command line: `NetCodeGameObjects.exe -mode <mode>` where mode can be `client`, `server` or `host`
- Starting server: `NetCodeGameObjects.exe -logfile log-server.txt -mode client`
- Starting headless server: `NetCodeGameObjects.exe -logfile log-server.txt -mode server -batchmode -nographics`
- Stopping server:
	- Finding process pdi: `tasklist | more | findstr  NetCodeGameObjects.exe`
	- Killing process: `taskkill /f /pid <pid>`
