using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeImp.DoomBuilder;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Config;
using System.Threading;


//  algo de Recursive Backtracking : génère le labyrinthe.
namespace GenerativeDoom
{




    class LevelLogic
    {
        int LevelMatrixSize;
        public DungeonRoom[,] Level;
        public List<Tuple<DungeonRoom, Direction>>  Points = new List<Tuple<DungeonRoom, Direction>>();
        

        private int Difficulty;
        private Random rng = new Random();
        public Point Start;
        public Point End;
        public int iterations = 0;





        public LevelLogic(int Size, int Diff)
        {
            LevelMatrixSize = Size;
            Difficulty = Diff;
            Level = new DungeonRoom[LevelMatrixSize, LevelMatrixSize];
            End = new Point(LevelMatrixSize - 1, LevelMatrixSize - 1);
            Start = new Point(0, 0);
            Initialise();
        }





        private void Initialise()
        {
            for (int row = 0; row < LevelMatrixSize; row++)
            {
                for (int col = 0; col < LevelMatrixSize; col++)
                {
                    this.Level[row, col] = new DungeonRoom();

                }
            }

            GénérerLab(Start);
            

        }


        public void GénérerLab(Point position)
        {
            Level[position.Y, position.X].Point = new Point(position.X, position.Y);
            Level[position.Y, position.X].Visited = true;
            Level[position.Y, position.X].position_in_iteration = ++iterations;

            List<Direction> validDirections = GetAllDirections();
            ValidateDirections(position, validDirections);

            //If there is no valid direction we have found a dead end.
            if (validDirections.Count == 0)
            {
                Level[position.Y, position.X].isdeadend = true;
                Points.Add(new Tuple<DungeonRoom, Direction>(Level[position.Y, position.X], Direction.Invalid));
            }

            while (validDirections.Count > 0)
            {
                Direction rndDirection = Direction.Invalid;

                if (validDirections.Count > 1)
                    rndDirection = validDirections[rng.Next(validDirections.Count)];
                else if (validDirections.Count == 1)
                    rndDirection = validDirections[0];

                Level[position.Y, position.X].visited_count = ++Level[position.Y, position.X].visited_count;

                RemoveWall(position, rndDirection);
                validDirections.Remove(rndDirection);
                Point newPos = GetAdjPos(position, rndDirection);
                Points.Add(new Tuple<DungeonRoom, Direction>(Level[position.Y, position.X], rndDirection));

                GénérerLab(newPos);

                ValidateDirections(position, validDirections);
            }

        }










        private void ValidateDirections(Point cellPos, List<Direction> directions)
        {
            List<Direction> invalidDirections = new List<Direction>();

            // Check for invalid moves
            for (int i = 0; i < directions.Count; i++)
            {
                switch (directions[i])
                {
                    case Direction.North:
                        if (cellPos.Y == 0 || CellVisited(cellPos.X, cellPos.Y - 1))
                            invalidDirections.Add(Direction.North);
                        break;
                    case Direction.East:
                        if (cellPos.X == LevelMatrixSize - 1 || CellVisited(cellPos.X + 1, cellPos.Y))
                            invalidDirections.Add(Direction.East);
                        break;
                    case Direction.South:
                        if (cellPos.Y == LevelMatrixSize - 1 || CellVisited(cellPos.X, cellPos.Y + 1))
                            invalidDirections.Add(Direction.South);
                        break;
                    case Direction.West:
                        if (cellPos.X == 0 || CellVisited(cellPos.X - 1, cellPos.Y))
                            invalidDirections.Add(Direction.West);
                        break;
                }
            }

            // Eliminating invalid moves
            foreach (var item in invalidDirections)
                directions.Remove(item);
        }

        private void RemoveWall(Point pos, Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    Level[pos.Y, pos.X].NorthWall = false;
                    Level[pos.Y - 1, pos.X].SouthWall = false;
                    break;
                case Direction.East:
                    Level[pos.Y, pos.X].EastWall = false;
                    Level[pos.Y, pos.X + 1].WestWall = false;
                    break;
                case Direction.South:
                    Level[pos.Y, pos.X].SouthWall = false;
                    Level[pos.Y + 1, pos.X].NorthWall = false;
                    break;
                case Direction.West:
                    Level[pos.Y, pos.X].WestWall = false;
                    Level[pos.Y, pos.X - 1].EastWall = false;
                    break;
            }
        }



        private bool CellVisited(int x, int y)
        {
            return Level[y, x].Visited;
        }



        private Point GetAdjPos(Point position, Direction direction)
        {
            Point adjPosition = position;

            switch (direction)
            {
                case Direction.North:
                    adjPosition.Y = adjPosition.Y - 1;
                    break;
                case Direction.East:
                    adjPosition.X = adjPosition.X + 1;
                    break;
                case Direction.South:
                    adjPosition.Y = adjPosition.Y + 1;
                    break;
                case Direction.West:
                    adjPosition.X = adjPosition.X - 1;
                    break;
            }

            return adjPosition;
        }


        private List<Direction> GetAllDirections()
        {
            return new List<Direction>() {
                Direction.North,
                Direction.East,
                Direction.South,
                Direction.West
            };

        }

    }


}
