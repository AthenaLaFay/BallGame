using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game2.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game2.Content
{
    class GameWorld
    {
        public Matrix World;
        public Vector3 Position;
        public Vector3 boxMax = new Vector3(-65, -40, -600);
        public Vector3 boxMin = new Vector3(65, 40, 0);
        public BoundingBox WorldBox;
        public BoundingBox LeftWall = new BoundingBox(new Vector3(-65, -40, -600), new Vector3(-65, 40, 0));
        public BoundingBox RightWall = new BoundingBox(new Vector3(65, -40, -600), new Vector3(65, 40, 0));
        public BoundingBox TopWall = new BoundingBox(new Vector3(-65, 40, -600), new Vector3(65, 40, 0));
        public BoundingBox BotWall = new BoundingBox(new Vector3(-65, -40, -600), new Vector3(65, -40, 0));
        public BoundingBox FrontWall = new BoundingBox(new Vector3(-130, - 80, 20), new Vector3(130,80 , 20));
        public BoundingBox BackWall = new BoundingBox(new Vector3(-130, -80, -620), new Vector3(130, 80, -620));
        public Model WorldModel;

        public GameWorld(Model worldModel, Vector3 position)
        {
            WorldModel = worldModel;
            Position = position;
            WorldBox = new BoundingBox(boxMin, boxMax);

        }
        public void Update()
        {

        }


        public void Draw(Matrix view, Matrix projection)
        {
            //Draws the game world
            World = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);
            World = World * Matrix.CreateRotationY(
                    MathHelper.ToRadians(180f));

            foreach (ModelMesh mesh in WorldModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {

                    effect.EnableDefaultLighting();
                    effect.World = World;
                    effect.View = view;
                    effect.Projection = projection;

                }

                mesh.Draw();
            }

        }


    }
}
