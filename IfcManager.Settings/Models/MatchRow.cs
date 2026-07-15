using System.Collections.Generic;

namespace IfcManager.Settings.Models
{
    public class MatchRow
    {
        public List<MatchCriterion> Criteria { get; set; }
            = new();

        public string TargetValue { get; set; }
            = string.Empty;
    }
}