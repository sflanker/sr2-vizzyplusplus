using System;
using System.Xml.Linq;
using ModApi.Craft.Program;
using UnityEngine;

namespace Assets.Scripts.Vizzy.Constants {
    [Serializable]
    public class NumericConstantExpression : ProgramExpression, IVizzyPlusPlusProgramNode {
        public const String XmlName = "NumericConstant";

        private String _type;
        public Constant Type { get; private set; }

        public override Boolean IsBoolean => false;

        public NumericConstantExpression() {
        }

        public NumericConstantExpression(String type) {
            this._type = type;
            this.Type = (Constant)Enum.Parse(typeof(Constant), type, ignoreCase: true);
        }

        public override ExpressionResult Evaluate(IThreadContext context) {
            switch (this.Type) {
                case Constant.PI:
                    return new ExpressionResult { NumberValue = Math.PI };
                case Constant.G:
                    return new ExpressionResult { NumberValue = 6.6743E-11 }; ;
                case Constant.E:
                    return new ExpressionResult { NumberValue = Math.E };
                case Constant.C:
                    return new ExpressionResult { NumberValue = 299792458 };
                default:
                    Debug.Log($"Unrecognized numeric constant '{this._type}'");
                    return new ExpressionResult { NumberValue = 0 };
            }
        }

        public override void OnDeserialized(XElement xml) {
            base.OnDeserialized(xml);

            this._type = xml.Attribute("type")?.Value;
            if (!String.IsNullOrEmpty(this._type)) {
                this.Type = (Constant)Enum.Parse(typeof(Constant), this._type, ignoreCase: true);
            }
        }

        public override void OnSerialized(XElement xml) {
            base.OnSerialized(xml);

            xml.SetAttributeValue("type", this._type);
        }
    }

    public enum Constant {
        PI = 1,
        G,
        E,
        C
    }
}
