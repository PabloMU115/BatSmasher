using BatSmasher.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGameGum;
using MGLibrary;
using MGLibrary.Factories;
using MGLibrary.GameObjects;
using MGLibrary.Graphics;
using MGLibrary.Player;
using MGLibrary.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BatSmasher.Scenes 
{
    public class GameScene : Scene
    {
        #region Attributes
        private ActorFactory _ActorFactory;

        private ActorFactory _ObjectFactory;

        private TextureAtlas atlas;

        private TextureAtlas effects;

        private TextureAtlas attack;

        TimeSpan _elapsed = new TimeSpan();

        TimeSpan _deathSpan = new TimeSpan();

        private int currentEnemies = 1;

        List<Enemy> enemyList = Enumerable.Range(0, 20)
            .Select(_ => new Enemy())
            .ToList();

        // Defines the Actor/Items to be used
        private Player _Link = new Player();
        private Item _Sword = new Item();
        private Item _Heart = new Item();
        private Item _SwordDrop = new Item();
        private Item _Shadow = new Item();

        private List<AnimatedSprite> _heartContainers = Enumerable.Range(0, 3)
            .Select(_ => new AnimatedSprite())
            .ToList();

        private enum GameState
        {
            Playing,
            Paused,
            GameOver
        }

        // Defines the tilemap to draw.
        private Tilemap _tilemap;

        // Defines the bounds of the room that link and bat are contained within.
        private Rectangle _roomBounds;

        // The SpriteFont Description used to draw text.
        private SpriteFont _font;

        private GameSceneUI _ui;

        private GameState _state;

        // Defines the position to draw the score text at.
        private Vector2 _heartContainerPosition;

        // Reference to the texture atlas that we can pass to UI elements when they
        // are created.
        private TextureAtlas _UIAtlas;

        private PlayerInput _PlayerInput = new PlayerInput();
        #endregion
        
        public override void Initialize()
        {
            // LoadContent is called during base.Initialize().
            base.Initialize();

            // During the game scene, we want to disable exit on escape. Instead,
            // the escape key will be used to return back to the title screen
            Core.ExitOnEscape = false;

            _Sword.loadOffsets();

            Rectangle screenBounds = Core.GraphicsDevice.PresentationParameters.Bounds;

            _roomBounds = new Rectangle(
                 (int)_tilemap.TileWidth,
                 (int)_tilemap.TileHeight,
                 screenBounds.Width - (int)_tilemap.TileWidth * 2,
                 screenBounds.Height - (int)_tilemap.TileHeight * 2
             );

            foreach (var enemy in enemyList)
            {
                enemy.randomBatPosition(_roomBounds);
            }

            // Set the position of the score text to align to the left edge of the
            // room bounds, and to vertically be at the center of the first tile.
            _heartContainerPosition = new Vector2(_roomBounds.Left, _tilemap.TileHeight * 0.5f);

            // Create any UI elements from the root element created in previous
            // scenes.
            GumService.Default.Root.Children.Clear();

            // Initialize the user interface for the game scene.
            InitializeUI();

            // Initialize a new game to be played.
            InitializeNewGame();
        }

        public override void LoadContent()
        {
            // Loading a SpriteFont Description using the content pipeline
            _font = Content.Load<SpriteFont>("fonts/04B_30");

            #region Load the sounds we want to manipulate and add them to their corresponding entity
            _Link.sfxList.Add(Content.Load<SoundEffect>("audio/MC_Link_Sword").CreateInstance());
            _Link.sfxList.Add(Content.Load<SoundEffect>("audio/MC_Link_Sword1").CreateInstance());
            _Link.sfxList.Add(Content.Load<SoundEffect>("audio/MC_Link_Hurt").CreateInstance());
            _Heart.sfx = Content.Load<SoundEffect>("audio/MMX - X Extra Life").CreateInstance();
            _Link.sfxList.Add(Content.Load<SoundEffect>("audio/MC_Link_Fall").CreateInstance());
            _Link.sfxList.Add(Content.Load<SoundEffect>("audio/MC_Link_Sword2").CreateInstance());
            _Link.sfxList.Add(Content.Load<SoundEffect>("audio/MC_Link_Roll").CreateInstance());
            _Shadow.sfx = Content.Load<SoundEffect>("audio/TP_PressStart").CreateInstance();
            _Link.sfxList.Add(Content.Load<SoundEffect>("audio/MC_Link_Magic_Sword").CreateInstance());
            #endregion

            _Link.sfxList[(int)Player.sfx.magicSword].Volume = 0.6f;

            // Adjust the properties of the instance as needed
            _Link.sfxList[(int)Player.sfx.hurt].Volume = 0.9f;      // Set half volume.
            _Link.sfxList[(int)Player.sfx.linkDeath].Volume = 0.7f;

            // Create the texture atlas from the XML configuration file
            _UIAtlas = TextureAtlas.FromFile(Core.Content, "images/atlas-definition-2.xml");

            // Create the texture atlas from the XML configuration file
            atlas = TextureAtlas.FromFile(Content, "images/atlas-definition.xml");
            effects = TextureAtlas.FromFile(Content, "images/Effect-definition.xml");
            attack = TextureAtlas.FromFile(Content, "images/slash-definition.xml");

            // Instancia fábrica de la LIBRERÍA
            _ActorFactory = new ActorFactory(atlas);
            _ObjectFactory = new ActorFactory(attack);

            // Create the tilemap from the XML configuration file.
            _tilemap = Tilemap.FromFile(Content, "images/tilemap-definition.xml");
            _tilemap.Scale = new Vector2(4.0f, 4.0f);

            // Create link's animated sprite from the atlas.
            _Link.LoadActor(_ActorFactory, _tilemap, "link-animation-iddle-down");
            _Shadow.LoadObject(_ActorFactory, "empty-sprite", Vector2.Zero, 10f);
            _Sword.LoadObject(_ObjectFactory, "no-slash", _Link.Position, 4f);

            // Create the bat animated sprite from the atlas.
            for (int i = 0; i < enemyList.Count; i++)
            {
                enemyList[i].Sound = Content.Load<SoundEffect>("audio/MMX - Enemy Die").CreateInstance();
                enemyList[i].Sprite = _ActorFactory.CreateSprite("bat-animation", scale: 3f);
            }

            for (int i = 0; i < _heartContainers.Count; i++)
            {
                _heartContainers[i] = _ActorFactory.CreateSprite("full-heart-container", scale: 3f);
            }

            _Heart.LoadObject(_ActorFactory, "empty-sprite", Vector2.Zero, 4f);
            _SwordDrop.LoadObject(_ActorFactory,"empty-sprite", Vector2.Zero, 4f);
        }

        public override void Update(GameTime gameTime)
        {
            // Ensure the UI is always updated.
            _ui.Update(gameTime);

            // If the game is in a game over state, immediately return back
            // here.
            if (_state == GameState.GameOver)
            {
                return;
            }

            // If the pause button is pressed, toggle the pause state.
            if (GameController.Pause())
            {
                TogglePause();
            }

            // At this point, if the game is paused, just return back early.
            if (_state == GameState.Paused)
            {
                return;
            }

            for (int i = 0; i < currentEnemies; i++)
            {
                enemyList[i].Sprite.Update(gameTime);
                enemyList[i].assignBatBounds(_roomBounds);
            }

            _Heart.Sprite.Update(gameTime);
            _SwordDrop.Sprite.Update(gameTime);

            // Creating a bounding circle for link
            _Link.Bounds = _ActorFactory.assignBounds(_Link.Sprite, _Link.Position, 0.5f, 0.5f);

            _Sword.UpdateHitBox(_ObjectFactory, _Sword.Position, 0.3f, 0.5f);

            _Link.KeepInBounds(_roomBounds);

            _Link.Sprite.Update(gameTime);
            _Sword.Sprite.Update(gameTime);

            _elapsed += gameTime.ElapsedGameTime;

            if (_elapsed >= _Link.AnimationSpan - _Link.Sprite.Animation.Delay * 2 && 
                _Link.AnimationSpan.TotalMilliseconds != 0 && 
                _Link.facing != Player.direction.powerUp)
            {
                _Sword.Sprite.Play(attack.GetAnimation("no-slash"));
            }

            #region Bat Move and collision check
            for (int i = 0; i < currentEnemies; i++)
            {
                enemyList[i].resetBatAnimation(_elapsed, atlas, _tilemap);
                if ((_Sword.Hitbox.Intersects(enemyList[i].Bounds) || _Shadow.Hitbox.Intersects(enemyList[i].Bounds)) &&
                    _Link.AnimationSpan.TotalMilliseconds != 0 &&
                    _Sword.Sprite.Animation != attack.GetAnimation("no-slash"))
                {
                    enemyList[i].Bounds = Circle.Empty;

                    enemyList[i].Velocity *= 0;

                    enemyList[i].Sprite.Play(effects.GetAnimation("bat-death-animation"));

                    _Link.score += 100;

                    if (_Link.score % 500 == 0 && currentEnemies < enemyList.Count)
                    {
                        currentEnemies++;
                    }

                    var rng = Random.Shared.Next(29, 46);

                    if (rng >= 30 && rng <= 40 && _Heart.AnimationSpan.TotalSeconds == 0 && _SwordDrop.AnimationSpan.TotalSeconds == 0)
                    {
                        _Heart.Sprite.Play(atlas.GetAnimation("heart-spinning-animation"));
                        _Heart.Position.X = enemyList[i].Position.X + 10;
                        _Heart.Position.Y = enemyList[i].Position.Y + 10;
                        TimeSpan time = new TimeSpan(0, 0, 5);
                        _Heart.AnimationSpan = _elapsed + time;
                    }
                    if (rng >= 41 && rng <= 45 && _Heart.AnimationSpan.TotalSeconds == 0 && _SwordDrop.AnimationSpan.TotalSeconds == 0)
                    {
                        _SwordDrop.Sprite.Play(atlas.GetAnimation("sword-flow-animation"));
                        _SwordDrop.Position.X = enemyList[i].Position.X + 10;
                        _SwordDrop.Position.Y = enemyList[i].Position.Y + 10;
                        TimeSpan time = new TimeSpan(0, 0, 5);
                        _SwordDrop.AnimationSpan = _elapsed + time;
                    }

                    if (enemyList[i].Sound.State == SoundState.Playing)
                    {
                        enemyList[i].Sound.Stop();
                    }
                    enemyList[i].Sound.Play();

                    enemyList[i].Span = _elapsed + enemyList[i].Sprite.Animation.Delay * 6;
                }
            }
            #endregion

            // Check for keyboard Core.Input and handle it.
            if (_Link.AnimationSpan.TotalMilliseconds == 0 && _Link.totalLives > 0)
            {
                _PlayerInput.CheckKeyboardInput(_elapsed, _Link, _Sword, atlas, attack);
            }

            _PlayerInput.roll(_elapsed, _Link);

            if (_elapsed > _Link.AnimationSpan && _elapsed > _SwordDrop.PowerUpSpan &&
                _Link.facing == Player.direction.powerUp)
            {
                _Sword.Sprite.Play(attack.GetAnimation("no-slash"));
                _SwordDrop.PowerUpSpan = new TimeSpan();
                _Link.facing = Player.direction.down;
            }

            _Link.changeLinkFacingAnimation(_elapsed, atlas, _Shadow, _SwordDrop, _Sword, _Link.totalLives);

            if (_elapsed >= _Heart.AnimationSpan) 
            {
                _Heart.Position = Vector2.Zero;
                _Heart.Sprite.Play(atlas.GetAnimation("empty-sprite"));
                _Heart.AnimationSpan = new TimeSpan();
            }

            if (_elapsed >= _SwordDrop.AnimationSpan)
            {
                _SwordDrop.Position = Vector2.Zero;
                _SwordDrop.Sprite.Play(atlas.GetAnimation("empty-sprite"));
                _SwordDrop.AnimationSpan = new TimeSpan();
            }

            for (int i = 0; i < currentEnemies; i++)
            {
                enemyList[i].Hitbox = _ActorFactory.assignBounds(enemyList[i].Sprite.Animation.Frames[0], new Vector2(enemyList[i].Bounds.X, enemyList[i].Bounds.Y), 0.5f, 0.5f);
            }

            //Hitbox update based on every object position
            _Link.UpdateHitBox(_ActorFactory, new Vector2(_Link.Bounds.X, _Link.Bounds.Y + 20 * 2), 0.4f, 99);

            _Shadow.UpdateHitBox(_ActorFactory, _Shadow.Position, 0.5f, 0.5f);

            _Heart.UpdateHitBox(_ActorFactory, new Vector2(_Heart.Position.X, _Heart.Position.Y), 0.5f, 0.5f);

            _SwordDrop.UpdateHitBox(_ActorFactory, new Vector2(_SwordDrop.Position.X, _SwordDrop.Position.Y), 0.5f, 0.5f);

            //Reset Links IFrames
            if (_elapsed.TotalSeconds >= _Link.InvincibilitySpan.TotalSeconds)
            {
                _Link.InvincibilitySpan = new TimeSpan();
            }

            if (_elapsed.TotalSeconds >= _Link.RollSpan.TotalSeconds)
            {
                _Link.RollSpan = new TimeSpan();
            }

            //Checks if a bat collided with Link
            for (int i = 0; i < currentEnemies; i++)
            {
                enemyList[i].checkLinkCollision(_elapsed, _Link, _heartContainers, atlas);
            }

            //Check if Link grabs a heart
            if (_Link.Hitbox.Intersects(_Heart.Hitbox))
            {
                if (_Heart.sfx.State == SoundState.Playing)
                {
                    _Heart.sfx.Stop();
                }
                _Heart.sfx.Play();
                _Heart.Position = Vector2.Zero;
                _Heart.Sprite.Play(atlas.GetAnimation("empty-sprite"));
                if (_Link.totalLives == 3)
                {
                    _Link.score += 500;
                    if (currentEnemies < enemyList.Count)
                    {
                        currentEnemies++;
                    }

                }
                if (_Link.totalLives == 2) 
                {
                    _heartContainers[2].Play(atlas.GetAnimation("full-heart-container"));
                    _Link.totalLives++;
                }
                if (_Link.totalLives == 1)
                {
                    _heartContainers[1].Play(atlas.GetAnimation("full-heart-container"));
                    _Link.totalLives++;
                }
                if (_Link.totalLives == 0)
                {
                    _heartContainers[0].Play(atlas.GetAnimation("full-heart-container"));
                    _Link.totalLives++;
                }
            }

            //Check if Link grabs a sword
            if (_Link.Hitbox.Intersects(_SwordDrop.Hitbox))
            {
                if (_Link.sfxList[(int)Player.sfx.magicSword].State == SoundState.Playing)
                {
                    _Link.sfxList[(int)Player.sfx.magicSword].Stop();
                }
                _Link.sfxList[(int)Player.sfx.magicSword].Play();
                _SwordDrop.Position = Vector2.Zero;
                _SwordDrop.Sprite.Play(atlas.GetAnimation("empty-sprite"));
                checkSwordPowerUp();
            }

            _ui.UpdateScoreText(_Link.score);

            //Checks if Link is out of lives
            if (_Link.totalLives == 0) 
            {
                if (_deathSpan.TotalMilliseconds == 0) 
                {
                    if (_Link.sfxList[(int)Player.sfx.linkDeath].State == SoundState.Playing)
                    {
                        _Link.sfxList[(int)Player.sfx.linkDeath].Stop();
                    }
                    _Link.sfxList[(int)Player.sfx.linkDeath].Play();
                    _Link.Sprite.Play(atlas.GetAnimation("link-death-animation"));
                    _deathSpan = _elapsed + _Link.Sprite.Animation.Delay * 5;
                }
            }

            if (_elapsed > _deathSpan && _deathSpan.TotalMilliseconds != 0)
            {
                _Link.Sprite.Play(atlas.GetAnimation("link-death-animation-iddle"));
                GameOver();
            }
        }

        #region Event Handlers
        private void TogglePause()
        {
            if (_state == GameState.Paused)
            {
                // We're now unpausing the game, so hide the pause panel.
                _ui.HidePausePanel();

                // And set the state back to playing.
                _state = GameState.Playing;
            }
            else
            {
                // We're now pausing the game, so show the pause panel.
                _ui.ShowPausePanel();

                // And set the state to paused.
                _state = GameState.Paused;
            }
        }

        private void GameOver()
        {
            // Show the game over panel.
            _ui.ShowGameOverPanel();

            // Set the game state to game over.
            _state = GameState.GameOver;
        }

        public void checkSwordPowerUp()
        {
            _Link.facing = Player.direction.powerUp;

            _Link.Sprite.Play(atlas.GetAnimation("link-power-up-animation"));

            _Sword.Sprite.Play(atlas.GetAnimation("sword-power-up-animation"));

            _Shadow.Sprite.Play(atlas.GetAnimation("sword-power-up-shadow"));

            _Link.InvincibilitySpan = _elapsed + new TimeSpan(0, 0, 3);

            _Link.AnimationSpan = _elapsed + _Link.Sprite.Animation.Delay * 3;

            _SwordDrop.PowerUpSpan = _elapsed + new TimeSpan(0, 0, 3);

            _Shadow.Position = new Vector2(_Link.Position.X - _Link.Sprite.Width * 0.9f, _Link.Position.Y - _Link.Sprite.Width * 0.5f);
            
            _Link.MOVEMENT_SPEED = 0f;

            if (_Shadow.sfx.State == SoundState.Playing)
            {
                _Shadow.sfx.Stop();
            }
            _Shadow.sfx.Play();
        }

        private void InitializeUI()
        {
            // Clear out any previous UI element incase we came here
            // from a different scene.
            GumService.Default.Root.Children.Clear();

            // Create the game scene ui instance.
            _ui = new GameSceneUI();

            // Subscribe to the events from the game scene ui.
            _ui.ResumeButtonClick += OnResumeButtonClicked;
            _ui.RetryButtonClick += OnRetryButtonClicked;
            _ui.QuitButtonClick += OnQuitButtonClicked;
        }

        private void OnResumeButtonClicked(object sender, EventArgs args)
        {
            // Change the game state back to playing.
            _state = GameState.Playing;
        }

        private void OnRetryButtonClicked(object sender, EventArgs args)
        {
            InitializeNewGame();
        }

        private void OnQuitButtonClicked(object sender, EventArgs args)
        {
            // Player has chosen to quit, so return back to the title scene.
            Core.ChangeScene(new TitleScene());
        }

        private void InitializeNewGame()
        {
            _Link.Position = _ActorFactory.CenterOfTilemap(_tilemap);
            _Heart.Position = Vector2.Zero;
            _SwordDrop.Position = Vector2.Zero;
            _Heart.Sprite.Play(atlas.GetAnimation("empty-sprite"));
            _SwordDrop.Sprite.Play(atlas.GetAnimation("empty-sprite"));
            _Sword.Sprite.Play(attack.GetAnimation("no-slash"));

            int column = Random.Shared.Next(1, _tilemap.Columns - 1);
            int row = Random.Shared.Next(1, _tilemap.Rows - 1);

            //Reset the amount of totalEnemies back to one
            currentEnemies = 1;

            //Reset the position, speed, animation and timespan of every bat
            foreach (var enemy in enemyList)
            {
                enemy.Speed = 0.5f;
                enemy.Sprite.Play(atlas.GetAnimation("bat-animation"));
                enemy.Span = new TimeSpan();
                enemy.randomBatPosition(_roomBounds);
            }

            // Reset the score.
            _Link.score = 0;
            _ui.UpdateScoreText(_Link.score);

            //Reset the amount of lives
            _Link.totalLives = 3;

            //Reset the sword sprite
            _Sword.Sprite.Play(attack.GetAnimation("no-slash"));

            //Reset Links sprite and facind Actor.direction
            _Link.Sprite.Play(atlas.GetAnimation("link-animation-iddle-down"));
            _Link.facing = Player.direction.down;

            //Reset the heart containers
            for (int i = 0; i < _heartContainers.Count; i++)
            {
                _heartContainers[i].Play(atlas.GetAnimation("full-heart-container"));
            }

            //Reset the movement speed
            _Link.MOVEMENT_SPEED = 5.0f;

            //Reset the various timespans
            _elapsed = new TimeSpan();
            _Link.AnimationSpan = new TimeSpan();
            _Link.InvincibilitySpan = new TimeSpan();
            _Heart.AnimationSpan = new TimeSpan();
            _SwordDrop.AnimationSpan = new TimeSpan();
            _deathSpan = new TimeSpan();

            // Set the game state to playing.
            _state = GameState.Playing;
        }
        #endregion

        public override void Draw(GameTime gameTime)
        {
            // Clear the back buffer.
            Core.GraphicsDevice.Clear(Color.CornflowerBlue);

            // Begin the sprite batch to prepare for rendering.
            Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

            // Draw the tilemap.
            _tilemap.Draw(Core.SpriteBatch);

            // Draw link's sprite.
            int i = _Link.Sprite.CurrentFrameIndex;
            Vector2 off = Vector2.Zero;

            off = (i >= 0 && i < _Sword.SwordOffsets[(int)_Link.facing].Length) ? _Sword.SwordOffsets[(int)_Link.facing][i] : Vector2.Zero;

            // Synchronize the sword srpite with links movements
            if (_Link.Sprite.Animation == atlas.GetAnimation("link-power-up-animation-still"))
            {
                _Link.Sprite.Draw(Core.SpriteBatch, _Link.Position, Color.White);
                _Sword.Sprite.Draw(Core.SpriteBatch, _Sword.Position, Color.White);
                _Sword.Position = _Link.Position + _Sword.SwordOffsets[(int)_Link.facing][2];
            }
            else
            {
                _Sword.Position = _Link.Position + off;
            }

            float Speed = 9f;
            float t = (float)gameTime.TotalGameTime.TotalSeconds;
            float alpha = 0.65f * 0.5f * (float)(Math.Sin(t * Speed) + 1.0) + 0.35f;

            // if a heart/sword that was spawned has been present 
            // in the gameboard for more than 2 seconds,
            // it starts blinking white for 2 more
            // seconds before dissapearing
            if (_Heart.AnimationSpan.TotalSeconds-_elapsed.TotalSeconds <= 2.0)
            {
                _Heart.Sprite.Draw(Core.SpriteBatch, _Heart.Position, Color.White * alpha);
            }
            else 
            {
                _Heart.Sprite.Draw(Core.SpriteBatch, _Heart.Position, Color.White);
            }

            if (_SwordDrop.AnimationSpan.TotalSeconds - _elapsed.TotalSeconds <= 2.0)
            {
                _SwordDrop.Sprite.Draw(Core.SpriteBatch, _SwordDrop.Position, Color.White * alpha);
            }
            else
            {
                _SwordDrop.Sprite.Draw(Core.SpriteBatch, _SwordDrop.Position, Color.White);
            }

            _Shadow.Sprite.Draw(Core.SpriteBatch, _Shadow.Position, Color.White);

            if (_Link.Sprite.Animation == atlas.GetAnimation("link-power-up-animation") || _Link.Sprite.Animation == atlas.GetAnimation("link-power-up-animation-still"))
            {
                //draws links sprite
                _Link.Sprite.Draw(Core.SpriteBatch, _Link.Position, Color.White);

                //draws the sword sprite
                _Sword.Sprite.Draw(Core.SpriteBatch, _Sword.Position, Color.White);
            }
            else
            {
                //draws the sword sprite
                _Sword.Sprite.Draw(Core.SpriteBatch, _Sword.Position, Color.White);

                //draws links sprite
                _Link.Sprite.Draw(Core.SpriteBatch, _Link.Position, Color.White);
            }

            //After link gets hit he starts blinking red while he has "Invincibility frames"
            if (_Link.InvincibilitySpan.TotalSeconds != 0 && _Link.totalLives > 0 && _Link.AnimationSpan.TotalMilliseconds == 0) 
            {
                _Link.Sprite.Draw(Core.SpriteBatch, _Link.Position, Color.Red * alpha);
            }

            // Draw the bat sprite.
            for (int j = 0; j < currentEnemies; j++)
            {
                enemyList[j].Sprite.Draw(Core.SpriteBatch, enemyList[j].Position, Color.White);
            }

            _heartContainers[0].Draw(Core.SpriteBatch, new Vector2(_heartContainerPosition.X + 470f, _heartContainerPosition.Y - 25f), Color.White);
            _heartContainers[1].Draw(Core.SpriteBatch, new Vector2(_heartContainerPosition.X + 530f, _heartContainerPosition.Y - 25f), Color.White);
            _heartContainers[2].Draw(Core.SpriteBatch, new Vector2(_heartContainerPosition.X + 590f, _heartContainerPosition.Y - 25f), Color.White);

            // Always end the sprite batch when finished.
            Core.SpriteBatch.End();

            // Draw the UI.
            _ui.Draw();
        }

    }
}