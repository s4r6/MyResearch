using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Domain.Stage.Object
{
    public class ObjectEntity
    {
        public string ObjectId;
        public string DisplayName;
        public string Description;
        public List<string> availableActionIds;

        public ObjectEntity(string ObjectId, string DisplayName, string Description, List<string> availableActionIds)
        {
            this.ObjectId = ObjectId;
            this.DisplayName = DisplayName; 
            this.Description = Description;
            this.availableActionIds = availableActionIds;
        }

        public List<string> GetAvailableActionIds(string heldObjectId)
        {
            return availableActionIds;
        }

        public virtual bool IsInspectable => false;
    }
}