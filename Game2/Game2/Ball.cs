using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game2.Content
{
    public class Ball
    {
        public Model BallModel { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Velocity { get; set; }
        public float Angle { get; set; }
        public float AngularVelocity { get; set; }
        public Color Color { get; set; }
        public float Size { get; set; }
        public int TTL { get; set; }
        public Matrix BallWorld { get; set; }
        public Vector3 ballPos = new Vector3(0, 0, -595);
        public BoundingSphere ballSphere;
        public bool drawn = false;
        public float ballTime;
        public string name;

        public Ball(Model ball, Vector3 position, Vector3 velocity,
                    float angle, float angularVelocity, Color color, float size, int ttl)
        {
            BallModel = ball;
            Position = position;
            Velocity = velocity;
            Velocity *= new Vector3(0.5f, 0.5f, 0.5f);
            //Console.WriteLine(Velocity);
            Angle = angle;
            AngularVelocity = angularVelocity;
            Color = color;
            Size = size;
            TTL = ttl;
            ballSphere = new BoundingSphere(ballPos, 10f);


        }

        public void Update(GameTime gameTime)
        {
            //TTL--;
            ballPos += Velocity;
            ballSphere.Center = ballPos;

            Angle += AngularVelocity;
            //Console.WriteLine("Sphere " + ballSphere.Center);
            //Console.WriteLine("Pos " + ballPos);
            //Console.WriteLine("Matrix " + BallWorld);
        }

        public bool IsCollision(BoundingBox boundingBox)
        {
            if (ballSphere.Intersects(boundingBox))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Draw(Matrix view, Matrix projection, Matrix ballWorld)
        {

            BallWorld = Matrix.CreateTranslation(ballPos);

            foreach (ModelMesh mesh in BallModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    
                    effect.EnableDefaultLighting();
                    effect.World = BallWorld;
                    
                    effect.View = view;
                    effect.Projection = projection; 

                }
                
                mesh.Draw();
            }
            //Console.WriteLine("ballPos:" + ballPos);
        }
    }


}
