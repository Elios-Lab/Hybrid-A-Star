using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathfindingForVehicles
{
    public class Map
    {
        //Map dimensions
        private int mapWidth;

        private float cellWidth;
    
        //All cells in the map
        public Cell[,] cellData;

        //All obstacles on the map
        //Should be List because we are refering to positions in this list from each cell
        public List<Obstacle> allObstacles;



        public Map(int mapWidth, float cellWidth)
        {
            this.mapWidth = mapWidth;

            this.cellWidth = cellWidth;
        
            //Generate the map
            cellData = new Cell[(int)(mapWidth / cellWidth), (int)(mapWidth / cellWidth)];
            
            float halfCellSize = Parameters.cellWidth * 0.5f;

            for (int x = 0; x < (int)(mapWidth / cellWidth); x++)
            {
                for (int z = 0; z < (int)(mapWidth / cellWidth); z++)
                {
                    Vector3 centerOfCell = new Vector3((x * cellWidth) + halfCellSize, 0f, (z * cellWidth) + halfCellSize);

                    cellData[x, z] = new Cell(centerOfCell);
                }
            }


            //Init the obstacles
            allObstacles = new List<Obstacle>();
        }



        //
        // Getters
        //

        //The number of cells in a row (the map is always square)
        public int MapWidth
        {
            get 
            {
                return mapWidth;
            }
        }

        //The width of one cell in [m]
        public float CellWidth
        {
            get
            {
                return cellWidth;
            }
        }


        //Sometimes we use standardized methods so we need separate arrays
        
        //The center position of each cell in world space
        public Vector3[,] CellCenterArray
        {
            get
            {
                Vector3[,] cellCenterArray = new Vector3[(int)(mapWidth / cellWidth), (int)(mapWidth / cellWidth)];
            
                for (int x = 0; x < (int)(mapWidth / cellWidth); x++)
                {
                    for (int z = 0; z < (int)(mapWidth / cellWidth); z++)
                    {
                        cellCenterArray[x, z] = cellData[x, z].centerPos;
                    }
                }

                return cellCenterArray;
            }
        }

        //Is this cell blocked by obstacle
        public bool[,] CellObstacleArray
        {
            get
            {
                bool[,] cellObstacleArray = new bool[(int)(mapWidth / cellWidth), (int)(mapWidth / cellWidth)];

                for (int x = 0; x < (int)(mapWidth / cellWidth); x++)
                {
                    for (int z = 0; z < (int)(mapWidth / cellWidth); z++)
                    {
                        cellObstacleArray[x, z] = cellData[x, z].isObstacleInCell;
                    }
                }

                return cellObstacleArray;
            }
        }

        //The voronoi field
        public VoronoiFieldCell[,] VoronoiField
        {
            get
            {
                VoronoiFieldCell[,] voronoiField = new VoronoiFieldCell[(int)(mapWidth / cellWidth), (int)(mapWidth / cellWidth)];

                for (int x = 0; x < (int)(mapWidth / cellWidth); x++)
                {
                    for (int z = 0; z < (int)(mapWidth / cellWidth); z++)
                    {
                        voronoiField[x, z] = cellData[x, z].voronoiFieldCell;
                    }
                }

                return voronoiField;
            }
        }



        //
        // Help methods
        //

        //Convert from world position to a cell pos
        public IntVector2 ConvertWorldToCell(Vector3 coordinate)
        {
            IntVector2 cellPos = new IntVector2();

            cellPos.x = Mathf.FloorToInt(coordinate.x / CellWidth);
            cellPos.z = Mathf.FloorToInt(coordinate.z / CellWidth);

            return cellPos;
        }

        //Is a cell position within the grid?
        public bool IsCellWithinGrid(IntVector2 cellPos)
        {
            bool isWithIn = false;

            if (cellPos.x >= 0 && cellPos.x < (int)(mapWidth / cellWidth) && cellPos.z >= 0 && cellPos.z < (int)(mapWidth / cellWidth))
            {
                isWithIn = true;
            }

            return isWithIn;
        }

        //Is a world position within the grid?
        public bool IsPosWithinGrid(Vector3 pos)
        {
            bool isWithIn = false;

            IntVector2 cellPos = ConvertWorldToCell(pos);

            if (IsCellWithinGrid(cellPos))
            {
                isWithIn = true;
            }

            return isWithIn;
        }

    }
}
