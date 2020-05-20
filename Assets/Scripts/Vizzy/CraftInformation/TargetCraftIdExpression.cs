using System;
using Assets.Scripts.Flight.Sim;
using ModApi.Craft.Program;

namespace Assets.Scripts.Vizzy.CraftInformation {
    [Serializable]
    public class TargetCraftIdExpression : ProgramExpression {
        public const String XmlName = "TargetCraftId";

        private static readonly ExpressionResult NoTargetResult =
            new ExpressionResult {
                NumberValue = -1
            };

        public override bool IsBoolean => false;

        public override ExpressionResult Evaluate(IThreadContext context) {
            if (context.Craft.Data.NavSphereTarget is CraftNode targetCraft) {
                return new ExpressionResult {
                    NumberValue = targetCraft.NodeId
                };
            }

            return NoTargetResult;
        }
    }
}
