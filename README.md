# NetCodeGameObjects
 ArgusLabs 


Client-Server architecture: Client authoritative and Server validator
	- Fast feedback for players on input
	- Server validates if player can move to the desired positions

Windows helpfull commands for running via headless server mode:
- Starting server:
	NetCodeGameObjects.exe -logfile log-server.txt -mode client -batchmode -nographics 
- Stopping server:
	- Finding process pdi
		tasklist | more | findstr  NetCodeGameObjects.exe
	- Killing process
		taskkill /f /pid <pid>
