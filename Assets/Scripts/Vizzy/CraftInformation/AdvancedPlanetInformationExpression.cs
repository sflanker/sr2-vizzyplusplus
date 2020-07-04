using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ModApi.Craft.Program;
using ModApi.Flight.Sim;
using UnityEngine;

namespace Assets.Scripts.Vizzy.CraftInformation {
    [Serializable]
    public class AdvancedPlanetInformationExpression : ProgramExpression, IVizzyPlusPlusProgramNode {
        public const String XmlName = "AdvancedPlanetInformation";

        [ProgramNodeProperty] private String _info;
        private PlanetInformation _planetInformationType;

        public override Boolean IsBoolean =>
            this._planetInformationType == PlanetInformation.IsTerrainDataLoaded ||
            this._planetInformationType == PlanetInformation.HasWater;

        public override List<ListItemInfo> GetListItems(string listId) {
            return new List<ListItemInfo> {
                new ListItemInfo(
                    "soi-distance",
                    "Sphere of Influence Distance",
                    "The radius at which a body will entry this planet's sphere of influence.",
                    ListItemInfoType.Number),
                new ListItemInfo(
                    "soi-exit-distance",
                    "Sphere of Influence Exit Distance",
                    "The radius at which a body will exit this planet's sphere of influence.",
                    ListItemInfoType.Number),
                new ListItemInfo(
                    "rotation-angle",
                    "Rotation Angle",
                    "Gets the rotation of the planet in degrees.",
                    ListItemInfoType.Degrees),
                new ListItemInfo(
                    "angular-velocity",
                    "Angular Velocity",
                    "Gets the angular velocity (rotation rate) of the planet in degrees per second.",
                    ListItemInfoType.Degrees),
                new ListItemInfo(
                    "is-terrain-data-loaded",
                    "Is Terrain Data Loaded",
                    "Gets a boolean value indicating whether or not terrain data has been loaded for the planet.",
                    ListItemInfoType.None),
                new ListItemInfo(
                    "has-water",
                    "Has Water",
                    "Gets a boolean value indicating whether or not the planet has water.",
                    ListItemInfoType.None),
                new ListItemInfo(
                    "sea-level",
                    "Sea Level",
                    "Gets the mean sea level radius of the planet.",
                    ListItemInfoType.Number),
                new ListItemInfo(
                    "structure-names",
                    "Structure Names",
                    "Gets a list of structure names.",
                    ListItemInfoType.List),
                new ListItemInfo(
                    "structure-locations",
                    "Structure Locations",
                    "Gets a list of structure locations (lat/long/elevation).",
                    ListItemInfoType.List),
                new ListItemInfo(
                    "structure-headings",
                    "Structure Headings",
                    "Gets a list of structure headings (degrees).",
                    ListItemInfoType.List),
                new ListItemInfo(
                    "launch-location-names",
                    "Launch Location Names",
                    "Gets a list of names for the of default launch locations.",
                    ListItemInfoType.List),
                new ListItemInfo(
                    "launch-locations",
                    "Launch Locations",
                    "Gets a list of locations (lat/long/elevation) for the default launch locations.",
                    ListItemInfoType.List),
                new ListItemInfo(
                    "launch-location-headings",
                    "Launch Location Headings",
                    "Gets a list of headings (degrees) for the default launch locations.",
                    ListItemInfoType.List),
            };
        }

        /// <summary>Gets the selected value of the specified list.</summary>
        /// <param name="listId">The list identifier.</param>
        /// <returns>The currently selected value.</returns>
        public override string GetListValue(string listId) {
            return this._info;
        }

        /// <summary>Sets the selected value of the specified list.</summary>
        /// <param name="listId">The list identifier.</param>
        /// <param name="value">The value to select.</param>
        public override void SetListValue(String listId, String value) {
            this._info = value;
            this.OnInfoChanged();
        }

        public override void OnDeserialized(XElement xml) {
            base.OnDeserialized(xml);
            this.OnInfoChanged();
        }

        public override ExpressionResult Evaluate(IThreadContext context) {
            var planetNameExpression = this.GetExpression(0).Evaluate(context);
            var planetName = planetNameExpression.TextValue;
            if (!String.IsNullOrEmpty(planetName)) {
                var planet = context.Craft.GetPlanet(planetName);

                if (planet != null) {
                    ExpressionResult result;
                    try {
                        switch (this._planetInformationType) {
                            case PlanetInformation.SphereOfInfluenceDistance:
                                result = new ExpressionResult {
                                    NumberValue = planet.SphereOfInfluence
                                };
                                break;
                            case PlanetInformation.SphereOfInfluenceExitDistance:
                                result = new ExpressionResult {
                                    NumberValue = planet.SphereOfInfluenceExitDistance
                                };
                                break;
                            case PlanetInformation.RotationAngle:
                                result = new ExpressionResult {
                                    NumberValue = planet.RotationAngle
                                };
                                break;
                            case PlanetInformation.AngularVelocity:
                                result = new ExpressionResult {
                                    NumberValue = planet.PlanetData.AngularVelocity / Math.PI * 180
                                };
                                break;
                            case PlanetInformation.IsTerrainDataLoaded:
                                result = new ExpressionResult {
                                    BoolValue = planet.TerrainDataLoaded
                                };
                                break;
                            case PlanetInformation.HasWater:
                                result = new ExpressionResult {
                                    BoolValue = planet.PlanetData.HasWater
                                };
                                break;
                            case PlanetInformation.SeaLevel:
                                result = new ExpressionResult {
                                    NumberValue = planet.PlanetData.SeaLevel
                                };
                                break;
                            // case PlanetInformation.StructureNames:
                            //     result = new ExpressionResult(
                            //         planet.PlanetData.StructureNodes
                            //             .Select(n => n.Name)
                            //             .ToList()
                            //     );
                            //     break;
                            // case PlanetInformation.StructureLocations:
                            //     result = new ExpressionResult(
                            //         planet.PlanetData.StructureNodes
                            //             .Select(n => $"({n.Latitude:R}, {n.Longitude:R}, {n.Elevation})")
                            //             .ToList()
                            //     );
                            //     break;
                            // case PlanetInformation.StructureHeadings:
                            //     result = new ExpressionResult(
                            //         planet.PlanetData.StructureNodes
                            //             .Select(n => n.Heading.ToString("R"))
                            //             .ToList()
                            //     );
                            //     break;
                            case PlanetInformation.LaunchLocationNames:
                                result = new ExpressionResult(
                                    planet.PlanetData.DefaultLaunchLocations
                                        .Select(l => l.Name)
                                        .ToList()
                                );
                                break;
                            case PlanetInformation.LaunchLocations:
                                result = new ExpressionResult(
                                    planet.PlanetData.DefaultLaunchLocations
                                        .Select(l => $"({l.Latitude}, {l.Longitude}, {l.LocationType})")
                                        .ToList()
                                );
                                break;
                            case PlanetInformation.LaunchLocationHeadings:
                                result = new ExpressionResult(
                                    planet.PlanetData.DefaultLaunchLocations
                                        .Select(l => l.HeadingSimple?.ToString("R"))
                                        .ToList()
                                );
                                break;
                            default:
                                Debug.LogWarning("Unrecognized planet information field: " + this._info);
                                return new ExpressionResult {
                                    NumberValue = 0
                                };
                        }
                    } catch (Exception ex) {
                        Debug.LogWarning($"An unexpected error occured getting planet information: {this._info} ({this._planetInformationType})");
                        Debug.LogError(ex);
                        return new ExpressionResult {
                            NumberValue = 0
                        };
                    }

                    return result;
                }
            }

            Debug.Log($"Craft or planet not found: {planetNameExpression.TextValue}");
            return new ExpressionResult {
                NumberValue = 0
            };
        }

        private void OnInfoChanged() {
            switch (this._info?.ToLower().Trim()) {
                case "soi-distance":
                    this._planetInformationType = PlanetInformation.SphereOfInfluenceDistance;
                    break;
                case "soi-exit-distance":
                    this._planetInformationType = PlanetInformation.SphereOfInfluenceExitDistance;
                    break;
                case "rotation-angle":
                    this._planetInformationType = PlanetInformation.RotationAngle;
                    break;
                case "angular-velocity":
                    this._planetInformationType = PlanetInformation.AngularVelocity;
                    break;
                case "is-terrain-data-loaded":
                    this._planetInformationType = PlanetInformation.IsTerrainDataLoaded;
                    break;
                case "has-water":
                    this._planetInformationType = PlanetInformation.HasWater;
                    break;
                case "sea-level":
                    this._planetInformationType = PlanetInformation.SeaLevel;
                    break;
                case "structure-names":
                    this._planetInformationType = PlanetInformation.StructureNames;
                    break;
                case "structure-locations":
                    this._planetInformationType = PlanetInformation.StructureLocations;
                    break;
                case "structure-headings":
                    this._planetInformationType = PlanetInformation.StructureHeadings;
                    break;
                case "launch-location-names":
                    this._planetInformationType = PlanetInformation.LaunchLocationNames;
                    break;
                case "launch-locations":
                    this._planetInformationType = PlanetInformation.LaunchLocations;
                    break;
                case "launch-location-headings":
                    this._planetInformationType = PlanetInformation.LaunchLocationHeadings;
                    break;
                default:
                    this._planetInformationType = default;
                    break;
            }
        }
    }

    public enum PlanetInformation {
        SphereOfInfluenceDistance = 1,
        SphereOfInfluenceExitDistance,
        RotationAngle,
        AngularVelocity,
        IsTerrainDataLoaded,
        HasWater,
        SeaLevel,
        StructureNames,
        StructureLocations,
        StructureHeadings,
        LaunchLocationNames,
        LaunchLocations,
        LaunchLocationHeadings,
    }
}
