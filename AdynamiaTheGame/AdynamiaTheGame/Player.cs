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
    class Player
    {
        // Attributes
        public Vector2 Position;
        public int Direction = 1;
        public bool IsAttacking;
        public string CurrentAnimation = "idle";

        // Sword Properties
        public float SwordRotation;
        public Rectangle SwordHitbox;

        // Duration of the animation/hitbox visibility
        private float attackActiveTimer;
        private float attackVisualDuration = 0.15f;

        // Cooldown Timers
        private float stabCooldownRemaining = 0f;
        private float slashCooldownRemaining = 0f;

        // Cooldown Constants (Requirements: 1s for Stab, 3s for Slash)
        private const float STAB_COOLDOWN_TIME = 1.0f;
        private const float SLASH_COOLDOWN_TIME = 3.0f;

        public Player(Vector2 startPosition)
        {
            Position = startPosition;
        }

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            MouseState mouse = Mouse.GetState();
            KeyboardState key = Keyboard.GetState();

            // 1. Reduce cooldown timers over time
            if (stabCooldownRemaining > 0) stabCooldownRemaining -= elapsed;
            if (slashCooldownRemaining > 0) slashCooldownRemaining -= elapsed;

            // 2. Movement logic
            if (key.IsKeyDown(Keys.A)) { Position.X -= 5; Direction = -1; }
            if (key.IsKeyDown(Keys.D)) { Position.X += 5; Direction = 1; }

            // 3. Input Handling with Cooldown Checks
            if (!IsAttacking)
            {
                // Right Click: Slash (3s cooldown)
                if (mouse.RightButton == ButtonState.Pressed && slashCooldownRemaining <= 0)
                {
                    SlashUp();
                    slashCooldownRemaining = SLASH_COOLDOWN_TIME;
                }
                // Left Click: Stab (1s cooldown)
                else if (mouse.LeftButton == ButtonState.Pressed && stabCooldownRemaining <= 0)
                {
                    Stab();
                    stabCooldownRemaining = STAB_COOLDOWN_TIME;
                }
                else
                {
                    SwordRotation = 0;
                }
            }
            else
            {
                // Manage how long the attack hitbox stays on screen
                attackActiveTimer += elapsed;
                if (attackActiveTimer >= attackVisualDuration)
                {
                    IsAttacking = false;
                    attackActiveTimer = 0;
                    CurrentAnimation = "idle";
                }
            }
        }

        public void SlashUp()
        {
            int width = 60;
            int height = 100;

            IsAttacking = true;
            CurrentAnimation = "slash up";
            SwordRotation = (Direction == 1) ? -1.0f : 1.0f;

            SwordHitbox = new Rectangle(
                (int)Position.X + (Direction == 1 ? 10 : -width),
                (int)Position.Y - 80,
                width, height
            );
            System.Diagnostics.Debug.WriteLine("Slash Performed! 3s Cooldown started.");
        }

        public void Stab()
        {
            int width = 40;
            int height = 20;

            IsAttacking = true;
            CurrentAnimation = "stab";
            SwordRotation = (Direction == 1) ? 0.5f : -0.5f;

            int offset = (Direction == 1) ? 30 : -width - 30;
            SwordHitbox = new Rectangle((int)Position.X + offset, (int)Position.Y + 10, width, height);
            System.Diagnostics.Debug.WriteLine("Stab Performed! 1s Cooldown started.");
        }
    }
}