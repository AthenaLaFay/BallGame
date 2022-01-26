using Game2.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Game2
{
    public class Polyball : Game
    {

        GraphicsDeviceManager graphics;
        Model GameWorldMod;
        Model userPaddle;
        Model compPaddle;
        Model Ball;
        Effect effect;
        Texture2D userPadTex;
        Texture2D texture;
        BallManager generator;
        GameWorld gameWorld;
        UserPaddle userPad;
        PolyballPaddle compPad;
        Vector3 camTarget;
        Vector3 camPosition;
        Vector3 worldPosition = new Vector3(0, 0, 0);
        Vector3 userPadPos = new Vector3(0, 0, -5);
        Vector3 compPadPos = new Vector3(0, 0, -595);
        Vector3 ballPos = new Vector3(0, 0, 0);
        Matrix projection;
        Matrix view;
        Matrix world;
        Matrix userWorld;
        Matrix compWorld;
        Matrix ballWorld;
        BasicEffect basicEffect;
        Vector3 viewVector;
        bool passed = false;
        float gameTimer = 0;
        List<Ball> polyballs = new List<Ball>();
        LinkedList<Vector2> coordinates;
        Ball quickest;
        float t = 0;
        int compScore = 0;
        int userScore = 0;
        private Game currentGame;
        private Game nextGame;
        public void ChangeGame(Game game)
        {
            nextGame = game;
        }

        public Polyball()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
        }

        protected override void Initialize()
        {

            base.Initialize();
            //Setup Camera
            camTarget = new Vector3(0f, 0f, 0f);
            camPosition = new Vector3(0f, 0f, 100f);

            projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(45f),
                GraphicsDevice.DisplayMode.AspectRatio, 1f, 1000f);

            view = Matrix.CreateLookAt(camPosition, camTarget, Vector3.Up);
           

            world = Matrix.CreateWorld(camTarget, Vector3.Forward, Vector3.Up);
            userWorld = Matrix.CreateTranslation(userPadPos);
            compWorld = Matrix.CreateTranslation(compPadPos);
            ballWorld = Matrix.CreateTranslation(ballPos);

            //BasicEffect
            basicEffect = new BasicEffect(GraphicsDevice);
            basicEffect.Alpha = 1.0f;
            basicEffect.VertexColorEnabled = false;
            basicEffect.LightingEnabled = false;
            basicEffect.PreferPerPixelLighting = false;
        }
        
        protected override void LoadContent()
        {
            List<Model> BallModel = new List<Model>();
            BallModel.Add(Content.Load<Model>("Ball1"));
            GameWorldMod = Content.Load<Model>("World2");
            //Texture2D lightMap = Content.Load<Texture2D>("Texture");
            userPaddle = Content.Load<Model>("UserPad");
            compPaddle = Content.Load<Model>("PaddleTC");
            userPadTex = Content.Load<Texture2D>("UserPadTex2");
            Ball = Content.Load<Model>("Ball1");
            generator = new BallManager(BallModel, compPadPos);
            gameWorld = new GameWorld(GameWorldMod, camTarget);
            userPad = new UserPaddle(userPaddle, userPadPos);
            compPad = new PolyballPaddle(compPaddle, compPadPos);
            texture = Content.Load<Texture2D>("UserPadTex2");


            effect = Content.Load<Effect>("Effects/Ambient");
            coordinates = new LinkedList<Vector2>();

        }


        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            /*The gameTimer starts from when the fist update is called.
              When the user hits the balls, a "ballTime" value is calculate to 
              find out which ball will reach the computer's paddle first
              The ball with the ballTime value closest to the game timer 
              is the quickest (closest to reaching the computer's paddle)
              and its computer paddle prediction will be calculated first.
              The rest is like the single ball game
            */
            gameTimer += 1;
            Console.WriteLine("Game Timer: "+ gameTimer);
            generator.EmitterLocation = ballPos;
            generator.total = 2; //Throws 2 balls;
            generator.Update(gameTime);


            quickest = generator.balls[0];
            for (int i = 0; i < generator.balls.Count; i++)
            {
                if (generator.balls[i].IsCollision(userPad.UserPadBox))
                {

                        generator.balls[i].Velocity *= new Vector3(1f, 1f, -1f);
                 
                    
                    generator.balls[i].ballTime = 570f / Math.Abs(generator.balls[i].Velocity.Z) + gameTimer;
                    Console.WriteLine("Ball Time: " + generator.balls[i].ballTime);

                    if (!polyballs.Contains(generator.balls[i]))
                        polyballs.Add(generator.balls[i]);

                }
            }



            for (int j = 0; j < polyballs.Count; j++)
            {
                if (Math.Abs(quickest.ballTime - gameTimer) > Math.Abs(polyballs[j].ballTime - gameTimer))
                {
                    quickest = polyballs[j];
                }

                passed = true;


            }
            compPad.Predictor(quickest, quickest.Velocity);
            for (int ball = 0; ball < generator.balls.Count; ball++)
            {
                if (generator.balls[ball].IsCollision(compPad.CompPadBox) && passed == true)
                {

                    generator.balls[ball].Velocity *= new Vector3(1f, 1f, -1f);

                    
                }

            }

          

            for (int ball = 0; ball < generator.balls.Count; ball++)
            {
                if (generator.balls[ball].IsCollision(gameWorld.LeftWall) || generator.balls[ball].IsCollision(gameWorld.RightWall))
                {

                        generator.balls[ball].Velocity *= new Vector3(-1f, 1f, 1f);
                        
                    //Console.WriteLine("HELLO!");
                    passed = true;
                }

                if (generator.balls[ball].IsCollision(gameWorld.TopWall) || generator.balls[ball].IsCollision(gameWorld.BotWall))
                {
                    generator.balls[ball].Velocity *= new Vector3(1f, -1f, 1f);
                    //Console.WriteLine("GOODBYE!");
                    passed = true;
                }
                if (generator.balls[ball].IsCollision(gameWorld.BackWall))
                {
                    if (generator.balls[ball].Velocity.Z < 0)
                        userScore += 1;
                    
                }
                if (generator.balls[ball].IsCollision(gameWorld.FrontWall))
                {
                    if (generator.balls[ball].Velocity.Z < 0)
                        compScore += 1;

                }
                Console.WriteLine("User: " + userScore + " Comp: " + compScore);


            }

                if (compPad.PredictedPos.X > compPad.CompPadPos.X)
                {
                    compPad.CompPadPos.X += 0.5f;
                }
                else if (compPad.PredictedPos.X < compPad.CompPadPos.X)
                {
                    compPad.CompPadPos.X -= 0.5f;
                }

                if (compPad.PredictedPos.Y > compPad.CompPadPos.Y)
                {
                    compPad.CompPadPos.Y += 0.5f;
                }
                else if (compPad.PredictedPos.Y < compPad.CompPadPos.Y)
                {
                    compPad.CompPadPos.Y -= 0.5f;
                }


            float unitX = Mouse.GetState().X - userPad.UserPadPos.X;
            float unitY = -Mouse.GetState().Y - userPad.UserPadPos.Y;

            if (Mouse.GetState().X >= -50 && Mouse.GetState().X <= 50)
            {
                if (Mouse.GetState().Y >= -25 && Mouse.GetState().Y <= 25)
                {
                    userPad.UserPadPos.Y += unitY;
                    userPad.UserPadPos.X += unitX;
                }
                else if (Mouse.GetState().Y >= 25)
                {
                    userPad.UserPadPos.X += unitX;
                    Mouse.SetPosition(Mouse.GetState().X, 25);
                }
                else if (Mouse.GetState().Y <= -25)
                {
                    userPad.UserPadPos.X += unitX;
                    Mouse.SetPosition(Mouse.GetState().X, -25);

                }
            }
            else if (Mouse.GetState().X >= 50)
            {
                if (Mouse.GetState().Y >= -25 && Mouse.GetState().Y <= 25)
                {
                    Mouse.SetPosition(50, Mouse.GetState().Y);
                    userPad.UserPadPos.Y += unitY;
                }
                else if (Mouse.GetState().Y >= 25)
                {
                    Mouse.SetPosition(50, Mouse.GetState().Y);
                    Mouse.SetPosition(Mouse.GetState().X, 25);

                }
                else if (Mouse.GetState().Y <= -25)
                {
                    Mouse.SetPosition(50, Mouse.GetState().Y);
                    Mouse.SetPosition(Mouse.GetState().X, -25);

                }
            }
            else if (Mouse.GetState().X <= -50)
            {
                Mouse.SetPosition(-50, Mouse.GetState().Y);
                if (Mouse.GetState().Y >= -25 && Mouse.GetState().Y <= 25)
                {
                    userPad.UserPadPos.Y += unitY;
                    Mouse.SetPosition(-50, Mouse.GetState().Y);
                }
                else if (Mouse.GetState().Y >= 25)
                {
                    Mouse.SetPosition(Mouse.GetState().X, 25);
                    Mouse.SetPosition(-50, Mouse.GetState().Y);

                }
                else if (Mouse.GetState().Y <= -25)
                {
                    Mouse.SetPosition(Mouse.GetState().X, -25);
                    Mouse.SetPosition(-50, Mouse.GetState().Y);

                }
            }
            
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {

                if (userPad.UserPadPos.X >= -50)
                {
                    userPad.UserPadPos.X -= 2f;
                }
                
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {

                if (userPad.UserPadPos.X <= 50)
                {
                    userPad.UserPadPos.X += 2f;
                }

            }
            if (Keyboard.GetState().IsKeyDown(Keys.Delete))
            {
                //game.ChangeState(new MenuState(game, graphics, graphicsDevice, content));
                Exit();
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                //camPosition.Y -= 1f;
                //camTarget.Y -= 1f;
                if (userPad.UserPadPos.Y <= 25f)
                {
                    userPad.UserPadPos.Y += 2f;
                }

            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                //camPosition.Y += 1f;
                //camTarget.Y += 1f;
                if (userPad.UserPadPos.Y >= -25f)
                {
                    userPad.UserPadPos.Y -= 2f;
                }

            }
            

            view = Matrix.CreateLookAt(camPosition, camTarget, Vector3.Up);
            compPad.Predictor(quickest, quickest.Velocity);
            userPad.Update();
            compPad.Update();

        }

        protected override void Draw(GameTime gameTime)
        {


            GraphicsDevice.Clear(Color.CornflowerBlue);
            

            userWorld = Matrix.CreateTranslation(userPadPos);
            
            Matrix rotPad = userWorld * Matrix.CreateRotationX(
                    MathHelper.ToRadians(0f));

            generator.Draw(view, projection, ballWorld);
            gameWorld.Draw(view, projection);
            userPad.Draw(view, projection, effect, texture);
            compPad.Draw(view, projection, effect, texture);

            base.Draw(gameTime);
        }


    }


}
