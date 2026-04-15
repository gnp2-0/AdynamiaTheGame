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

namespace AdynamiaTheGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Player player;
        Texture2D playerTexture;
        Texture2D swordTexture;
        SpriteFont debugFont; // Ensure you have a font in your Content project

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            player = new Player(new Vector2(300, 300));
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

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

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}