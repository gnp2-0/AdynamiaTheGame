using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace AdynamiaTheGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D[] effectIcons = new Texture2D[6];
        string[] effectNames = new string[6];
        List<Effect> effects = new List<Effect>(); 
        int width, height;
        KeyboardState kb, oldKb;

        Player player;
        Texture2D playerTexture;
        Texture2D swordTexture;
        SpriteFont debugFont; // Ensure you have a font in your Content project

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            width = graphics.PreferredBackBufferWidth;
            height = graphics.PreferredBackBufferHeight;
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            kb = Keyboard.GetState();
            oldKb = Keyboard.GetState();
            effectNames[0] = "healthBoon";
            effectNames[1] = "speedBoon";
            effectNames[2] = "jumpBoon";
            effectNames[3] = "respawnBoon";
            effectNames[4] = "poisonCurse";
            effectNames[5] = "darknessCurse";
            
            player = new Player(new Vector2(300, 300));
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            for (int i = 0; i < effectNames.Length; i++)
            {
                effectIcons[i] = Content.Load<Texture2D>(effectNames[i]);
                effects.Add(new Effect(effectIcons[i], new Rectangle(width - ((i + 1) * 50), 0, 50, 50), (Type)i));
            }
        }
            playerTexture = new Texture2D(GraphicsDevice, 1, 1);
            playerTexture.SetData(new[] { Color.Red });

            swordTexture = new Texture2D(GraphicsDevice, 1, 1);
            swordTexture.SetData(new[] { Color.LightGray });

            // Note: To use debugFont, you must add a SpriteFont to your Content Pipeline
            // Otherwise, comment out the DrawString lines below.
            // debugFont = Content.Load<SpriteFont>("Arial"); 
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            kb = Keyboard.GetState();
            oldKb = kb;
            foreach (Effect effect in effects)
            {
                --effect.duration;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();
            player.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            // Draw Player
            spriteBatch.Draw(playerTexture, new Rectangle((int)player.Position.X, (int)player.Position.Y, 40, 40), Color.White);

            // Draw Sword Attack
            if (player.IsAttacking)
            {
                Color attackColor = (player.CurrentAnimation == "slash up") ? Color.Yellow : Color.White;
                spriteBatch.Draw(swordTexture, player.SwordHitbox, attackColor);
            }

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            for (int i = 0; i < effects.Count; i++)
            {
                if (kb.IsKeyDown(Keys.Tab))
                {
                    if (effects[i].duration > 0)
                        spriteBatch.Draw(effects[i].texture, effects[i].location, Color.White);
                }
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}