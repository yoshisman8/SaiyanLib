using SaiyanLib.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SaiyanLib.Core.Helpers
{
    public static class TransformationHelper
    {
        public static Dictionary<string, (LocalizedText Name, int Type, bool IsStackable)> Transformations { get; private set; } = new();

        /// <summary>
        /// Register a transformation to the backed. 
        /// <para>YOU NORMALLY DO NOT HAVE TO USE THIS METHOD! AS ALL TRANSFORMATIONS AUTOMATICALLY CALL THIS METHOD ON THEIR OWN!</para>
        /// </summary>
        /// <param name="ID">The {FullName} property of your Transformation class.</param>
        /// <param name="Data">The Type and IsStackable properties of your Transformation class.</param>
        public static void RegisterTransformation(string ID, (LocalizedText Name, int Type, bool IsStackable)Data)
        {
            Transformations.Add(ID, Data);
        }

        /// <summary>
        /// Fetches a transformation by its ID, which typically the name of the mod that adds it + the class name of the transformation class.
        /// </summary>
        /// <param name="ID">The id of the transformation.</param>
        /// <returns>The Transformation instance.</returns>
        public static Transformation GetTransformation(string ID)
        {
            if (!Transformations.ContainsKey(ID)) return null;
            return ModContent.GetModBuff(Transformations[ID].Type) as Transformation;
        }
        /// <summary>
        /// Fetches a transformation by its Type.
        /// </summary>
        /// <param name="Type">The Type of the transformation.</param>
        /// <returns>The Transformation ID.</returns>
        public static string GetTransformation(int Type)
        {
            if (Transformations.Any(x => x.Value.Type == Type)) return Transformations.Where(x => x.Value.Type == Type).FirstOrDefault().Key;
            return null;
        }

        /// <summary>
        /// Fetches all Non-Stacking transformations by their Type.
        /// <para>Used for quickly checking for form overlap.</para>
        /// </summary>
        /// <param name="exclude">Optional array of buff types to excluse from the resulting list.</param>
        /// <returns>A list of all Types for all registered transformations.</returns>
        public static List<int> FetchNonStackingTransformations(params int[] exclude)
        {
            return Transformations.Values.Where(x=> !x.IsStackable && !exclude.Contains(x.Type)).Select(x=>x.Type).ToList();
        }

        /// <summary>
        /// Fetches all stacking transformations by their Type
        /// </summary>
        /// <param name="exclude">Optionall array of buff types to exclude from the resulting list.</param>
        /// <returns>A list of all Types for all registered transformations.</returns>
        public static List<int> FetchStackingTransformations(params int[] exclude)
        {
            return Transformations.Values.Where(x => x.IsStackable && !exclude.Contains(x.Type)).Select(x => x.Type).ToList();
        }

        internal static void ClearTransformationCache()
        {
            Transformations.Clear();
        }
    }

}
