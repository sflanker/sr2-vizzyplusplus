using System;
using ModApi.Craft.Program;
using ModApi.Flight.Sim;
using UnityEngine;

namespace Assets.Scripts.Vizzy.CraftInformation {
    public abstract class OrbitNodeInformationExpression : ProgramExpression {
        public override ExpressionResult Evaluate(IThreadContext context) {
            var selectedNodeExpression = this.GetExpression(0).Evaluate(context);
            IOrbitNode node;
            if (selectedNodeExpression.ExpressionType == ExpressionType.Number) {
                var craftId = (Int32)selectedNodeExpression.NumberValue;
                node = craftId >= 0 ? context.Craft.GetCraftNode(craftId) : context.Craft.CraftScript.CraftNode;
            } else {
                var nodeName = selectedNodeExpression.TextValue;
                if (nodeName != String.Empty) {
                    node = context.Craft.GetPlanet(nodeName) ??
                        (IOrbitNode)context.Craft.GetCraftNodeByName(nodeName);

                    if (node == null && Int32.TryParse(selectedNodeExpression.TextValue, out var craftId)) {
                        node = craftId >= 0 ? context.Craft.GetCraftNode(craftId) : context.Craft.CraftScript.CraftNode;
                    }
                } else {
                    node = context.Craft.CraftScript.CraftNode;
                }
            }

            if (node != null) {
                return GetOrbitNodeProperty(node);
            } else {
                Debug.Log($"Craft or planet not found: {selectedNodeExpression.TextValue}");
                return new ExpressionResult {
                    NumberValue = 0
                };
            }
        }

        protected abstract ExpressionResult GetOrbitNodeProperty(IOrbitNode node);
    }
}
