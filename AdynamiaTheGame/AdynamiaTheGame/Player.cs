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
        public bool IsStunned { get { return stunTimer > 0f; } }
        public bool IsPoisoned { get { return poisonTimer > 0f; } }

        // More properties (player combat properties/states)
        public float MaxHealth = 100f;
        public float Health;
        public float DamageMultiplier = 1f;
        public float SpeedMultiplier = 1f;
        public bool HasBoon = false;
        public bool IsGrounded = false;
        public bool IsCrouching = false;
        public bool IsDashing = false;

        // More constants (movement)
        private float moveSpeed = 200f;
        private float jumpForce = -500f;
        private const float GRAVITY_FORCE = 900f;
        private const float CROUCH_FACTOR = 0.4f;
        private const float DASH_SPEED = 480f;
        private const float DASH_DURATION_MAX = 0.18f;

        // Timers/stackables
        private float dashTimer = 0f;
        private Vector2 dashDir;
        private float stunTimer = 0f;
        private float poisonTimer = 0f;
        private readonly float poisonDps = 4f;
        private float weaknessStacks = 0f;
        private int groundY;

        // Attack cooldowns + maxes
        private float quickAttackCooldown = 0f;
        private float dashAttackCooldown = 0f;
        private float heavyAttackCooldown = 0f;
        private float boonAttackCooldown = 0f;
        private const float QuickCooldownMax = 0.3f;
        private const float DashCooldownMax = 2f;
        private const float HeavyCooldownMax = 5f;
        private const float BoonCooldownMax = 3.5f;

        // Just learned about these, these lowk seem useful
        // Allow for external systems (like the Game1 class) to subscribe to player attacks without needing to modify the Player class itself for each new attack type (kinda paraphrasing the actual docs but you get the point)
        public event Action<float, bool> OnQuickAttack;
        public event Action<float> OnDashAttack;
        public event Action<float, float> OnHeavyAttack;
        public event Action<float> OnBoonAttack;

        private MouseState oldMouse;
        private KeyboardState oldKB;

        // Self-explanatory constructor and functions ngl (figure this out yourself)
        public Player(Vector2 startPosition, int groundY)
        {
            Position = startPosition;
            this.groundY = groundY;
            Health = MaxHealth;
        }

        public void TakeDamage(float amount)
        {
            Health -= amount;
            Health = Math.Max(0f, Health);
        }

        public void Heal(float amount)
        {
            Health = Math.Min(MaxHealth, Health + amount);
        }

        public void ApplyStun(float duration)
        {
            stunTimer = duration;
        }

        public void ApplyPoison(float duration)
        {
            poisonTimer = duration;
        }
        public void ApplyHealth(int health)
        {
            Health += health;
        }
        public void ApplySpeed(float speed)
        {
            moveSpeed += speed;
        }
        public void ApplyJump(float jump)
        {
            jumpForce -= jump;
        }
        public void ApplyRespawn()
        {
            if (Health <= 0)
            {
                Health = MaxHealth / 2;
            }
        }
        public void ApplyPoison(int poison)
        {
            Health -= poison;
        }
        public void ApplyDarkness()
        {
            //idk
        }
        public void ApplyWeakness(float stacks)
        {
            weaknessStacks += stacks;
            MaxHealth = Math.Max(20f, 100f - weaknessStacks * 8f);
            Health = Math.Min(Health, MaxHealth);
            DamageMultiplier = Math.Max(0.2f, 1f - weaknessStacks * 0.04f);
        }

        public void CurePoison()
        {
            poisonTimer = 0f;
            weaknessStacks = 0f;
            MaxHealth = 100f;
            DamageMultiplier = 1f;
        }

        public void Update(GameTime gameTime, KeyboardState ks, MouseState ms)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (poisonTimer > 0f)
            {
                poisonTimer -= dt;
                TakeDamage(poisonDps * dt);
            }

            UpdateCooldowns(dt);

            if (stunTimer > 0f)
            {
                stunTimer -= dt;
                ApplyGravity(dt);
                oldMouse = ms;
                oldKB = ks;
                return;
            }

            if (IsDashing)
            {
                dashTimer -= dt;
                Position += dashDir * DASH_SPEED * dt;
                if (dashTimer <= 0f) IsDashing = false;
            }
            else
            {
                HandleMovement(ks, dt);
            }

            ApplyGravity(dt);
            HandleAttacks(ks, ms);

            oldMouse = ms;
            oldKB = ks;
        }

        private void UpdateCooldowns(float dt)
        {
            if (quickAttackCooldown > 0f) quickAttackCooldown -= dt;
            if (dashAttackCooldown > 0f) dashAttackCooldown -= dt;
            if (heavyAttackCooldown > 0f) heavyAttackCooldown -= dt;
            if (boonAttackCooldown > 0f) boonAttackCooldown -= dt;
        }

        private void HandleMovement(KeyboardState kb, float dt)
        {
            bool crouchInput = (kb.IsKeyDown(Keys.S) || kb.IsKeyDown(Keys.Down)) && IsGrounded;
            IsCrouching = crouchInput;

            float speed = moveSpeed * SpeedMultiplier * (IsCrouching ? CROUCH_FACTOR : 1f);
            velocity.X = 0f;

            if (kb.IsKeyDown(Keys.A) || kb.IsKeyDown(Keys.Left)) velocity.X = -speed;
            if (kb.IsKeyDown(Keys.D) || kb.IsKeyDown(Keys.Right)) velocity.X = speed;

            bool jumpHeld = kb.IsKeyDown(Keys.W) || kb.IsKeyDown(Keys.Up) || kb.IsKeyDown(Keys.Space);
            bool jumpWasUp = !oldKB.IsKeyDown(Keys.W) && !oldKB.IsKeyDown(Keys.Up) && !oldKB.IsKeyDown(Keys.Space);

            if (jumpHeld && jumpWasUp && IsGrounded && !IsCrouching)
            {
                velocity.Y = jumpForce;
                IsGrounded = false;
            }

            Position.X += velocity.X * dt;
        }

        private void HandleAttacks(KeyboardState kb, MouseState ms)
        {
            bool lmb = ms.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released;
            bool rmb = ms.RightButton == ButtonState.Pressed && oldMouse.RightButton == ButtonState.Released;
            bool q = kb.IsKeyDown(Keys.Q) && !oldKB.IsKeyDown(Keys.Q);
            bool e = kb.IsKeyDown(Keys.E) && !oldKB.IsKeyDown(Keys.E);
            bool f = kb.IsKeyDown(Keys.F) && !oldKB.IsKeyDown(Keys.F) && HasBoon;

            if (lmb && quickAttackCooldown <= 0f)
            {
                quickAttackCooldown = QuickCooldownMax;
                OnQuickAttack?.Invoke(10f * DamageMultiplier, true);
            }
            else if (rmb && quickAttackCooldown <= 0f)
            {
                quickAttackCooldown = QuickCooldownMax;
                OnQuickAttack?.Invoke(10f * DamageMultiplier, false);
            }

            if (q && dashAttackCooldown <= 0f)
            {
                dashAttackCooldown = DashCooldownMax;
                IsDashing = true;
                dashTimer = DASH_DURATION_MAX;
                dashDir = velocity.X >= 0f ? Vector2.UnitX : -Vector2.UnitX;
                OnDashAttack?.Invoke(20f * DamageMultiplier);
            }

            if (e && heavyAttackCooldown <= 0f)
            {
                heavyAttackCooldown = HeavyCooldownMax;
                velocity.Y = jumpForce * 0.6f;
                IsGrounded = false;
                OnHeavyAttack?.Invoke(40f * DamageMultiplier, 80f);
            }

            if (f && boonAttackCooldown <= 0f)
            {
                boonAttackCooldown = BoonCooldownMax;
                OnBoonAttack?.Invoke(30f * DamageMultiplier);
            }
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
            // Lowk love ternaries
            // Y'all gotta refactor them ternaries ngl if you want to make this readable, but I don't care enough to do it myself so here we are
            int drawH = IsCrouching ? CROUCH_HEIGHT : HEIGHT;
            Color bodyColor = IsStunned ? Color.LightGray : (IsPoisoned ? Color.LightGreen : Color.DodgerBlue);
            spriteBatch.Draw(pixel, new Rectangle((int)Position.X, (int)Position.Y, WIDTH, drawH), bodyColor);

            float pct = Health / MaxHealth;
            spriteBatch.Draw(pixel, new Rectangle((int)Position.X, (int)Position.Y - 10, WIDTH, 5), Color.DarkGray);
            spriteBatch.Draw(pixel, new Rectangle((int)Position.X, (int)Position.Y - 10, (int)(WIDTH * pct), 5), Color.LimeGreen);
            
        }
    }
}