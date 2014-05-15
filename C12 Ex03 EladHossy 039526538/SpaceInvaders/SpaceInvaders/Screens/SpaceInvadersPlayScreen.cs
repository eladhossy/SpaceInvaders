using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.ObjectModel.Screens;
using Microsoft.Xna.Framework;
using Infrastructure.ServiceInterfaces;
using Infrastructure.ObjectModel;
using Infrastructure.Managers;
using Microsoft.Xna.Framework.Audio;

namespace SpaceInvaders.Screens
{
    public class SpaceInvadersPlayScreen : GameScreen
    {
        private Dictionary<SpaceShip, Player> m_PlayersAndSpaceshipMapping;
        private InvadersArmy m_InvadersArmy;
        private MotherShip m_MotherShip;
        private BarrierGroupMgr m_Barriers;
        private SoulsBoard m_SoulsBoard;
        private ScoreBoard m_ScoreBoard;
        private TextWriter m_TextWriter;
        private int m_LevelNumber = 1;
        private List<Player> m_Players = null;
        private int m_SpaceshipCartrigeSize = 2;
       
        public int LevelNumber
        {
            set
            {
                m_LevelNumber = value;
            }
        }
        
        private bool m_IsLevelCircleStart = true;
        private int m_InvadersPerRow = 9;
        private bool m_IsBarriersMoving = false;
        private int m_InvaderAddedPoints = 0;
        private int m_InvadersNumOfBullets = 2;
        private float m_BarriersVelocity = 60f;
        private SoundBank m_SoundBank;
        private string m_SoundBankName;
        private IAudioManager m_AudioManager;

        public enum eGameMode
        {
            OnePlayer,
            TwoPlayers
        }

        private eGameMode m_GameMode = eGameMode.OnePlayer;
      
        public SpaceInvadersPlayScreen(Game i_Game, string i_SoundBankName, eGameMode i_GameMode)
            : base(i_Game)
        {
            m_GameMode = i_GameMode;
            InitSoundBank(i_SoundBankName);
            CreateComponents();
            addComponents();
        }

        private void InitSoundBank(string i_SoundBankName)
        {
            m_AudioManager = (IAudioManager)Game.Services.GetService(typeof(IAudioManager));
            m_SoundBank = m_AudioManager.GetSoundBank(i_SoundBankName);
            m_SoundBankName = i_SoundBankName;
        }

        public SpaceInvadersPlayScreen(Game i_Game, int i_LevelNumber, List<Player> i_Players, string i_SoundBankName, eGameMode i_GameMode)
            : base(i_Game)
        {
            m_LevelNumber = i_LevelNumber;
            m_IsLevelCircleStart = m_LevelNumber % 4 == 1;
            if (!m_IsLevelCircleStart)
            {
                int levelAddition = (m_LevelNumber - 1) % 4;
                m_IsBarriersMoving = true;
                m_InvadersPerRow += levelAddition;
                m_InvaderAddedPoints += 100 * levelAddition;
                m_BarriersVelocity *= 1 + (levelAddition * 0.1f);
                m_InvadersNumOfBullets += levelAddition;
            }

            m_Players = i_Players;
            m_SoundBankName = i_SoundBankName;
            m_GameMode = i_GameMode;
            InitSoundBank(i_SoundBankName);
            CreateComponents();
            addComponents();
        }

        private void CreateComponents()
        {
            // create the players
            Player player1 = null;
            Player player2 = null;
            m_PlayersAndSpaceshipMapping = new Dictionary<SpaceShip, Player>(); // hold a map 'connecting' between players and spaceships
            if (m_Players == null)
            {
                player1 = new Player() { Name = "P1", Color = Color.Blue };
                if (m_GameMode == eGameMode.TwoPlayers)
                {
                    player2 = new Player() { Name = "P2", Color = Color.Green };
                }
            }
            else
            {
                if (m_Players.Count == 2)
                {
                    player1 = m_Players[0];
                    player2 = m_Players[1];
                }
                else
                {
                    if (m_Players[0].Name == "P1")
                    {
                        player1 = m_Players[0];
                    }
                    else
                    {
                        player2 = m_Players[0];
                    }
                }
            }
            
            // create the spaceships
            SpaceShip SpaceShip1 = new SpaceShip(Game, @"Sprites\Ship01_32x32", this, m_SpaceshipCartrigeSize);
            SpaceShip1.SpaceShipCollided += new SpaceShipCollidedEventHandler(m_SpaceShip_SpaceShipCollided);
            SpaceShip1.SetSpaceshipKeys(
                Microsoft.Xna.Framework.Input.Keys.Right,
                Microsoft.Xna.Framework.Input.Keys.Left,
                Microsoft.Xna.Framework.Input.Keys.Enter);
            SpaceShip1.IsMouseControlled = true;
            SpaceShip1.SetShootSound(m_SoundBank, "spaceShipShoot");
            SpaceShip1.SetHitSound(m_SoundBank, "lifeDie");

            SpaceShip SpaceShip2 = new SpaceShip(Game, @"Sprites\Ship02_32x32", this, m_SpaceshipCartrigeSize);
            SpaceShip2.SpaceShipCollided += new SpaceShipCollidedEventHandler(m_SpaceShip_SpaceShipCollided);
            SpaceShip2.SetSpaceshipKeys(
                Microsoft.Xna.Framework.Input.Keys.D,
                Microsoft.Xna.Framework.Input.Keys.A,
                Microsoft.Xna.Framework.Input.Keys.Space);
            SpaceShip2.SetShootSound(m_SoundBank, "spaceShipShoot");
            SpaceShip2.SetHitSound(m_SoundBank, "lifeDie");

            if (player1 != null)
            {
                m_PlayersAndSpaceshipMapping.Add(SpaceShip1, player1);
            }

            if (player2 != null)
            {
                m_PlayersAndSpaceshipMapping.Add(SpaceShip2, player2);
            }
            
            // create the soulsBoard
            Dictionary<Player, string> playersAndIcons = new Dictionary<Player, string>();
            Player player;
            foreach (SpaceShip spaceShip in m_PlayersAndSpaceshipMapping.Keys.ToList<SpaceShip>())
            {
                player = m_PlayersAndSpaceshipMapping[spaceShip];
                playersAndIcons.Add(player, spaceShip.TextureName);
            }

            m_SoulsBoard = new SoulsBoard(Game, playersAndIcons, this);

            // create the score board
            m_ScoreBoard = new ScoreBoard(Game, m_PlayersAndSpaceshipMapping.Values.ToList<Player>());
            
            // create the invaders army:
            m_InvadersArmy = new InvadersArmy(
                Game, 
                this, 
                m_InvadersPerRow, 
                m_InvaderAddedPoints, 
                m_InvadersNumOfBullets,
                m_SoundBank, 
                "blueInvaderKill", 
                "pinkInvaderKill", 
                "yellowInvaderKill", 
                "invaderShoot");

            m_InvadersArmy.AllInvadersAreDead += new AllInvadersAreDeadEventHandler(spaceshipsWon);
            m_InvadersArmy.InvaderIsDead += new InvaderIsDeadEventHandler(enemyIsShooted);
            m_InvadersArmy.InvadersReachedBottom += new InvadersReachedBottomOfScreenEventHandler(invadersWon);

            // create the mothership
            m_MotherShip = new MotherShip(Game, Color.Red);
            m_MotherShip.MotherShipIsDead += new MotherShipIsDeadEventHandler(enemyIsShooted);
            m_MotherShip.SetHitSound(m_SoundBank, "motherShipKill");

            // create the barriers
            m_Barriers = new BarrierGroupMgr(Game, this, m_BarriersVelocity, m_SoundBank, "barrierHit");

            // create the level indicator
            m_TextWriter = new TextWriter(Game, @"Fonts\Calibri14");
            m_TextWriter.TextToWrite = "Level: " + m_LevelNumber;
        }

        private void addComponents()
        {
            foreach (SpaceShip spaceship in m_PlayersAndSpaceshipMapping.Keys.ToList<SpaceShip>())
                {
                    Add(spaceship);
                }

                Add(m_ScoreBoard);
                Add(m_SoulsBoard);
            Add(m_InvadersArmy);
            Add(m_MotherShip);
            Add(m_Barriers);
            Add(m_TextWriter);
        }
    
        public override void Initialize()
        {
            // Position the barriers
            base.Initialize();
            int spaceShipHeight = 32;
            int spaceShipY = GraphicsDevice.Viewport.Height - spaceShipHeight;
            float barriersCenterX = GraphicsDevice.Viewport.Width / 2;
            float barriersCenterY = spaceShipY - (spaceShipHeight * 2);
            m_Barriers.PositionCenterOfBarriersAt(new Vector2(barriersCenterX, barriersCenterY));
            m_Barriers.isMoving = m_IsBarriersMoving;
            
            // Position the Spaceships
            foreach (SpaceShip spaceShip in m_PlayersAndSpaceshipMapping.Keys)
            {
                spaceShip.StartingPosition = new Vector2(0, spaceShipY);
                spaceShip.Position = spaceShip.StartingPosition;
            }

            // Position the Invaders Army
            int invaderHeight = 32;
            m_InvadersArmy.Position = new Vector2(0, 3 * invaderHeight);

            // Position the Souls board
            m_SoulsBoard.Position = new Vector2(GraphicsDevice.Viewport.Width - 10, 10);
            m_SoulsBoard.locateIconsPositions();
            
            // Position the Level indicator
            m_TextWriter.Position = new Vector2(CenterOfViewPort.X, 0);

            base.Initialize();
        }

        private void m_SpaceShip_SpaceShipCollided(SpaceShip i_SpaceShip, ICollidable i_Collidable)
        {
            if (i_Collidable is IBullet)
            {
                spaceShipWasShot(i_SpaceShip);
            }
            else // the space ship collided an invader
            {
                invadersWon();
            }
        }

        private void m_MotherShip_MotherShipIsDead(MotherShip i_MotherShip, ICollidable i_Collidable)
        {
            SpaceShip shooterSpaceShip = null;
            Player shooterPlayer;
            if (i_Collidable is IBullet)
            {
                shooterSpaceShip = (i_Collidable as IBullet).Gun.GunHolder as SpaceShip;
            }

            shooterPlayer = m_PlayersAndSpaceshipMapping[shooterSpaceShip];
            shooterPlayer.Scores += i_MotherShip.PointsForDestruction;
        }

        private void spaceShipWasShot(SpaceShip i_SpaceShip)
        {
            Player spaceShipPlayer = m_PlayersAndSpaceshipMapping[i_SpaceShip];
            spaceShipPlayer.KillSoul();
            if (spaceShipPlayer.Souls == 0)
            {
                m_PlayersAndSpaceshipMapping.Remove(i_SpaceShip);
                Remove(i_SpaceShip);
                i_SpaceShip.Dispose();
            }

            if (m_PlayersAndSpaceshipMapping.Count == 0)
            {
                invadersWon();
            }
        }

        private void enemyIsShooted(Sprite i_EnemyShooted, ICollidable i_Collidable)
        {
            SpaceShip shooterSpaceShip = null;
            Player shooterPlayer;
            if (i_Collidable is IBullet)
            {
                shooterSpaceShip = (i_Collidable as IBullet).Gun.GunHolder as SpaceShip;
            }

            if (m_PlayersAndSpaceshipMapping.ContainsKey(shooterSpaceShip)) // if the spaceship is not removed while the bullet was it the air
            {
                shooterPlayer = m_PlayersAndSpaceshipMapping[shooterSpaceShip];

                if (i_EnemyShooted is MotherShip)
                {
                    shooterPlayer.Scores += (i_EnemyShooted as MotherShip).PointsForDestruction;
                }
                else
                {
                    shooterPlayer.Scores += (i_EnemyShooted as Invader).PointsForDestruction;
                }
            }
        }

        private void invadersWon()
        {
            GameOver();
        }

        private void spaceshipsWon()
        {
            LevelOver();
        }

        private void LevelOver()
        {
            m_SoundBank.PlayCue("levelWin");
            Game.Services.RemoveService(typeof(ICollisionManager));
            new CollisionManager(Game);
            m_Barriers.Dispose();
            List<Player> players = m_PlayersAndSpaceshipMapping.Values.ToList<Player>();
            ScreensManager.SetCurrentScreen(new SpaceInvadersPlayScreen(Game, ++m_LevelNumber, players, m_SoundBankName, m_GameMode));
            ScreensManager.SetCurrentScreen(new LevelTransitionScreen(Game, m_LevelNumber, m_SoundBankName));
        }
        
        private void GameOver()
        {
            m_SoundBank.PlayCue("gameOver");
            Game.Services.RemoveService(typeof(ICollisionManager));
            new CollisionManager(Game);
            m_Barriers.Dispose();
            List<Player> players = m_PlayersAndSpaceshipMapping.Values.ToList<Player>();
            
            ScreensManager.SetCurrentScreen(new SpaceInvadersPlayScreen(Game, m_SoundBankName, m_GameMode));
            ScreensManager.SetCurrentScreen(new LevelTransitionScreen(Game, 1, m_SoundBankName));
            ScreensManager.SetCurrentScreen(new GameOverScreen(Game, m_SoundBankName, m_ScoreBoard));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (InputManager.KeyPressed(Microsoft.Xna.Framework.Input.Keys.P))
            {
                ScreensManager.SetCurrentScreen(new PauseScreen(Game));
            }
        }
    }
}
