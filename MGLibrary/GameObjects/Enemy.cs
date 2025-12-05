using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using MGLibrary.Graphics;
using System;
using System.Collections.Generic;

namespace MGLibrary.GameObjects
{
    public class Enemy
    {
        #region Attributes
        public AnimatedSprite Sprite = new AnimatedSprite();
        public Vector2 Position = new Vector2();
        public Vector2 Velocity = new Vector2();
        public Circle Bounds = new Circle();
        public Circle Hitbox = new Circle();
        public TimeSpan Span = new TimeSpan();
        public SoundEffectInstance Sound;
        public float Speed = new float();
        #endregion

        public Enemy()
        {
            loadEnemy();
        }

        #region Methods
        public void loadEnemy()
        {
            Span = new TimeSpan();
            Velocity = new Vector2();
            Bounds = new Circle();
            Hitbox = new Circle();
            Speed = 0.5f;
        }

        public void randomBatPosition(Rectangle _roomBounds)
        {
            var rng = Random.Shared.Next(0, 4);
            if (rng == 0)
            {
                // Initial bat position will be in the top left corner of the room
                Position = new Vector2(_roomBounds.Left + 50f, _roomBounds.Top + 50f);
            }
            if (rng == 1)
            {
                // Initial bat position will be in the top left corner of the room
                Position = new Vector2(_roomBounds.Right + 50f, _roomBounds.Top + 50f);
            }
            if (rng == 2)
            {
                // Initial bat position will be in the top left corner of the room
                Position = new Vector2(_roomBounds.Right + 50f, _roomBounds.Bottom + 50f);
            }
            if (rng == 3)
            {
                // Initial bat position will be in the top left corner of the room
                Position = new Vector2(_roomBounds.Left + 50f, _roomBounds.Bottom + 50f);
            }
            // Assign the initial random velocity to the bat.
            AssignRandomBatVelocity();
        }

        public void AssignRandomBatVelocity()
        {
            // Generate a random angle
            float angle = (float)(Random.Shared.NextDouble() * Math.PI * 2);

            // Convert angle to a direction vector
            float x = (float)Math.Cos(angle);
            float y = (float)Math.Sin(angle);
            Vector2 direction = new Vector2(x, y);

            // Multiply the direction vector by the movement speed
            Velocity = direction * Speed;
        }

        public void resetBatAnimation(TimeSpan _elapsed, TextureAtlas atlas, Tilemap _tilemap) 
        {
            if (_elapsed > Span && Span.TotalMilliseconds != 0f)
            {
                // Choose a random row and column based on the total number of each
                int column = Random.Shared.Next(1, _tilemap.Columns - 1);
                int row = Random.Shared.Next(1, _tilemap.Rows - 1);

                // Change the bat position by setting the x and y values equal to
                // the column and row multiplied by the width and height.
                Position = new Vector2(column * Sprite.Width, row * Sprite.Height);

                Sprite.Play(atlas.GetAnimation("bat-animation"));

                if (Speed <= 13f)
                {
                    Speed += 1f;
                }

                // Assign a new random velocity to the bat
                AssignRandomBatVelocity();

                Span = new TimeSpan();
            }
        }

        public void assignBatBounds(Rectangle _roomBounds)
        {
            // Create a bounding circle for the bat
            if (Span.TotalMilliseconds == 0f)
            {
                // Calculate the new position of the bat based on the velocity
                Vector2 newBatPosition = Position + Velocity;

                Bounds = new Circle(
                    (int)(newBatPosition.X + (Sprite.Width * 0.4f)),
                    (int)(newBatPosition.Y + (Sprite.Height * 0.4f)),
                    (int)(Sprite.Width * 0.4f)
                );

                Vector2 normal = Vector2.Zero;

                // Use distance based checks to determine if the bat is within the
                // bounds of the game screen, and if it is outside that screen edge,
                // reflect it about the screen edge normal
                if (Bounds.Left < _roomBounds.Left)
                {
                    normal.X = Vector2.UnitX.X;
                    newBatPosition.X = _roomBounds.Left;
                }
                else if (Bounds.Right > _roomBounds.Right)
                {
                    normal.X = -Vector2.UnitX.X;
                    newBatPosition.X = _roomBounds.Right - Sprite.Width;
                }

                if (Bounds.Top < _roomBounds.Top)
                {
                    normal.Y = Vector2.UnitY.Y;
                    newBatPosition.Y = _roomBounds.Top;
                }
                else if (Bounds.Bottom > _roomBounds.Bottom)
                {
                    normal.Y = -Vector2.UnitY.Y;
                    newBatPosition.Y = _roomBounds.Bottom - Sprite.Height;
                }

                // If the normal is anything but Vector2.Zero, this means the bat had
                // moved outside the screen edge so we should reflect it about the
                // normal.
                if (normal != Vector2.Zero)
                {
                    normal.Normalize();
                    Velocity = Vector2.Reflect(Velocity, normal);
                }

                Position = newBatPosition;
            }
            else
            {
                Bounds = Circle.Empty;
            }
        }

        public void checkLinkCollision(TimeSpan _elapsed, Player.Player _Link, List<AnimatedSprite> hearts, TextureAtlas atlas)
        {
            if (_Link.Hitbox.Intersects(Hitbox) &&
                _Link.InvincibilitySpan.TotalSeconds == 0f &&
                _Link.RollSpan.TotalMilliseconds == 0f)
            {
                TimeSpan time = new TimeSpan(0, 0, 2);
                _Link.InvincibilitySpan = _elapsed + time;
                if (_Link.totalLives == 1)
                {
                    hearts[0].Play(atlas.GetAnimation("empty-heart-container"));
                    _Link.totalLives--;
                }
                if (_Link.totalLives == 2)
                {
                    hearts[1].Play(atlas.GetAnimation("empty-heart-container"));
                    _Link.totalLives--;
                }
                if (_Link.totalLives == 3)
                {
                    hearts[2].Play(atlas.GetAnimation("empty-heart-container"));
                    _Link.totalLives--;
                }
                if (_Link.totalLives > 0)
                {
                    _Link.sfxList[(int)Player.Player.sfx.hurt].Play();
                }
            }
        }
    }
    #endregion
}
