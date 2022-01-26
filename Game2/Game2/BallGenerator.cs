using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game2.Content
{
    public class BallManager
    {
        private Random random;
        public int total;
        public Vector3 EmitterLocation { get; set; }
        public List<Ball> balls;
        private List<Model> ballModels;


        public BallManager(List<Model> ballModels, Vector3 location)
        {
            EmitterLocation = location;
            this.ballModels = ballModels;
            this.balls = new List<Ball>();
            random = new Random();
        }

        public void Update(GameTime gameTime)
        {

            for (int i = 0; i < total; i++)
            {
                if (balls.Count < total)
                {
                    balls.Add(GenerateNewBall());
                    balls[i].name = "Ball" + i.ToString();
                }
            }

            for (int ball = 0; ball < balls.Count; ball++)
            {
                //Console.WriteLine(balls.Count);
                balls[ball].Update(gameTime);
               
                if (balls[ball].TTL <= 0)
                {
                    balls.RemoveAt(ball);
                    ball--;
                }
            }
        }

        private Ball GenerateNewBall()
        {
            Model ballModel = ballModels[random.Next(ballModels.Count)];
            Vector3 position = EmitterLocation;
            float z = 100;
            float x = 65;
            float y = 40;
            float speed = (float)(random.NextDouble() * 2 + 2);
            Vector3 velocity = new Vector3(
                                    speed * (float)(random.NextDouble() * (x - (-x)) - x)/z,//0.1f * (float)(random.NextDouble() * 10 ),
                                    speed * (float)(random.NextDouble() * (y-(-y)) - y) /z,//0.1f * (float)(random.NextDouble() * 10 ),
                                    speed * z/z);//0.1f * (float)(random.NextDouble() * 10));
            //velocity = new Vector3(1, 1, 2);
            float angle = 0;
            float angularVelocity = 0.5f * (float)(random.NextDouble() * 2 - 1);
            Color color = new Color(
                        (float)random.NextDouble(),
                        (float)random.NextDouble(),
                        (float)random.NextDouble());
            float size = (float)random.NextDouble();
            int ttl = 20 + random.Next(40);

            return new Ball(ballModel, position, velocity, angle, angularVelocity, color, size, ttl);
        }

        public void Draw(Matrix view, Matrix projection, Matrix world)
        {
            
            for (int index = 0; index < balls.Count; index++)
            {
                balls[index].Draw(view, projection, world );
            }
            
        }
    }
}
