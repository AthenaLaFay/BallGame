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
    class PolyballPaddle
    {
        public Matrix World;
        public Vector3 CompPadPos;
        public Model CompPadMod;
        Vector3 BoxMin;
        Vector3 BoxMax;
        Vector3 MinTemp;
        Vector3 MaxTemp;
        int UserHits = 0;
        int CompHits = 0;
        public Vector3 PredictedPos;
        Random random = new Random();
        public BoundingBox CompPadBox;



        public PolyballPaddle(Model compPadMod, Vector3 compPadPos)
        {
            CompPadMod = compPadMod;
            CompPadPos = compPadPos;
            PredictedPos = compPadPos;
            CompPadBox = new BoundingBox(BoxMin, BoxMax);
            BoxMin = new Vector3(-15, -15, 0);
            BoxMax = new Vector3(15, 15, 0);
            MinTemp = new Vector3(-15, -15, 0);
            MaxTemp = new Vector3(15, 15, 0);

        }
        //Exact same as CompPad except that the distance is not 570 S
        //is calculated from whe ball to the computer's paddle
        public void Update()
        {
            CompPadBox.Min = MinTemp + CompPadPos;
            CompPadBox.Max = MaxTemp + CompPadPos;
        }

        private float Wall1(float startpos, float dist, float wall1, float wall2)
        {
            float newdist = dist - Math.Abs(wall1 - startpos);
            if(newdist <= 0)
            {
                return startpos - dist;
            }
            else
            {
                startpos = wall1;
                return Wall2(startpos, newdist, wall1, wall2);
            }
        }

        private float Wall2(float startpos, float dist, float wall1, float wall2)
        {
            float newdist = dist - Math.Abs(wall2 - startpos);
            if (newdist <= 0)
            {
                return startpos + dist;
            }
            else
            {
                startpos = wall2;
                return Wall1(startpos, newdist, wall1, wall2);
            }
        }

        public void Predictor(Ball Ball, Vector3 Velocity)
        {

            float distance = Accuracy(100, 2, 20) * (Ball.ballPos.Z - CompPadPos.Z - 20);
            float distX = Math.Abs(Ball.Velocity.X * distance/ Ball.Velocity.Z);
            if(Ball.Velocity.X >= 0)
            {

                PredictedPos.X = Wall2(Ball.ballPos.X, distX, -55f, 55f);
            }
            else
            {

                PredictedPos.X = Wall1(Ball.ballPos.X, distX, -55f, 55f);
            }

            float distY = Math.Abs(Ball.Velocity.Y * distance/ Ball.Velocity.Z);
            if (Ball.Velocity.Y >= 0)
            {
                PredictedPos.Y = Wall2(Ball.ballPos.Y, distY, -30f, 30f);
            }
            else
            {
                PredictedPos.Y = Wall1(Ball.ballPos.Y, distY, -30f, 30f);
            }
        }

        public float Accuracy(float prob, float accuracy, float worstCase)
        {
            float maxRand = 100;
            float rand = (float)(random.NextDouble());
            if (maxRand * rand <= maxRand * prob/100)
            {
                float first = (maxRand - accuracy) / 100;
                float second = (maxRand + accuracy)/100;

                float result = (float)(random.NextDouble() * (first - second) + second);
                return result;
            }
            else
            {
                float result = 0;
                float probResult = maxRand * rand;
                float remainder = 1 - (accuracy / 100) - 1 - (worstCase/100);
                float first;
                float second;
                float third;
                float fourth;
                for (int i = 1; i<=4; i++)
                {
                    if (probResult - (accuracy / 100) < i * remainder / 4)
                    {
                        
 
                        first = 1 - i * remainder / 4 + accuracy / 100;
                        second = 1 - (i - 1) * remainder / 4 + accuracy / 100;
                        third = 1 + (i - 1) * remainder / 4 + accuracy / 100;
                        fourth = 1 + i * remainder / 4 + accuracy / 100;
                        float num1 = (float)(random.NextDouble() * (second - first) + first);
                        float num2 = (float)(random.NextDouble() * (fourth - third) + third);
                        result = random.Next(0, 2) == 1 ? num1 : num2;
                    }
                }
                return result;
            }
        }
        

        public void LoadContent()
        {

        }


        public void Draw(Matrix view, Matrix projection, Effect effect, Texture texture)
        {
            World = Matrix.CreateTranslation(CompPadPos);
            foreach (ModelMesh mesh in CompPadMod.Meshes)
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
