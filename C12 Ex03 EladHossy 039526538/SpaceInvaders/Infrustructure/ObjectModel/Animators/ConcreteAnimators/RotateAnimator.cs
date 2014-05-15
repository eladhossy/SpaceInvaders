using System;
using Infrastructure.ObjectModel;
using Infrastructure.ObjectModel.Animators.ConcreteAnimators;
using Infrastructure.ServiceInterfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Infrastructure.ObjectModel.Animators.ConcreteAnimators
{
    public class RotateAnimator : SpriteAnimator
    {
        private float m_RotateSpeed;

        public RotateAnimator(string i_Name, float i_RotateSpeed, TimeSpan i_AnimationLength)
            : base(i_Name, i_AnimationLength)
        {
            m_RotateSpeed = i_RotateSpeed;
        }


        protected override void RevertToOriginal()
        {
            this.BoundSprite.Rotation = m_OriginalSpriteInfo.Rotation;
        }

        protected override void DoFrame(GameTime i_GameTime)
        {
            BoundSprite.Rotation += m_RotateSpeed * (float)i_GameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
