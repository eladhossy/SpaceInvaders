using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Infrastructure.ServiceInterfaces;
using Infrastructure.ObjectModel.Screens;

namespace SpaceInvaders
{
    public delegate void AllInvadersAreDeadEventHandler();
    
    public delegate void InvadersReachedBottomOfScreenEventHandler();

    public class InvadersArmy : Microsoft.Xna.Framework.GameComponent
    {
        public event AllInvadersAreDeadEventHandler AllInvadersAreDead;
        
        public event InvaderIsDeadEventHandler InvaderIsDead;
        
        public event InvadersReachedBottomOfScreenEventHandler InvadersReachedBottom;
        
        private float m_VerticalSpace;
        private float m_HorizontalSpace;
        private float m_VerticalSpaceFactor = 0.6f;
        private float m_HorizonalSpaceFactor = 0.6f;
        private int m_InvaderWidth;
        private int m_InvaderHeight;
        private float m_matrixJumpSpeed;
        private List<List<Invader>> m_Invaders;
        private Dictionary<string, InvaderDescription> m_InvaderTypes;
        private Vector2 m_Position;
        private double m_timeToJump;
        private double m_timeBetweenJumps = 0.5;
        private bool startGame = true;
        private int m_NumOfLivingInvaders = 0;
        GameScreen m_ContainingScreen;
        private int m_InvaderNumOfBullets;
        private SoundBank m_SoundBank;
        private string m_BlueInvaderCueName;
        private string m_PinkInvaderCueName;
        private string m_YellowInvaderCueName;
        private string m_InvaderShootCueName;
       
        public InvadersArmy(Game i_Game, GameScreen i_GameScreen, int i_InvadersPerRow, int i_InvaderAddedPoints, int i_InvaderNumOfBullets,
            SoundBank i_SoundBank, string i_BlueInvaderCueName, string i_PinkInvaderCueName, string i_YellowInvaderCueName, string i_InvaderShootCueName)
            : base(i_Game)
        {
            m_InvaderNumOfBullets = i_InvaderNumOfBullets;
            m_InvaderAddedPoints = i_InvaderAddedPoints;
            m_InvadersPerRow = i_InvadersPerRow;
            m_ContainingScreen = i_GameScreen;
            m_Invaders = new List<List<Invader>>(); // 'holds' all the invaders
            m_SoundBank = i_SoundBank;
            m_BlueInvaderCueName = i_BlueInvaderCueName;
            m_PinkInvaderCueName = i_PinkInvaderCueName;
            m_YellowInvaderCueName = i_YellowInvaderCueName;
            m_InvaderShootCueName = i_InvaderShootCueName;

            // in ordedr to enable 'safe' creation of invaders according to our specific army
            // we hold the definitions for 'legal' invader types, in a dictionary:
            m_InvaderTypes = new Dictionary<string, InvaderDescription>();
            m_InvaderTypes.Add("PINK", new InvaderDescription()
            {
                InvaderColor = Color.LightPink,
                InvaderFirstSourceRectangle = 0, // each invader has 2 pictures that swap, hence first, second source rec
                InvaderSecondSourceRectangle = 1,
                InvaderDestructionPoints = 180,
                InvaderHitCueName = m_PinkInvaderCueName,
                InvaderShootCueName = m_InvaderShootCueName
            });

            // 2 kinds of 'blue' invader, and 2 kinds of 'yellow', since each row starts with a different texture
            m_InvaderTypes.Add("BLUE1", new InvaderDescription() 
            {
                InvaderColor = Color.LightBlue,
                InvaderFirstSourceRectangle = 2,
                InvaderSecondSourceRectangle = 3,
                InvaderDestructionPoints = 170,
                InvaderHitCueName = m_BlueInvaderCueName,
                InvaderShootCueName = m_InvaderShootCueName
            });

            m_InvaderTypes.Add("BLUE2", new InvaderDescription()
            {
                InvaderColor = Color.LightBlue,
                InvaderFirstSourceRectangle = 3,
                InvaderSecondSourceRectangle = 2,
                InvaderDestructionPoints = 170,
                InvaderHitCueName = m_BlueInvaderCueName,
                InvaderShootCueName = m_InvaderShootCueName
            });

            m_InvaderTypes.Add("YELLOW1", new InvaderDescription()
            {
                InvaderColor = Color.LightYellow,
                InvaderFirstSourceRectangle = 4,
                InvaderSecondSourceRectangle = 5,
                InvaderDestructionPoints = 150,
                InvaderHitCueName = m_YellowInvaderCueName,
                InvaderShootCueName = m_InvaderShootCueName
            });

            m_InvaderTypes.Add("YELLOW2", new InvaderDescription()
            {
                InvaderColor = Color.LightYellow,
                InvaderFirstSourceRectangle = 5,
                InvaderSecondSourceRectangle = 4,
                InvaderDestructionPoints = 150,
                InvaderHitCueName = m_YellowInvaderCueName,
                InvaderShootCueName = m_InvaderShootCueName
            });

            // creating the army's rows:
            m_Invaders.Add(InitInvadersRow(m_InvaderTypes["PINK"]));
            m_Invaders.Add(InitInvadersRow(m_InvaderTypes["BLUE1"]));
            m_Invaders.Add(InitInvadersRow(m_InvaderTypes["BLUE2"]));
            m_Invaders.Add(InitInvadersRow(m_InvaderTypes["YELLOW1"]));
            m_Invaders.Add(InitInvadersRow(m_InvaderTypes["YELLOW2"]));
        }

        public int SizeOfArmy 
        {
            get { return calculateArmySize(); }
        }
        
        public Vector2 Position
        {
            get { return m_Position; }
            set { m_Position = value; }
        }
        
        public float Width { get; set; }
        
        public float Height { get; set; }
        
        private bool m_HasRaisedEvent = false;
        private int m_InvadersPerRow;
        private int m_InvaderAddedPoints;
       
        private class InvaderDescription  // help us to encapsulate the definitions for an invader's type:
        {
           public Color InvaderColor { get; set; }
           
           public int InvaderDestructionPoints { get; set; }
           
           public int InvaderFirstSourceRectangle { get; set; }
           
           public int InvaderSecondSourceRectangle { get; set; }

           public string InvaderHitCueName { get; set; }

           public string InvaderShootCueName { get; set; }
        }

        private List<Invader> InitInvadersRow(InvaderDescription i_InvaderDescription)
        {
            List<Invader> retRow = new List<Invader>();
            Color invaderColor = i_InvaderDescription.InvaderColor;
            int invaderPoints = i_InvaderDescription.InvaderDestructionPoints;
            int invaderFirstSourceRectangle = i_InvaderDescription.InvaderFirstSourceRectangle;
            string invaderHitCueName = i_InvaderDescription.InvaderHitCueName;
            string invaderShootCueName = i_InvaderDescription.InvaderShootCueName;
            int invaderSecondSourceRectangle = i_InvaderDescription.InvaderSecondSourceRectangle;
            for (int i = 0; i < m_InvadersPerRow; i++)
            {
                Invader newInvader = new Invader(this.Game, invaderColor, invaderPoints, invaderFirstSourceRectangle, invaderSecondSourceRectangle, m_ContainingScreen, m_InvaderNumOfBullets);
                newInvader.SetHitSound(m_SoundBank, invaderHitCueName);
                newInvader.SetShootSound(m_SoundBank, invaderShootCueName);
                newInvader.InvaderIsDead += new InvaderIsDeadEventHandler(invader_InvaderIsDead);
                newInvader.InvaderReachedBottomOfScreen += new InvaderReachedBottomOfScreenEventHandler(invader_InvaderReachedBottomOfScreen);
                retRow.Add(newInvader);
                m_NumOfLivingInvaders++;
                m_ContainingScreen.Add(newInvader);
                newInvader.PointsForDestruction += m_InvaderAddedPoints;
            }

            return retRow;
        }

        private void invader_InvaderReachedBottomOfScreen()
        {
            if (InvadersReachedBottom != null && m_HasRaisedEvent == false)
            {
                InvadersReachedBottom.Invoke();
                m_HasRaisedEvent = true;
            }
        }   
            
        public override void Initialize()
        {
            base.Initialize();
            initMatrixBounds();
        }

        public void initMatrixBounds()
        {
            Invader invader = m_Invaders[0][0]; // to get the info about the invader width and height
            m_InvaderWidth = (int)invader.Width;
            m_InvaderHeight = (int)invader.Height;
            m_matrixJumpSpeed = m_InvaderWidth / 2f;
            m_VerticalSpace = m_InvaderHeight * m_VerticalSpaceFactor; //// according to the demands, factor = 0.6
            m_HorizontalSpace = m_InvaderWidth * m_HorizonalSpaceFactor; ////according to the demands, factor = 0.6
            
            int numOfCols = m_Invaders[0].Count;
            int numOfColSpaces = numOfCols - 1;
            int numOfRows = m_Invaders.Count;
            int numOfRowSpaces = numOfRows - 1;
            Width = (numOfCols * m_InvaderWidth) + (numOfColSpaces * m_HorizontalSpace);
            Height = (numOfRows * m_InvaderHeight) + (numOfRowSpaces * m_VerticalSpace);
            m_Position = new Vector2(0, 3 * m_InvaderHeight);
            updateAllArmyMatrix(m_Position);
        }

        private void updateAllArmyMatrix(Vector2 newPosition)
        {
            foreach (List<Invader> invadersRow in m_Invaders)
            {
                updateRowPosition(invadersRow, newPosition);
                newPosition.Y += m_InvaderWidth + m_VerticalSpace;
            }
        }
        
        private void updateRowPosition(List<Invader> i_InvadersRow, Vector2 i_NewPosition)
        {
            for (int i = 0; i < i_InvadersRow.Count; i++)
            {
                Invader currentInvader = i_InvadersRow[i];
                float x = i_NewPosition.X + (i * m_InvaderWidth) + (i * m_HorizontalSpace);
                float y = i_NewPosition.Y;
                currentInvader.Position = new Vector2(x, y);
                currentInvader.InvaderCelAnimator.FrameLength = TimeSpan.FromSeconds(m_timeBetweenJumps);
            }
        }

        public override void Update(GameTime i_GameTime)
        {
           // sum the frams' time, each time we get to 0.5 seconds, the invaders matrix will jump 16px
            m_timeToJump += i_GameTime.ElapsedGameTime.TotalSeconds;
            Vector2 newPosition = new Vector2();
            Vector2 rightEdgeOfEnemyMatrix = Vector2.Zero;
            Vector2 leftEdgeOfEnemyMatrix = Vector2.Zero;

            if (m_timeToJump >= m_timeBetweenJumps)
            {
                if (m_Position.X == Game.GraphicsDevice.Viewport.Width - Width || (m_Position.X == 0 && startGame == false))
                {
                    newPosition.Y = m_Position.Y + (m_InvaderHeight / 2);
                    newPosition.X = m_Position.X + 0.1f;
                    m_timeBetweenJumps *= 0.8; // after each down, time between jumps is getting shorter by 5%
                    startGame = false;
                }
                else
                {
                    newPosition.X = m_Position.X + m_matrixJumpSpeed;
                    newPosition.Y = m_Position.Y;
                    rightEdgeOfEnemyMatrix.X = newPosition.X + Width;
                    leftEdgeOfEnemyMatrix.X = newPosition.X;

                    if (rightEdgeOfEnemyMatrix.X >= Game.GraphicsDevice.Viewport.Width || leftEdgeOfEnemyMatrix.X <= 0)
                    {
                        newPosition.X = MathHelper.Clamp(newPosition.X, 0, Game.GraphicsDevice.Viewport.Width - Width);
                        m_matrixJumpSpeed = -m_matrixJumpSpeed;
                    }
                }
               
                updateAllArmyMatrix(newPosition);
                m_Position = newPosition;
                m_timeToJump -= m_timeBetweenJumps;
            }

            base.Update(i_GameTime);
        }

        private void invader_InvaderIsDead(Invader i_Invader, ICollidable i_Collidable)
        {
            m_NumOfLivingInvaders--;
            InvaderIsDead.Invoke(i_Invader, i_Collidable);
            if (m_NumOfLivingInvaders == 0)
            {
                if (AllInvadersAreDead != null)
                {
                    AllInvadersAreDead.Invoke();
                }
            }
        }
        
        private int calculateArmySize()
        {
            int armySize = 0;
            foreach (List<Invader> invadersRow in m_Invaders)
            {
                foreach (Invader invader in invadersRow)
                {
                    if (invader.Visible)
                    {
                        armySize++;
                    }
                }
            }

            return armySize;
        }
    }
}
