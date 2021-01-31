using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Sidekick.Domain.Cache.Commands;
using Sidekick.Domain.Game.Languages;
using Sidekick.Domain.Game.Languages.Commands;
using Sidekick.Domain.Game.Leagues.Queries;
using Sidekick.Domain.Initialization.Commands;
using Sidekick.Domain.Settings;
using Sidekick.Domain.Settings.Commands;
using Sidekick.Extensions;
using Sidekick.Presentation.Localization;

namespace Sidekick.Presentation.Blazor.Settings
{
    public class SettingsViewModel : ISidekickSettings
    {
        private readonly IUILanguageProvider uiLanguageProvider;
        private readonly IGameLanguageProvider gameLanguageProvider;
        private readonly ISidekickSettings sidekickSettings;
        private readonly IMediator mediator;

        public SettingsViewModel(
            IUILanguageProvider uiLanguageProvider,
            IGameLanguageProvider gameLanguageProvider,
            ISidekickSettings sidekickSettings,
            IMediator mediator)
        {
            this.uiLanguageProvider = uiLanguageProvider;
            this.gameLanguageProvider = gameLanguageProvider;
            this.sidekickSettings = sidekickSettings;
            this.mediator = mediator;

            gameLanguageProvider.AvailableLanguages.ForEach(x => ParserLanguageOptions.Add(x.Name, x.LanguageCode));
            /*
            foreach (var setting in Chat_CustomCommands)
                CustomChatSettings.Add(new CustomChatModel { ChatCommand = setting.ChatCommand, Key = setting.Key });
            */
        }

        public async Task Initialize()
        {
            sidekickSettings.CopyValuesTo(this);

            WikiOptions = new Dictionary<string, string>()
            {
                { "POE Wiki", WikiSetting.PoeWiki.ToString() },
                { "POE Db", WikiSetting.PoeDb.ToString() },
            };

            ParserLanguageOptions = gameLanguageProvider.AvailableLanguages
                .ToDictionary(x => x.LanguageCode, x => x.Name);

            UILanguageOptions = uiLanguageProvider.AvailableLanguages
                .ToDictionary(x => x.Name, x => x.NativeName.First().ToString().ToUpper() + x.NativeName[1..]);

            var leagues = await mediator.Send(new GetLeaguesQuery(true));
            LeagueOptions = leagues.ToDictionary(x => x.Id, x => x.Text);
        }

        #region Settings

        public Dictionary<string, string> WikiOptions { get; private set; }

        public Dictionary<string, string> LeagueOptions { get; private set; }

        public Dictionary<string, string> UILanguageOptions { get; private set; }

        public Dictionary<string, string> ParserLanguageOptions { get; private set; } = new Dictionary<string, string>();

        // public Dictionary<CustomChatModel> CustomChatSettings { get; private set; }

        public Guid? CurrentKey { get; set; }

        // public CustomChatModel CurrentCustomChat { get; set; }

        public bool SettingCustom { get; set; }

        public List<string> Price_Mods_Accessory { get; set; }

        public List<string> Price_Mods_Armour { get; set; }

        public string Character_Name { get; set; }

        public bool Price_CloseWithMouse { get; set; }

        public bool Map_CloseWithMouse { get; set; }

        public string Map_Dangerous_Regex { get; set; }

        public bool Stash_EnableCtrlScroll { get; set; }

        public bool Price_Prediction_Enable { get; set; }

        public List<string> Price_Mods_Flask { get; set; }

        public List<string> Price_Mods_Jewel { get; set; }

        public string Price_Key_Check { get; set; }

        public string Price_Key_Close { get; set; }

        public string Map_Key_Close { get; set; }

        public string Chat_Key_Exit { get; set; }

        public string Key_FindItems { get; set; }

        public string Chat_Key_Hideout { get; set; }

        public string Chat_Key_ReplyToLastWhisper { get; set; }

        public string Chat_Key_LeaveParty { get; set; }

        public string Map_Key_Check { get; set; }

        public string Cheatsheets_Key_Open { get; set; }

        public string Price_Key_OpenSearch { get; set; }

        public string Key_OpenSettings { get; set; }

        public string Wiki_Key_Open { get; set; }

        public string Stash_Key_Left { get; set; }

        public string Stash_Key_Right { get; set; }

        public string Language_Parser { get; set; }

        public string Language_UI { get; set; }

        public int Cheatsheets_SelectedIndex { get; set; }

        public string LeagueId { get; set; }

        public string LeaguesHash { get; set; }

        public List<string> Price_Mods_Map { get; set; }

        public bool RetainClipboard { get; set; }

        public bool ShowSplashScreen { get; set; }

        public List<string> Price_Mods_Weapon { get; set; }

        public WikiSetting Wiki_Preferred { get; set; }

        public List<CustomChatSetting> Chat_CustomCommands { get; set; } = new List<CustomChatSetting>();

        #endregion

        public async Task Save()
        {
            var leagueHasChanged = LeagueId != sidekickSettings.LeagueId;
            var languageHasChanged = gameLanguageProvider.Current.LanguageCode != Language_Parser;
            /*
                        Chat_CustomCommands.Clear();
            foreach (var setting in CustomChatSettings)
                            Chat_CustomCommands.Add(new CustomChatSetting { ChatCommand = setting.ChatCommand, Key = setting.Key });
            */
            uiLanguageProvider.SetLanguage(Language_UI);
            await mediator.Send(new SetGameLanguageCommand(Language_Parser));
            await mediator.Send(new SaveSettingsCommand(this));

            if (languageHasChanged || leagueHasChanged) await ResetCache();
        }

        public bool IsKeybindUsed(string keybind, string ignoreKey = null)
        {
            // Allow close commands to have the same keybinds
            if (ignoreKey == nameof(ISidekickSettings.Price_Key_Close) || ignoreKey == nameof(ISidekickSettings.Map_Key_Close))
            {
                return false;
            }

            return GetType()
                .GetProperties()
                .Any(x => x.Name != ignoreKey && x.GetValue(this)?.ToString() == keybind)
                    || Chat_CustomCommands.Any(x => x.Key == keybind);
        }

        public async Task ResetCache()
        {
            await mediator.Send(new ClearCacheCommand());
            await mediator.Send(new InitializeCommand(false));
        }

        #region Custom Commands
        /*
        public void NewCommand()
        {
            if (!CustomChatSettings.Any(x => x.ChatCommand == "New Command"))
            {
                CustomChatSettings.Add(new CustomChatModel { ChatCommand = "New Command", Key = "" });
            }
        }
        */
        #endregion
    }
}