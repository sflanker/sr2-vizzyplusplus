using System;
using System.Collections.Generic;
using ModApi.Craft.Program;
using ModApi.Flight.Sim;
using UnityEngine;

namespace Assets.Scripts.Vizzy.CraftInformation {
    public class CartesianStateVectorExpression  : ProgramExpression {
        public const String XmlName = "CartesianStateVector";

        [ProgramNodeProperty] private String _vector;
        private CartesianStateVector _vectorType;

        public override bool IsBoolean => false;

        public override List<ListItemInfo> GetListItems(string listId) {
            return new List<ListItemInfo> {
                new ListItemInfo(
                    "position",
                    "Position",
                    "The body's current position vector.",
                    ListItemInfoType.Vector),
                new ListItemInfo(
                    "velocity",
                    "Velocity",
                    "The body's current velocity vector.",
                    ListItemInfoType.Vector)
            };
        }

        /// <summary>Gets the selected value of the specified list.</summary>
        /// <param name="listId">The list identifier.</param>
        /// <returns>The currently selected value.</returns>
        public override string GetListValue(string listId) {
            return this._vector;
        }

        /// <summary>Sets the selected value of the specified list.</summary>
        /// <param name="listId">The list identifier.</param>
        /// <param name="value">The value to select.</param>
        public override void SetListValue(String listId, String value) {
            this._vector = value;
            this.OnVectorChanged();
        }

        public override ExpressionResult Evaluate(IThreadContext context) {
            var selectedNodeExpression = this.GetExpression(0).Evaluate(context);
            IOrbitNode node;
            if (selectedNodeExpression.ExpressionType == ExpressionType.Number) {
                node = context.Craft.GetCraftNode((Int32)selectedNodeExpression.NumberValue);
            } else {
                var nodeName = selectedNodeExpression.TextValue;
                node = context.Craft.GetPlanet(nodeName) ??
                    (IOrbitNode)context.Craft.GetCraftNodeByName(nodeName);
            }

            switch (this._vectorType) {
                case CartesianStateVector.Position:
                    return new ExpressionResult {
                        VectorValue = node.Orbit.Position
                    };
                case CartesianStateVector.Velocity:
                    return new ExpressionResult {
                        VectorValue = node.Orbit.Velocity
                    };
                default:
                    Debug.Log(
                        $"Unrecognized cartesian state vector: {this._vector}"
                    );
                    return new ExpressionResult {
                        VectorValue = new Vector3d(0, 0, 0)
                    };
            }
        }

        private void OnVectorChanged() {
            switch (this._vector?.ToLower().Trim()) {
                case "position":
                    this._vectorType = CartesianStateVector.Position;
                    break;
                case "velocity":
                    this._vectorType = CartesianStateVector.Velocity;
                    break;
                default:
                    this._vectorType = default;
                    break;
            }
        }
    }

    public enum CartesianStateVector {
        Position = 1,
        Velocity
    }
}
