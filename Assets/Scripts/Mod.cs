using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Assets.Scripts.Craft.Parts.Modifiers;
using Assets.Scripts.Vizzy;
using Assets.Scripts.Vizzy.Constants;
using Assets.Scripts.Vizzy.CraftInformation;
using Assets.Scripts.Vizzy.Operators;
using Assets.Scripts.Vizzy.UI;
using ModApi.Craft;
using ModApi.Craft.Program;
using ModApi.Craft.Program.Instructions;
using ModApi.Ui;
using ModApi.Ui.Events;
using UnityEngine;

namespace Assets.Scripts {
    /// <summary>
    /// A singleton object representing this mod that is instantiated and initialize when the mod is loaded.
    /// </summary>
    public class Mod : ModApi.Mods.GameMod {
        private static readonly FieldInfo VizzyToolboxColorsField;
        private static readonly FieldInfo VizzyToolboxStylesField;

        private static readonly IDictionary<String, (Type, Func<ProgramNode>)> ModExpressionAndInstructions;

        private static VizzyToolbox _vizzyGLToolbox;

        private static VizzyToolbox VizzyPlusPlusToolbox {
            get {
                if (_vizzyGLToolbox == null) {
                    var vizzyGLToolboxXml =
                        ModApi.Common.Game.Instance.UserInterface.ResourceDatabase.GetResource<TextAsset>("VizzyPlusPlus/Vizzy/VizzyPlusPlusToolbox");
                    if (vizzyGLToolboxXml != null) {
                        _vizzyGLToolbox =
                            new VizzyToolbox(XElement.Parse(vizzyGLToolboxXml.text));
                    } else {
                        Debug.Log("VizzyPlusPlus: The VizzyPlusPlusToolbox Resource Was Not Found.");
                    }
                }

                return _vizzyGLToolbox;
            }
        }

        static Mod() {
            VizzyToolboxColorsField =
                typeof(VizzyToolbox).GetField("_colors", BindingFlags.NonPublic | BindingFlags.Instance);
            VizzyToolboxStylesField =
                typeof(VizzyToolbox).GetField("_styles", BindingFlags.NonPublic | BindingFlags.Instance);

            ModExpressionAndInstructions = new Dictionary<string, (Type, Func<ProgramNode>)> {
                { NumericConstantExpression.XmlName, (typeof(NumericConstantExpression), () => new NumericConstantExpression()) },
                { OrbitalElementExpression.XmlName, (typeof(OrbitalElementExpression), () => new OrbitalElementExpression()) },
                { AdvancedOrbitalElementExpression.XmlName, (typeof(AdvancedOrbitalElementExpression), () => new AdvancedOrbitalElementExpression()) },
                { CartesianStateVectorExpression.XmlName, (typeof(CartesianStateVectorExpression), () => new CartesianStateVectorExpression()) },
                { GetCraftIdExpression.XmlName, (typeof(GetCraftIdExpression), () => new GetCraftIdExpression()) },
                { GetCraftIdExpression.LegacyXmlName, (typeof(GetCraftIdExpression), () => new GetCraftIdExpression()) },
                { AdvancedPlanetInformationExpression.XmlName, (typeof(AdvancedPlanetInformationExpression), () => new AdvancedPlanetInformationExpression()) },
                { StringComparisonExpression.XmlName, (typeof(StringComparisonExpression), () => new StringComparisonExpression()) },
                { StringTransformExpression.XmlName, (typeof(StringTransformExpression), () => new StringTransformExpression()) },
                { StringSplitExpression.XmlName, (typeof(StringSplitExpression), () => new StringSplitExpression()) },
            };

            var programNodeCreatorType =
                typeof(ProgramSerializer).GetNestedType("ProgramNodeCreator", BindingFlags.NonPublic);

            var programSerializerTypeNameLookupField =
                typeof(ProgramSerializer).GetField("_typeNameLookup", BindingFlags.NonPublic | BindingFlags.Static);
            var programSerializerXmlNameLookupField =
                typeof(ProgramSerializer).GetField("_xmlNameLookup", BindingFlags.NonPublic | BindingFlags.Static);

            if (programNodeCreatorType != null &&
                programSerializerTypeNameLookupField != null &&
                programSerializerXmlNameLookupField != null) {
                var programNodeCreatorConstructor =
                    programNodeCreatorType.GetConstructor(new[] {
                        typeof(String),
                        typeof(Type),
                        typeof(Func<ProgramNode>)
                    });

                if (programNodeCreatorConstructor != null) {
                    var typeNameLookup = (IDictionary)programSerializerTypeNameLookupField.GetValue(null);
                    var xmlNameLookup = (IDictionary)programSerializerXmlNameLookupField.GetValue(null);

                    foreach (var kvp in ModExpressionAndInstructions) {
                        var xmlName = kvp.Key;
                        var (type, ctor) = kvp.Value;

                        Debug.Log($"VizzyPlusPlus: Registering Program Node {xmlName}");

                        var programNodeCreator =
                            programNodeCreatorConstructor.Invoke(
                                new System.Object[] {
                                    xmlName,
                                    type,
                                    ctor
                                });

                        xmlNameLookup[xmlName] = typeNameLookup[type.Name] = programNodeCreator;
                    }
                } else {
                    Debug.Log("VizzyPlusPlus: Constructor for ProgramNodeCreator not found.");
                }
            } else {
                Debug.LogError(
                    "VizzyPlusPlus: Reflection Failed. Unable to find expected internal type ProgramSerializer.ProgramNodeCreator, or one of the expected private fields _typeNameLookup or _xmlNameLookup on ProgramSerializer.");
            }
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="Mod"/> class from being created.
        /// </summary>
        private Mod() {
        }

        /// <summary>
        /// Gets the singleton instance of the mod object.
        /// </summary>
        /// <value>The singleton instance of the mod object.</value>
        public static Mod Instance { get; } = GetModInstance<Mod>();

        protected override void OnModInitialized() {
            ModApi.Common.Game.Instance.UserInterface.UserInterfaceLoading += UiOnUserInterfaceLoading;

            base.OnModInitialized();
        }

        private void UiOnUserInterfaceLoading(object sender, UserInterfaceLoadingEventArgs e) {
            var vizzyUI = e.XmlLayout.GameObject.GetComponent<VizzyUIController>();
            if (e.UserInterfaceId == UserInterfaceIds.Vizzy) {
                if (VizzyPlusPlusToolbox != null) {
                    if (vizzyUI.VizzyUI.Toolbox != null) {
                        MergeToolbox(
                            vizzyUI.VizzyUI.Toolbox,
                            VizzyPlusPlusToolbox
                        );
                    } else {
                        Debug.Log("VizzyPlusPlus: The default Vizzy Toolbox isn't loaded yet.");
                    }
                } else {
                    Debug.Log("VizzyPlusPlus: Unable to load VizzyPlusPlusToolbox.");
                }
            }
        }

        public override Boolean IsModRequiredForCraft(CraftData craft) {
            return base.IsModRequiredForCraft(craft) || this.DoesCraftUseVizzyPlusPlus(craft);
        }

        private Boolean DoesCraftUseVizzyPlusPlus(CraftData craft) {
            Debug.Log($"Checking craft {craft.Name} for VizzyPlusPlus usage.");
            if (craft.Assembly != null) {
                foreach (var part in craft.Assembly.Parts) {
                    var flightProgramData = part.GetModifier<FlightProgramData>();

                    if (flightProgramData != null) {
                        var flightProgram = flightProgramData.Script.FlightProgram ??
                            new ProgramSerializer().DeserializeFlightProgram(flightProgramData.FlightProgramXml);
                        if (ContainsVizzyPlusPlus(flightProgram.RootInstructions) ||
                            ContainsVizzyPlusPlus(flightProgram.RootExpressions) ||
                            ContainsVizzyPlusPlus(flightProgram.CustomExpressions) ||
                            ContainsVizzyPlusPlus(flightProgram.CustomInstructions)) {

                            Debug.Log($"Flight Program on part {part.Name} contains VizzyPlusPlus.");
                            return true;
                        } else {
                            Debug.Log($"Flight Program on part {part.Name} does not contain VizzyPlusPlus.");
                        }
                    }
                }
            } else {
                Debug.LogWarning("Unable to check craft for VizzyPlusPlus because Assembly is null.");
            }

            return false;
        }

        private Boolean ContainsVizzyPlusPlus(IEnumerable<ProgramInstruction> instructions) {
            var instructionStack = new Stack<ProgramInstruction>(instructions);
            while (instructionStack.Count> 0) {
                var instruction = instructionStack.Pop();
                if (instruction is IVizzyPlusPlusProgramNode) {
                    return true;
                }

                if (instruction.Expressions != null && ContainsVizzyPlusPlus(instruction.Expressions)) {
                    return true;
                }

                if (instruction.Next != null) {
                    instructionStack.Push(instruction.Next);
                }
                if (instruction.SupportsChildren) {
                    instructionStack.Push(instruction.FirstChild);
                }
            }

            return false;
        }

        private Boolean ContainsVizzyPlusPlus(IEnumerable<ProgramExpression> expressions) {
            var expressionStack = new Stack<ProgramExpression>(expressions);
            while (expressionStack.Count > 0) {
                var expression = expressionStack.Pop();
                if (expression is IVizzyPlusPlusProgramNode) {
                    return true;
                } else if (expression.Expressions != null) {
                    foreach (var child in expression.Expressions.Reverse()) {
                        expressionStack.Push(child);
                    }
                }
            }

            return false;
        }

        private static void MergeToolbox(
            VizzyToolbox baseToolbox,
            VizzyToolbox extensionToolbox) {
            if (VizzyToolboxColorsField == null) {
                throw new InvalidOperationException($"{nameof(VizzyToolboxColorsField)} is null.");
            } else if (VizzyToolboxStylesField == null) {
                throw new InvalidOperationException($"{nameof(VizzyToolboxStylesField)} is null.");
            }

            var baseColors = (Dictionary<String, Color>)VizzyToolboxColorsField.GetValue(baseToolbox);
            var baseStyles = (Dictionary<String, NodeStyle>)VizzyToolboxStylesField.GetValue(baseToolbox);

            var extensionColors = (Dictionary<String, Color>)VizzyToolboxColorsField.GetValue(extensionToolbox);
            foreach (var color in extensionColors) {
                Debug.Log($"VizzyPlusPlus: Adding new color {color.Key}");
                baseColors[color.Key] = color.Value;
            }

            var extensionStyles = (Dictionary<String, NodeStyle>)VizzyToolboxStylesField.GetValue(extensionToolbox);
            foreach (var style in extensionStyles) {
                Debug.Log($"VizzyPlusPlus: Adding new style {style.Key}");
                baseStyles[style.Key] = style.Value;
            }

            foreach (var category in extensionToolbox.Categories) {
                var existingCategory =
                    baseToolbox.Categories.SingleOrDefault(c => String.Equals(c.Name, category.Name, StringComparison.OrdinalIgnoreCase));
                if (existingCategory == null) {
                    baseToolbox.Categories.Add(category);
                } else {
                    foreach (var node in category.Nodes) {
                        if (!existingCategory.Nodes.Any(n => String.Equals(n.Style, node.Style))) {
                            // TODO: Make position configurable some how.
                            existingCategory.Nodes.Add(node);
                        } else {
                            Debug.Log($"VizzyPlusPlus: Unable to add new vizzy node, duplicate style id: '{node.Style}'");
                        }
                    }
                }
            }
        }
    }
}
