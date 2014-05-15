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
using Infrastructure.ObjectModel.Animators;

namespace SpaceInvaders
{
    public class Sprite : DrawableGameComponent
    {
        protected CompositeAnimator m_Animations;
        public CompositeAnimator Animations
        {
            get { return m_Animations; }
            set { m_Animations = value; }
        }
        protected Texture2D m_Texture;
        protected Vector2 m_Position = Vector2.Zero;

        public Texture2D Texture
        {
            get
            {
                return m_Texture;
            }
        }

        public Vector2 Position
        {
            get { return m_Position; }
            set
            {
                if (m_Position != value)
                {
                    m_Position = value;
                    OnPositionChanged();
                }
            }
        }
        public Sprite ShallowClone()
        {
            return this.MemberwiseClone() as Sprite;
        }

        protected BlendState m_BlendState = BlendState.AlphaBlend;
        public BlendState BlendState
        {
            get { return m_BlendState; }
            set { m_BlendState = value; }
        }

        public Vector2 m_PositionOrigin;
        public Vector2 PositionOrigin
        {
            get { return m_PositionOrigin; }
            set { m_PositionOrigin = value; }
        }

        public Vector2 m_RotationOrigin;
        public Vector2 RotationOrigin
        {
            get { return m_RotationOrigin; }
            set { m_RotationOrigin = value; }
        }


        protected Vector2 PositionForDraw
        {
            get { return this.Position - this.PositionOrigin + this.RotationOrigin; }
        }

        public Vector2 SourceRectangleCenter
        {
            get { return new Vector2((float)(m_SourceRectangle.Width / 2), (float)(m_SourceRectangle.Height / 2)); }
        }

        protected float m_Rotation = 0;
        public float Rotation
        {
            get { return m_Rotation; }
            set { m_Rotation = value; }
        }

        protected Rectangle m_SourceRectangle;
        public Rectangle SourceRectangle
        {
            get { return m_SourceRectangle; }
            set { m_SourceRectangle = value; }
        }

        protected Rectangle? m_DestRectangle;
        public Rectangle? DestRectangle
        {
            get { return m_DestRectangle; }
            set { m_DestRectangle = value; }
        }

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle((int)m_Position.X, (int)m_Position.Y, (int)Width, (int)Height);
            }
        }

        protected Vector2 m_Scales = Vector2.One;
        public Vector2 Scales
        {
            get { return m_Scales; }
            set
            {
                if (m_Scales != value)
                {
                    m_Scales = value;
                    OnPositionChanged();
                }
            }
        }

        protected float m_WidthBeforeScale;
        public float WidthBeforeScale
        {
            get { return m_WidthBeforeScale; }
            set { m_WidthBeforeScale = value; }
        }

        protected float m_HeightBeforeScale;
        public float HeightBeforeScale
        {
            get { return m_HeightBeforeScale; }
            set { m_HeightBeforeScale = value; }
        }

        public float Width
        {
            get { return m_WidthBeforeScale * m_Scales.X; }
            set { m_WidthBeforeScale = value / m_Scales.X; }
        }

        public float Height
        {
            get { return m_HeightBeforeScale * m_Scales.Y; }
            set { m_HeightBeforeScale = value / m_Scales.Y; }
        }


        protected bool m_UseSharedBatch = false;
        protected SpriteBatch m_SpriteBatch;
        public SpriteBatch SpriteBatch
        {
            set
            {
                m_SpriteBatch = value;
                m_UseSharedBatch = true;
            }
        }

        protected string m_TextureName;
        protected Color m_TintColor = Color.White;

        public Color TintColor
        {
            get { return m_TintColor; }
            set { m_TintColor = value; }
        }

        protected Vector2 m_Velocity;
        public Vector2 Velocity
        {
            get { return m_Velocity; }
            set { m_Velocity = value; }
        }

        protected float m_AngularVelocity = 0;
        public float AngularVelocity
        {
            get { return m_AngularVelocity; }
            set { m_AngularVelocity = value; }
        }

        protected Dictionary<string, Dictionary<string, SoundBank>> m_Sounds; // each Sprite now has the ability to play souds


        //public int Width { get; set; }

        //public int Height { get; set; }

        public Sprite(Game game, string i_TextureName)
            : base(game)
        {
            m_TextureName = i_TextureName;
        }

        public override void Initialize()
        {
            m_SpriteBatch = new SpriteBatch(this.Game.GraphicsDevice);

            if (this is ICollidable)
            {
                ICollisionManager collisionManager = this.Game.Services.GetService(typeof(ICollisionManager))
                    as ICollisionManager;

                if (collisionManager != null)
                {
                    collisionManager.AddObjectToMonitor(this as ICollidable);
                }
            }

            base.Initialize();
            m_Animations = new CompositeAnimator(this);

        }

        protected override void LoadContent()
        {
            m_Texture = Game.Content.Load<Texture2D>(m_TextureName);
            base.LoadContent();

            InitBounds();
        }

        protected virtual void InitBounds()
        {
            m_WidthBeforeScale = m_Texture.Width;
            m_HeightBeforeScale = m_Texture.Height;
            m_Position = Vector2.Zero;

            InitSourceRectangle();

            InitOrigins();
        }

        protected virtual void InitOrigins()
        {
        }

        protected virtual void InitSourceRectangle()
        {
            m_SourceRectangle = new Rectangle(0, 0, (int)m_WidthBeforeScale, (int)m_HeightBeforeScale);
        }

        private float m_LayerDepth = 1;
        public override void Draw(GameTime gameTime)
        {
            if (!m_UseSharedBatch)
            {
                m_SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
            }
            m_SpriteBatch.Draw(m_Texture, PositionForDraw, SourceRectangle, TintColor, Rotation, RotationOrigin, Scales, SpriteEffects.None,0);

            if (!m_UseSharedBatch)
            {
                m_SpriteBatch.End();
            }
            base.Draw(gameTime);
        }

        public event PositionChangedEventHandler PositionChanged;

        protected virtual void OnPositionChanged()
        {
            if (PositionChanged != null)
            {
                PositionChanged.Invoke(this as ICollidable);
            }
        }

        public virtual bool CheckCollision(ICollidable i_Collidable)
        {
            bool collided = false;
            Sprite source = i_Collidable as Sprite;
            if (i_Collidable != null)
            {
                collided = source.Bounds.Intersects(this.Bounds);
            }

            return collided;
        }

        public virtual void Collided(ICollidable i_Collidable)
        {

            Sprite otherSprite = i_Collidable as Sprite;
            Color[] myPixels = new Color[Texture.Width * Texture.Height];
            Color[] otherSpritePixels = new Color[otherSprite.Texture.Width * otherSprite.Texture.Height];

            Texture.GetData<Color>(myPixels);
            otherSprite.Texture.GetData<Color>(otherSpritePixels);

            int dataSize = myPixels.Count();
            int otherDataSize = otherSpritePixels.Count();
            float x, xOnScreen, otherX, otherXOnScreen;
            float y, yOnScreen, otherY, otherYOnScreen;


            for (int i = 0; i < dataSize; i++)
            {
                if (myPixels[i].A != 0)
                {
                    y = i / Width;
                    x = i % Width;
                    yOnScreen = y + Position.Y;
                    xOnScreen = x + Position.X;

                    for (int j = 0; j < otherDataSize; j++)
                    {
                        if (otherSpritePixels[j].A != 0)
                        {
                            otherY = j / otherSprite.Width;
                            otherX = j % otherSprite.Width;
                            otherYOnScreen = otherY + otherSprite.Position.Y;
                            otherXOnScreen = otherX + otherSprite.Position.X;

                            if (otherXOnScreen == xOnScreen && otherYOnScreen == yOnScreen)
                            {
                                myPixels[i] = new Color(0, 0, 0);
                                Texture.SetData(myPixels);
                            }
                        }
                    }
                }
            }
        }

        public float Opacity
        {
            get { return (float)m_TintColor.A / (float)byte.MaxValue; }
            set { m_TintColor.A = (byte)(value * (float)byte.MaxValue); }

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            this.Animations.Update(gameTime);
        }
        
    }


    }

