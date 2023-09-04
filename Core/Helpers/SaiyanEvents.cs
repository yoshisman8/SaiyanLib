using SaiyanLib.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace SaiyanLib.Core.Helpers
{
    /// <summary>
    /// A collection of events which are triggered at various times during SaiyanLib's runtime.
    /// </summary>
    public class SaiyanEvents
    {
        /// <summary>
        /// An event raised each time a Player's mastery increases.
        /// </summary>
        public event EventHandler<MasteryEventArgs> OnMasteryGain;
        /// <summary>
        /// An event raised each time a Player's mastery level increases.
        /// </summary>
        public event EventHandler<MasteryLevelArgs> OnMasteryLevelIncrease;
        /// <summary>
        /// An event raised each time a player's ki changes, both increases and decreases.
        /// </summary>
        public event EventHandler<KiChageArgs> OnKiChange;
        /// <summary>
        /// An event which is invoked each time a transformation's "CanTransform" method is ran.
        /// <para>Calling <u>CanTransformArgs.StopTransformation()</u> inside your event listener will stop the transformation from occuring.</para>
        /// </summary>
        public event EventHandler<CanTransformArgs> OnCanTransform;
        /// <summary>
        /// An event which is invoked each time a node in the transformation chain is checked for visibility.
        /// <para>Calling <u>CanSeeNodeArgs.HideNode()</u> inside your event listener will cause the node to be hidden.</para>
        /// </summary>
        public event EventHandler<CanSeeNodeArgs> OnCanSeeNode;
        /// <summary>
        /// An event which is invoked each time a transformation is checked for visibility.
        /// <para>Calling <u>CanSeeChainArgs.HideNode()</u> inside your event listener will cause the node to be hidden.</para>
        /// </summary>
        public event EventHandler<CanSeeChainArgs> OnCanSeeChain;

        internal virtual void InvokeMasteryGain(MasteryEventArgs args)
        {
            OnMasteryGain?.Invoke(this, args);
        }
        internal virtual void InvokeOnMasteryLevelIncrease(MasteryLevelArgs args)
        {
            OnMasteryLevelIncrease?.Invoke(this, args);
        }
        internal virtual void InvokeKiChange(KiChageArgs args)
        {
            OnKiChange?.Invoke(this, args);
        }
        internal virtual bool InvokeCanTransform(CanTransformArgs args)
        {
            OnCanTransform?.Invoke(this, args);
            return args.CanTransform;
        }
        internal virtual bool InvokeCanSeeNode(CanSeeNodeArgs args)
        {
            OnCanSeeNode?.Invoke(this, args);
            return args.CanSee;
        }
        internal virtual bool InvokeCanSeeChain(CanSeeChainArgs args)
        {
            OnCanSeeChain?.Invoke(this, args);
            return args.CanSee;
        }
    }

    public class MasteryEventArgs : EventArgs
    {
        public Player Player { get; protected set; }
        public SaiyanPlayer SaiyanPlayer { get; protected set; }
        public float Amount { get; protected set; }
        public MasteryEventArgs(Player player, float amount) 
        {
            Player = player;
            SaiyanPlayer = player.GetModPlayer<SaiyanPlayer>();
            Amount = amount;
        }
    }
    public class MasteryLevelArgs : EventArgs
    {
        public Player Player { get; protected set; }
        public SaiyanPlayer SaiyanPlayer { get; protected set; }
        public int PreviousLevel { get; protected set; }
        public int CurrentLevel { get; protected set; }
        public int LevelsGained { get; protected set; }
        public MasteryLevelArgs(Player player, int PrevLevel, int CurrLevel, int LvsGained)
        {
            Player = player;
            SaiyanPlayer = player.GetModPlayer<SaiyanPlayer>();
            PreviousLevel = PrevLevel;
            CurrentLevel = CurrLevel;
            LevelsGained = LvsGained;
        }
    }
    public class KiChageArgs : EventArgs
    {
        public Player Player { get; protected set; }
        public SaiyanPlayer SaiyanPlayer { get; protected set; }
        public float Amount { get; protected set; }
        public KiChageArgs(Player player, float amount)
        {
            Player = player;
            SaiyanPlayer = player.GetModPlayer<SaiyanPlayer>();
            Amount = amount;
        }
    }
    public class CanTransformArgs : EventArgs
    {
        public Player Player { get; protected set; }
        public SaiyanPlayer SaiyanPlayer { get; protected set; }
        public Transformation Transformation { get; protected set; }
        public bool CanTransform => !Overrides.Any();
        internal List<bool> Overrides { get; private set; } = new();
        public CanTransformArgs(Player player, Transformation transformation)
        {
            Player = player;
            Transformation = transformation;
            SaiyanPlayer = player.GetModPlayer<SaiyanPlayer>();
        }
        public void StopTransformation()
        {
            Overrides.Add(true);
        }
    }
    public class CanSeeNodeArgs : EventArgs
    {
        public Player Player { get; protected set; }
        public SaiyanPlayer SaiyanPlayer { get; protected set; }
        public string TransformationID { get; protected set; }
        public string ChainID { get; protected set; }
        public bool CanSee => !Overrides.Any();
        internal List<bool> Overrides { get; private set; } = new();
        public CanSeeNodeArgs(Player player, string chainID, string transformationID)
        {
            Player = player;
            TransformationID = transformationID;
            ChainID = chainID;
            SaiyanPlayer = player.GetModPlayer<SaiyanPlayer>();
        }
        public void HideNode()
        {
            Overrides.Add(true);
        }
    }
    public class CanSeeChainArgs : EventArgs
    {
        public Player Player { get; protected set; }
        public SaiyanPlayer SaiyanPlayer { get; protected set; }
        public string ChainID { get; protected set; }
        public bool CanSee => !Overrides.Any();
        internal List<bool> Overrides { get; private set; } = new();
        public CanSeeChainArgs(Player player, string chainID)
        {
            Player = player;
            ChainID = chainID;
            SaiyanPlayer = player.GetModPlayer<SaiyanPlayer>();
        }
        public void HideChain()
        {
            Overrides.Add(true);
        }
    }
}
