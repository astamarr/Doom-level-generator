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
    public partial class GDForm : Form
    {

        public
        int RoomSize ;
        int SectorSize ;
        LevelLogic Lvl;
        List<DrawnVertex> pSector ;

        bool Shotgun = false;
        bool Minigun = false;
        bool tronconneuse = false;
        bool laser = false;



        private IList<DrawnVertex> points;

        public GDForm()
        {
            InitializeComponent();
            points = new List<DrawnVertex>();
        }

        // We're going to use this to show the form
       


        private Thing addThing(Vector2D pos, int TypeThing)
        {
            if (pos.x < General.Map.Config.LeftBoundary || pos.x > General.Map.Config.RightBoundary ||
             pos.y > General.Map.Config.TopBoundary || pos.y < General.Map.Config.BottomBoundary)
            {
                Console.WriteLine("Error Generaetive Doom: Failed to insert thing: outside of map boundaries.");
                return null;
            }

            // Create thing
            Thing t = General.Map.Map.CreateThing();
            if (t != null)
            {
                General.Settings.ApplyDefaultThingSettings(t);
                
                t.Move(pos);
                t.Type = TypeThing;

                t.UpdateConfiguration();

                // Update things filter so that it includes this thing
                General.Map.ThingsFilter.Update();

                // Snap to map format accuracy
                t.SnapToAccuracy();
            }

            return t;
        }

        private void correctMissingTex()
        {

            String defaulttexture = "-";
            if (General.Map.Data.TextureNames.Count > 1)
                defaulttexture = General.Map.Data.TextureNames[1];

            // Go for all the sidedefs
            foreach (Sidedef sd in General.Map.Map.Sidedefs)
            {
                // Check upper texture. Also make sure not to return a false
                // positive if the sector on the other side has the ceiling
                // set to be sky
                if (sd.HighRequired() && sd.HighTexture[0] == '-')
                {
                    if (sd.Other != null && sd.Other.Sector.CeilTexture != General.Map.Config.SkyFlatName)
                    {
                        sd.SetTextureHigh("BIGBRIK2");
                    }
                }

                // Check middle texture
                if (sd.MiddleRequired() && sd.MiddleTexture[0] == '-')
                {
                    sd.SetTextureMid("BIGBRIK2");
                }

                // Check lower texture. Also make sure not to return a false
                // positive if the sector on the other side has the floor
                // set to be sky
                if (sd.LowRequired() && sd.LowTexture[0] == '-')
                {
                    if (sd.Other != null && sd.Other.Sector.FloorTexture != General.Map.Config.SkyFlatName)
                    {
                        sd.SetTextureLow("BIGBRIK2");
                    }
                }

            }
        }

      
    
        private void LaunchGeneration(int NewRoomSize, int NewSectorSize)
        {
            RoomSize = NewRoomSize;
            SectorSize = NewSectorSize;
             Lvl = new LevelLogic(SectorSize, 1);
            pSector = new List<DrawnVertex>();
            Console.WriteLine("stop");
            pSector.Clear();

            DrawnVertex aa = new DrawnVertex();


            // On commence par dessiner les limites de la map.
            aa.pos.x = -5;
            aa.pos.y = 5;
            aa.stitch = true;
            aa.stitchline = true;
            pSector.Add(aa);
            aa.pos.x = (SectorSize * RoomSize) + 5;
            aa.pos.y = 5;
            aa.stitch = true;
            aa.stitchline = true;
            pSector.Add(aa);
            aa.pos.x = (SectorSize * RoomSize) + 5;
            aa.pos.y = -(SectorSize * RoomSize) - 5;
            aa.stitch = true;
            aa.stitchline = true;
            pSector.Add(aa);
            aa.pos.x = -5;
            aa.pos.y = -(SectorSize * RoomSize) - 5;
            aa.stitch = true;
            aa.stitchline = true;
            pSector.Add(aa);
            aa.pos.x = (-5);
            aa.pos.y = (5);
            aa.stitch = true;
            aa.stitchline = true;
            pSector.Add(aa);

            Tools.DrawLines(pSector);


            // Snap to map format accuracy
            // General.Map.Map.SnapAllToAccuracy();
            General.Map.Map.ClearMarkedSectors(false);
            // Clear selection
            General.Map.Map.ClearAllSelected();

            // Update cached values
            General.Map.Map.Update();




            for (int iY = 0; iY < SectorSize; iY++)
            {

                for (int iX = 0; iX < SectorSize; iX++)
                {
                    GenerateRoom(iX, iY);
                    PopulateWithThings(iX, iY);
                }

            }

              General.Interface.RedrawDisplay();
    
            // Update cached values
            General.Map.Map.Update();
            correctMissingTex();
        }


        private void GenerateRoom(int iX, int iY)
        {


            pSector.Clear();
            DrawnVertex a = new DrawnVertex();


            DungeonRoom CurrentRoom = Lvl.Level[iY, iX];








            if (CurrentRoom.EastWall == true && iX != SectorSize - 1)
            {

                pSector.Clear();
                a.pos.x = (iX * RoomSize) + RoomSize + 0.5f;
                a.pos.y = -(iY * RoomSize) - 0.5f;
                a.stitch = true;
                a.stitchline = true;

                pSector.Add(a);
                a.pos.x = (iX * RoomSize) + RoomSize + 0.8f;
                a.pos.y = -(iY * RoomSize) - 0.5f;
                a.stitch = true;
                a.stitchline = true;
                pSector.Add(a);
                a.pos.x = (iX * RoomSize) + RoomSize + 0.8f;
                a.pos.y = -(iY * RoomSize) - RoomSize + 0.5f;
                a.stitch = true;
                a.stitchline = true;

                pSector.Add(a);

                a.pos.x = (iX * RoomSize) + RoomSize + 0.5f;
                a.pos.y = -(iY * RoomSize) - RoomSize + 0.5f;
                a.stitch = true;
                a.stitchline = true;

                pSector.Add(a);

                a.pos.x = (iX * RoomSize) + RoomSize + 0.5f;
                a.pos.y = -(iY * RoomSize) - 0.5f;
                a.stitch = true;
                a.stitchline = true;


                pSector.Add(a);

                Tools.DrawLines(pSector);


                List<Sector> newsectors = General.Map.Map.GetMarkedSectors(true);
                List<Sidedef> newLines = General.Map.Map.GetMarkedSidedefs(true);



                foreach (Sector s in newsectors)
                {

                    s.CeilHeight = 128;
                    s.FloorHeight = 128;
                    s.Brightness = 0;
                    s.UpdateCeilingSurface();
                    s.UpdateFloorSurface();
                    foreach (Sidedef line in s.Sidedefs)
                    {

                        line.SetTextureMid("BIGBRIK2");
                        line.SetTextureLow("STARBR2");
                        line.SetTextureHigh("STARBR2");
                    }


                    foreach (Linedef line in General.Map.Map.GetMarkedLinedefs(true))
                    {
                        line.SetFlag(General.Map.Config.SoundLinedefFlag, true);


                        line.Marked = false;


                    }

                    foreach (Sidedef line in newLines)
                    {

                        line.SetTextureMid("BIGBRIK2");
                        line.SetTextureLow("STARBR2");
                        line.SetTextureHigh("STARBR2");




                    }

                }
                // Update the used textures
                General.Map.Data.UpdateUsedTextures();

                // Map is changed
                General.Map.IsChanged = true;

                General.Interface.RedrawDisplay();



                // Update cached values
                General.Map.Map.Update();



            }


            if (CurrentRoom.NorthWall == true && iY != 0)
            {
                pSector.Clear();

                a.pos.x = (iX * RoomSize) + 1.2f;
                a.pos.y = -(iY * RoomSize) - 0.5f;
                a.stitch = true;
                a.stitchline = true;
                pSector.Add(a);
                a.pos.x = (iX * RoomSize) + RoomSize + 0.2f;
                a.pos.y = -(iY * RoomSize) - 0.5f;
                a.stitch = true;
                a.stitchline = true;

                pSector.Add(a);

                a.pos.x = (iX * RoomSize) + RoomSize + 0.2f;
                a.pos.y = -(iY * RoomSize) - 1f;
                a.stitch = true;
                a.stitchline = true;

                pSector.Add(a);

                a.pos.x = (iX * RoomSize) + 1.2f;
                a.pos.y = -(iY * RoomSize) - 1f;
                a.stitch = true;
                a.stitchline = true;

                pSector.Add(a);

                a.pos.x = (iX * RoomSize) + 1.2f;
                a.pos.y = -(iY * RoomSize) - 0.5f;
                a.stitch = true;
                a.stitchline = true;
                pSector.Add(a);

                Tools.DrawLines(pSector);



                List<Sector> newsectors = General.Map.Map.GetMarkedSectors(true);
                List<Sidedef> newLines = General.Map.Map.GetMarkedSidedefs(true);



                foreach (Sector s in newsectors)
                {


                    s.CeilHeight = 128;
                    s.FloorHeight = 128;
                    s.Brightness = 0;

                    s.UpdateCeilingSurface();
                    s.UpdateFloorSurface();
                    s.Marked = false;



                    foreach (Sidedef line in s.Sidedefs)
                    {
                        line.SetTextureMid("BIGBRIK2");
                    }
                }

                foreach (Linedef line in General.Map.Map.GetMarkedLinedefs(true))
                {
                    line.SetFlag(General.Map.Config.SoundLinedefFlag, true);

                    line.Marked = false;


                }


                foreach (Sidedef line in newLines)
                {

                    line.SetTextureMid("BIGBRIK2");
                    line.SetTextureLow("STARBR2");
                    line.SetTextureHigh("STARBR2");
                    line.Marked = false;
                }


                // Update the used textures
                General.Map.Data.UpdateUsedTextures();

                // Map is changed
                General.Map.IsChanged = true;

                General.Interface.RedrawDisplay();



                // Update cached values
                General.Map.Map.Update();




            }


            // Update the used textures
            General.Map.Data.UpdateUsedTextures();

            // Map is changed
            General.Map.IsChanged = true;

            //     General.Interface.RedrawDisplay();
            //Ajoute, on enleve la marque sur les nouveaux secteurs
            General.Map.Map.ClearMarkedSectors(false);
            General.Map.Map.ClearMarkedLinedefs(false);
            General.Map.Map.ClearMarkedSidedefs(false);
            // Clear selection
            General.Map.Map.ClearAllSelected();

            // Update cached values
            //  General.Map.Map.Update();

        }




        private void PopulateWithThings(int iX, int iY)
        {
            Thing FreshSpawn;
            //add player spawn top left.
            if (iX == 0 && iY == 0)
            {
                FreshSpawn = addThing(new Vector2D((iX) + RoomSize / 2, -(iY) - RoomSize / 2), 1);
                FreshSpawn = addThing(new Vector2D((iX) + RoomSize / 3, -(iY) - RoomSize / 3), 1);

            }
            else if ( iX == SectorSize - 1 && iY == SectorSize - 1)
            {
                FreshSpawn = addThing(new Vector2D((iX * RoomSize) + RoomSize / 2, -(iY * RoomSize) - RoomSize / 2), 67);
                //on le rends sourd 
                FreshSpawn.SetFlag("8", true);
                FreshSpawn.UpdateConfiguration();

            }
            else
            {
            
                // les dead ends contiennent des armes
                // chaque arme ne spawn qu'une fois pour forcer le joueur à chercher.
                // si l'arme a déja spawn, on spawn un ennemi.
                if (Lvl.Level[iY, iX].isdeadend)
                {
                    int Distance = iY + iX;
            


                    if (Distance < SectorSize*2 / 4)
                    {
                        if (!tronconneuse)
                        {
                            FreshSpawn = addThing(new Vector2D((iX * RoomSize) + RoomSize / 2, -(iY * RoomSize) - RoomSize / 2), 2005);
                            tronconneuse = true;
                        }
                        else
                        {
                            FreshSpawn = addThing(new Vector2D((iX * RoomSize) + RoomSize / 2, -(iY * RoomSize) - RoomSize / 2), 3004);
                            //on le rends sourd 
                            FreshSpawn.SetFlag("8", true);
                            FreshSpawn.UpdateConfiguration();

                        }




                    }
                  
                    else if (Distance < SectorSize*2 / 3)
                    {
                        if(!Shotgun)
                        {
                            FreshSpawn = addThing(new Vector2D((iX * RoomSize) + RoomSize / 2, -(iY * RoomSize) - RoomSize / 2), 82);
                            Shotgun = true;

                        }
                        else
                        {
                            FreshSpawn = addThing(new Vector2D((iX * RoomSize) + RoomSize / 2, -(iY * RoomSize) - RoomSize / 2), 9);
                            //on le rends sourd 
                            FreshSpawn.SetFlag("8", true);
                            FreshSpawn.UpdateConfiguration();

                        }
                       
                        FreshSpawn = addThing(new Vector2D((iX * RoomSize + 1) + RoomSize / 2, -(iY * RoomSize - 1) - RoomSize / 2), 2008);

                    }
                    else if (Distance < SectorSize * 2 / 2)
                    {
                        if(!Minigun)
                        {
                            FreshSpawn = addThing(new Vector2D((iX * RoomSize) + RoomSize / 2, -(iY * RoomSize) - RoomSize / 2), 2002);
                            Minigun = true;

                        }
                        else
                        {
                            FreshSpawn = addThing(new Vector2D((iX * RoomSize) + RoomSize / 2, -(iY * RoomSize) - RoomSize / 2), 3001);
                            //on le rends sourd 
                            FreshSpawn.SetFlag("8", true);
                            FreshSpawn.UpdateConfiguration();

                        }
            
                        FreshSpawn = addThing(new Vector2D((iX * RoomSize + 1) + RoomSize / 2, -(iY * RoomSize - 1) - RoomSize / 2), 2048);
                        FreshSpawn = addThing(new Vector2D((iX * RoomSize + 1) + RoomSize / 2, -(iY * RoomSize - 1) - RoomSize / 2), 2012);

                    }
                    else
                    {
                        if(!laser)
                        {
                            FreshSpawn = addThing(new Vector2D((iX * RoomSize) + RoomSize / 2, -(iY * RoomSize) - RoomSize / 2), 2004);
                            laser = true;

                        }
                        else
                        {
                            FreshSpawn = addThing(new Vector2D((iX * RoomSize) + RoomSize / 2, -(iY * RoomSize) - RoomSize / 2), 66);
                            Dictionary<String, bool> az = FreshSpawn.GetFlags();
                    //on le rends sourd 
                            FreshSpawn.SetFlag("8", true);
                            FreshSpawn.UpdateConfiguration();

                        }
             
                        FreshSpawn = addThing(new Vector2D((iX * RoomSize + 1) + RoomSize / 2, -(iY * RoomSize - 1) - RoomSize / 2), 2047);
                        FreshSpawn = addThing(new Vector2D((iX * RoomSize + 1) + RoomSize / 2, -(iY * RoomSize - 1) - RoomSize / 2), 2012);

                    }
                }

            else
                {
                    // sert a savoir quelle déco on va poser
                    Random r = new Random();
                    int decoration = r.Next(0, 7);

                    // décentre la déco 
                    Random deviation = new Random();
                    int pos = deviation.Next(0, (RoomSize / 2) - (RoomSize / 4));

                    switch(decoration)
                    {
                        case 0:
                            FreshSpawn = addThing(new Vector2D((iX * RoomSize ) + (RoomSize / 2) + pos, -(iY * RoomSize)  - (RoomSize / 2)  - pos ), 28);

                            FreshSpawn = addThing(new Vector2D((iX * RoomSize + (RoomSize / 4) ) + RoomSize / 2, -(iY * RoomSize - (RoomSize / 4)) - RoomSize / 2), 3004);
                            FreshSpawn.SetFlag("8", true);
                            FreshSpawn.UpdateConfiguration();


                            break;
                        case 1:
                            FreshSpawn = addThing(new Vector2D((iX * RoomSize) + (RoomSize / 2) + pos, -(iY * RoomSize) - (RoomSize / 2) - pos), 12);
                            break;
                        case 2:
                            FreshSpawn = addThing(new Vector2D((iX * RoomSize) + (RoomSize / 2) + pos, -(iY * RoomSize) - (RoomSize / 2) - pos), 41);
                            FreshSpawn = addThing(new Vector2D((iX * RoomSize + (RoomSize / 4)) + RoomSize / 2, -(iY * RoomSize - (RoomSize / 4)) - RoomSize / 2), 2014);
                            break;
                        case 3:
                            FreshSpawn = addThing(new Vector2D((iX * RoomSize) + pos, -(iY * RoomSize) - (RoomSize / 2) - pos), 78);
                            FreshSpawn = addThing(new Vector2D((iX * RoomSize + (RoomSize / 4)) + RoomSize / 2, -(iY * RoomSize - (RoomSize / 4)) - RoomSize / 2), 3004);
                            FreshSpawn.SetFlag("8", true);
                            FreshSpawn.UpdateConfiguration();
                            break;
                        case 4:
                            FreshSpawn = addThing(new Vector2D((iX * RoomSize) + (RoomSize / 2) + pos, -(iY * RoomSize) - (RoomSize / 2) - pos), 53);
                            break;
                        case 5:
                            FreshSpawn = addThing(new Vector2D((iX * RoomSize) + (RoomSize / 2) + pos, -(iY * RoomSize) - (RoomSize / 2) - pos), 26);
                            break;
                        case 6:
                            FreshSpawn = addThing(new Vector2D((iX * RoomSize) + (RoomSize / 2) + pos, -(iY * RoomSize) - (RoomSize / 2) - pos), 48);

                            break;






                    }




                }
            }
     
        }



        public void ShowWindow(Form owner)
        {
            // Position this window in the left-top corner of owner
            this.Location = new Point(owner.Location.X + 20, owner.Location.Y + 90);

            // Show it
            base.Show(owner);
        }

        // Form is closing event
        private void GDForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // When the user is closing the window we want to cancel this, because it
            // would also unload (dispose) the form. We only want to hide the window
            // so that it can be re-used next time when this editing mode is activated.
            if (e.CloseReason == CloseReason.UserClosing)
            {
                // Just cancel the editing mode. This will automatically call
                // OnCancel() which will switch to the previous mode and in turn
                // calls OnDisengage() which hides this window.
                General.Editing.CancelMode();
                e.Cancel = true;
            }
        }

        private void GDForm_Load(object sender, EventArgs e)
        {

        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            General.Editing.CancelMode();
        }

        private void btnDoMagic_Click(object sender, EventArgs e)
        {
            LaunchGeneration(250, 10);
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            LaunchGeneration(250, 12);

        }



    }


















}

