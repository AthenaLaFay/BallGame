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
    class CompPaddle
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



        public CompPaddle(Model compPadMod, Vector3 compPadPos)
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

        public void Update()
        {
            //Changes the position of the computer's paddle in order to move it
            CompPadBox.Min = MinTemp + CompPadPos;
            CompPadBox.Max = MaxTemp + CompPadPos;
            //Console.WriteLine("Min" + CompPadBox.Min);
            //Console.WriteLine("Max" + CompPadBox.Max);


        }
        /*Walls 1 and 2 are used to predict where the ball will be when it reaches the computers paddle
          The walls are either the top and bottom walls or the leeft and right.
          It works by using the distance between the ball and the paddle 
          Distance is 570 because the ball is -5 when it hits the user's 
          paddle and the computer's paddle is at 595 and the diameter of 
          the ball is 20 which also needs to be taken into account and deducted.
          It works by pretending to bounce the ball off of the each wall and 
          subtracting the distance from the distance before it cannot anymore 
          and returning where it ends up when bounced on the last wall.
          This is a lot easier to explain in person with diagrams. Feel free to call me.*/
        private float Wall1(float startpos, float dist, float wall1, float wall2)
        {
            //Calculates the distance between where the ball would be starting 
            //and the first wall it would meet and subtracts it from the distance left from 
            //the original distance
            //If its less than zero then it doesn't meet a wall and the remaining 
            //distance is subtracted  from or added from the starting position
            //If not it means that it bounces on a wall
            //Wal11 will call wall2 and vice versa using the new distance
            //calculated until the new distance is less than zero
            float newdist = dist - Math.Abs(wall1 - startpos);
            if(newdist <= 0)
            {
                //Console.WriteLine("Dist: " + (startpos - dist));
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
            //Will call Wall 1 or 2 depending on the direcion the ball is going 
            float distance = Accuracy(100, 2, 20) * 570;
            //Console.WriteLine("Distance: " + distance);
            float distX = Math.Abs(Ball.Velocity.X * distance/ Ball.Velocity.Z);
            if(Ball.Velocity.X >= 0)
            {
                //there is a difference of 10 between the wall values put in 
                //and the actual wall values to take into account the ball's radius of 20
                PredictedPos.X = Wall2(Ball.ballPos.X, distX, -55f, 55f);
            }
            else
            {
                //CompPadPos.X = Wall1(Ball.ballPos.X, distX, -55f, 55f);
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
        //The accuracy of the computers perdiction.
        //if probability is 80 and accuracy in 2 and worse case is 20,
        //There will be a 80 pecent chance that the ball will hit between 2 percent of the correct distance
        //Worst case scenario the it will hit 20 percent of the correct distance
        //returns a value between 0 and 2 the closer the value is to 1 the more accurate it will be
        //This would be used to change the dificulty for different levels
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


        //Draws the computer paddle
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
