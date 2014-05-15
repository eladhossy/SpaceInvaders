using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.ObjectModel;
using Microsoft.Xna.Framework;
using Infrastructure.ObjectModel.Screens;
using Infrastructure.ObjectModel.Animators.ConcreteAnimators;
using Microsoft.Xna.Framework.Input;
using Infrastructure.ServiceInterfaces;
using Microsoft.Xna.Framework.Audio;

// this class is a base class for all the screens in the game, that function as a menu.
// we wrote it so that we can reuse the logic of the menu for all the menus in the game.
namespace SpaceInvaders
{
    public class GameMenu : GameScreen
    {
        protected List<MenuItem> m_MenuItems = new List<MenuItem>();
        
        protected string m_FontName = @"Fonts\Calibri25";
        
        public Vector2 MenuPosition { get; set; }
        
        protected int m_SpaceBetweenItems = 30;

        protected Color m_SelectedItemColor = Color.RoyalBlue;
        
        protected Color m_UnSelectedItemColor = Color.White;
        
        protected int m_SelectedItemIndex = 0;
        
        protected int m_MouseSelectionSensitivity = 250;
        
        protected string m_MenuName = "Menu";
        
        protected TextWriter m_MenuTitle;
        
        private bool m_AnimationsAttached = false;
        
        private int m_MouseWheelThreshold = 250;
        
        private int m_WheelAccumulator = 0;

        private SoundBank m_SoundBank;

        IAudioManager m_AudioManager;

        string m_CueName;

        public GameMenu(Game i_Game, string i_SoundBankName, string i_CueName) 
            : base(i_Game)
        {
            m_AudioManager = (IAudioManager)i_Game.Services.GetService(typeof(IAudioManager));
            m_CueName = i_CueName;
            m_SoundBank = m_AudioManager.GetSoundBank(i_SoundBankName);
            m_MenuTitle = new TextWriter(i_Game, @"Fonts\Calibri70");
            Add(m_MenuTitle);
        }

        public void PositionMenuItems()
        {
            int i = 0;
            foreach (MenuItem menuItem in m_MenuItems)
            {
                TextWriter itemText = menuItem.ItemTextWriter;
                itemText.Position = new Vector2(MenuPosition.X, MenuPosition.Y + (i * m_SpaceBetweenItems));
                i++;
            }
        }

        public override void Initialize()
        {
            m_SelectedItemIndex = 0;
            m_MenuTitle.TextToWrite = m_MenuName;
            MenuPosition = CenterOfViewPort - new Vector2(100);
            m_MenuTitle.Position = new Vector2(10);
            base.Initialize();
            if (this.m_Sprites.Count > 0)
            {
                PositionMenuItems();
                if (!m_AnimationsAttached)
                {
                    attachPulserToMenuItems();
                }

                toggleSelection(m_SelectedItemIndex);
            }
        }

        private void attachPulserToMenuItems()
        {
            foreach (MenuItem menuItem in m_MenuItems)
            {
                TextWriter itemText = menuItem.ItemTextWriter;
                itemText.Animations.Add(new PulseAnimator("Pulse", TimeSpan.Zero, 1.05f, 0.7f));
            }

            m_AnimationsAttached = true;
        }

        public void AddCommandMenuItem(string i_Text, Action i_Action)
        {
            MenuItem menuItem = new CommandMenuItem(i_Text, m_FontName, i_Action, Game);
            Add(menuItem.ItemTextWriter);
            m_MenuItems.Add(menuItem);
        }

        public void AddScrollableMenuItem(string i_Text, string[] i_Options, int i_IndexOfDefaultValue, Action<string> i_Action)
        {
            MenuItem menuItem = new ScrollableMenuItem(i_Text, i_Options, i_IndexOfDefaultValue, m_FontName, i_Action, Game);
            Add(menuItem.ItemTextWriter);
            m_MenuItems.Add(menuItem);
        }

        public void RemoveMenuItem(MenuItem i_MenuItem)
        {
            m_MenuItems.Remove(i_MenuItem);
        }

        public override void Update(GameTime gameTime)
        {
            MenuItem selectedItem;
            int prev = m_SelectedItemIndex;
            base.Update(gameTime);
            if (InputManager.KeyPressed(Microsoft.Xna.Framework.Input.Keys.Down))
            {
                m_SelectedItemIndex++;
                m_SoundBank.PlayCue(m_CueName);
                m_SelectedItemIndex %= m_MenuItems.Count;
            }

            if (InputManager.KeyPressed(Microsoft.Xna.Framework.Input.Keys.Up))
            {
                m_SelectedItemIndex--;
                m_SoundBank.PlayCue(m_CueName);
                if (m_SelectedItemIndex < 0)
                {
                    m_SelectedItemIndex = m_MenuItems.Count - 1;
                }
            }

            if (Game.IsMouseVisible)
            {
                MouseState m = InputManager.MouseState;
               
                Rectangle rec = new Rectangle(m.X - m_MouseSelectionSensitivity, m.Y - m_MouseSelectionSensitivity, m_MouseSelectionSensitivity, m_MouseSelectionSensitivity);
                foreach (MenuItem menuItem in m_MenuItems)
                {
                    if (menuItem.ItemTextWriter.Bounds.Intersects(rec))
                    {
                        m_SelectedItemIndex = m_MenuItems.IndexOf(menuItem);
                        selectedItem = m_MenuItems[m_SelectedItemIndex];
                    }
                }
            }

            selectedItem = m_MenuItems[m_SelectedItemIndex];

            if (InputManager.KeyPressed(Keys.Enter) || InputManager.ButtonPressed(eInputButtons.Left))
            {
                if (selectedItem is CommandMenuItem)
                {
                    (m_MenuItems[m_SelectedItemIndex] as CommandMenuItem).ItemCommand.Invoke();
                }
            }

            m_WheelAccumulator += InputManager.ScrollWheelDelta;
            if (InputManager.KeyPressed(Keys.PageUp) || m_WheelAccumulator > m_MouseWheelThreshold || InputManager.ButtonPressed(eInputButtons.Right))
            {
                m_WheelAccumulator = 0;
                if (selectedItem is ScrollableMenuItem)
                {
                    (selectedItem as ScrollableMenuItem).Index++;
                }
            }

            if (InputManager.KeyPressed(Keys.PageDown) || m_WheelAccumulator < -m_MouseWheelThreshold)
            {
                m_WheelAccumulator = 0;
                if (selectedItem is ScrollableMenuItem)
                {
                    (selectedItem as ScrollableMenuItem).Index--;
                }
            }

            toggleSelection(prev);
            toggleSelection(m_SelectedItemIndex);
        }

        private void toggleSelection(int i_MenuItemIndex)
        {
            TextWriter selectedItem = m_MenuItems[i_MenuItemIndex].ItemTextWriter;
            selectedItem.TintColor = selectedItem.TintColor == m_SelectedItemColor ? m_UnSelectedItemColor : m_SelectedItemColor;
            selectedItem.Animations.Enabled = !selectedItem.Animations.Enabled;
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();
        }
    }
}
