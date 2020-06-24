using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ModApi.Craft.Program;
using UnityEngine;

namespace Assets.Scripts.Vizzy.Operators {
    [Serializable]
    public class StringComparisonExpression : ProgramExpression {
        public const String XmlName = "StringComparison";

        private ConcurrentDictionary<String, Regex> regexCache =
            new ConcurrentDictionary<String, Regex>();

        /// <summary>The operator.</summary>
        [ProgramNodeProperty] private String _op = "equals";

        public override bool IsBoolean => true;

        /// <summary>Gets or sets the operator.</summary>
        /// <value>The operator.</value>
        public String Operator {
            get => this._op;
            set => this._op = value;
        }

        public override List<ListItemInfo> GetListItems(String listId) {
            return new List<ListItemInfo> {
                new ListItemInfo(
                    "equals",
                    "=",
                    "Checks whether or not the text values are exactly equal.",
                    ListItemInfoType.None),
                new ListItemInfo(
                    "less-than",
                    "<",
                    "Checks whether or not the first text value comes before the second alphabetically.",
                    ListItemInfoType.None),
                new ListItemInfo(
                    "less-than-or-equal",
                    "<=",
                    "Checks whether or not the first text value comes before or is the same as the second alphabetically.",
                    ListItemInfoType.None),
                new ListItemInfo(
                    "greater-than",
                    ">",
                    "Checks whether or not the first text value comes after the second alphabetically.",
                    ListItemInfoType.None),
                new ListItemInfo(
                    "greater-than-or-equal",
                    ">=",
                    "Checks whether or not the first text value comes after or is the same as the second alphabetically.",
                    ListItemInfoType.None),
                new ListItemInfo(
                    "starts-with",
                    "Starts With",
                    "Checks whether or not the text starts with the specified prefix.",
                    ListItemInfoType.None),
                new ListItemInfo(
                    "ends-with",
                    "Ends With",
                    "Checks whether or not the text ends with the specified suffix.",
                    ListItemInfoType.None),
                new ListItemInfo(
                    "matches",
                    "Matches",
                    "Checks whether or not the text matches the specified regular expression.",
                    ListItemInfoType.None)
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
            var string1 = this.GetExpression(0).Evaluate(context).TextValue;
            var string2 = this.GetExpression(1).Evaluate(context).TextValue;

            Boolean result;
            switch (this.Operator) {
                case "equals":
                    result = String.Equals(string1, string2, StringComparison.Ordinal);
                    break;
                case "less-than":
                    result = String.Compare(string1, string2, StringComparison.Ordinal) < 0;
                    break;
                case "less-than-or-equal":
                    result = String.Compare(string1, string2, StringComparison.Ordinal) <= 0;
                    break;
                case "greater-than":
                    result = String.Compare(string1, string2, StringComparison.Ordinal) > 0;
                    break;
                case "greater-than-or-equal":
                    result = String.Compare(string1, string2, StringComparison.Ordinal) >= 0;
                    break;
                case "starts-with":
                    result = string1.StartsWith(string2, StringComparison.Ordinal);
                    break;
                case "ends-with":
                    result = string1.EndsWith(string2, StringComparison.Ordinal);
                    break;
                case "matches":
                    var regex = this.regexCache.GetOrAdd(string2, str => new Regex(str, RegexOptions.Compiled));
                    result = regex.IsMatch(string1);
                    break;
                default:
                    Debug.LogWarning($"Unknown string comparison operator: '{this.Operator}'");
                    result = false;
                    break;
            }

            return new ExpressionResult {
                BoolValue = result
            };
        }
    }
}
