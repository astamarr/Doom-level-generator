using System;
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


namespace GenerativeDoom
{
    class DungeonRoom
    {

        public bool Visited = false;
        public bool NorthWall = true;
        public bool SouthWall = true;
        public bool EastWall = true;
    
        public bool WestWall = true;
        public int position_in_iteration;
        public bool isdeadend = false;
        public Point Point;
        public int visited_count = 0;


        public DungeonRoom()
        {
          
        }











    }












}
