using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace SaiyanLib.Core.Config
{
    public class SaiyanServerConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;
        public static SaiyanServerConfig GetInstance() { return ModContent.GetInstance<SaiyanServerConfig>(); }


        [Header("Ki System")]
        [DefaultValue(100)]
        [ReloadRequired]
        public int StartingKi;

        [DefaultValue(10f)]
        [Range(0f, 100f)]
        [ReloadRequired]
        public float BaseKiChargeRate;

        [DefaultValue(1f)]
        [Range(0f, 100f)]
        [ReloadRequired]
        public float BaseKiRegenRate;

        [DefaultValue(false)]
        [ReloadRequired]
        public bool ForceInstantTransformations;
    }
}
