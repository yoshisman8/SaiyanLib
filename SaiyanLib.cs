using SaiyanLib.Abstracts;
using SaiyanLib.Core;
using SaiyanLib.Core.Config;
using SaiyanLib.Core.Helpers;
using System.Net.Security;
using Terraria.ID;
using Terraria.ModLoader;

namespace SaiyanLib
{
    public class SaiyanLib : Mod
	{
        public static SaiyanServerConfig ServerConfig { get; private set; }
        public static ModKeybind ChargeKey { get; private set; }
        public static ModKeybind PowerDown { get; private set; }
        public static ModKeybind TransformationMenu { get; private set; }
        /// <summary>
        /// A collection of events which are triggered at various times during SaiyanLib's runtime.
        /// </summary>
        public static SaiyanEvents Events { get; private set; }

        public override void Load()
        {
            Events = new();
            ChargeKey = KeybindLoader.RegisterKeybind(this, "ChargeKey", Microsoft.Xna.Framework.Input.Keys.LeftShift);
            PowerDown = KeybindLoader.RegisterKeybind(this, "PowerDown", Microsoft.Xna.Framework.Input.Keys.V);
            TransformationMenu = KeybindLoader.RegisterKeybind(this, "PowerDown", Microsoft.Xna.Framework.Input.Keys.Z);
        }
        public override void PostSetupContent()
        {
            ServerConfig = SaiyanServerConfig.GetInstance();
            ChainHelper.ProcessInsertions();
        }
        
        public override void Unload()
        {
            TransformationHelper.ClearTransformationCache();
            ChainHelper.ClearTransformationChains();
            ChargeKey = null;
            PowerDown = null;
            ServerConfig = null;
            Events = null;
        }
    }

}