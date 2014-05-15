using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.ObjectModel;
using Microsoft.Xna.Framework.Audio;
using Infrastructure.ServiceInterfaces;
using Microsoft.Xna.Framework;

namespace Infrastructure.Managers
{
    public class AudioManager : GameService, IAudioManager
    {
        private AudioEngine m_AudioEngine;
        private List<WaveBank> m_WaveBanks = new List<WaveBank>();
        private Dictionary<string, SoundBank> m_SoundBanks = new Dictionary<string, SoundBank>();
        private Dictionary<string, float[]> m_AudioCatagoriesVolumes = new Dictionary<string, float[]>();
        private eVolumeStates m_CurrentAudioState;

        public AudioManager(Game i_Game, AudioEngine i_AudioEngine)
            : base(i_Game)
        {
            m_AudioEngine = i_AudioEngine;
        }
        
        protected override void RegisterAsService()
        {
            Game.Services.AddService(typeof(IAudioManager), this);
        }

        public SoundBank GetSoundBank(string i_BankName)
        {
            if (m_SoundBanks.ContainsKey(i_BankName))
            {
                return m_SoundBanks[i_BankName];
            }
            else
            {
                return null;
            }
        }

        public void AddSoundBank(string i_SoundBankName, SoundBank i_SoundBank)
        {
            m_SoundBanks.Add(i_SoundBankName, i_SoundBank);
        }

        public void AddWaveBank(WaveBank i_WaveBank)
        {
            m_WaveBanks.Add(i_WaveBank);
        }

        public void AddAudioCategory(string i_Name, float i_Volume)
        {
            m_AudioCatagoriesVolumes.Add(i_Name, new float[] { i_Volume, i_Volume });
            applyAllCategoriesLevels();
        }

        public float GetCategoryVolume(string i_CategoryName)
        {
            float volume = 0;
            if (m_AudioCatagoriesVolumes.ContainsKey(i_CategoryName))
            {
                volume = m_AudioCatagoriesVolumes[i_CategoryName][0];
            }

            return volume;
        }

        public void SetCategoryVolume(string i_CategoryName, float i_NewVolume)
        {
            if (m_AudioCatagoriesVolumes.ContainsKey(i_CategoryName))
            {
                m_AudioCatagoriesVolumes[i_CategoryName][1] = i_NewVolume;
                m_AudioCatagoriesVolumes[i_CategoryName][0] = i_NewVolume;
                applyAllCategoriesLevels();
            }
        }

        public void MuteCategory(string i_CategoryName)
        {
            m_AudioCatagoriesVolumes[i_CategoryName][0] = 0f;
            applyCategorySoundLevel(i_CategoryName);
        }
        
        public void UnMuteCategory(string i_CategoryName)
        {
            m_AudioCatagoriesVolumes[i_CategoryName][0] =
                m_AudioCatagoriesVolumes[i_CategoryName][1];

            applyCategorySoundLevel(i_CategoryName);
        }

        public void SetAudioState(eVolumeStates i_NewState)
        {
            if (i_NewState == eVolumeStates.On)
            {
                foreach (string categoryName in m_AudioCatagoriesVolumes.Keys)
                {
                    UnMuteCategory(categoryName);
                }
            }
            else
            {
                foreach (string categoryName in m_AudioCatagoriesVolumes.Keys)
                {
                    MuteCategory(categoryName);
                }
            }

            m_CurrentAudioState = i_NewState;
        }

        public eVolumeStates GetAudioState()
        {
            return m_CurrentAudioState;
        }

        private void applyAllCategoriesLevels()
        {
            foreach (string categoryName in m_AudioCatagoriesVolumes.Keys)
            {
                applyCategorySoundLevel(categoryName);
            }
        }

        private void applyCategorySoundLevel(string i_CategoryName)
        {
            m_AudioEngine.GetCategory(i_CategoryName).SetVolume(m_AudioCatagoriesVolumes[i_CategoryName][0]);
        }
    }
}
