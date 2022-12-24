using System.Collections.Generic;
using System.ComponentModel;
using PlayerRoles;
using PluginAPI.Core.Items;
using UnityEngine;

namespace SpyUtilityNW.Spies
{
    public class CiSpyBase : SpyBase
    {
        
        /// <summary>
        /// Default ammo for spy
        /// </summary>
        [Description("Default items for spy")]
        public override List<ItemType> SpyStartingItems { get; set; } = new()
        {
            ItemType.ArmorHeavy,
            ItemType.Radio,
            ItemType.GrenadeHE,
            ItemType.Medkit,
            ItemType.Adrenaline,
            ItemType.GunE11SR,
            ItemType.KeycardNTFLieutenant,
            ItemType.KeycardChaosInsurgency
        };

        /// <summary>
        /// Default ammo for spy
        /// </summary>
        [Description("Default ammo for spy")]
        public override Dictionary<ItemType, ushort> SpyStartingAmmo { get; set; } = new()
        {
            {ItemType.Ammo9x19, 40},
            {ItemType.Ammo556x45, 120},
        };

        /// <summary>
        /// Override spawn position
        /// </summary>
        [Description("Override spy spawn position (Vector3). I DO NOT RECOMMEND.")]
        public override Vector3 SpawnPosition { get; set; } = Vector3.zero;
        
        /// <summary>
        /// Spies fake role, role that is used in change appearance
        /// </summary>
        [Description("The fake role the spy will be, aka, what they will look like until exposed")]
        public override RoleTypeId SpyFakeRole { get; set; } = RoleTypeId.NtfSergeant;
        
        /// <summary>
        /// Spies real role, role that is shown when revealed.
        /// </summary>
        [Description("The real role the spy will be, aka, what they will look like after exposed")]
        public override RoleTypeId SpyRealRole { get; set; } = RoleTypeId.ChaosRifleman;
    }
}