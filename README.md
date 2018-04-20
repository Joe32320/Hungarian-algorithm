# Hungarian-algorithm

Description:
An implementation of the Hungarian Algorithm for the Transportation problem, often stated as a problem where x supplies from n number of factories need to deliever y supplies to m number of warehouses, with an integer transportation cost associated between each warehouse and factory pairing. The problem is to find a/the minimum cost solution.

Implementation details:
Algorithm was generally copied and adapted from a description in a textbook, hence the weird method names.
The code was written to run within the Unity Game engine, as part of a another project, so will not compile currently on its own

How to Use:
First step is to call the constructor which deals with setting up the problem to be solved. supplyVertices and demandVertices represent the objects acting as the supply and demand vertices in the problem. costDict is a dictionary containing the costs between each supply and demand vertex, the supply vertices are represented by the outside dictionary. supplyAmount and demandAmount are the amount of supplies at, or demanded by each supply/demand vertex respectfully. Important: The sum of each supplyAmount and demandAmount must be equal for the program to run correctly, there is no currenlty implemented defensive programming protcol implemented to check this. Also the weights in costDict must be greaterthan or equal to 0, negative numbers will cause unwanted behaviour.

To run the algorthim, call "solveProblem()", which will then proceed to solve the problem, and print out the amount of supplies to be sent from each supply vertex to each demand vertex, and the total cost associated with the problem in the Debug log within Unity.

Todo:

Make variation to work out the box, with an example of it working printing via command line rather than relying on Unity's debug log.
