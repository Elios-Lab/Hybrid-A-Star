using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SelfDrivingVehicle;

namespace PathfindingForVehicles
{
    //Generates all obstacles, u�ncluding the flowfield showing the distance to the closest obstacle
    public class ObstaclesGenerator : MonoBehaviour
    {
        //Drags
        //The parent of the obstacle to get a cleaner workspace
        public Transform obstaclesParent;
        //Obstacle cube we add to the scene
        // public GameObject obstaclePrefabObj;

        [SerializeField] public GameObject environment;
        [SerializeField] public GameObject car;
        [SerializeField] public GameObject goal;  
        [SerializeField] public float carLength;
        [SerializeField] public float carWidth;
        [SerializeField] public enum EnvironmentComplexity {BASIC=1, ENTRY=2, MEDIUM=3, ADVANCED=4, EXTREME=5, ROOM1=6, ROOM2=7, ROOM3=8, ROOM4=9};
        [SerializeField] public EnvironmentComplexity environmentComplexity = EnvironmentComplexity.BASIC;
        [SerializeField] public GameObject obstacleToSpawn, staticObstacleToSpawn, staticAlignedObstacleToSpawn;

        Transform max;
        float maxX = 0, maxZ = 0;
        [HideInInspector] public VehicleDataController agent;
        private static System.Random rand = new System.Random();
            

        void Start() 
    {
            // max = GameObject.FindWithTag("barrier").GetComponentInChildren<Transform>();
            GameObject[] maxAll = GameObject.FindGameObjectsWithTag("barrier");
            foreach (GameObject barrier in maxAll)
            {
                if (barrier.transform.position.x > maxX)
                {
                    maxX = barrier.transform.position.x;
                }
                if (barrier.transform.position.z > maxZ)
                {
                    maxZ = barrier.transform.position.z;
                }
            }
            // maxX = max.localScale.x / 2 - carLength;            
            // maxZ = maxX;
            agent = car.GetComponent<VehicleDataController>();

            // RandomObstaclesPositioning();  
    }

        public void InitObstacles(Map map)
        {
            //Generate obstacles
            DestroyOldObss();
            RandomAgentPositioning();
            RepositionTargetRandom();
            AddObstacle(map);
        }

        public void MoveObstacles(Map map)
        {
            RandomAgentPositioning();
            RepositionTargetRandom();
            RandomizeObstacleAttribute(map);
            GenerateField(map);
        }

        //Generate obstacles and return the center coordinates of them in a list 
        //We need the car data so we can avoid adding obstacles at that position
        private void GenerateObstacles(Map map)
        {
            //The rectangle where the car starts so we can remove obstacles in that area
            
            /*float marginOfSafety = 0f;

            float halfLength = (4f + 11f + marginOfSafety) * 0.5f;
            float halfWidth = (3f + marginOfSafety) * 0.5f;

            //The center pos is not the startPos because the semi is not the center of trailer + semi
            startPos += Vector3.forward * -6f;

            Vector3 FL = startPos + Vector3.forward * halfLength - Vector3.right * halfWidth;
            Vector3 FR = startPos + Vector3.forward * halfLength + Vector3.right * halfWidth;
            Vector3 BL = startPos - Vector3.forward * halfLength - Vector3.right * halfWidth;
            Vector3 BR = startPos - Vector3.forward * halfLength + Vector3.right * halfWidth;

            Rectangle avoidRect = new Rectangle(FL, FR, BL, BR);
            */
            //for (int i = 0; i < Parameters.obstaclesToAdd; i++)
            //{
            //}
        }

        public void RandomAgentPositioning()
        {
            float rotation;
            float[] randomXZ = GenerateRandomXZ();

            float x_base = randomXZ[0];
            float z_base = randomXZ[1];

            if (agent != null)
            {           
                switch(environmentComplexity)
                {
                    case EnvironmentComplexity.BASIC:
                        x_base = 0f;
                        z_base = 0f;
                        rotation = 0f;
                        break;
                    case EnvironmentComplexity.ENTRY:
                        rotation = GenerateRandomValueInRange(0,360);
                        break;
                    case EnvironmentComplexity.MEDIUM:
                        rotation = GenerateRandomValueInRange(0, 360);
                        break;
                    case EnvironmentComplexity.ADVANCED:
                        rotation = GenerateRandomValueInRange(0, 360);
                        break;
                    case EnvironmentComplexity.ROOM1:
                        rotation = GenerateRandomValueInRange(-45, 45);
                        break;
                    case EnvironmentComplexity.ROOM2:
                        rotation = GenerateRandomValueInRange(-45, 45);
                        break;
                    case EnvironmentComplexity.ROOM3:
                        rotation = GenerateRandomValueInRange(-45, 45);
                        break;
                    case EnvironmentComplexity.ROOM4:
                        rotation = GenerateRandomValueInRange(-45, 45);
                        break;
                    default:
                        rotation = GenerateRandomValueInRange(0, 360);
                        break;
                }            

                //Setting properties
                // agent.GetComponent<Rigidbody>().velocity = Vector3.zero;
                // agent.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                // agent.GetComponent<CarController>().CurrentSteeringAngle = 0f;
                // agent.GetComponent<CarController>().CurrentAcceleration = 0f;
                // agent.GetComponent<CarController>().CurrentBrakeTorque = 0f;

                //Setting agent rotation
                agent.transform.rotation = Quaternion.Euler(0, rotation, 0);            

                //Setting agent position              
                agent.transform.position = new Vector3(x_base, 1, z_base);      

            }
        }



        void DestroyOldObss()
        {
            GameObject[] obstacles = GameObject.FindGameObjectsWithTag("obstacle");
            GameObject[] obstaclesAligned = GameObject.FindGameObjectsWithTag("obstacleAligned");
            foreach (GameObject obstacle in obstacles)
            {
                Destroy(obstacle);
            }
            foreach (GameObject obstacle in obstaclesAligned)
            {
                Destroy(obstacle);
            }
        }
        //Instantiate one cube and add its position to the array
        void AddObstacle(Map map)
        {
            // for (int i = 0; i < 5; i++) 
            // {
            //     for(int j = 0; j < 2; j++) 
            //     {
            //         //Generate random coordinates in the map
            //         //float posX = GenerateRandomValueInRange(1f, map.MapWidth - 1f);
            //         //float posZ = GenerateRandomValueInRange(1f, map.MapWidth - 1f);
            //         float posX = 2.4f + (4f * i);
            //         float posZ;
            //         if(j == 0) {posZ = 3f;}
            //         else {posZ = 16f;}
            //         //Rotation
            //         float rotY;
            //         if(j == 0) {rotY = 0f;}
            //         else {rotY = 180f;}
            //         //Size
            //         //float sizeX = GenerateRandomValueInRange(Parameters.minObstacleSize, Parameters.maxObstacleSize);
            //         //float sizeZ = GenerateRandomValueInRange(Parameters.minObstacleSize, Parameters.maxObstacleSize);

            //         Vector3 pos = new Vector3(posX, -0.5f, posZ);

            //         Quaternion rot = Quaternion.Euler(0f, rotY, 0f);

            //         //Vector3 scale = new Vector3(sizeX, 1f, sizeZ);

            //         //Update the prefab with the new data
            //         obstaclePrefabObj.transform.position = pos;
            //         obstaclePrefabObj.transform.rotation = rot;
            //         //obstaclePrefabObj.transform.localScale = scale;

            //         for (int n = 0; n < obstaclePrefabObj.transform.childCount; n++)
            //         {
            //             Obstacle newObstacle = new Obstacle(obstaclePrefabObj.transform.GetChild(n));

            //             //The obstacle shouldnt intersect with the start area
            //             //if (Intersections.AreRectangleRectangleIntersecting(avoidRect, newObstacle.cornerPos))
            //             //{
            //             //    return;
            //             //}

            //             map.allObstacles.Add(newObstacle);
            //         }

            //         //Add a new obstacle object at this position
            //         Instantiate(obstaclePrefabObj, obstaclesParent);
            //     }
            // }  

            // Gatti code starts here

            int numberOfDynamicObstacle, numberOfAlignedObstacle;
            float x_base = 0;
            float z_base = 0;
            float rotation = 0;

            switch(environmentComplexity)
            {
                case EnvironmentComplexity.BASIC:
                    numberOfDynamicObstacle = 0;
                    numberOfAlignedObstacle = 0;
                    break;
                case EnvironmentComplexity.ENTRY:
                    numberOfDynamicObstacle = 0;
                    numberOfAlignedObstacle = 0;
                    break;
                case EnvironmentComplexity.MEDIUM:
                    numberOfDynamicObstacle = 0;
                    numberOfAlignedObstacle = 1;               
                    break;
                case EnvironmentComplexity.ADVANCED:
                    numberOfDynamicObstacle = 0;
                    numberOfAlignedObstacle = 6;
                    break;
                case EnvironmentComplexity.EXTREME:
                    numberOfDynamicObstacle = 0;
                    numberOfAlignedObstacle = 1;
                    break;
                case EnvironmentComplexity.ROOM1:
                    numberOfDynamicObstacle = 0;
                    numberOfAlignedObstacle = 1;
                    break;
                case EnvironmentComplexity.ROOM2:
                    numberOfDynamicObstacle = 1;
                    numberOfAlignedObstacle = 1;
                    StaticObstaclePositioning();
                    break;
                case EnvironmentComplexity.ROOM3:
                    numberOfDynamicObstacle = 4;
                    numberOfAlignedObstacle = 1;
                    // obstacleToSpawn.GetComponent<ObstacleMovement>().speed = 2;
                    StaticObstaclePositioning();
                    break;
                case EnvironmentComplexity.ROOM4:
                    numberOfDynamicObstacle = 8;
                    numberOfAlignedObstacle = 1;
                    // obstacleToSpawn.GetComponent<ObstacleMovement>().speed = 2;
                    StaticObstaclePositioning();
                    break;
                default:
                    numberOfDynamicObstacle = 0;
                    numberOfAlignedObstacle = 0;
                    break;
            }
            for (int i=0; i<numberOfDynamicObstacle; i++)
            {
                x_base = GenerateRandomValueInRange(-Mathf.Floor(maxX), Mathf.Floor(maxX));
                z_base = GenerateRandomValueInRange(-Mathf.Floor(maxZ), Mathf.Floor(maxZ));
                rotation = GenerateRandomValueInRange(0, 360);
                obstacleToSpawn.transform.position = new Vector3(x_base + environment.transform.position.x, 
                                                        GameObject.FindWithTag("barrier").GetComponentInChildren<Transform>().position.y , 
                                                        z_base + environment.transform.position.z);
                Instantiate(obstacleToSpawn, obstacleToSpawn.transform.position, Quaternion.Euler(0, rotation, 0), environment.transform);
            }

            for (int i = 0; i < numberOfAlignedObstacle; i++)
            {
                Vector3 center = (goal.transform.position + agent.transform.position) * 0.5f;    
                center.y = 0f;       
                Instantiate(staticAlignedObstacleToSpawn, center, Quaternion.Euler(0, rotation, 0), environment.transform);
            }
        }

        private void RandomizeObstacleAttribute(Map map)
        {
            GameObject[] obstacles = GameObject.FindGameObjectsWithTag("obstacle");
            GameObject[] obstaclesAligned = GameObject.FindGameObjectsWithTag("obstacleAligned");

            foreach (GameObject obstacle in obstacles)
            {
                obstacle.transform.rotation = Quaternion.Euler(0, GenerateRandomValueInRange(0, 360), 0);
                // obstacleToSpawn.GetComponent<ObstacleMovement>().speed = GenerateRandomValueInRange(0.5f, 3);
                //Debug.Log("randObstacle");
            }

            Vector3 targetPosition = goal.transform.position;
            Vector3 agentPosition = agent.transform.position;
            //Vector3 center = ( (targetPosition + agentPosition) * 0.5f );
            Vector3 center1 = ( (targetPosition + agentPosition) * 0.5f );
            Vector3 center2 = ((targetPosition + center1) * 0.5f);
            Vector3 center3 = ( (targetPosition + center2) * 0.5f );

            float angleRad = -Mathf.Atan((targetPosition.z - agentPosition.z) / (targetPosition.x - agentPosition.x));
            float angleGrad;
            float tempRand;

            angleRad += Mathf.PI / 2;
            angleGrad = angleRad * 180 / Mathf.PI;

            for(int i=0; i<obstaclesAligned.Length; i++)
            {
                obstaclesAligned[i].transform.rotation = Quaternion.Euler(0, angleGrad + GenerateRandomValueInRange(-30, 30), 0);
                obstaclesAligned[i].transform.position = center1;
                switch(i)
                {
                    case 0:
                        break;
                    case 1:
                    case 2:
                        tempRand = GenerateRandomValueInRange(12, 15);
                        obstaclesAligned[i].transform.position = new Vector3(center1.x + tempRand * Mathf.Pow(-1.0f, i) * Mathf.Cos(Mathf.PI - angleRad), center1.y,
                                                                        center1.z + tempRand * Mathf.Pow(-1.0f, i) * Mathf.Sin(Mathf.PI - angleRad));
                        tempRand = GenerateRandomValueInRange(0, Mathf.Sqrt(Mathf.Pow(targetPosition.x - center1.x, 2) + Mathf.Pow(targetPosition.z - center1.z, 2)) * 4 / 5);
                        obstaclesAligned[i].transform.position += new Vector3(tempRand * Mathf.Pow(-1.0f, i) * Mathf.Cos(Mathf.PI - angleRad - Mathf.PI / 2), 0,
                                                                        tempRand * Mathf.Pow(-1.0f, i) * Mathf.Sin(Mathf.PI - angleRad - Mathf.PI / 2));
                        break;
                    case 3:
                        obstaclesAligned[i].transform.position = new Vector3(center2.x + Mathf.Cos(Mathf.PI - angleRad), center2.y,
                                                                        center2.z + Mathf.Sin(Mathf.PI - angleRad));
                        break;
                    case 4:
                    case 5:
                        tempRand = GenerateRandomValueInRange(12, 15);
                        obstaclesAligned[i].transform.position = new Vector3(center2.x + tempRand * Mathf.Pow(-1.0f, i) * Mathf.Cos(Mathf.PI - angleRad), center2.y,
                                                                        center2.z + tempRand * Mathf.Pow(-1.0f, i) * Mathf.Sin(Mathf.PI - angleRad));
                        tempRand = GenerateRandomValueInRange(0, Mathf.Sqrt(Mathf.Pow(targetPosition.x - center2.x, 2) + Mathf.Pow(targetPosition.z - center2.z, 2)) * 4 / 5);
                        obstaclesAligned[i].transform.position += new Vector3(tempRand * Mathf.Pow(-1.0f, i) * Mathf.Cos(Mathf.PI - angleRad - Mathf.PI / 2), 0,
                                                                        tempRand * Mathf.Pow(-1.0f, i) * Mathf.Sin(Mathf.PI - angleRad - Mathf.PI / 2));
                        break;
                    case 6:
                        obstaclesAligned[i].transform.position = new Vector3(center3.x + Mathf.Cos(Mathf.PI - angleRad), center3.y,
                                                                        center3.z + Mathf.Sin(Mathf.PI - angleRad));
                        break;
                    default:
                        tempRand = GenerateRandomValueInRange(12, 15);
                        obstaclesAligned[i].transform.position = new Vector3(center3.x + tempRand * Mathf.Pow(-1.0f, i) * Mathf.Cos(Mathf.PI - angleRad), center3.y,
                                                                        center3.z + tempRand * Mathf.Pow(-1.0f, i) * Mathf.Sin(Mathf.PI - angleRad));
                        tempRand = GenerateRandomValueInRange(0, Mathf.Sqrt(Mathf.Pow(targetPosition.x - center3.x, 2) + Mathf.Pow(targetPosition.z - center3.z, 2)) * 4 / 5);
                        obstaclesAligned[i].transform.position += new Vector3(tempRand * Mathf.Pow(-1.0f, i) * Mathf.Cos(Mathf.PI - angleRad - Mathf.PI / 2), 0,
                                                                        tempRand * Mathf.Pow(-1.0f, i) * Mathf.Sin(Mathf.PI - angleRad - Mathf.PI / 2));
                        break;
                }
                Obstacle newObstacle = new Obstacle(obstaclesAligned[i].transform);
                map.allObstacles.Add(newObstacle);    
                // print("Obstacle "+i+" rotation: "+obstaclesAligned[i].transform.rotation.eulerAngles.y);  
            }
        }


        public void RepositionTargetRandom()
        {
            float x_base;
            float z_base; 
            // float distanceX, distanceZ;
            float deltaTargetAgent = 5 * carLength;
            float[] randomXZ = GenerateRandomXZ(x: agent.GetCarTransform().position.x, z: agent.GetCarTransform().position.z, minDistance: deltaTargetAgent);

            x_base = randomXZ[0];
            z_base = randomXZ[1];


            // To avoid target too near the agent
            // float euclideanDistance = Mathf.Sqrt(Mathf.Pow((x_base - agent.GetCarTransform().position.x), 2) + Mathf.Pow((z_base - agent.GetCarTransform().position.z), 2));
            // // Debug.Log("AgentX: " + agent.GetCarTransform().position.x + "AgentZ: " + agent.GetCarTransform().position.z);
            // // distanceX = x_base - agent.GetCarTransform().position.x; // agent.transform.position.x;
            // // distanceZ = z_base - agent.GetCarTransform().position.z; // agent.transform.position.z;
            // // print("Agent Pos: " + agent.GetCarTransform().position.x + " " + agent.GetCarTransform().position.z);
            // if(euclideanDistance < deltaTargetAgent) {
            //     // Move target far from the agent keeping it into the (maxX, maxZ) boundaries
                
            // }
            
            // if(Mathf.Abs(distanceX) < deltaTargetAgent)
            // {                
            //     if(distanceX > 0)
            //     {
            //         x_base += (deltaTargetAgent - distanceX);
            //         if (x_base >= maxX)
            //             x_base = maxX;

            //         //Debug.Log("Minore X positivo");
            //     }
            //     else
            //     {
            //         x_base -= (deltaTargetAgent - Mathf.Abs(distanceX));
            //         if (x_base <= -maxX)
            //             x_base = maxX;

            //         //Debug.Log("Minore X negativo");
            //     }
            // }

            // if(Mathf.Abs(distanceZ) < deltaTargetAgent)
            // {                
            //     if(distanceZ > 0)
            //     {
            //         z_base += (deltaTargetAgent - distanceZ);
            //         if (z_base >= maxZ)
            //             z_base = maxZ;

            //         //Debug.Log("Minore Z positivo");
            //     }
            //     else
            //     {
            //         z_base -= (deltaTargetAgent - Mathf.Abs(distanceZ));
            //         if (z_base <= -maxZ)
            //             z_base = maxX;

            //         //Debug.Log("Minore Z negativo");
            //     }
            // }

            //Setting target position              

            goal.transform.position = new Vector3(x_base, 0, z_base);
            
        }   


        //Generate the flow field showing distance to closest obstacle from each cell
        private void GenerateObstacleFlowField(Map map, bool check8Cells)
        {
            int mapWidth = map.MapWidth;
            float cellWidth = map.CellWidth;

            //The flow field will be stored in this array
            FlowFieldNode[,] flowField = new FlowFieldNode[(int)(mapWidth / cellWidth), (int)(mapWidth / cellWidth)];

            //Init
            Cell[,] cellData = map.cellData;

            for (int x = 0; x < (int)(mapWidth / cellWidth); x++)
            {
                for (int z = 0; z < (int)(mapWidth / cellWidth); z++)
                {
                    //All nodes are walkable because we are generating the flow from each obstacle
                    bool isWalkable = true;

                    FlowFieldNode node = new FlowFieldNode(isWalkable, cellData[x,z].centerPos, new IntVector2(x, z));

                    flowField[x, z] = node;
                }
            }

            //A flow field can have several start nodes, which are the obstacles in this case
            List<FlowFieldNode> startNodes = new List<FlowFieldNode>();

            for (int x = 0; x < (int)(mapWidth / cellWidth); x++)
            {
                for (int z = 0; z < (int)(mapWidth / cellWidth); z++)
                {
                    //If this is an obstacle
                    if (cellData[x, z].isObstacleInCell)
                    {
                        startNodes.Add(flowField[x, z]);
                    }
                }
            }

            //Generate the flow field
            FlowField.Generate(startNodes, flowField, check8Cells);


            //Add the values to the celldata that belongs to the map
            for (int x = 0; x < (int)(mapWidth / cellWidth); x++)
            {
                for (int z = 0; z < (int)(mapWidth / cellWidth); z++)
                {
                    cellData[x, z].distanceToClosestObstacle = flowField[x, z].totalCostFlowField;
                }
            }
        }

        private void GenerateField(Map map) {
            int mapWidth = map.MapWidth;
            float cellWidth = map.CellWidth;

            //Figure out which cells the obstacle touch and set them to blocked by obstacle
            ObstaclesDetection.WhichCellsAreObstacle(map);

            //Generate the flow field showing how far to the closest obstacle from each cell
            GenerateObstacleFlowField(map, check8Cells: true);

            //Generate the voronoi field
            VoronoiFieldCell[,] voronoiField = VoronoiField.GenerateField(map.CellCenterArray, map.CellObstacleArray);

            for (int x = 0; x < (int)(map.MapWidth / map.CellWidth); x++)
            {
                for (int z = 0; z < (int)(map.MapWidth / map.CellWidth); z++)
                {
                    map.cellData[x, z].voronoiFieldCell = voronoiField[x, z];
                }
            }
        }
        private float[] GenerateRandomXZ(float x = -1, float z = -1, float minDistance = -1) {
            float distanceFromWalls = carLength;
            float x_base, z_base;
            if(x < 0 && z < 0) {
                x_base = (float)rand.NextDouble() * (maxX - 2*distanceFromWalls) + distanceFromWalls;
                z_base = (float)rand.NextDouble() * (maxZ - 2*distanceFromWalls) + distanceFromWalls;
                return new float[] {x_base, z_base};
            }
            float distU = maxX - x;
            float distD = x;
            z_base = GenerateRandomValueInRange(distanceFromWalls, maxZ - distanceFromWalls);
            if (distU > distD) // distU is bigger than distD, I'm in the lower half of the map
                x_base = GenerateRandomValueInRange(distD + minDistance, maxX - distanceFromWalls);
            else // distD is bigger than distU, I'm in the upper half of the map
                x_base = GenerateRandomValueInRange(distanceFromWalls, distD - minDistance);

            if(x_base < 0 || x_base > maxX)
            {
                Debug.Log("x_base" + x_base + " out of range");
                Debug.Log("X and z of the agent: " + x + " " + z);
                Debug.Log("minDistance: " + minDistance);
            }

            return new float[] {x_base, z_base};
        }

        private float GenerateRandomValueInRange(float from, float to) {
            // Generate a random double between 0.0 and 1.0
            double randomDouble = rand.NextDouble();

            // Scale and shift the random double to fit within the specified range
            float randomValue = (float)(randomDouble * (from - to) + to);

            return randomValue;
        }
        public void StaticObstaclePositioning()
        {
            float x_base = 0;
            float z_base = 0;

            // first static obstacle
            x_base = -max.localScale.x / 2 + staticObstacleToSpawn.transform.localScale.x / 2;
            z_base = -(max.localScale.x / 2 - max.localScale.x / 3);
            staticObstacleToSpawn.transform.position = new Vector3(x_base + environment.transform.position.x, 
                                                GameObject.FindWithTag("barrier").GetComponentInChildren<Transform>().position.y, 
                                                z_base + environment.transform.position.z); 
            Instantiate(staticObstacleToSpawn, staticObstacleToSpawn.transform.position, Quaternion.Euler(0, 0, 0), environment.transform);   
            
            // second static obstacle
            x_base = max.localScale.x / 2 - staticObstacleToSpawn.transform.localScale.x / 2;
            z_base = max.localScale.x / 2 - max.localScale.x / 3;
            staticObstacleToSpawn.transform.position = new Vector3(x_base + environment.transform.position.x, 
                                                GameObject.FindWithTag("barrier").GetComponentInChildren<Transform>().position.y, 
                                                z_base + environment.transform.position.z);
            Instantiate(staticObstacleToSpawn, staticObstacleToSpawn.transform.position, Quaternion.Euler(0, 0, 0), environment.transform);   
            
        }

    }
}
