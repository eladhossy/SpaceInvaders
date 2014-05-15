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
using Infrastructure.ObjectModel.Screens;

namespace SpaceInvaders
{
    public class BarrierGroupMgr : Microsoft.Xna.Framework.GameComponent
    {
        public List<Barrier> m_Barriers = new List<Barrier>();
        private int m_NumOfBarriers = 4;
        public Vector2 m_Position;
        private int m_BarrierWidth = 44;
        public Vector2 m_StartingPosition;
        private float m_Velocity;
        private float m_DistanceBetweenBarriers = 66f;
        
        public bool isMoving { get; set; }
        
        private Color[] m_BarrierStartingPixels;

        public BarrierGroupMgr(Game game, GameScreen i_GameScreen, float i_BarriersVelocity, SoundBank i_SoundBank, string i_BarrierHitCue)
            : base(game)
        {
            m_Velocity = i_BarriersVelocity;
      
            for (int i = 0; i < m_NumOfBarriers; i++)
            {
                Barrier newBarrier = new Barrier(game, "0" + i);
                newBarrier.SetHitSound(i_SoundBank, i_BarrierHitCue);
                i_GameScreen.Add(newBarrier);
                m_Barriers.Add(newBarrier);
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            Barrier firstBarrier = m_Barriers[0];
            m_BarrierStartingPixels = new Color[firstBarrier.Texture.Width * firstBarrier.Texture.Height];
            m_Barriers[0].Texture.GetData<Color>(m_BarrierStartingPixels); // save the starting texture of a barrier
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            if (isMoving)
            {
                float maxDistance = m_BarrierWidth / 2;
                float maxX = m_StartingPosition.X + maxDistance;
                float minX = m_StartingPosition.X - maxDistance;

                m_Position.X += m_Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (m_Position.X >= maxX || m_Position.X <= minX)
                {
                    MathHelper.Clamp(m_Position.X, minX, maxX);
                    m_Velocity = -m_Velocity;
                }
            }

            // 'tell' all the barriers about their new position
            changeBarriersPosition();
        }

        private void changeBarriersPosition()
        {
            int barrierIndex = 0;
            float barrierX;
            foreach (Barrier barrier in m_Barriers)
            {
                barrierX = barrierIndex * (m_BarrierWidth + m_DistanceBetweenBarriers);
                barrier.Position = new Vector2(this.m_Position.X + barrierX, this.m_Position.Y);
                barrierIndex++;
                
                if (!isMoving)
                {
                    barrier.ReportPositionChanged(); // for the collision mechanism to check for collision
                }
            }
        }

        public void PositionCenterOfBarriersAt(Vector2 i_Position)
        {
            float barriersWidthCenter = ((m_NumOfBarriers * m_BarrierWidth) + ((m_DistanceBetweenBarriers * m_NumOfBarriers) - 1)) / 2;
            m_Position = m_StartingPosition = new Vector2(i_Position.X - barriersWidthCenter, i_Position.Y);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            foreach (Barrier barrier in m_Barriers)
            {
                barrier.Texture.SetData<Color>(m_BarrierStartingPixels);
            }
        }
    }
}
