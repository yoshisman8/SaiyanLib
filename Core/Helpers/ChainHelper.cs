using SaiyanLib.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace SaiyanLib.Core.Helpers
{
    public static class ChainHelper
    {
        public static Dictionary<string, Chain> Chains { get; private set; } = new();

        internal static List<(string ChainID, string TransformationID, Func<Player, bool> CanSee, int Index)> Insertions = new();


        /// <summary>
        /// Registers a new chain to the system. Once added, it cannot be removed in runtime.
        /// </summary>
        /// <param name="mod">Your Mod's instance.</param>
        /// <param name="ChainID">ID of the chain. Must be unique within your own mod.</param>
        /// <param name="CanSee">Function ran to check if the player can see this chain in their transformation menu.</param>
        /// <exception cref="InvalidCharacterException">Thrown if ChainID contains the character '/'.</exception>
        public static void RegisterChain(Mod mod, string ChainID, Func<Player,bool> CanSee)
        {
            if (ChainID.Contains('/')) throw new InvalidCharacterException("Invalid Character: Chain IDs cannot contain the character '/' in them.");
            Chains.Add($"{mod.Name}/{ChainID}", new($"{mod.Name}/{ChainID}", CanSee));
        }

        /// <summary>
        /// Registers a node into a transformation tree. 
        /// <para>NodeIDs are typically [ModName]/[ChainName].</para>
        /// <para>TransformationIDs are always [ModName]/[TransforamtionClassName]</para>
        /// <para>Insertions are processed in ModLoad order. If you wish to insert a node into a chain from another mod, consider setting your mod to load AFTER the one you are inserting to.</para>
        /// </summary>
        /// <param name="chainID">The ID of the chain.</param>
        /// <param name="transformationID">The ID of the transformation.</param>
        /// <param name="canSee">Delegate which checks if you can see this transformation or not.</param>
        /// <param name="index">Position on the chain to insert the transformation into. Defaulst to inserting at the end of the chian.</param>
        public static void RegisterNode(string chainID, string transformationID, Func<Player, bool> canSee, int index = -1)
        {
            Insertions.Add(new(chainID, transformationID, canSee, index));
        }
        internal static void ProcessInsertions()
        {
            foreach (var entry in Insertions)
            {
                if (Chains.ContainsKey(entry.ChainID))
                {
                    if (TransformationHelper.Transformations.ContainsKey(entry.TransformationID))
                    {
                        Chains[entry.ChainID].Append(entry.TransformationID, entry.CanSee, entry.Index);
                    }
                }
            }
            Insertions.Clear();
        }
        internal static void ClearTransformationChains()
        {
            Chains.Clear();
        }
    }
    public class Chain
    {
        public string ChainID { get; private set; }
        public List<ChainNode> Nodes { get; internal set; } = new();
        public Func<Player, bool> CanSeeNode { get; private set; }

        public Chain(string chainID, Func<Player, bool> canSeeNode)
        {
            ChainID = chainID;
            CanSeeNode = canSeeNode;
        }

        public void Append(string TransformationID, Func<Player, bool> CanSee, int index = -1)
        {
            if (index > -1)
            {
                Nodes.Insert(Math.Min(index, Nodes.Count-1), new ChainNode(TransformationID,CanSee));
            }
            else
            {
                Nodes.Add(new(TransformationID, CanSee));
            }
        }

        public bool ProcessVisibility(Player player)
        {
            return CanSeeNode(player) && SaiyanLib.Events.InvokeCanSeeChain(new CanSeeChainArgs(player, ChainID));
        }

        public bool ProcessNodeVisibility(Player player, string nodeID)
        {
            if (Nodes.Any(x => x.TransformationID == nodeID))
            {
                return Nodes.First(x => x.TransformationID == nodeID).CanSee(player) && SaiyanLib.Events.InvokeCanSeeNode(new(player, ChainID, nodeID));
            }
            else return false;
        }
    }
    public class ChainNode
    {
        public string TransformationID { get; private set; }
        public Func<Player, bool> CanSee { get; private set; }

        /// <summary>
        /// A node inside of a transformation chain.
        /// </summary>
        /// <param name="transformation">The Transformation to be added.</param>
        /// <param name="canSee">Delegate function ran to see whether the user can see this node.</param>
        public ChainNode(string transformation, Func<Player,bool> canSee)
        {
            TransformationID = transformation;
            CanSee = canSee;
        }
    }

    public class InvalidCharacterException : Exception
    {
        public InvalidCharacterException(string message) : base(message) { }
    }
}
