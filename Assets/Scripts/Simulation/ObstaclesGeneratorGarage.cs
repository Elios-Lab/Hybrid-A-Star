using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SelfDrivingVehicle;

namespace PathfindingForVehicles
{
    //Generates all obstacles, u�ncluding the flowfield showing the distance to the closest obstacle
    public class ObstaclesGeneratorGarage : ObstaclesGenerator
    {
        public GameObject obstaclePrefabObj;

        public override void InitObstacles(Map map)
        {
            AddObstacle(map);
            RandomAgentPositioning();
            RepositionTargetRandom();
        }
        public override void MoveObstacles(Map map)
        {
            RandomAgentPositioning();
            RepositionTargetRandom();
            GenerateField(map);
        }

        public override void RandomAgentPositioning()
        {
            if (agent != null)
            {
                //Call the function for the generation of the random spot position
                float maxXAgent = 0, maxZAgent = 0, minXAgent = 0, minZAgent = 0;

                float rotation = GenerateRandomValueInRange(0,360);

                float x_base;
                float z_base;

                maxXAgent = 21.25f - (carLength / 2); // * Mathf.Abs(Mathf.Cos(Mathf.Deg2Rad * rotation)) - (carLength / 2) * Mathf.Abs(Mathf.Sin(Mathf.Deg2Rad * rotation));
                maxZAgent = 14.25f - (carLength / 2); // * Mathf.Abs(Mathf.Sin(Mathf.Deg2Rad * rotation)) - (carLength / 2) * Mathf.Abs(Mathf.Cos(Mathf.Deg2Rad * rotation));
                minXAgent = 2f + (carLength / 2); // * Mathf.Abs(Mathf.Cos(Mathf.Deg2Rad * rotation)) + (carLength / 2) * Mathf.Abs(Mathf.Sin(Mathf.Deg2Rad * rotation));  
                minZAgent = 9f + (carLength / 2); // * Mathf.Abs(Mathf.Sin(Mathf.Deg2Rad * rotation)) - (carLength / 2) * Mathf.Abs(Mathf.Cos(Mathf.Deg2Rad * rotation));

                //X coordinate for possible available position randomized after the rotation of the vehicle
                x_base = GenerateRandomValueInRange(Mathf.Floor(minXAgent), Mathf.Floor(maxXAgent));

                //Z coordinate for possible available position randomized after the rotation of the vehicle
                z_base = GenerateRandomValueInRange(Mathf.Floor(minZAgent), Mathf.Floor(maxZAgent));

                //Creating a random boolean... 
                // if (GenerateRandomValueInRange(0, 2) == 1)
                //     x_base = -x_base;

                // if (Random.Range(0, 2) == 1)
                //     z_base = -z_base;

                //Setting properties
                // agent.GetComponent<Rigidbody>().velocity = Vector3.zero;
                // agent.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                // agent.GetComponent<CarController>().CurrentSteeringAngle = 0f;
                // agent.GetComponent<CarController>().CurrentAcceleration = 0f;
                // agent.GetComponent<CarController>().CurrentBrakeTorque = 0f;

                //Setting random rotation
                agent.transform.rotation = Quaternion.Euler(0, rotation, 0);

                //Setting random position
                agent.transform.position = new Vector3(x_base, 1, z_base);

            }
        }

        public override void RepositionTargetRandom()
        {
            // int i = Random.Range(0, garage.Length);
            int i = (int)GenerateRandomValueInRange(0, 10);
            float x = 0;
            float z = 0;

            //int i = 5;
            float[] possibleX = {4f, 8f, 12f, 16f, 20f};

            //Set the goal rotation -90 garage above 90 garage below
            if (i >= 0 && i <= 4)  
            {
                goal.transform.rotation = Quaternion.Euler(0, 180, 0);
                z = 4.5f;
            }
            else
            {
                goal.transform.rotation = Quaternion.Euler(0, -180, 0);
                z = 17.5f;
            }   
            
            x = possibleX[(int)GenerateRandomValueInRange(0, 5)];
            //Set the goal position to the TargetSpawn(child object of garage) position
            // goal.transform.position = garage[i].GetChild(4).position;
            goal.transform.position = new Vector3(x, 0.01f, z);

        }

        protected override void AddObstacle(Map map)
        {
            for (int i = 0; i < 5; i++) 
            {
                for(int j = 0; j < 2; j++) 
                {
                    //Generate random coordinates in the map
                    //float posX = GenerateRandomValueInRange(1f, map.MapWidth - 1f);
                    //float posZ = GenerateRandomValueInRange(1f, map.MapWidth - 1f);
                    float posX = 3.9f + (4f * i);
                    float posZ;
                    if(j == 0) {posZ = 4.5f;}
                    else {posZ = 17.5f;}
                    //Rotation
                    float rotY;
                    if(j == 0) {rotY = 0f;}
                    else {rotY = 180f;}
                    //Size
                    //float sizeX = GenerateRandomValueInRange(Parameters.minObstacleSize, Parameters.maxObstacleSize);
                    //float sizeZ = GenerateRandomValueInRange(Parameters.minObstacleSize, Parameters.maxObstacleSize);

                    Vector3 pos = new Vector3(posX, -0.5f, posZ);

                    Quaternion rot = Quaternion.Euler(0f, rotY, 0f);

                    //Vector3 scale = new Vector3(sizeX, 1f, sizeZ);

                    //Update the prefab with the new data
                    obstaclePrefabObj.transform.position = pos;
                    obstaclePrefabObj.transform.rotation = rot;
                    //obstaclePrefabObj.transform.localScale = scale;

                    // for (int n = 0; n < obstaclePrefabObj.transform.childCount; n++)
                    // {
                    //     Obstacle newObstacle = new Obstacle(obstaclePrefabObj.transform.GetChild(n).transform);

                    //     //The obstacle shouldnt intersect with the start area
                    //     //if (Intersections.AreRectangleRectangleIntersecting(avoidRect, newObstacle.cornerPos))
                    //     //{
                    //     //    return;
                    //     //}

                    //     map.allObstacles.Add(newObstacle);
                    // }

                    //Add a new obstacle object at this position
                    Instantiate(obstaclePrefabObj, obstaclesParent);
                }
            }  

            GameObject[] barriers = GameObject.FindGameObjectsWithTag("barrier");
            foreach (GameObject barrier in barriers)
            {
                Obstacle newObstacle = new Obstacle(barrier.transform);
                map.allObstacles.Add(newObstacle);
            }
        }
    }
}
