using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using MGLibrary.Factories;
using MGLibrary.GameObjects;
using MGLibrary.Graphics;
using RenderingLibrary.Math.Geometry;
using System;
using System.Collections.Generic;

namespace MGLibrary.Player
{
    public class Player
    {
        public AnimatedSprite Sprite = new AnimatedSprite();
        public Circle Hitbox = new Circle();
        public Circle Bounds = new Circle();
        public Vector2 Position = new Vector2();
        public TimeSpan AnimationSpan = new TimeSpan();
        public TimeSpan InvincibilitySpan = new TimeSpan();
        public TimeSpan RollSpan = new TimeSpan();
        public float MOVEMENT_SPEED = 5.0f;
        public List<SoundEffectInstance> sfxList = new List<SoundEffectInstance>();
        public int totalLives = 3;
        public int score = 0;
        private PlayerInput _PlayerInput = new PlayerInput();

        public enum sfx
        {
            slash,
            cry,
            hurt,
            linkDeath,
            linkRoll,
            roll,
            magicSword
        }

        public enum direction
        {
            left,
            right,
            down,
            up,
            powerUp
        }

        public Player()
        {
            
        }


        public direction facing = direction.down;

        public void LoadActor(ActorFactory actorFactory, Tilemap tilemap, string animationName) 
        {
            Sprite = actorFactory.CreateSprite(animationName, scale: 4f);
            Position = actorFactory.CenterOfTilemap(tilemap);
            AnimationSpan = new TimeSpan();
        }

        public void UpdateHitBox(ActorFactory actorFactory, Vector2 position, float radius, float measure) 
        {
            Hitbox = actorFactory.assignBounds(Sprite, position, radius, measure);
        }

        public void changeLinkFacingAnimation(TimeSpan _elapsed, TextureAtlas atlas, Item _Shadow, Item _SwordDrop, Item _Sword, int totalLives)
        {
            if (_elapsed >= AnimationSpan && AnimationSpan.TotalMilliseconds != 0f && totalLives > 0 && facing != direction.powerUp)
            {
                _Shadow.Sprite.Play(atlas.GetAnimation("empty-sprite"));
                _Shadow.Hitbox = Circle.Empty;
                _Shadow.Position = Vector2.Zero;
                switch (facing)
                {
                    case direction.left:
                        Sprite.Play(atlas.GetAnimation("link-animation-iddle-left"));
                        break;

                    case direction.right:
                        Sprite.Play(atlas.GetAnimation("link-animation-iddle-right"));
                        break;

                    case direction.down:
                        Sprite.Play(atlas.GetAnimation("link-animation-iddle-down"));
                        break;

                    case direction.up:
                        Sprite.Play(atlas.GetAnimation("link-animation-iddle-up"));
                        break;
                }
                MOVEMENT_SPEED = 5.0f;

                _PlayerInput.keysDown(this, atlas);

                AnimationSpan = new TimeSpan();
            }
            if (_elapsed >= AnimationSpan && _SwordDrop.PowerUpSpan.TotalMilliseconds != 0f && facing == direction.powerUp)
            {
                Sprite.Play(atlas.GetAnimation("link-power-up-animation-still"));
                _Sword.Sprite.Play(atlas.GetAnimation("sword-power-up-animation-still"));
            }
        }

        public void KeepInBounds(Rectangle _roomBounds)
        {
            // Use distance based checks to determine if link is within the
            // bounds of the game screen, and if it is outside that screen edge,
            // move it back inside.
            if (Bounds.Left < _roomBounds.Left)
            {
                Position.X = _roomBounds.Left;
            }
            else if (Bounds.Right > _roomBounds.Right)
            {
                Position.X = _roomBounds.Right - Sprite.Width;
            }
            if (Bounds.Top < _roomBounds.Top)
            {
                Position.Y = _roomBounds.Top;
            }
            else if (Bounds.Bottom > _roomBounds.Bottom)
            {
                Position.Y = _roomBounds.Bottom - Sprite.Height;
            }
        }
    }
}
