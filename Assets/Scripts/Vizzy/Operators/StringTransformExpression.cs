using System;
using System.Collections.Generic;
using ModApi.Craft.Program;
using UnityEngine;

namespace Assets.Scripts.Vizzy.Operators {
    [Serializable]
    public class StringTransformExpression : ProgramExpression {
        public const String XmlName = "UnaryStringTransform";
        /// <summary>The operator.</summary>
        [ProgramNodeProperty] private String _op = "to-lower";

        public override bool IsBoolean =>
            this._op == "is-empty";

        /// <summary>Gets or sets the operator.</summary>
        /// <value>The operator.</value>
        public String Operator {
            get => this._op;
            set => this._op = value;
        }

        public override List<ListItemInfo> GetListItems(String listId) {
            return new List<ListItemInfo> {
                new ListItemInfo(
                    "to-lower",
                    "Lowercase",
                    "Convert text to lowercase.",
                    ListItemInfoType.Text),
                new ListItemInfo(
                    "to-upper",
                    "Uppercase",
                    "Convert text to uppercase.",
                    ListItemInfoType.Text),
                new ListItemInfo(
                    "trim",
                    "Trim",
                    "Trim leading and trailing whitespace.",
                    ListItemInfoType.Text),
                new ListItemInfo(
                    "trim-start",
                    "Trim Start",
                    "Trim leading whitespace.",
                    ListItemInfoType.Text),
                new ListItemInfo(
                    "trim-end",
                    "Trim End",
                    "Trim trailing whitespace.",
                    ListItemInfoType.Text),
                new ListItemInfo(
                    "is-blank",
                    "Is Blank?",
                    "Checks if a text value is empty or only whitespace.",
                    ListItemInfoType.None),
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
        }

        public override ExpressionResult Evaluate(IThreadContext context) {
            var value = this.GetExpression(0).Evaluate(context).TextValue;

            switch (this.Operator) {
                case "to-lower":
                    return new ExpressionResult {
                        TextValue = value.ToLower()
                    };
                case "to-upper":
                    return new ExpressionResult {
                        TextValue = value.ToUpper()
                    };
                case "trim":
                    return new ExpressionResult {
                        TextValue = value.Trim()
                    };
                case "trim-start":
                    return new ExpressionResult {
                        TextValue = value.TrimStart()
                    };
                case "trim-end":
                    return new ExpressionResult {
                        TextValue = value.TrimEnd()
                    };
                case "is-blank":
                    return new ExpressionResult {
                        BoolValue = String.IsNullOrWhiteSpace(value)
                    };
                default:
                    Debug.LogWarning($"Unknown string comparison operator: '{this.Operator}'");
                    return new ExpressionResult {
                        TextValue = String.Empty
                    };
            }
        }
    }
}
