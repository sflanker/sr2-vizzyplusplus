using System;
using System.Collections.Generic;
using System.Xml.Linq;
using ModApi.Craft.Program;
using ModApi.Flight.Sim;
using UnityEngine;

namespace Assets.Scripts.Vizzy.CraftInformation {
    [Serializable]
    public class AdvancedOrbitalElementExpression : ProgramExpression {
        public const String XmlName = "AdvancedOrbitalElement";

        [ProgramNodeProperty] private String _element;
        private AdvancedOrbitalElement _elementType;

        public override bool IsBoolean => false;

        public override List<ListItemInfo> GetListItems(string listId) {
            return new List<ListItemInfo> {
                new ListItemInfo(
                    "angular-momentum",
                    "Angular Momentum",
                    "A vector representing the body's rate of rotation around its parent.",
                    ListItemInfoType.Vector),
                new ListItemInfo(
                    "apoapsis",
                    "Apoapsis",
                    "A vector representing the apoapsis of the orbit.",
                    ListItemInfoType.Vector),
                new ListItemInfo(
                    "periapsis",
                    "Periapsis",
                    "A vector representing the periapsis of the orbit.",
                    ListItemInfoType.Vector),
                new ListItemInfo(
                    "orbital-plane-normal",
                    "Orbital Plane Normal",
                    "A unit vector perpendicular to the orbital plane.",
                    ListItemInfoType.Vector),
                new ListItemInfo(
                    "eccentricity-vector",
                    "Eccentricity Vector",
                    "A vector pointing toward the periapsis of the orbit with a magnitude equal to the eccentricity.",
                    ListItemInfoType.Vector),
                new ListItemInfo(
                    "eccentric-anomaly",
                    "Eccentric Anomaly",
                    "The current eccentric anomaly. See: https://en.wikipedia.org/wiki/Eccentric_anomaly",
                    ListItemInfoType.Degrees),
                new ListItemInfo(
                    "mean-anomaly",
                    "Mean Anomaly",
                    "The faction of an orbits period that has elapsed since the orbit passed periapsis in degrees from 0 to 360.",
                    ListItemInfoType.Degrees),
                new ListItemInfo(
                    "mean-motion",
                    "Mean Motion",
                    "The average angular speed of the orbit in degrees.",
                    ListItemInfoType.Degrees)
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
            this.OnElementChanged();
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
                case AdvancedOrbitalElement.AngularMomentum:
                    return new ExpressionResult {
                        VectorValue = node.Orbit.AngularMomentum
                    };
                case AdvancedOrbitalElement.Apoapsis:
                    return new ExpressionResult {
                        VectorValue = node.Orbit.Apoapsis
                    };
                case AdvancedOrbitalElement.Periapsis:
                    return new ExpressionResult {
                        VectorValue = node.Orbit.Periapsis
                    };
                case AdvancedOrbitalElement.OrbitalPlaneNormal:
                    return new ExpressionResult {
                        VectorValue = node.Orbit.OrbitalPlaneNormal
                    };
                case AdvancedOrbitalElement.EccentricityVector:
                    return new ExpressionResult {
                        VectorValue = node.Orbit.EccentricityVector
                    };
                case AdvancedOrbitalElement.EccentricAnomaly:
                    return new ExpressionResult {
                        NumberValue = node.Orbit.EccentricAnomaly / Math.PI * 180
                    };
                case AdvancedOrbitalElement.MeanAnomaly:
                    return new ExpressionResult {
                        NumberValue = node.Orbit.MeanAnomaly / Math.PI * 180
                    };
                case AdvancedOrbitalElement.MeanMotion:
                    return new ExpressionResult {
                        NumberValue = node.Orbit.MeanMotion
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
                case "angular-momentum":
                    this._elementType = AdvancedOrbitalElement.AngularMomentum;
                    break;
                case "apoapsis":
                    this._elementType = AdvancedOrbitalElement.Apoapsis;
                    break;
                case "periapsis":
                    this._elementType = AdvancedOrbitalElement.Periapsis;
                    break;
                case "orbital-plane-normal":
                    this._elementType = AdvancedOrbitalElement.OrbitalPlaneNormal;
                    break;
                case "eccentricity-vector":
                    this._elementType = AdvancedOrbitalElement.EccentricityVector;
                    break;
                case "eccentric-anomaly":
                    this._elementType = AdvancedOrbitalElement.EccentricAnomaly;
                    break;
                case "mean-anomaly":
                    this._elementType = AdvancedOrbitalElement.MeanAnomaly;
                    break;
                case "mean-motion":
                    this._elementType = AdvancedOrbitalElement.MeanMotion;
                    break;
                default:
                    this._elementType = default;
                    break;
            }
        }
    }

    public enum AdvancedOrbitalElement {
        AngularMomentum = 1,
        Apoapsis,
        Periapsis,
        OrbitalPlaneNormal,
        EccentricityVector,
        EccentricAnomaly,
        MeanAnomaly,
        MeanMotion
    }
}
