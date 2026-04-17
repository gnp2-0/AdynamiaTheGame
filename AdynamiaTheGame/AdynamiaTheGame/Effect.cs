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
        darknessCurse
    }

    class Effect
    {

        public Texture2D texture;
        public Rectangle location;
        public float duration = 0f;
        readonly Type type;
        private readonly Dictionary<string, float> durationsInSeconds = new Dictionary<string, float>() { { "health", 8.0f }, { "speed", 10.0f }, { "jump", 5.0f }, { "respawn", -1.0f }, { "poison", 15.0f }, { "darkness", 20.0f } };

        public Effect(Texture2D texture, Rectangle location, Type type)
        {
            this.texture = texture;
            this.location = location;
            this.type = type;
            switch (this.type)
            {
                case Type.healthBoon:
                    duration = durationsInSeconds["health"];
                    break;
                case Type.speedBoon:
                    duration = durationsInSeconds["speed"];
                    break;
                case Type.jumpBoon:
                    duration = durationsInSeconds["jump"];
                    break;
                case Type.respawnBoon:
                    duration = durationsInSeconds["respawn"];
                    break;
                case Type.poisonCurse:
                    duration = durationsInSeconds["poison"];
                    break;
                case Type.darknessCurse:
                    duration = durationsInSeconds["darkness"];
                    break;
            }
            duration *= 60;
        }

        public void applyEffect(Player player)
        {
            switch (type)
            {
                case Type.healthBoon:
                    player.ApplyHealth(health: 2);
                    break;
                case Type.speedBoon:
                    player.ApplySpeed(speed: 5);
                    break;
                case Type.jumpBoon:
                    player.ApplyJump(jump: 2);
                    break;
                case Type.respawnBoon:
                    player.ApplyRespawn();
                    break;
                case Type.poisonCurse:
                    player.ApplyPoison(poison: 2);
                    break;
                case Type.darknessCurse:
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
