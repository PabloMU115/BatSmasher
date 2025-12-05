using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using MGLibrary.Factories;
using MGLibrary.Graphics;
using System;
using System.Collections.Generic;

namespace MGLibrary.GameObjects
{
    public class Item
    {
        #region Attributes
        public AnimatedSprite Sprite = new AnimatedSprite();
        public Circle Hitbox = new Circle();
        public Vector2 Position = new Vector2();
        public TimeSpan AnimationSpan = new TimeSpan();
        public TimeSpan PowerUpSpan = new TimeSpan();
        public SoundEffectInstance sfx;
        public List<Vector2[]> SwordOffsets = new List<Vector2[]>();
        #endregion

        public Item() { }

        #region Methods
        public void loadOffsets()
        {
            //left
            SwordOffsets.Add(
                new Vector2[]
                {
                    new Vector2(2 * 4f, -5 * 4f),//1
                    new Vector2(-2 * 4f, -3 * 4f),//2
                    new Vector2(-8 * 4f, -3 * 4f),//3
                    new Vector2(-9 * 4f, 4 * 4f),//4
                    new Vector2(-9 * 4f, 8 * 4f),//5
                    new Vector2(-7 * 4f, 10 * 4f),//6
                    new Vector2(-5 * 4f, 17 * 4f),//7
                    new Vector2(-5 * 4f, 23 * 4f),//8
                    new Vector2(8 * 4f, 23 * 4f)//9
                }
            );

            //right
            SwordOffsets.Add(
                new Vector2[]
                {
                    new Vector2(6 * 4f, -5 * 4f),//1
                    new Vector2(15 * 4f, -3 * 4f),//2
                    new Vector2(19 * 4f, -1 * 4f),//3
                    new Vector2(19 * 4f, 4 * 4f),//4
                    new Vector2(24 * 4f, 8 * 4f),//5
                    new Vector2(21 * 4f, 14 * 4f),//6
                    new Vector2(15 * 4f, 20 * 4f),//7
                    new Vector2(10 * 4f, 23 * 4f),//8
                    new Vector2(7 * 4f, 23 * 4f)//9
                }
            );

            //down
            SwordOffsets.Add(
                new Vector2[]
                {
                    new Vector2(-9 * 4f, 16 * 4f),
                    new Vector2(-6 * 4f, 20 * 4f),
                    new Vector2(-9 * 4f, 18 * 4f),
                    new Vector2(-1 * 4f, 23 * 4f),
                    new Vector2(3 * 4f, 24 * 4f),
                    new Vector2(10 * 4f, 23 * 4f),
                    new Vector2(17 * 4f, 21 * 4f),
                    new Vector2(19 * 4f, 20 * 4f),
                    new Vector2(20 * 4f, 14 * 4f)
                }
            );

            //up
            SwordOffsets.Add(
                new Vector2[]
                {
                    new Vector2(10 * 4f, 6 * 4f),//1
                    new Vector2(8 * 4f, 3 * 4f),//2
                    new Vector2(6 * 4f, 0 * 4f),//3
                    new Vector2(4 * 4f, -3 * 4f),//4
                    new Vector2(2 * 4f, -3 * 4f),//5
                    new Vector2(-6 * 4f, -3 * 4f),//6
                    new Vector2(-10 * 4f, -1 * 4f),//7
                    new Vector2(-12 * 4f, 6 * 4f),//8
                    new Vector2(-15 * 4f, 10 * 4f)//9
                }
            );

            //power up
            SwordOffsets.Add(
                new Vector2[]
                {
                    Vector2.Zero,//1
                    new Vector2(-8 * 4f, 11 * 4f),//1
                    new Vector2(13.5f * 4f, -9 * 4f),//2
                }
            );
        }

        public void LoadObject(ActorFactory actorFactory, string animationName, Vector2 position, float scale)
        {
            Sprite = actorFactory.CreateSprite(animationName, scale: scale);
            Position = position;
            AnimationSpan = new TimeSpan();
        }

        public void UpdateHitBox(ActorFactory actorFactory, Vector2 position, float radius, float measure)
        {
            Hitbox = actorFactory.assignBounds(Sprite, position, radius, measure);
        }
        #endregion
    }
}
