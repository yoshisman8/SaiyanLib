using Microsoft.Xna.Framework;
using SaiyanLib.Core;
using SaiyanLib.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace SaiyanLib.Abstracts
{
    public abstract class Trait : ModBuff
    {
        /// <summary>
        /// List of options a Trait can have to change your Ki Color. if left empty, the trait will not affect Ki Color.
        /// <para>This is unaffected by the Silent property.</para>
        /// </summary>
        public Color[] KiColors;

        /// <summary>
        /// Determines the visibility of this trait.
        /// <para>When set to true, this trait will not grant a buff to the player.</para>
        /// </summary>
        public bool Silent = false;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = false;

            ConfigureTrait(); 

        }
        /// <summary>
        /// A method called during SetStaticDefaults() <b>BEFORE</b> the trait is registered.
        /// <para>Override this instead of SetStaticDefaults() to safely configure aspects of your trait!</para>
        /// </summary>
        public abstract void ConfigureTrait();

        public abstract void UpdateTrait(Player player, SaiyanPlayer saiyanPlayer, ref int buffIndex);

        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);


        }
    }
}
