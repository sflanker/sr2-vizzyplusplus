using System;
using System.Linq;
using ModApi.Craft.Program;

namespace Assets.Scripts.Vizzy.Operators {
    [Serializable]
    public class StringSplitExpression : ProgramExpression, IVizzyPlusPlusProgramNode {
        public const String XmlName = "Split";

        public override Boolean IsBoolean => false;

        public override ExpressionResult Evaluate(IThreadContext context) {
            var string1 = this.GetExpression(0).Evaluate(context).TextValue;
            var string2 = this.GetExpression(1).Evaluate(context).TextValue;

            return new ExpressionResult(string1.Split(string2.ToCharArray()).ToList());
        }
    }
}
