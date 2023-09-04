using Microsoft.Xna.Framework;
using SaiyanLib.Core;
using SaiyanLib.Core.Helpers;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SaiyanLib.Abstracts
{
    public abstract class Transformation : ModBuff
    {
        /// <summary>
        /// Localized name for the transformation.
        /// </summary>
        public virtual LocalizedText TransformationName => this.GetLocalization("TransformationName", base.PrettyPrintName);

        /// <summary>
        /// Localized description for the transformation.
        /// </summary>
        public virtual LocalizedText TransformationDescription => this.GetLocalization("TransformationDescription", base.PrettyPrintName);

        /// <summary>
        /// Time in seconds it takes to enter this transformation. Defaults to 3 seconds.
        /// </summary>
        public float TransformationTime { get; set; } = 3f;
        /// <summary>
        /// The color of the text that pops up when you enter this transformation.
        /// </summary>
        public Color TextColor { get; set; } = Color.White;

        /// <summary>
        /// Color to change your Ki to while this transformation is active. Defaults to null (No change).
        /// </summary>
        public Color? KiColor { get; set; } = null;
        /// <summary>
        /// <para>Whether this form can be stacked with other forms. Defaults to false.</para>
        /// </summary>
        public bool IsStackable { get; set; } = false;

        /// <summary>
        /// <para>How much Ki <b>PER SECOND</b> is used by this form.</para>
        /// <para>When set to 0f, the form does not consume Ki.</para>
        /// </summary>
        public float KiUsage { get; set; } = 0f;

        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = false;

            ConfigureTransformation();
            TransformationHelper.RegisterTransformation($"{FullName}", (TransformationName, Type, IsStackable));
        }

        /// <summary>
        /// A method called during SetStaticDefaults() <b>BEFORE</b> the transformation is registered.
        /// <para>Override this instead of SetStaticDefaults() to safely configure aspects of your transformation!</para>
        /// </summary>
        public abstract void ConfigureTransformation();

        /// <summary>
        /// Virtual method which is called every frame while the player is starting to transform. Only runs on transformations with an actual transformation duration. Does nothing by default.
        /// </summary>
        /// <param name="player"></param>
        public virtual void OnTransform(Player player) { }
        /// <summary>
        /// Virtual method which is called after the player successfully enters this transformation. Does nothing by default.
        /// </summary>
        /// <param name="player"></param>
        public virtual void OnPostTransform(Player player) { }
        /// <summary>
        /// Virtual method which is called after the user exits this transformation. Does nothing by default.
        /// </summary>
        /// <param name="player"></param>
        public virtual void OnEndTransformation(Player player) { }

        
        /// <summary>
        /// Method called each frame the player has this transformation active.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="saiyanPlayer"></param>
        /// <param name="buffIndex"></param>
        public abstract void UpdateTransformation(Player player, SaiyanPlayer saiyanPlayer, ref int buffIndex);

        /// <summary>
        /// Method called each time the user attempts to start this transformation. 
        /// Can be used to check for unlock conditions, flags, timers, cooldowns or any other special mechanic that would allow or prevent usage of this transformation.
        /// <para>Returns true by default.</para>
        /// </summary>
        /// <param name="player">The player attempting this transformation.</param>
        /// <param name="saiyanPlayer">The SaiyanPlayer instance for the player.</param>
        /// <returns><para>True if the user can start this transformation.</para><para>False if the user should not be able to start transforming.</para></returns>
        public virtual bool CanTransform(Player player, SaiyanPlayer saiyanPlayer) { return true; }

        /// <summary>
        /// Method called to determine whether this transformation should be visible in the transformation menu.
        /// <para>Returns true by default.</para>
        /// </summary>
        /// <param name="player">The player attempting this transformation.</param>
        /// <param name="saiyanPlayer">The SaiyanPlayer instance for the player.</param>
        /// <returns><para>True if the user can see this transformation in the transformation menu.</para><para>False if the user should not be able to see it.</para></returns>
        public virtual bool CanSeeTransformation(Player player, SaiyanPlayer saiyanPlayer) { return true; }

        internal bool ProcessTransformationCodnitions(Player player, SaiyanPlayer saiyanPlayer)
        {
            return CanTransform(player, saiyanPlayer) && SaiyanLib.Events.InvokeCanTransform(new(player, this));
        }
        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);

            SaiyanPlayer saiyanPlayer = player.GetModPlayer<SaiyanPlayer>();

            if (!IsStackable)
            {
                if (saiyanPlayer.IsAnythingBut(Type))
                {
                    saiyanPlayer.ClearTransformations();
                }
            }

            if (KiUsage > 0f)
            {
                if (saiyanPlayer.Ki < SaiyanHelpers.KiHelpers.KiPerFrame(KiUsage))
                {
                    player.DelBuff(buffIndex);
                    return;
                }
                else
                {
                    saiyanPlayer.Ki -= SaiyanHelpers.KiHelpers.KiPerFrame(KiUsage);
                }
            }

            player.buffTime[buffIndex] = 10;
            
            UpdateTransformation(player, saiyanPlayer, ref buffIndex);
        }
    }
}
