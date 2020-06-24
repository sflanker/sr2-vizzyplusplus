using System;
using System.Collections.Generic;
using ModApi.Craft.Program;
using ModApi.Flight.Sim;
using UnityEngine;

namespace Assets.Scripts.Vizzy.CraftInformation {
    public class ClosestApproachExpression : ProgramExpression {
        public const String XmlName = "ClosestApproach";

        public override bool IsBoolean => false;

        public override List<ListItemInfo> GetListItems(string listId) {
            return new List<ListItemInfo> {
                new ListItemInfo(
                    "time-of",
                    "Time of",
                    "Time of next Closest Approach.",
                    ListItemInfoType.Number),
                new ListItemInfo(
                    "distance-at",
                    "Distance at",
                    "Distance at next Closest Approach.",
                    ListItemInfoType.Number)
            };
        }
    }
}
