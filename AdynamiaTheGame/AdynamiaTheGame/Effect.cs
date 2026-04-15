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
    public enum Type
    {
        healthBoon,
        speedBoon,
        jumpBoon,
        respawnBoon, //if you die, you will respawn with half health
        poisonCurse, //makes you less durable ?
        darknessCurse,
        brobrobrosahur
    }

    class Effect
    {

        public Texture2D texture;
        public Rectangle location;
        readonly Type type;

        public Effect(Texture2D texture, Rectangle location, Type type)
        {
            this.texture = texture;
            this.location = location;
            this.type = type;
        }

        public void applyEffect(Player player)
        {
            switch ((int)type)
            {
                case 0:
                    player.ApplyHealth(health: 2);
                    break;
                case 1:
                    player.ApplySpeed(speed: 5);
                    break;
                case 2:
                    player.ApplyJump(jump: 2);
                    break;
                case 3:
                    player.ApplyRespawn();
                    break;
                case 4:
                    player.ApplyPoison(poison: 2);
                    break;
                case 5:
                    player.ApplyDarkness();
                    break;
            }
        }
        public void Draw(GameTime gameTime, GraphicsDevice graphicsDevice)
        {
            SpriteBatch spriteBatch;
            spriteBatch = new SpriteBatch(graphicsDevice);
            spriteBatch.Begin();
            spriteBatch.Draw(texture, location, Color.White);
            spriteBatch.End();
        }
    }
}
