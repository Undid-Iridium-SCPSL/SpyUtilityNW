using System;
using System.Collections.Generic;
using PlayerRoles;
using UnityEngine;

namespace SpyUtilityNW.Spies
{
    public abstract class SpyBase
    {
        public abstract List<ItemType> SpyStartingItems { get; set; }
        
        public abstract Dictionary<ItemType, ushort> SpyStartingAmmo { get; set; }
        
        public abstract Tuple<float, float, float> SpawnPosition { get; set; }
        
        public abstract RoleTypeId SpyFakeRole { get; set; }
        
        public abstract RoleTypeId SpyRealRole { get; set; }
    }
}