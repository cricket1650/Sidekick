using System;
using Sidekick.Domain.Game.Languages;

namespace Sidekick.Application.Game.Languages.Implementations
{
    [GameLanguage("Spanish", "Rareza", "es")]
    public class GameLanguageES : IGameLanguage
    {
        public Uri PoeTradeSearchBaseUrl => new Uri("https://es.pathofexile.com/trade/search/");
        public Uri PoeTradeExchangeBaseUrl => new Uri("https://es.pathofexile.com/trade/exchange/");
        public Uri PoeTradeApiBaseUrl => new Uri("https://es.pathofexile.com/api/trade/");
        public Uri PoeCdnBaseUrl => new Uri("https://web.poecdn.com/");
        public string RarityUnique => "Único";
        public string RarityRare => "Raro";
        public string RarityMagic => "Mágico";
        public string RarityNormal => "Normal";
        public string RarityCurrency => "Objetos Monetarios";
        public string RarityGem => "Gema";
        public string RarityDivinationCard => "Carta de Adivinación";
        public string DescriptionUnidentified => "Sin identificar";
        public string DescriptionQuality => "Calidad";
        public string DescriptionAlternateQuality => "Calidad alternativa";
        public string DescriptionCorrupted => "Corrupto";
        public string DescriptionRarity => "Rareza";
        public string DescriptionSockets => "Engarces";
        public string DescriptionItemLevel => "Nivel de Objeto";
        public string DescriptionExperience => "Experiencia";
        public string DescriptionOrgan => "Usa";
        public string PrefixSuperior => "Superior";
        public string InfluenceShaper => "Creador";
        public string InfluenceElder => "Antiguo";
        public string InfluenceCrusader => "Cruzado";
        public string InfluenceHunter => "Cazador";
        public string InfluenceRedeemer => "Redentora";
        public string InfluenceWarlord => "Jefe de guerra";
        public string DescriptionMapTier => "Grado del Mapa";
        public string DescriptionItemQuantity => "Cantidad de Objetos";
        public string DescriptionItemRarity => "Rareza de Objetos";
        public string DescriptionMonsterPackSize => "Tamaño de Grupos de Monstruos";
        public string PrefixBlighted => "Infestado";
        public string KeywordVaal => "Vaal";

        public string DescriptionPhysicalDamage => "__TranslationRequired__";

        public string DescriptionElementalDamage => "__TranslationRequired__";

        public string DescriptionAttacksPerSecond => "__TranslationRequired__";

        public string DescriptionCriticalStrikeChance => "__TranslationRequired__";

        public string DescriptionEnergyShield => "__TranslationRequired__";

        public string DescriptionArmour => "__TranslationRequired__";

        public string DescriptionEvasion => "__TranslationRequired__";
        public string DescriptionChanceToBlock => "__TranslationRequired__";
        public string DescriptionLevel => "__TranslationRequired__";

        public string ModifierIncreased => "__TranslationRequired__";
        public string ModifierReduced => "__TranslationRequired__";
        public string AdditionalProjectile => "__TranslationRequired__";
        public string AdditionalProjectiles => "__TranslationRequired__";

        public string PrefixAnomalous => "anómala";
        public string PrefixDivergent => "divergente";
        public string PrefixPhantasmal => "fantasmal";
    }
}
