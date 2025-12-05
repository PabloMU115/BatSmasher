using Microsoft.Xna.Framework.Input;
using MGLibrary.GameObjects;
using MGLibrary.Graphics;
using System;

namespace MGLibrary.Player
{
    public class PlayerInput
    {
        public PlayerInput() 
        {

        }

        private void checkSpace(TimeSpan _elapsed ,Player _Link, Item _sword, TextureAtlas attack)
        {
            if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Space))
            {

                switch (_Link.facing)
                {
                    case Player.direction.left:
                        _Link.Sprite.Play(attack.GetAnimation("link-slash-left-animation"));
                        _sword.Sprite.Play(attack.GetAnimation("sword-slash-left-animation"));
                        break;

                    case Player.direction.right:
                        _Link.Sprite.Play(attack.GetAnimation("link-slash-right-animation"));
                        _sword.Sprite.Play(attack.GetAnimation("sword-slash-right-animation"));
                        break;

                    case Player.direction.down:
                        _Link.Sprite.Play(attack.GetAnimation("link-slash-down-animation"));
                        _sword.Sprite.Play(attack.GetAnimation("sword-slash-down-animation"));
                        break;

                    case Player.direction.up:
                        _Link.Sprite.Play(attack.GetAnimation("link-slash-up-animation"));
                        _sword.Sprite.Play(attack.GetAnimation("sword-slash-up-animation"));
                        break;
                }
                _Link.AnimationSpan = _elapsed + _Link.Sprite.Animation.Delay * 11;
                _Link.MOVEMENT_SPEED = 0f;
                _Link.sfxList[(int)Player.sfx.cry].Play();
                _Link.sfxList[(int)Player.sfx.slash].Play();
            }
        }

        private void checkShift(TimeSpan _elapsed, Player _Link, Item _sword, TextureAtlas atlas, TextureAtlas attack)
        {
            if (Core.Input.Keyboard.WasKeyJustPressed(Keys.LeftShift))
            {
                switch (_Link.facing)
                {
                    case Player.direction.left:
                        _Link.Sprite.Play(atlas.GetAnimation("link-animation-left-roll"));
                        break;

                    case Player.direction.right:
                        _Link.Sprite.Play(atlas.GetAnimation("link-animation-right-roll"));
                        break;

                    case Player.direction.down:
                        _Link.Sprite.Play(atlas.GetAnimation("link-animation-down-roll"));
                        break;

                    case Player.direction.up:
                        _Link.Sprite.Play(atlas.GetAnimation("link-animation-up-roll"));
                        break;
                }
                _Link.RollSpan = _elapsed + _Link.Sprite.Animation.Delay * 8;
                _Link.AnimationSpan = _elapsed + _Link.Sprite.Animation.Delay * 8;
                _Link.MOVEMENT_SPEED = 9.0f;
                _Link.sfxList[(int)Player.sfx.linkRoll].Play();
            }
        }

        public void roll(TimeSpan _elapsed, Player _Link) 
        {
            if (_Link.MOVEMENT_SPEED > 5.0f)
            {
                _Link.sfxList[(int)Player.sfx.roll].Play();
                switch (_Link.facing)
                {
                    case Player.direction.left:
                        _Link.Position.X -= _Link.MOVEMENT_SPEED;
                        break;

                    case Player.direction.right:
                        _Link.Position.X += _Link.MOVEMENT_SPEED;
                        break;

                    case Player.direction.down:
                        _Link.Position.Y += _Link.MOVEMENT_SPEED;
                        break;

                    case Player.direction.up:
                        _Link.Position.Y -= _Link.MOVEMENT_SPEED;
                        break;
                }
            }
        }

        public void CheckKeyboardInput(TimeSpan _elapsed, Player _Link, Item _sword, TextureAtlas atlas, TextureAtlas attack)
        {
            if (_Link.AnimationSpan.TotalMilliseconds == 0)
            {
                checkShift(_elapsed, _Link, _sword, atlas, attack);
                checkSpace(_elapsed, _Link, _sword, attack);
            }

            if (_Link.AnimationSpan.TotalMilliseconds == 0)
            {
                if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Left))
                {
                    _Link.facing = Player.direction.left;
                    _Link.Sprite.Play(atlas.GetAnimation("link-animation-left"));
                }

                if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Right))
                {
                    _Link.facing = Player.direction.right;
                    _Link.Sprite.Play(atlas.GetAnimation("link-animation-right"));
                }

                if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Up))
                {
                    _Link.facing = Player.direction.up;
                    _Link.Sprite.Play(atlas.GetAnimation("link-animation-up"));
                }

                if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Down))
                {
                    _Link.facing = Player.direction.down;
                    _Link.Sprite.Play(atlas.GetAnimation("link-animation-down"));
                }

                if (Core.Input.Keyboard.WasKeyJustReleased(Keys.Left))
                {
                    _Link.Sprite.Play(atlas.GetAnimation("link-animation-iddle-left"));
                    if (Core.Input.Keyboard.IsKeyDown(Keys.Down))
                    {
                        _Link.facing = Player.direction.down;
                        _Link.Sprite.Play(atlas.GetAnimation("link-animation-down"));
                    }
                    if (Core.Input.Keyboard.IsKeyDown(Keys.Up))
                    {
                        _Link.facing = Player.direction.up;
                        _Link.Sprite.Play(atlas.GetAnimation("link-animation-up"));
                    }
                    if (Core.Input.Keyboard.IsKeyDown(Keys.Right))
                    {
                        _Link.facing = Player.direction.right;
                        _Link.Sprite.Play(atlas.GetAnimation("link-animation-right"));
                    }
                    if (Core.Input.Keyboard.IsKeyDown(Keys.Up) &&
                            Core.Input.Keyboard.IsKeyDown(Keys.Down))
                    {
                        if (_Link.facing == Player.direction.up)
                        {
                            _Link.Sprite.Play(atlas.GetAnimation("link-animation-iddle-up"));
                        }
                        if (_Link.facing == Player.direction.down)
                        {
                            _Link.Sprite.Play(atlas.GetAnimation("link-animation-iddle-down"));
                        }
                    }
                }

                if (Core.Input.Keyboard.WasKeyJustReleased(Keys.Right))
                {
                    _Link.Sprite.Play(atlas.GetAnimation("link-animation-iddle-right"));
                    if (Core.Input.Keyboard.IsKeyDown(Keys.Left))
                    {
                        _Link.facing = Player.direction.left;
                        _Link.Sprite.Play(atlas.GetAnimation("link-animation-left"));
                    }
                    if (Core.Input.Keyboard.IsKeyDown(Keys.Up))
                    {
                        _Link.facing = Player.direction.up;
                        _Link.Sprite.Play(atlas.GetAnimation("link-animation-up"));
                    }
                    if (Core.Input.Keyboard.IsKeyDown(Keys.Down))
                    {
                        _Link.facing = Player.direction.down;
                        _Link.Sprite.Play(atlas.GetAnimation("link-animation-down"));
                    }
                    if (Core.Input.Keyboard.IsKeyDown(Keys.Up) &&
                            Core.Input.Keyboard.IsKeyDown(Keys.Down))
                    {
                        if (_Link.facing == Player.direction.up)
                        {
                            _Link.Sprite.Play(atlas.GetAnimation("link-animation-iddle-up"));
                        }
                        if (_Link.facing == Player.direction.down)
                        {
                            _Link.Sprite.Play(atlas.GetAnimation("link-animation-iddle-down"));
                        }
                    }
                }

                if (Core.Input.Keyboard.WasKeyJustReleased(Keys.Up))
                {
                    _Link.Sprite.Play(atlas.GetAnimation("link-animation-iddle-up"));
                    if (Core.Input.Keyboard.IsKeyDown(Keys.Left) && !Core.Input.Keyboard.IsKeyDown(Keys.Right))
                    {
                        _Link.facing = Player.direction.left;
                        _Link.Sprite.Play(atlas.GetAnimation("link-animation-left"));
                    }
                    if (Core.Input.Keyboard.IsKeyDown(Keys.Down))
                    {
                        _Link.facing = Player.direction.down;
                        _Link.Sprite.Play(atlas.GetAnimation("link-animation-down"));
                    }
                    if (Core.Input.Keyboard.IsKeyDown(Keys.Right) && !Core.Input.Keyboard.IsKeyDown(Keys.Left))
                    {
                        _Link.facing = Player.direction.right;
                        _Link.Sprite.Play(atlas.GetAnimation("link-animation-right"));
                    }
                }

                if (Core.Input.Keyboard.WasKeyJustReleased(Keys.Down))
                {
                    _Link.Sprite.Play(atlas.GetAnimation("link-animation-iddle-down"));
                    if (Core.Input.Keyboard.IsKeyDown(Keys.Left) && !Core.Input.Keyboard.IsKeyDown(Keys.Right))
                    {
                        _Link.facing = Player.direction.left;
                        _Link.Sprite.Play(atlas.GetAnimation("link-animation-left"));
                    }
                    if (Core.Input.Keyboard.IsKeyDown(Keys.Up))
                    {
                        _Link.facing = Player.direction.up;
                        _Link.Sprite.Play(atlas.GetAnimation("link-animation-up"));
                    }
                    if (Core.Input.Keyboard.IsKeyDown(Keys.Right) && !Core.Input.Keyboard.IsKeyDown(Keys.Left))
                    {
                        _Link.facing = Player.direction.right;
                        _Link.Sprite.Play(atlas.GetAnimation("link-animation-right"));
                    }
                }

                keysDown(_Link, atlas);
            }
        }

        public void keysDown(Player _Link, TextureAtlas atlas) 
        {
            // If the A or Left keys are down, move link left on the screen.
            if (Core.Input.Keyboard.IsKeyDown(Keys.Left))
            {
                if (_Link.AnimationSpan.TotalMilliseconds > 0f)
                {
                    if (Core.Input.Keyboard.IsKeyUp(Keys.Right))
                    {
                        _Link.facing = Player.direction.left;
                        _Link.Sprite.Play(atlas.GetAnimation("link-animation-left"));
                    }
                }
                if (Core.Input.Keyboard.WasKeyJustReleased(Keys.Right))
                {
                    _Link.facing = Player.direction.left;
                    _Link.Sprite.Play(atlas.GetAnimation("link-animation-left"));
                    if (Core.Input.Keyboard.IsKeyDown(Keys.Up))
                    {
                        _Link.facing = Player.direction.up;
                        _Link.Sprite.Play(atlas.GetAnimation("link-animation-up"));
                    }
                    if (Core.Input.Keyboard.IsKeyDown(Keys.Down))
                    {
                        _Link.facing = Player.direction.down;
                        _Link.Sprite.Play(atlas.GetAnimation("link-animation-down"));
                    }
                }
                if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Right) && !Core.Input.Keyboard.IsKeyDown(Keys.Up) && !Core.Input.Keyboard.IsKeyDown(Keys.Down))
                {
                    _Link.facing = Player.direction.right;
                    _Link.Sprite.Play(atlas.GetAnimation("link-animation-iddle-right"));
                }
                _Link.Position.X -= _Link.MOVEMENT_SPEED;
            }

            // If the D or Right keys are down, move link right on the screen.
            if (Core.Input.Keyboard.IsKeyDown(Keys.Right))
            {
                if (_Link.AnimationSpan.TotalMilliseconds > 0f)
                {
                    if (Core.Input.Keyboard.IsKeyUp(Keys.Left))
                    {
                        _Link.facing = Player.direction.right;
                        _Link.Sprite.Play(atlas.GetAnimation("link-animation-right"));
                    }
                }
                if (Core.Input.Keyboard.WasKeyJustReleased(Keys.Left))
                {
                    _Link.facing = Player.direction.right;
                    _Link.Sprite.Play(atlas.GetAnimation("link-animation-right"));
                    if (Core.Input.Keyboard.IsKeyDown(Keys.Up))
                    {
                        _Link.facing = Player.direction.up;
                        _Link.Sprite.Play(atlas.GetAnimation("link-animation-up"));
                    }
                    if (Core.Input.Keyboard.IsKeyDown(Keys.Down))
                    {
                        _Link.facing = Player.direction.down;
                        _Link.Sprite.Play(atlas.GetAnimation("link-animation-down"));
                    }
                }
                if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Left) && !Core.Input.Keyboard.IsKeyDown(Keys.Up) && !Core.Input.Keyboard.IsKeyDown(Keys.Down))
                {
                    _Link.facing = Player.direction.left;
                    _Link.Sprite.Play(atlas.GetAnimation("link-animation-iddle-left"));
                }
                _Link.Position.X += _Link.MOVEMENT_SPEED;
            }

            // If the W or Up keys are down, move link up on the screen.
            if (Core.Input.Keyboard.IsKeyDown(Keys.Up))
            {
                if (_Link.AnimationSpan.TotalMilliseconds > 0f)
                {
                    if (Core.Input.Keyboard.IsKeyUp(Keys.Down))
                    {
                        _Link.facing = Player.direction.up;
                        _Link.Sprite.Play(atlas.GetAnimation("link-animation-up"));
                    }
                }
                if (Core.Input.Keyboard.WasKeyJustReleased(Keys.Down))
                {
                    _Link.facing = Player.direction.up;
                    _Link.Sprite.Play(atlas.GetAnimation("link-animation-up"));
                }
                if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Down))
                {
                    _Link.facing = Player.direction.down;
                    _Link.Sprite.Play(atlas.GetAnimation("link-animation-iddle-down"));

                    if (Core.Input.Keyboard.IsKeyDown(Keys.Left))
                    {
                        _Link.facing = Player.direction.left;
                        _Link.Sprite.Play(atlas.GetAnimation("link-animation-left"));
                    }
                    if (Core.Input.Keyboard.IsKeyDown(Keys.Right))
                    {
                        _Link.facing = Player.direction.right;
                        _Link.Sprite.Play(atlas.GetAnimation("link-animation-right"));
                    }
                }
                if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Left))
                {
                    _Link.facing = Player.direction.left;
                    if (Core.Input.Keyboard.IsKeyDown(Keys.Down))
                    {
                        _Link.facing = Player.direction.left;
                        _Link.Sprite.Play(atlas.GetAnimation("link-animation-left"));
                        if (Core.Input.Keyboard.IsKeyDown(Keys.Right))
                        {
                            _Link.facing = Player.direction.left;
                            _Link.Sprite.Play(atlas.GetAnimation("link-animation-iddle-left"));
                        }
                    }
                    else
                    {
                        _Link.facing = Player.direction.up;
                        _Link.Sprite.Play(atlas.GetAnimation("link-animation-up"));
                    }
                }
                if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Right))
                {
                    _Link.facing = Player.direction.right;
                    if (Core.Input.Keyboard.IsKeyDown(Keys.Down))
                    {
                        _Link.facing = Player.direction.right;
                        _Link.Sprite.Play(atlas.GetAnimation("link-animation-right"));
                        if (Core.Input.Keyboard.IsKeyDown(Keys.Left))
                        {
                            _Link.facing = Player.direction.left;
                            _Link.Sprite.Play(atlas.GetAnimation("link-animation-iddle-left"));
                        }
                    }
                    else
                    {
                        _Link.facing = Player.direction.up;
                        _Link.Sprite.Play(atlas.GetAnimation("link-animation-up"));
                    }
                }
                if (Core.Input.Keyboard.IsKeyDown(Keys.Down))
                {
                    if (Core.Input.Keyboard.WasKeyJustReleased(Keys.Right) && Core.Input.Keyboard.IsKeyDown(Keys.Left))
                    {
                        _Link.facing = Player.direction.left;
                        _Link.Sprite.Play(atlas.GetAnimation("link-animation-left"));
                    }
                    if (Core.Input.Keyboard.WasKeyJustReleased(Keys.Left) && Core.Input.Keyboard.IsKeyDown(Keys.Right))
                    {
                        _Link.facing = Player.direction.right;
                        _Link.Sprite.Play(atlas.GetAnimation("link-animation-right"));
                    }
                }

                _Link.Position.Y -= _Link.MOVEMENT_SPEED;
            }

            // if the S or Down keys are down, move link down on the screen.
            if (Core.Input.Keyboard.IsKeyDown(Keys.Down))
            {
                if (_Link.AnimationSpan.TotalMilliseconds > 0f)
                {
                    if (Core.Input.Keyboard.IsKeyUp(Keys.Up))
                    {
                        _Link.facing = Player.direction.down;
                        _Link.Sprite.Play(atlas.GetAnimation("link-animation-down"));
                    }
                }
                if (Core.Input.Keyboard.WasKeyJustReleased(Keys.Up))
                {
                    _Link.facing = Player.direction.down;
                    _Link.Sprite.Play(atlas.GetAnimation("link-animation-down"));
                }
                if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Up))
                {
                    _Link.facing = Player.direction.up;
                    _Link.Sprite.Play(atlas.GetAnimation("link-animation-iddle-up"));
                    if (Core.Input.Keyboard.IsKeyDown(Keys.Left))
                    {
                        _Link.facing = Player.direction.left;
                        _Link.Sprite.Play(atlas.GetAnimation("link-animation-left"));
                    }
                    if (Core.Input.Keyboard.IsKeyDown(Keys.Right))
                    {
                        _Link.facing = Player.direction.right;
                        _Link.Sprite.Play(atlas.GetAnimation("link-animation-right"));
                    }
                }
                if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Left))
                {
                    _Link.facing = Player.direction.left;
                    if (Core.Input.Keyboard.IsKeyDown(Keys.Up))
                    {
                        _Link.facing = Player.direction.left;
                        _Link.Sprite.Play(atlas.GetAnimation("link-animation-left"));
                        if (Core.Input.Keyboard.IsKeyDown(Keys.Right))
                        {
                            _Link.facing = Player.direction.left;
                            _Link.Sprite.Play(atlas.GetAnimation("link-animation-iddle-left"));
                        }
                    }
                    else
                    {
                        _Link.facing = Player.direction.down;
                        _Link.Sprite.Play(atlas.GetAnimation("link-animation-down"));
                    }
                }
                if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Right))
                {
                    _Link.facing = Player.direction.right;
                    if (Core.Input.Keyboard.IsKeyDown(Keys.Up))
                    {
                        _Link.facing = Player.direction.right;
                        _Link.Sprite.Play(atlas.GetAnimation("link-animation-right"));
                        if (Core.Input.Keyboard.IsKeyDown(Keys.Left))
                        {
                            _Link.facing = Player.direction.right;
                            _Link.Sprite.Play(atlas.GetAnimation("link-animation-iddle-right"));
                        }
                    }
                    else
                    {
                        _Link.facing = Player.direction.down;
                        _Link.Sprite.Play(atlas.GetAnimation("link-animation-down"));
                    }
                }
                if (Core.Input.Keyboard.WasKeyJustReleased(Keys.Right) && Core.Input.Keyboard.IsKeyDown(Keys.Up))
                {
                    _Link.facing = Player.direction.right;
                    _Link.Sprite.Play(atlas.GetAnimation("link-animation-iddle-right"));
                }
                if (Core.Input.Keyboard.WasKeyJustReleased(Keys.Left) && Core.Input.Keyboard.IsKeyDown(Keys.Up))
                {
                    _Link.facing = Player.direction.left;
                    _Link.Sprite.Play(atlas.GetAnimation("link-animation-iddle-left"));
                }
                if (Core.Input.Keyboard.IsKeyDown(Keys.Up))
                {
                    if (Core.Input.Keyboard.WasKeyJustReleased(Keys.Right) && Core.Input.Keyboard.IsKeyDown(Keys.Left))
                    {
                        _Link.facing = Player.direction.left;
                        _Link.Sprite.Play(atlas.GetAnimation("link-animation-left"));
                    }
                    if (Core.Input.Keyboard.WasKeyJustReleased(Keys.Left) && Core.Input.Keyboard.IsKeyDown(Keys.Right))
                    {
                        _Link.facing = Player.direction.right;
                        _Link.Sprite.Play(atlas.GetAnimation("link-animation-right"));
                    }
                }

                _Link.Position.Y += _Link.MOVEMENT_SPEED;
            }
        }
    }
}
