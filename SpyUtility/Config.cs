using System.Collections.Generic;
using System.ComponentModel;

namespace SpyUtilityNW
{
    public class Config
    {
        /// <summary>
        /// ah
        /// </summary>
        [Description("Probability of CISpy's per wave.")]
        public Dictionary<int, float> probabilityOfCISpy { get; set; } = new()
        {
            {1, 100f},
            {2, 50f},
            {3, 5f}
        };

        /// <summary>
        /// ah
        /// </summary>
        [Description("Probability of MTF spy per wave.")]
        public Dictionary<int, float> probabilityOfMtfSpy { get; set; } = new()
        {
            {1, 70f},
            {2, 50f},
            {3, 5f}
        };
        


        public bool IsEnabled { get; set; } = true;

        /// <summary>
        ///     Gets or sets a value indicating whether debug logs should be shown.
        /// </summary>
        [Description("Whether debug logs should be shown.")]
        public bool Debug { get; set; } = false;
        
        /// <summary>
        ///     Gets or sets a value indicating whether debug logs should be shown.
        /// </summary>
        [Description("Whether debug logs should be shown.")]
        public int HowManyRetries { get; set; } = 3;
    }
}