using SaiyanLib.Abstracts;
using SaiyanLib.Core.Config;
using SaiyanLib.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace SaiyanLib.Core
{
    public class SaiyanPlayer : ModPlayer
    {
        /// <summary>
        /// The player's current Ki.
        /// </summary>
        public float Ki;
        /// <summary>
        /// Returns your maximu amount of Ki. 
        /// Decimals are always truncated.
        /// <para>Starting value is normally 100. Can be changed in server config.</para>
        /// </summary>
        /// <returns></returns>
        public int GetMaxKi() => (int)MaxKi.ApplyTo(SaiyanLib.ServerConfig.StartingKi);

        /// <summary>
        /// Adds to current Ki.
        /// <para>Negative values are modified by SaiyanPlayer.KiUsage</para>
        /// <para>Reducing ki also adds mastery.</para>
        /// </summary>
        /// <param name="amount"></param>
        public void AddKi(float amount)
        {
            if (amount < 0)
            {
                amount = KiUsage.ApplyTo(amount);
                GainMastery();
            }

            Ki += amount;

            SaiyanLib.Events.InvokeKiChange(new(Player, amount));
        }

        /// <summary>
        /// Modifiers to maximum ki.
        /// <para>Default values are:</para>
        /// <para>Additive: 1</para>
        /// <para>Multiplicative: 1</para>
        /// <para>Base: 100 (Can be changed in Server Config)</para>
        /// </summary>
        public StatModifier MaxKi = new(1,1);

        /// <summary>
        /// Modifiers to active ki charge rate per second. Ki charge rate is a % of your maximum Ki.
        /// <para>Default values are:</para>
        /// <para>Additive: 1</para>
        /// <para>Multiplicative: 1</para>
        /// <para>Base: 10% (Can be changed in Server Config)</para>
        /// </summary>
        public StatModifier KiChargeRate = new(SaiyanLib.ServerConfig.BaseKiChargeRate / 100f, 1f);

        /// <summary>
        /// Modifiers to passive ki renegeration per second.
        /// <para>Default values are:</para>
        /// <para>Additive: 0.01f </para>
        /// <para>Multiplicative: 1</para>
        /// <para>Flat: 0</para>
        /// </summary>
        public StatModifier KiRegenRate = new(SaiyanLib.ServerConfig.BaseKiRegenRate / 100f, 1f);

        /// <summary>
        /// How much Ki you actually spend each time AddKi() is used with a negative number. Defaults to 100%.
        /// <para>Negative values mean less ki is used each time.</para>
        /// <para>Positive values mear more ki is used each time.</para>
        /// </summary>
        public StatModifier KiUsage = new(1,1);

        /// <summary>
        /// How much mastery you gain each time. Defaults to 100%
        /// <para>Defaults to 100% mastery gain.</para>
        /// <para>Mastery is normally +0.0001% each tick of mastery gain.</para>
        /// </summary>
        public StatModifier KiMasteryRate = new(1,1);

        /// <summary>
        /// Mastery experience the player has. Starts at 0 for all players. Each 1000 experience, this value resets to 0.
        /// </summary>
        public float Mastery { get; private set; } = 0;

        /// <summary>
        /// Mastery level of the user. Starts at 1. Goes up automatically each time SaiyanPlayer.Mastery reaches 1000.
        /// </summary>
        public int MasteryLevel { get; private set; } = 1;

        public string Trait { get; private set; } = null;
        public string Chain { get; set; } = null;
        public string Transformation { get; set; } = null;

        public DateTime? TransformationTimer;

        public void GainMastery()
        {
            float gain = KiMasteryRate.ApplyTo(0.0001f);

            Mastery += gain;

            SaiyanLib.Events.InvokeMasteryGain(new(Player, gain));

            if (Mastery >= 1000)
            {
                int levels = (int)Math.Floor(Mastery/1000);

                MasteryLevel += levels;

                Main.NewText($"Your ki mastery level has reached {MasteryLevel}!");

                Mastery -= 1000 * levels;

                SaiyanLib.Events.InvokeOnMasteryLevelIncrease(new(Player, MasteryLevel - levels, MasteryLevel, levels));
            }
        }

        public override void OnEnterWorld()
        {
            base.OnEnterWorld();

            Ki = MaxKi.ApplyTo(SaiyanLib.ServerConfig.StartingKi);
        }

        public override void SaveData(TagCompound tag)
        {
            tag.Add("SaiyanLib_MasteryLevel", MasteryLevel);
            tag.Add("SaiyanLib_Mastery", Mastery);
            tag.Add("SaiyanLib_Trait", Trait);
            tag.Add("SaiyanLib_Chain", Chain);
            tag.Add("SaiyanLib_Transformation", Transformation);

        }
        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey("SaiyanLib_Mastery")) Mastery = tag.GetFloat("SaiyanLib_Mastery");
            if (tag.ContainsKey("SaiyanLib_MasteryLevel")) MasteryLevel= tag.GetInt("SaiyanLib_MasteryLevel");
            if (tag.ContainsKey("SaiyanLib_Trait")) Trait = tag.GetString("SaiyanLib_Trait");
            if (tag.ContainsKey("SaiyanLib_Chain")) Chain = tag.GetString("SaiyanLib_Chain");
            if (tag.ContainsKey("SaiyanLib_Transformation")) Transformation = tag.GetString("SaiyanLib_Transformation");
        }

        public override void PostUpdateBuffs()
        {
            // Only process ki regen if the player is not already at maximum Ki.
            if (Ki < GetMaxKi())
            {
                Ki += SaiyanHelpers.KiHelpers.KiPerFrame(KiRegenRate.ApplyTo(GetMaxKi()));

                if (SaiyanLib.ChargeKey.Current)
                {
                    Ki += SaiyanHelpers.KiHelpers.KiPerFrame(KiChargeRate.ApplyTo(GetMaxKi()));
                }
            }


        }

        public override void ResetEffects()
        {
            if (Ki > GetMaxKi()) Ki = GetMaxKi();
            if (Ki < 0) Ki = 0;

            KiChargeRate = new(SaiyanLib.ServerConfig.BaseKiChargeRate / 100f, 1f);
            KiRegenRate = new(SaiyanLib.ServerConfig.BaseKiRegenRate / 100f, 1f);
            KiUsage = new(1f, 1f);
            MaxKi = new(1f + (MasteryLevel * 0.02f), 1f);
            KiUsage = new(1f - (MasteryLevel * 0.02f), 1f);
        }

        #region Transformation Handlers
        /// <summary>
        /// Checks whether this player has an active transformation.
        /// </summary>
        /// <param name="IngoreStackables">Whether to ignore stackable transformations.</param>
        /// <returns>True or False boolean denoting whether the player is transformed or not.</returns>
        public bool IsTransformed(bool IngoreStackables = true)
        {
            foreach (var T in TransformationHelper.Transformations.Values)
            {
                if (T.IsStackable && IngoreStackables) continue;

                if (Player.HasBuff(T.Type)) return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the playter's active non-stackable tranformation.
        /// </summary>
        /// <returns>Tranformation object if the user is transformed. Or Null if they are not transformed.</returns>
        public string GetCurrentTransformation()
        {
            foreach (var T in TransformationHelper.Transformations)
            {
                if (T.Value.IsStackable) continue;

                if (Player.HasBuff(T.Value.Type)) return T.Key;
            }
            return null;
        }
        /// <summary>
        /// Returns whether the player is currently under another non-stackable transformation other than the passed transformation.
        /// </summary>
        /// <param name="transformation">Transformation to be checked against</param>
        /// <returns>True if this player has 2 or more non-stackable tranformations. False otherwise.</returns>
        public bool IsAnythingBut(int transformation)
        {
            var forms = TransformationHelper.FetchNonStackingTransformations(transformation);
            if (forms.Intersect(Player.buffType).Any()) return true;
            else return false;
        }

        /// <summary>
        /// Returns a list of all stackable tranformations active on the player.
        /// </summary>
        /// <returns>A list of all active stackable tranformations. Otherwise returns an empty list.</returns>
        public List<string> GetStackableTransformations()
        {
            var forms = TransformationHelper.FetchStackingTransformations().Intersect(Player.buffType);
            return forms.Select(TransformationHelper.GetTransformation).ToList();
        }

        /// <summary>
        /// Clears all transformations on the player.
        /// </summary>
        public void ClearTransformations(bool IgnoreStackables = true)
        {
            foreach (var T in TransformationHelper.Transformations.Values)
            {
                if (Player.HasBuff(T.Type))
                {
                    if (T.IsStackable && IgnoreStackables) continue;
                    Player.DelBuff(Player.FindBuffIndex(T.Type));
                }

            }
        }
        #endregion
        #region TraitHandlers
        /// <summary>
        /// Ran each frame to ensure the player has a trait and has the buff mandated by said trait (if enabled).
        /// </summary>
        public void Traitcheck()
        {

        }
        #endregion
    }
}
