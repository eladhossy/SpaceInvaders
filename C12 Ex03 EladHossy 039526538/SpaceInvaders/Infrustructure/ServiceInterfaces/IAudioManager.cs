using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace Infrastructure.ServiceInterfaces
{
    public enum eVolumeStates
    {
        On,
        Off
    }

    public interface IAudioManager
    {
        SoundBank GetSoundBank(string i_BankName);
        
        void AddSoundBank(string i_SoundBankName, SoundBank i_SoundBank);
        
        void AddWaveBank(WaveBank i_WaveBank);
        
        void AddAudioCategory(string i_Name, float i_Volume);
        
        float GetCategoryVolume(string i_CategoryName);
        
        void SetCategoryVolume(string i_CategoryName, float i_NewVolume);

        eVolumeStates GetAudioState();
        
        void SetAudioState(eVolumeStates i_NewState);
       
        void MuteCategory(string i_CategoryName);
        
        void UnMuteCategory(string i_CategoryName);
    }
}
