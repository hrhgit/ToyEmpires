using System;
using NodeEditorFramework;
using UnityEngine;
namespace Tools.TechTreeNodeEditor.Nodes {
	public class TechConnectionType : ValueConnectionType {
		public override string Identifier => "TechConnection";
		public override Color  Color      => Color.cyan;
		public override Type   Type       => typeof(string);

	}
}
