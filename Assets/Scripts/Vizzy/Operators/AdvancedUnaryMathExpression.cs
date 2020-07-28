using System;
using System.Collections.Generic;
using System.Xml.Linq;
using ModApi.Craft.Program;
using UnityEngine;

namespace Assets.Scripts.Vizzy.Operators {
    [Serializable]
    public class AdvancedUnaryMathExpression : ProgramExpression, IVizzyPlusPlusProgramNode {
        public const String XmlName = "AdvancedUnaryMath";

        [ProgramNodeProperty] private String _op;
        private UnaryMathExpressionType _opType;

        public override Boolean IsBoolean => false;

        public override List<ListItemInfo> GetListItems(string listId) {
            return new List<ListItemInfo> {
                new ListItemInfo(
                    "exp",
                    "exp",
                    "Returns e (Euler's number) raised to the specified power.",
                    ListItemInfoType.Number),
                new ListItemInfo(
                    "sinh",
                    "sinh",
                    "Returns the hyperbolic sine of the specified angle (in radians).",
                    ListItemInfoType.Number),
                new ListItemInfo(
                    "cosh",
                    "cosh",
                    "Returns the hyperbolic cosine of the specified angle (in radians).",
                    ListItemInfoType.Number),
                new ListItemInfo(
                    "tanh",
                    "tanh",
                    "Returns the hyperbolic tangent of the specified angle (in radians).",
                    ListItemInfoType.Number),
                new ListItemInfo(
                    "asinh",
                    "asinh",
                    "Returns the inverse of the hyperbolic sine (in radians) of the specified value.",
                    ListItemInfoType.Radians),
                new ListItemInfo(
                    "acosh",
                    "acosh",
                    "Returns the inverse of the hyperbolic cosine (in radians) of the specified value.",
                    ListItemInfoType.Radians),
                new ListItemInfo(
                    "atanh",
                    "atanh",
                    "Returns the inverse of the hyperbolic tangent (in radians) of the specified value.",
                    ListItemInfoType.Radians),
            };
        }

        /// <summary>Gets the selected value of the specified list.</summary>
        /// <param name="listId">The list identifier.</param>
        /// <returns>The currently selected value.</returns>
        public override string GetListValue(String listId) {
            return this._op;
        }

        /// <summary>Sets the selected value of the specified list.</summary>
        /// <param name="listId">The list identifier.</param>
        /// <param name="value">The value to select.</param>
        public override void SetListValue(String listId, String value) {
            this._op = value;
            this.OnElementChanged();
        }

        public override void OnDeserialized(XElement xml) {
            base.OnDeserialized(xml);
            this.OnElementChanged();
        }

        public override ExpressionResult Evaluate(IThreadContext context) {
            var value = this.GetExpression(0).Evaluate(context).NumberValue;
            Double result;
            switch (this._opType) {
                case UnaryMathExpressionType.Exponential:
                    result = Math.Exp(value);
                    break;
                case UnaryMathExpressionType.HyperbolicSine:
                    result = Math.Sinh(value);
                    break;
                case UnaryMathExpressionType.HyperbolicCosine:
                    result = Math.Cosh(value);
                    break;
                case UnaryMathExpressionType.HyperbolicTangent:
                    result = Math.Tanh(value);
                    break;
                case UnaryMathExpressionType.HyperbolicArcSine:
                    result = Math.Log(value + Math.Sqrt(Math.Pow(value, 2) + 1));
                    break;
                case UnaryMathExpressionType.HyperbolicArcCosine:
                    result = Math.Log(value + Math.Sqrt(Math.Pow(value, 2) - 1));
                    break;
                case UnaryMathExpressionType.HyperbolicArcTangent:
                    result = Math.Log((1 + value) / (1 - value)) / 2;
                    break;
                default:
                    Debug.LogWarning(
                        $"Unrecognized unary math operator: {this._op}"
                    );
                    result = 0;
                    break;
            }

            return new ExpressionResult { NumberValue = result };
        }

        private void OnElementChanged() {
            switch (this._op?.ToLower().Trim()) {
                case "exp":
                    this._opType = UnaryMathExpressionType.Exponential;
                    break;
                case "sinh":
                    this._opType = UnaryMathExpressionType.HyperbolicSine;
                    break;
                case "cosh":
                    this._opType = UnaryMathExpressionType.HyperbolicCosine;
                    break;
                case "tanh":
                    this._opType = UnaryMathExpressionType.HyperbolicTangent;
                    break;
                case "asinh":
                    this._opType = UnaryMathExpressionType.HyperbolicArcSine;
                    break;
                case "acosh":
                    this._opType = UnaryMathExpressionType.HyperbolicArcCosine;
                    break;
                case "atanh":
                    this._opType = UnaryMathExpressionType.HyperbolicArcTangent;
                    break;
                default:
                    this._opType = default;
                    break;
            }
        }
    }

    public enum UnaryMathExpressionType {
        Exponential = 1,
        HyperbolicSine,
        HyperbolicCosine,
        HyperbolicTangent,
        HyperbolicArcSine,
        HyperbolicArcCosine,
        HyperbolicArcTangent,
    }
}
