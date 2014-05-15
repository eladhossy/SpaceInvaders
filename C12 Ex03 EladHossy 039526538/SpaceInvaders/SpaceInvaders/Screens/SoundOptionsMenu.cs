using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Infrastructure.Managers;
using Infrastructure.ServiceInterfaces;

namespace SpaceInvaders.Screens
{
    public class SoundOptionsMenu : GameMenu
    {
        private IAudioManager m_AudioManager;

        public SoundOptionsMenu(Game i_Game, string i_SoundBankName, string i_CueName)
            : base(i_Game, i_SoundBankName, i_CueName)
        {
            m_MenuName = "Sound Options";
            m_AudioManager = (IAudioManager)Game.Services.GetService(typeof(IAudioManager));

            AddScrollableMenuItem("Toggle Sound: ", new string[] { "On", "Off" }, 0,  (string onOrOff) =>
            {
                if (onOrOff == "On")
                {
                    m_AudioManager.SetAudioState(eVolumeStates.On);
                }
                else
                {
                    m_AudioManager.SetAudioState(eVolumeStates.Off);
                }
            });

            AddScrollableMenuItem("Sounds Effects Volume: ", new string[] { "0", "10", "20", "30", "40", "50", "60", "70", "80", "90", "100" },
                (int)m_AudioManager.GetCategoryVolume("SoundFX"),
                (string volume) =>
            {
                float newVolume = float.Parse(volume) / 10;  // normalization of the values presented int the UI, to decibal values;
                m_AudioManager.SetCategoryVolume("SoundFX", newVolume);
            });

            AddScrollableMenuItem("Background Music Volume: ", new string[] { "0", "10", "20", "30", "40", "50", "60", "70", "80", "90", "100" },
                (int)m_AudioManager.GetCategoryVolume("Music"),
                (string volume) =>
               {
                   float newVolume = float.Parse(volume) / 10;  // normalization of the values presented int the UI, to decibal values;
                   m_AudioManager.SetCategoryVolume("Music", newVolume);
               });

            AddCommandMenuItem("Done", () => ExitScreen());
        }
    }
}
