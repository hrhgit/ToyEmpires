using System;
using NodeEditorFramework;
using UnityEngine;
namespace Tools.TechTreeNodeEditor.Nodes {
	public class TechNodeConnectionKnobAttribute  : ValueConnectionKnobAttribute  {

		public TechNodeConnectionKnobAttribute (string name, Direction direction, string type) : base(name, direction, type) {
		}
		public TechNodeConnectionKnobAttribute (string name, Direction direction, string type, ConnectionCount maxCount) : base(name, direction, type, maxCount) {
		}
		public TechNodeConnectionKnobAttribute (string name, Direction direction, string type, NodeSide nodeSide, float nodeSidePos = 0) : base(name, direction, type, nodeSide, nodeSidePos) {
		}
		public TechNodeConnectionKnobAttribute (string name, Direction direction, string type, ConnectionCount maxCount, NodeSide nodeSide, float nodeSidePos = 0) : base(name, direction, type, maxCount, nodeSide, nodeSidePos) {
		}
		public TechNodeConnectionKnobAttribute (string name, Direction direction, Type type) : base(name, direction, type) {
		}
		public TechNodeConnectionKnobAttribute (string name, Direction direction, Type type, ConnectionCount maxCount) : base(name, direction, type, maxCount) {
		}
		public TechNodeConnectionKnobAttribute (string name, Direction direction, Type type, NodeSide nodeSide, float nodeSidePos = 0) : base(name, direction, type, nodeSide, nodeSidePos) {
		}
		public TechNodeConnectionKnobAttribute (string name, Direction direction, Type type, ConnectionCount maxCount, NodeSide nodeSide, float nodeSidePos = 0) : base(name, direction, type, maxCount, nodeSide, nodeSidePos) {
		}

		public override ConnectionPort CreateNew (Node body) {
			ConnectionPort knob = base.CreateNew(body);
			knob.color = Color.yellow;
			return knob;
		}
	}
}
