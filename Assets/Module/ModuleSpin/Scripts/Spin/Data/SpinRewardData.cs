using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spin
{
    /// <summary>
    /// Represents a single spin reward configuration.
    /// </summary>
    [Serializable]
    public class SpinReward : RewardBaseData
    {
        [Tooltip("The probability weight (percent chance) of this reward appearing.")]
        [Range(1, 100)]
        public int Percent;
    }

    /// <summary>
    /// Stores a list of possible spin rewards as a ScriptableObject.
    /// </summary>
    [CreateAssetMenu(fileName = "SpinRewardData", menuName = "Spin/Reward Data", order = 0)]
    public class SpinRewardData : ScriptableObject
    {
        [Header("Spin Reward Settings")]
        [Tooltip("List of all possible rewards that can appear on the spin wheel.")]
        public List<SpinReward> Rewards = new List<SpinReward>();

        /// <summary>
        /// Gets the reward configuration by id.
        /// </summary>
        /// <param name="index">The reward id in the list.</param>
        /// <returns>The reward object, or null if id is invalid.</returns>
        public SpinReward GetRewardByIndex(int index)
        {
            if (index < 0 || index >= Rewards.Count)
            {
                Debug.LogWarning($"[Spin] Invalid reward id: {index}");
                return null;
            }

            return Rewards[index];
        }
    }
}