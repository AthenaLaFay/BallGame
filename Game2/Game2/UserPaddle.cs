using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game2.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game2
{
    class UserPaddle
    {

        public Matrix World;
        public Vector3 UserPadPos;
        public Model UserPadMod;
        Vector3 BoxMin;
        Vector3 BoxMax;
        Vector3 MinTemp;
        Vector3 MaxTemp;
        public BoundingBox UserPadBox;



        public UserPaddle(Model userPadMod, Vector3 userPadPos)
        {
            UserPadMod = userPadMod;
            UserPadPos = userPadPos;
            BoxMin = new Vector3(-15, -15, -1);
            BoxMax = new Vector3(15, 15, 1);
            MinTemp = new Vector3(-15, -15, -1);
            MaxTemp = new Vector3(15, 15, 1);
            UserPadBox = new BoundingBox(BoxMin, BoxMax);

        }
        //Moves the user's paddle box with with actual paddle
        public void Update()
        {
            UserPadBox.Min = MinTemp + UserPadPos;
            UserPadBox.Max = MaxTemp + UserPadPos;
            //Console.WriteLine("Min" + UserPadBox.Min);
            //Console.WriteLine("Max" + UserPadBox.Max);

        }

        public void LoadContent()
        {

        }


        // Draws the user paddle
        public void Draw(Matrix view, Matrix projection, Effect effect, Texture texture)
        {
            World = Matrix.CreateTranslation(UserPadPos);

            foreach (ModelMesh mesh in UserPadMod.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    
                    part.Effect = effect;
                    effect.Parameters["World"].SetValue(World * mesh.ParentBone.Transform);
                    effect.Parameters["View"].SetValue(view);
                    effect.Parameters["Projection"].SetValue(projection);
                    effect.Parameters["ModelTexture"].SetValue(texture);
                    effect.Parameters["Transparency"].SetValue(8f);
  
                }

                mesh.Draw();
            }
        }
    }
}
