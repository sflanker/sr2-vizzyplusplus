using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Assets.Scripts.Flight.Sim;
using ModApi.Craft.Program;
using UnityEngine;

namespace Assets.Scripts.Vizzy.CraftInformation {
    [Serializable]
    public class GetCraftIdExpression : ProgramExpression {
        public const String LegacyXmlName = "TargetCraftId";
        public const String XmlName = "GetCraftId";

        [ProgramNodeProperty] private String _type;
        private CraftType _craftType;

        private static readonly ExpressionResult NoTargetResult =
            new ExpressionResult {
                NumberValue = -1
            };

        public override bool IsBoolean => false;

        public override List<ListItemInfo> GetListItems(string listId) {
            return new List<ListItemInfo> {
                new ListItemInfo(
                    "current",
                    "Current",
                    "Gets the ID of the craft that is currently running this program.",
                    ListItemInfoType.None),
                new ListItemInfo(
                    "target",
                    "Target",
                    "Gets the target craft ID if a craft is currently targeted (returns -1 if no craft is targeted).",
                    ListItemInfoType.None),
                new ListItemInfo(
                    "active",
                    "Active",
                    "Gets the ID of the craft that is currently controlled by the player.",
                    ListItemInfoType.None)
            };
        }

        /// <summary>Gets the selected value of the specified list.</summary>
        /// <param name="listId">The list identifier.</param>
        /// <returns>The currently selected value.</returns>
        public override string GetListValue(string listId) {
            return this._type;
        }

        /// <summary>Sets the selected value of the specified list.</summary>
        /// <param name="listId">The list identifier.</param>
        /// <param name="value">The value to select.</param>
        public override void SetListValue(String listId, String value) {
            this._type = value;
            this._craftType =
                Enum.TryParse<CraftType>(value, ignoreCase: true, result: out var craftType) ?
                    craftType :
                    default;
        }

        public override ExpressionResult Evaluate(IThreadContext context) {
            switch (this._craftType) {
                case CraftType.Current:
                    Debug.Log($"Returning current craft id: {context.Craft.CraftScript.CraftNode.NodeId}");
                    return new ExpressionResult {
                        NumberValue = context.Craft.CraftScript.CraftNode.NodeId
                    };
                case CraftType.Target:
                    if (context.Craft.Data.NavSphereTarget is CraftNode targetCraft) {
                        Debug.Log($"Returning target craft id: {targetCraft.NodeId}");
                        return new ExpressionResult {
                            NumberValue = targetCraft.NodeId
                        };
                    }

                    break;
                case CraftType.Active when Game.InFlightScene:
                    Debug.Log($"Returning active craft id: {Game.Instance.FlightScene.CraftNode.NodeId}");
                    return new ExpressionResult {
                        NumberValue = Game.Instance.FlightScene.CraftNode.NodeId
                    };
                case CraftType.Active when !Game.InFlightScene:
                    Debug.Log($"Unable to get Active Craft ID when not in flight scene.");
                    break;
                default:
                    Debug.Log($"Unrecognized craft type: {this._type}");
                    break;
            }

            return NoTargetResult;
        }

        public override void OnDeserialized(XElement xml) {
            base.OnDeserialized(xml);
            // Default to target for legacy reasons
            this.SetListValue("type", this._type ?? "target");
        }

        public override void OnSerialized(XElement xml) {
            base.OnSerialized(xml);
            xml.Name = XName.Get(XmlName, xml.Name.NamespaceName);
        }
    }

    public enum CraftType {
        Current = 1,
        Target,
        Active
    }
}
