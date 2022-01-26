using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game2.Content.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Game2.Content;

namespace Game2.Content.States
{
    public class MenuState : Game
    {
        private List<Component> components;
        public GraphicsDeviceManager graphics;
        private Game PolyGame;
        private Game currentGame;
        private Game nextGame;
        private int game;
        bool poly = false;
        public void ChangeGame(Game game)
        {
            nextGame = game;
        }
        public MenuState()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            
        }

        private void NewGameButtonClick(object sender, EventArgs e)
        {
            ChangeGame(new Game1());
        }
        private void NewSmasherButtonClick(object sender, EventArgs e)
        {
            ChangeGame(new Polyball());
            poly = true;
        }
        private void NewPolyballButtonClick(object sender, EventArgs e)
        {
            game = 1;
            PolyGame = new Polyball();
            ChangeGame(new Polyball());
        }

        private void QuitGameButtonClick(object sender, EventArgs e)
        {
            Exit();
        }

        private void LoadGameButtonClick(object sender, EventArgs e)
        {
            Console.WriteLine("Load Game");
        }

        protected override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = new SpriteBatch(GraphicsDevice);
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            foreach (var Component in components)
                Component.Draw(gameTime, spriteBatch);

            spriteBatch.End();
        }



        protected override void Update(GameTime gameTime)
        {
            if (nextGame != null)
            {
                currentGame = nextGame;
                nextGame = null;
                System.Diagnostics.Process.Start("C: \\Users\\hogan\\OneDrive\\Documents\\Games Programming\\GP - Project - Copy\\Game2\\Game2\\Polyball.cs");
                
            }
            foreach (var Component in components)
                Component.Update(gameTime);


        }

        protected override void Initialize()
        {
            if (currentGame == this)
            {
                IsMouseVisible = true;
                var MonoballTexture = Content.Load<Texture2D>("Controls/MonoballButton");
                var PolyballTexture = Content.Load<Texture2D>("Controls/PolyballButton");
                var TournamentTexture = Content.Load<Texture2D>("Controls/TournamentButton");
                var SmasherTexture = Content.Load<Texture2D>("Controls/SmasherButton");
                var buttonFont = Content.Load<SpriteFont>("Fonts/Font");
                var newGameButton = new Button(MonoballTexture, buttonFont)
                {
                    Position = new Vector2(300, 20),
                    Text = "",
                };
                newGameButton.Click += NewGameButtonClick;
                var newPolyballButton = new Button(PolyballTexture, buttonFont)
                {
                    Position = new Vector2(300, 120),
                    Text = "",
                };
                newPolyballButton.Click += NewSmasherButtonClick;
                var newSmasherButton = new Button(SmasherTexture, buttonFont)
                {
                    Position = new Vector2(300, 220),
                    Text = "",
                };
                newSmasherButton.Click += NewSmasherButtonClick;
                var loadGameButton = new Button(MonoballTexture, buttonFont)
                {
                    Position = new Vector2(300, 420),
                    Text = "",
                };
                loadGameButton.Click += LoadGameButtonClick;
                var quitGameButton = new Button(MonoballTexture, buttonFont)
                {
                    Position = new Vector2(300, 320),
                    Text = "",
                };
                quitGameButton.Click += QuitGameButtonClick;
                components = new List<Component>()
                {
                    newGameButton,
                    newPolyballButton,
                    newSmasherButton,
                    quitGameButton,
                };//Initialise
            }
            
            
        }

        protected override void LoadContent()
        {
            //Load Content
        }
    }
}
