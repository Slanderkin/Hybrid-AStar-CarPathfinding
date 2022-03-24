# Hybrid-AStar-CarPathfinding

This is a project to implement a hybrid A* pathfinding algorithm applied to a car. Hybrid A* allows for a grid based systems where the 2D movement of the car is kept in a 3D grid (X,Y,Theta). This has the advantage of allowing smoother movement as compared to traditional A* which forces position to be taken at the center of a grid cell. Currently the project has only implemented the raw pathfinding algorithm, however I am working on adding obstacles (and border collision) using a Voronoi field to generate a density map which will allow for the car to know how close it is to obstacles to avoid them. 

To start, hold left click anywhere within the white border and move your mouse to set the end angle. Use WASD to move the camera and scroll to zoom in/out. You can step through the pathfinding one path or one node at a time by enabling the debug menu.


