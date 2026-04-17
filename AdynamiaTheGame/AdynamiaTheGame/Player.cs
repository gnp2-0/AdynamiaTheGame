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
    // For y'all
    // Most of this is lowk self-explanatory code
    class Player
    {
        // Physics
        public Vector2 Position;
        private Vector2 velocity;

        // Properties
        private const int WIDTH = 32;
        private const int HEIGHT = 64;
        private const int CROUCH_HEIGHT = 32;

        public Rectangle Bounds
        {
            get
            {
                int h = IsCrouching ? CROUCH_HEIGHT : HEIGHT;
                return new Rectangle((int)Position.X, (int)Position.Y, WIDTH, h);
            }
        }

        // Player states
        public bool IsAlive { get { return Health > 0f; } }

        // More properties (player combat properties/states)
        public float MaxHealth = 100f;
        public float Health;
        public float DamageMultiplier = 1f;
        public float SpeedMultiplier = 1f;
        public bool IsGrounded = false;
        public bool IsCrouching = false;
        public bool IsDashing = false;

        // More constants (movement)
        private const float MOVE_SPEED = 200f;
        private const float JUMP_FORCE = -500f;
        private const float GRAVITY_FORCE = 900f;
        private const float CROUCH_FACTOR = 0.4f;

        // Timers/stackables
        private int groundY;

        // Allow for external systems (like the Game1 class) to subscribe to player attacks without needing to modify the Player class itself for each new attack type (kinda paraphrasing the actual docs but you get the point)
        public event Action<float, bool> OnQuickAttack;
        public event Action<float> OnDashAttack;
        public event Action<float, float> OnHeavyAttack;
        public event Action<float> OnBoonAttack;

        private KeyboardState oldKB;
        public Player(Vector2 startPosition, int groundY)
        {
            Position = startPosition;
            this.groundY = groundY;
            Health = MaxHealth;
        }

        public void Update(GameTime gameTime, KeyboardState ks, MouseState ms)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            HandleMovement(ks, dt);

            ApplyGravity(dt);

            oldKB = ks;
        }

        private void HandleMovement(KeyboardState kb, float dt)
        {
            bool crouchInput = (kb.IsKeyDown(Keys.S) || kb.IsKeyDown(Keys.Down)) && IsGrounded;
            IsCrouching = crouchInput;

            float speed = MOVE_SPEED * SpeedMultiplier * (IsCrouching ? CROUCH_FACTOR : 1f);
            velocity.X = 0f;

            if (kb.IsKeyDown(Keys.A) || kb.IsKeyDown(Keys.Left)) velocity.X = -speed;
            if (kb.IsKeyDown(Keys.D) || kb.IsKeyDown(Keys.Right)) velocity.X = speed;

            bool jumpHeld = kb.IsKeyDown(Keys.W) || kb.IsKeyDown(Keys.Up) || kb.IsKeyDown(Keys.Space);
            bool jumpWasUp = !oldKB.IsKeyDown(Keys.W) && !oldKB.IsKeyDown(Keys.Up) && !oldKB.IsKeyDown(Keys.Space);

            if (jumpHeld && jumpWasUp && IsGrounded && !IsCrouching)
            {
                velocity.Y = JUMP_FORCE;
                IsGrounded = false;
            }

            Position.X += velocity.X * dt;
        }

        private void ApplyGravity(float dt)
        {
            if (!IsGrounded)
            {
                velocity.Y += GRAVITY_FORCE * dt;
                Position.Y += velocity.Y * dt;
                int floorY = groundY - (IsCrouching ? CROUCH_HEIGHT : HEIGHT);
                if (Position.Y >= floorY)
                {
                    Position.Y = floorY;
                    velocity.Y = 0f;
                    IsGrounded = true;
                }
            }
            else
            {
                Position.Y = groundY - (IsCrouching ? CROUCH_HEIGHT : HEIGHT);
            }
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D pixel)
        {
            float pct = Health / MaxHealth;
            spriteBatch.Draw(pixel, new Rectangle((int)Position.X, (int)Position.Y - 10, WIDTH, 5), Color.DarkGray);
            spriteBatch.Draw(pixel, new Rectangle((int)Position.X, (int)Position.Y - 10, (int)(WIDTH * pct), 5), Color.LimeGreen);
        }
    }
}