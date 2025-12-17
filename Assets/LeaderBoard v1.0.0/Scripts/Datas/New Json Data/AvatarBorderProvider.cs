using UnityEngine;
using System.Collections.Generic;

namespace ps.modules.leaderboard
{
    public class AvatarBorderProvider : LeaderBoardCtrBase
    {

        [Header("Avatar Sprites")]
        public List<Sprite> avatarList;

        [Header("Border Sprites")]
        public List<Sprite> borderList;

        [Header("Default Sprites")]
        public Sprite defaultAvatar;
        public Sprite defaultBorder;


        // ============================================================
        // GET AVATAR SPRITE
        // ============================================================
        public Sprite GetAvatar(int index)
        {
            if (avatarList == null || avatarList.Count == 0)
                return defaultAvatar;

            if (index < 0 || index >= avatarList.Count)
                return defaultAvatar;

            return avatarList[index] ?? defaultAvatar;
        }

        // ============================================================
        // GET BORDER SPRITE
        // ============================================================
        public Sprite GetBorder(int index)
        {
            if (borderList == null || borderList.Count == 0)
                return defaultBorder;

            if (index < 0 || index >= borderList.Count)
                return defaultBorder;

            return borderList[index] ?? defaultBorder;
        }
    }
}

