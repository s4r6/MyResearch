using System.Collections.Generic;
using System.Linq;
using Domain.Stage.Object;

namespace Infrastructure.Repository
{
    public class ActionLabelData
    {
        public string ActionId;
        public string Label;
    }

    public class ActionLabelRepository
    {
        private readonly Dictionary<string, string> _actionLabels;

        public ActionLabelRepository(IEnumerable<ActionLabelData> actionDataList)
        {
            _actionLabels = actionDataList.ToDictionary(x => x.ActionId, x => x.Label);
        }

        public string GetLabelByActionId(string actionId)
        {
            return _actionLabels.TryGetValue(actionId, out var label) ? label : null;
        }
    }
}