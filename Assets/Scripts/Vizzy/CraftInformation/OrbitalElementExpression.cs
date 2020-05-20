using System;
using System.Collections.Generic;
using System.Xml.Linq;
using ModApi.Craft.Program;
using ModApi.Flight.Sim;
using UnityEngine;

namespace Assets.Scripts.Vizzy.CraftInformation {
    [Serializable]
    public class OrbitalElementExpression : ProgramExpression {
        public const String XmlName = "OrbitalElement";

        [ProgramNodeProperty] private String _element;
        private OrbitalElement _elementType;

        public override bool IsBoolean => false;

        public override List<ListItemInfo> GetListItems(string listId) {
            return new List<ListItemInfo> {
                new ListItemInfo(
                    "right-ascension",
                    "Right Ascension",
                    "The angle between the reference direction and the ascending node.",
                    ListItemInfoType.Degrees),
                new ListItemInfo("inclination", "Inclination", "The degree of tilt.", ListItemInfoType.Degrees),
                new ListItemInfo(
                    "argument-of-periapsis",
                    "Argument of Periapsis",
                    "The angle between the ascending node and the direction of periapse.",
                    ListItemInfoType.Degrees),
                new ListItemInfo(
                    "true-anomaly",
                    "True Anomaly",
                    "The angle between the direction of the periapse and the current position from the main focus of the orbital ellipse.",
                    ListItemInfoType.Degrees),
                new ListItemInfo("semi-major-axis", "Semi-Major Axis", "Half of the sum of the apoapse and periapse distances.", ListItemInfoType.Number),
                new ListItemInfo(
                    "eccentricity",
                    "Eccentricity",
                    "The shape of the orbit, from circular (0), to parabolic (1), to hyperbolic (>1).",
                    ListItemInfoType.Number)
            };
        }

        /// <summary>Gets the selected value of the specified list.</summary>
        /// <param name="listId">The list identifier.</param>
        /// <returns>The currently selected value.</returns>
        public override string GetListValue(string listId) {
            return this._element;
        }

        /// <summary>Sets the selected value of the specified list.</summary>
        /// <param name="listId">The list identifier.</param>
        /// <param name="value">The value to select.</param>
        public override void SetListValue(String listId, String value) {
            this._element = value;
            OnElementChanged();
        }

        private void OnElementChanged() {
            switch (this._element?.ToLower().Trim()) {
                case "right-ascension":
                    this._elementType = OrbitalElement.RightAscension;
                    break;
                case "inclination":
                    this._elementType = OrbitalElement.Inclination;
                    break;
                case "argument-of-periapsis":
                    this._elementType = OrbitalElement.ArgumentOfPeriapsis;
                    break;
                case "true-anomaly":
                    this._elementType = OrbitalElement.TrueAnomaly;
                    break;
                case "semi-major-axis":
                    this._elementType = OrbitalElement.SemiMajorAxis;
                    break;
                case "eccentricity":
                    this._elementType = OrbitalElement.Eccentricity;
                    break;
                default:
                    this._elementType = default;
                    break;
            }
        }

        public override void OnDeserialized(XElement xml) {
            base.OnDeserialized(xml);
            this.OnElementChanged();
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

            switch (this._elementType) {
                case OrbitalElement.RightAscension:
                    return new ExpressionResult {
                        NumberValue = node.Orbit.RightAscensionOfAscendingNode / Math.PI * 180
                    };
                case OrbitalElement.Inclination:
                    return new ExpressionResult {
                        NumberValue = node.Orbit.Inclination / Math.PI * 180
                    };
                case OrbitalElement.ArgumentOfPeriapsis:
                    return new ExpressionResult {
                        NumberValue = node.Orbit.PeriapsisAngle / Math.PI * 180
                    };
                case OrbitalElement.TrueAnomaly:
                    return new ExpressionResult {
                        NumberValue = node.Orbit.TrueAnomaly / Math.PI * 180
                    };
                case OrbitalElement.SemiMajorAxis:
                    return new ExpressionResult {
                        NumberValue = node.Orbit.SemiMajorAxis
                    };
                case OrbitalElement.Eccentricity:
                    return new ExpressionResult {
                        NumberValue = node.Orbit.Eccentricity
                    };
                default:
                    Debug.Log("Unrecognized orbital element: ");
                    return new ExpressionResult {
                        NumberValue = 0
                    };
            }
        }
    }

    public enum OrbitalElement {
        RightAscension = 1,
        Inclination,
        ArgumentOfPeriapsis,
        TrueAnomaly,
        SemiMajorAxis,
        Eccentricity
    }
}
