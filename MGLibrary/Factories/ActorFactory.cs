using Microsoft.Xna.Framework;
using MGLibrary.Graphics;
using RenderingLibrary.Math.Geometry;
using System;

namespace MGLibrary.Factories
{
    /// <summary>
    /// Fábrica genérica para crear sprites animados y utilitarios de spawn/movimiento.
    /// No conoce “jugador” o “enemigos”; el juego decide qué animación y escala usar.
    /// </summary>
    public class ActorFactory
    {
        private readonly TextureAtlas _atlas;
        private readonly Random _rng = new();
        public Circle bounds;
        public Circle hitBox;
        public AnimatedSprite sprite;

        public ActorFactory(TextureAtlas atlas)
        {
            _atlas = atlas ?? throw new ArgumentNullException(nameof(atlas));
        }

        /// <summary>
        /// Crea un AnimatedSprite a partir del nombre de animación del atlas.
        /// </summary>
        public AnimatedSprite CreateSprite(string animationName, float scale = 1f)
        {
            if (string.IsNullOrWhiteSpace(animationName))
                throw new ArgumentException("animationName requerido", nameof(animationName));

            var s = _atlas.CreateAnimatedSprite(animationName);
            s.Scale = new Vector2(scale, scale);
            sprite = s;
            return s;
        }

        public Circle assignBounds(dynamic sprite, Vector2 pos, float radius, float measure) 
        {
            if (measure == 99) 
            {
                return new Circle(
                (int)pos.X,
                (int)pos.Y,
                (int)(sprite.Width * radius)
                );
            }
            return new Circle(
                (int)(pos.X + (sprite.Width * measure)),
                (int)(pos.Y + (sprite.Height * measure)),
                (int)(sprite.Width * radius)
            );
        }

        /// <summary>
        /// Centro del tilemap (útil para spawnear al jugador).
        /// </summary>
        public Vector2 CenterOfTilemap(Tilemap tilemap)
        {
            if (tilemap == null) throw new ArgumentNullException(nameof(tilemap));
            int centerRow = tilemap.Rows / 2;
            int centerCol = tilemap.Columns / 2;
            return new Vector2(centerCol * tilemap.TileWidth, centerRow * tilemap.TileHeight);
        }

        /// <summary>
        /// Posición aleatoria dentro de bounds (típicamente, el “room”).
        /// </summary>
        public Vector2 RandomPosition(Rectangle bounds)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0)
                throw new ArgumentException("bounds inválido", nameof(bounds));

            int x = _rng.Next(bounds.Left, bounds.Right);
            int y = _rng.Next(bounds.Top, bounds.Bottom);
            return new Vector2(x, y);
        }

        /// <summary>
        /// Velocidad aleatoria con magnitud entre [minSpeed, maxSpeed].
        /// </summary>
        public Vector2 RandomVelocity(float minSpeed = 1.5f, float maxSpeed = 3.5f)
        {
            if (minSpeed < 0 || maxSpeed <= 0 || maxSpeed < minSpeed)
                throw new ArgumentException("rango de velocidades inválido");

            float angle = (float)(_rng.NextDouble() * MathHelper.TwoPi);
            float speed = MathHelper.Lerp(minSpeed, maxSpeed, (float)_rng.NextDouble());
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * speed;
        }

        /// <summary>
        /// Asegura que la posición esté dentro de bounds.
        /// </summary>
        public Vector2 ClampToBounds(Vector2 position, Rectangle bounds)
        {
            float x = MathHelper.Clamp(position.X, bounds.Left, bounds.Right);
            float y = MathHelper.Clamp(position.Y, bounds.Top, bounds.Bottom);
            return new Vector2(x, y);
        }
    }
}
