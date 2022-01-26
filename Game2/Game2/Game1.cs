/*The code is not complete. I attempted to create a menu using game states 
 * and the buttons all the buttons work and rand the game. Unfortunatley 
 * this broke the actual games and I never figured out why. All of the objects 
 * were acting strangely like they were all on completely different axis
 *There shouldn't have been any difference as it was essentially the same code.
 *when the ball got towards the end of the game world it stopped bouncing on things.
 *I counldn't find any other ways to create new games and switch states
 *I also couldn't figure out how to reset the game so that I could create my scoring system.
 */
using System;
using System.Collections.Generic;
using Game2.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game2
{
    public class Game1 : Game
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
        CompPaddle compPad;

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
        DirectionalLight light;
        BasicEffect basicEffect;
        Vector3 viewVector;
        bool passed = false;
        bool once = false;
        //Geometric Info

        //Geometric Info



        //Orbit
        bool orbit;

        public Game1()
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
            //view = Matrix.CreateLookAt(camPosition * new Vector3((float)Math.Sin(angle, camTarget, Vector3.Up);

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
            compPad = new CompPaddle(compPaddle, compPadPos);
            texture = Content.Load<Texture2D>("UserPadTex2");


            effect = Content.Load<Effect>("Effects/Ambient");


        }


        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {

            generator.EmitterLocation = ballPos;
            //Creates throws 1 ball
            generator.total = 1;
            generator.Update(gameTime);
            

            for (int ball = 0; ball < generator.balls.Count; ball++)
            {
                
                //Changes the velocity of the ball/balls if they collide with the walls or paddles
                //so that they bounce in the opposite direction
                if(generator.balls[ball].IsCollision(gameWorld.LeftWall) || generator.balls[ball].IsCollision(gameWorld.RightWall))
                {
                    generator.balls[ball].Velocity *= new Vector3(-1f, 1f, 1f);
                    //Console.WriteLine("HELLO!");
                    passed = true;
                }

                if (generator.balls[ball].IsCollision(gameWorld.TopWall)||generator.balls[ball].IsCollision(gameWorld.BotWall))
                {
                    generator.balls[ball].Velocity *= new Vector3(1f, -1f, 1f);
                    //Console.WriteLine("GOODBYE!");
                    passed = true;
                }

                if(generator.balls[ball].IsCollision(userPad.UserPadBox))
                {
                    generator.balls[ball].Velocity *= new Vector3(1f, 1f, -1f);
                    passed = true;
                    //Predicts where the the ball will be when it gets to the computer's paddle
                    compPad.Predictor(generator.balls[ball], generator.balls[ball].Velocity);
                }
                    //The computer paddle navigates to predicted destination
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

                //This is needed so that the ball won't get stuck the first time it's
                //shot from the paddle
                if (passed == true)
                {
                    if (generator.balls[ball].IsCollision(compPad.CompPadBox))
                    {
                        generator.balls[ball].Velocity *= new Vector3(1f, 1f, -1f);

                    }

                }
                //Mouse.WindowHandle = Window.Handle;
                //calculate how far the mouse has moved from its original point
                float unitX = Mouse.GetState().X - userPad.UserPadPos.X;
                float unitY = -Mouse.GetState().Y - userPad.UserPadPos.Y;
                //Moves the paddle in the whichever direction depending on how far the
                //Mouse moved but prevents the mouse and padle from going outside the bounds
                //of the game world
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
                        //userPad.UserPadPos.Y = -25;
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
                        //userPad.UserPadPos.Y = 25;
                    }
                    else if (Mouse.GetState().Y <= -25)
                    {
                        Mouse.SetPosition(50, Mouse.GetState().Y);
                        Mouse.SetPosition(Mouse.GetState().X, -25);
                        //userPad.UserPadPos.Y = -25;
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
                        //userPad.UserPadPos.Y = 25;
                    }
                    else if (Mouse.GetState().Y <= -25)
                    {
                        Mouse.SetPosition(Mouse.GetState().X, -25);
                        Mouse.SetPosition(-50, Mouse.GetState().Y);
                        //userPad.UserPadPos.Y = -25;
                    }
                }

                
            }
            //Enables the user to move the paddle with the keyboard
            //Only works when the mouse option is disabled
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {

                if (userPad.UserPadPos.X >= -50)
                {
                    userPad.UserPadPos.X -= 2f;
                }


            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                //camPosition.X += 1f;
                //camTarget.X += 1f;
                if (userPad.UserPadPos.X <= 50)
                {
                    userPad.UserPadPos.X += 2f;
                }

            
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
            //Exits game if delete key is pushed
            if (Keyboard.GetState().IsKeyDown(Keys.Delete))
            {
                Exit();

            }



            view = Matrix.CreateLookAt(camPosition, camTarget, Vector3.Up);
            userPad.Update();
            compPad.Update();

            base.Update(gameTime);

        }

        protected override void Draw(GameTime gameTime)
        {
            //Implements the draw functions of all of the models


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
