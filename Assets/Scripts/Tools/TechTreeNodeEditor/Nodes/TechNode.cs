using System.Collections.Generic;
using System.Linq;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;
using UnityEditor;
using UnityEngine;
namespace Tools.TechTreeNodeEditor.Nodes {
	[Node(false, "TechTree/TechNode")]
	public class TechNode : Node {
		// IMPORTANT : unique Mode ID
		public  const   string ID = "TechNode";
		public override string GetID => ID;
		public override string Title => "Tech Node";
		
		// 布局
		public override Vector2 MinSize    { get { return new Vector2(350, 60); } }
		public override bool    AutoLayout { get { return true; } }
		
		//链接
		private ValueConnectionKnobAttribute inputs        = new ValueConnectionKnobAttribute("Input", Direction.In, "TechConnection");
		[ValueConnectionKnob("Output", Direction.Out, "TechConnection")]
		public ValueConnectionKnob output;
		
		
		//科技
		public  string       techName;
		public  string       techDetail;
		private List<string> formerTechs = new List<string>()
		                                   {
			                                   ""
		                                   };
		
		private Vector2 scroll;
		protected override void OnCreate () {

		}

		public override void NodeGUI () {
			// Display knob and properties here!
			int portCount = (dynamicConnectionPorts.Count((port => port.connected())) + 1);
			
			if (dynamicConnectionPorts.Count != portCount)
			{ // Make sure labels and ports are synchronised
				for (int i = dynamicConnectionPorts.Count - 1; i > 0; i--) {
					if (!dynamicConnectionPorts[i].connected())
						DeleteConnectionPort(i);
				}
				while (dynamicConnectionPorts.Count < portCount)
					CreateValueConnectionKnob(inputs);
			}
			
			for (int i = 0; i < dynamicConnectionPorts.Count; i++)
			{ // Display label and delete button
				GUILayout.BeginHorizontal();
				GUILayout.Label(dynamicConnectionPorts[i].connected() ? this.formerTechs[i] : "");
				((ValueConnectionKnob)dynamicConnectionPorts[i]).SetPosition();

				GUILayout.EndHorizontal();
			}
			
			output.DisplayLayout();;

			EditorGUILayout.BeginVertical("Box");
			GUILayout.BeginHorizontal();
			techName = EditorGUILayout.TextField("Tech Name",techName);
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			
			GUILayout.Space(5);

			GUILayout.BeginHorizontal();
			scroll                          = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(100));
			EditorStyles.textField.wordWrap = true;
			techDetail                      = EditorGUILayout.TextArea(techDetail, GUILayout.ExpandHeight(true));
			EditorStyles.textField.wordWrap = false;
			EditorGUILayout.EndScrollView();
			GUILayout.EndHorizontal();
			
			
			if (GUI.changed) // update canvas
				NodeEditor.curNodeCanvas.OnNodeChange(this);
		}
		
		
		
		

		public override bool Calculate () {
			this.output.SetValue<string>(this.techName);
			this.formerTechs = (from port in this.dynamicConnectionPorts
			                    select ((ValueConnectionKnob)port).GetValue<string>()).ToList();
			this.formerTechs.Add("");
			return true;
		}
		

	}

}