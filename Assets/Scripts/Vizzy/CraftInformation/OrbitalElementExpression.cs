using System;
using System.Collections.Generic;
using System.Xml.Linq;
using ModApi.Craft.Program;
using ModApi.Flight.Sim;
using UnityEngine;

namespace Assets.Scripts.Vizzy.CraftInformation {
    [Serializable]
    public class OrbitalElementExpression : OrbitNodeInformationExpression {
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
                    ListItemInfoType.Number),
                new ListItemInfo(
                    "period",
                    "Period",
                    "The period of the orbit in seconds.",
                    ListItemInfoType.Number),
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

        public override void OnDeserialized(XElement xml) {
            base.OnDeserialized(xml);
            this.OnElementChanged();
        }

        protected override ExpressionResult GetOrbitNodeProperty(IOrbitNode node) {
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
                case OrbitalElement.Period:
                    return new ExpressionResult {
                        NumberValue = node.Orbit.Period
                    };
                default:
                    Debug.Log("Unrecognized orbital element: ");
                    return new ExpressionResult {
                        NumberValue = 0
                    };
            }
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
                case "period":
                    this._elementType = OrbitalElement.Period;
                    break;
                default:
                    this._elementType = default;
                    break;
            }
        }
    }

    public enum OrbitalElement {
        RightAscension = 1,
        Inclination,
        ArgumentOfPeriapsis,
        TrueAnomaly,
        SemiMajorAxis,
        Eccentricity,
        Period
    }
}
