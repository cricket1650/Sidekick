using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using Sidekick.Business.Apis.Poe.Parser;

namespace Sidekick.Business.Tests.ItemParserTests
{
    public class EquipmentParsing : TestContext<ParserService>
    {
        [Test]
        public void ParseUnidentifiedUnique()
        {
            var actual = Subject.ParseItem(UnidentifiedUnique);

            using (new AssertionScope())
            {
                actual.Type.Should().Be("Jade Hatchet");
                actual.Identified.Should().BeFalse();
            }
        }

        [Test]
        public void ParseSixLinkUniqueBodyArmor()
        {
            var actual = Subject.ParseItem(UniqueSixLink);

            var expectedExplicits = new[]
            {
                "128% increased Evasion and Energy Shield (Local)",
                "+55 to maximum Life",
                "+12% to all Elemental Resistances",
                "44% increased Area of Effect",
                "47% increased Area Damage",
                "Extra gore"
            };

            var expectedPseudoMods = new[]
            {
                "+12% total to all Elemental Resistances",
                "+36% total Elemental Resistance",
                "+36% total Resistance",
                "+55 total maximum Life"
            };

            using (new AssertionScope())
            {
                actual.Name.Should().Be("Carcass Jack");
                actual.Type.Should().Be("Varnished Coat");

                actual.Properties.Quality.Should().Be(20);
                actual.Properties.Evasion.Should().Be(960);
                actual.Properties.EnergyShield.Should().Be(186);

                actual.Modifiers.Explicit
                    .Select(mod => mod.Text)
                    .Should().Contain(expectedExplicits);

                actual.Modifiers.Pseudo
                    .Select(mod => mod.Text)
                    .Should().Contain(expectedPseudoMods);

                actual.Sockets
                    .Should().HaveCount(6)
                    .And.OnlyContain(socket => socket.Group == 0);
            }
        }

        [Test]
        public void ParseRareGloves()
        {
            var actual = Subject.ParseItem(GlovesAssasinsMitts);

            var expectedExplicits = new[]
            {
                "+18 to Intelligence",
                "+73 to maximum Life",
                "+14% to Lightning Resistance",
                "0.23% of Physical Attack Damage Leeched as Mana"
            };

            using (new AssertionScope())
            {
                actual.NameLine.Should().Be("Death Nails");
                actual.Type.Should().Be("Assassin's Mitts");

                actual.Modifiers.Explicit
                    .Select(mod => mod.Text)
                    .Should().Contain(expectedExplicits);

                actual.Sockets.Count.Should().Be(1);
            }
        }

        [Test]
        public void ParseJewel()
        {
            var actual = Subject.ParseItem(JewelBlightCut);

            var expectedExplicits = new[]
            {
                "+8 to Strength and Intelligence",
                "14% increased Spell Damage while Dual Wielding",
                "19% increased Burning Damage",
                "15% increased Damage with Wands"
            };

            using (new AssertionScope())
            {
                actual.NameLine.Should().Be("Blight Cut");
                actual.Type.Should().Be("Cobalt Jewel");

                actual.Modifiers.Explicit
                    .Select(mod => mod.Text)
                    .Should().Contain(expectedExplicits);

                actual.ItemLevel.Should().Be(68);
            }
        }

        [Test]
        public void ParseInfluencedWeapon()
        {
            var actual = Subject.ParseItem(InfluencedWand);

            var expectedExplicits = new[]
            {
                "Adds 10 to 16 Physical Damage",
                "24% increased Fire Damage",
                "14% increased Critical Strike Chance for Spells",
                "Attacks with this Weapon Penetrate 10% Lightning Resistance"
            };

            using (new AssertionScope())
            {
                actual.NameLine.Should().Be("Miracle Chant");
                actual.Type.Should().Be("Imbued Wand");

                actual.Modifiers.Explicit
                    .Select(mod => mod.Text)
                    .Should().Contain(expectedExplicits);

                actual.Modifiers.Implicit
                    .Should().ContainSingle(mod => mod.Text == "33% increased Spell Damage");

                actual.Influences.Crusader.Should().BeTrue();
            }
        }

        [Test]
        public void ParseMagicWeapon()
        {
            var actual = Subject.ParseItem(MagicWeapon);

            actual.Type.Should().Be("Shadow Axe");
            actual.Rarity.Should().Be(Apis.Poe.Models.Rarity.Magic);

            var expectedExplicits = new[]
            {
                "11% reduced Enemy Stun Threshold"
            };

            actual.Modifiers.Explicit
                            .Select(mod => mod.Text)
                            .Should().Contain(expectedExplicits);
        }

        [Test]
        public void ParseFracturedItem()
        {
            var actual = Subject.ParseItem(FracturedItem);

            actual.Type.Should().Be("Iron Greaves");
            actual.Rarity.Should().Be(Apis.Poe.Models.Rarity.Rare);

            var expectedFractured = new[]
            {
                "10% increased Movement Speed"
            };

            actual.Modifiers.Fractured
                            .Select(mod => mod.Text)
                            .Should().Contain(expectedFractured);
        }

        /// <summary>
        /// This unique item can have multiple possible bases.
        /// </summary>
        [Test]
        public void ParseUniqueItemWithDifferentBases()
        {
            var actual = Subject.ParseItem(UniqueItemWithDifferentBases);

            actual.Name.Should().Be("Wings of Entropy");
            actual.Type.Should().Be("Ezomyte Axe");
            actual.Rarity.Should().Be(Apis.Poe.Models.Rarity.Unique);

            actual.Properties.PhysicalDps.Should().Be(243.68);
            actual.Properties.ElementalDps.Should().Be(172.8);
            actual.Properties.DamagePerSecond.Should().Be(416.48);
        }

        [Test]
        public void ParseWeaponWithMultipleElementalDamages()
        {
            var actual = Subject.ParseItem(WeaponWithMultipleElementalDamages);

            actual.Properties.PhysicalDps.Should().Be(53.94);
            actual.Properties.ElementalDps.Should().Be(314.07);
            actual.Properties.DamagePerSecond.Should().Be(368.01);
        }

        [Test]
        public void ParseEnchantWithAdditionalProjectiles()
        {
            var actual = Subject.ParseItem(EnchantWithAdditionalProjectiles);

            actual.Modifiers.Enchant.First().Text.Should().Be("Split Arrow fires an additional Projectile");
            actual.Modifiers.Enchant.First().Values.First().Should().Be(2);
        }

        #region ItemText

        private const string UniqueSixLink = @"Rarity: Unique
Carcass Jack
Varnished Coat
--------
Quality: +20% (augmented)
Evasion Rating: 960 (augmented)
Energy Shield: 186 (augmented)
--------
Requirements:
Level: 70
Str: 68
Dex: 96
Int: 111
--------
Sockets: B-B-R-B-B-B 
--------
Item Level: 81
--------
128% increased Evasion and Energy Shield
+55 to maximum Life
+12% to all Elemental Resistances
44% increased Area of Effect
47% increased Area Damage
Extra gore
--------
""...The discomfort shown by the others is amusing, but none
can deny that my work has made quite the splash...""
- Maligaro's Journal
";

        private const string UnidentifiedUnique = @"Rarity: Unique
Jade Hatchet
--------
One Handed Axe
Physical Damage: 10-15
Critical Strike Chance: 5.00%
Attacks per Second: 1.45
Weapon Range: 11
--------
Requirements:
Str: 21
--------
Sockets: R-G-B 
--------
Item Level: 71
--------
Unidentified
;";

        private const string GlovesAssasinsMitts = @"Rarity: Rare
Death Nails
Assassin's Mitts
--------
Evasion Rating: 104
Energy Shield: 20
--------
Requirements:
Level: 58
Dex: 45
Int: 45
--------
Sockets: G 
--------
Item Level: 61
--------
+18 to Intelligence
+73 to maximum Life
+14% to Lightning Resistance
0.23% of Physical Attack Damage Leeched as Mana
";

        private const string JewelBlightCut = @"Rarity: Rare
Blight Cut
Cobalt Jewel
--------
Item Level: 68
--------
+8 to Strength and Intelligence
14% increased Spell Damage while Dual Wielding
19% increased Burning Damage
15% increased Damage with Wands
--------
Place into an allocated Jewel Socket on the Passive Skill Tree.Right click to remove from the Socket.
";

        private const string InfluencedWand = @"Rarity: Rare
Miracle Chant
Imbued Wand
--------
Wand
Physical Damage: 38-69 (augmented)
Critical Strike Chance: 7.00%
Attacks per Second: 1.50
--------
Requirements:
Level: 59
Int: 188
--------
Sockets: R B 
--------
Item Level: 70
--------
33% increased Spell Damage (implicit)
--------
Adds 10 to 16 Physical Damage
24% increased Fire Damage
14% increased Critical Strike Chance for Spells
Attacks with this Weapon Penetrate 10% Lightning Resistance
--------
Crusader Item
";

        private const string MagicWeapon = @"Rarity: Magic
Shadow Axe of the Boxer
--------
Two Handed Axe
Physical Damage: 42-62
Critical Strike Chance: 5.00%
Attacks per Second: 1.25
Weapon Range: 13
--------
Requirements:
Level: 33
Str: 80
Dex: 37
--------
Sockets: R-R 
--------
Item Level: 50
--------
11% reduced Enemy Stun Threshold
";

        private const string FracturedItem = @"Rarity: Rare
Invasion Track
Iron Greaves
--------
Armour: 6
--------
Sockets: B B 
--------
Item Level: 2
--------
10% increased Movement Speed (fractured)
+5 to maximum Life
Regenerate 1.9 Life per second
+8% to Cold Resistance
--------
Fractured Item
";

        private const string UniqueItemWithDifferentBases = @"Rarity: Unique
Wings of Entropy
Ezomyte Axe
--------
Two Handed Axe
Physical Damage: 144-217 (augmented)
Elemental Damage: 81-175 (augmented)
Chaos Damage: 85-177 (augmented)
Critical Strike Chance: 5.70%
Attacks per Second: 1.35
Weapon Range: 13
--------
Requirements:
Level: 62
Str: 140 (unmet)
Dex: 86
--------
Sockets: R-B-R 
--------
Item Level: 70
--------
7% Chance to Block Spell Damage
+11% Chance to Block Attack Damage while Dual Wielding
66% increased Physical Damage
Adds 81 to 175 Fire Damage in Main Hand
Adds 85 to 177 Chaos Damage in Off Hand
Counts as Dual Wielding
--------
Fire and Anarchy are the most reliable agents of change.";

        private const string WeaponWithMultipleElementalDamages = @"Rarity: Rare
Honour Beak
Ancient Sword
--------
One Handed Sword
Quality: +20% (augmented)
Physical Damage: 22-40 (augmented)
Elemental Damage: 26-48 (augmented), 47-81 (augmented), 4-155 (augmented)
Critical Strike Chance: 5.00%
Attacks per Second: 1.74 (augmented)
Weapon Range: 11
--------
Requirements:
Level: 50
Str: 44
Dex: 44
--------
Sockets: R-R B 
--------
Item Level: 68
--------
Attribute Modifiers have 8% increased Effect (enchant)
--------
+165 to Accuracy Rating (implicit)
--------
+37 to Dexterity
Adds 26 to 48 Fire Damage
Adds 47 to 81 Cold Damage
Adds 4 to 155 Lightning Damage
20% increased Attack Speed
+21% to Global Critical Strike Multiplier";

        private const string EnchantWithAdditionalProjectiles = @"Rarity: Rare
Doom Glance
Hubris Circlet
--------
Energy Shield: 111 (augmented)
--------
Requirements:
Level: 69
Int: 154
--------
Sockets: B-B 
--------
Item Level: 69
--------
Split Arrow fires 2 additional Projectiles (enchant)
--------
+26 to Intelligence
+4 to maximum Energy Shield
39% increased Energy Shield
+25 to maximum Life";

        #endregion
    }
}
