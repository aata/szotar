using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Szotar.WindowsForms.Preferences;

namespace Szotar.WindowsForms.Forms {
	public partial class Preferences : Form {
		TreeNode displayedNode;
		PreferencePage displayedPage;
		List<PreferencePage> commitList = new List<PreferencePage>();

		public Preferences() {
			InitializeComponent();

			ThemeHelper.UseExplorerTheme(tree);
			tree.Nodes.Clear();

			foreach(Type type in System.Reflection.Assembly.GetExecutingAssembly().GetTypes()) {
				Attribute attr = Attribute.GetCustomAttribute(type, typeof(PreferencePageAttribute), true);
				if(attr != null)
					FindOrCreateNode(new NodeTag { Type = type, Attribute = attr as PreferencePageAttribute });
			}

			tree.TreeViewNodeSorter = new ComparePreferencePagesByOrder();
			tree.Sort();

			tree.AfterSelect += new TreeViewEventHandler(tree_AfterSelect);
		}

		private TreeNode FindOrCreateNode(NodeTag tag) {
			TreeNodeCollection parentNodeCollection = null;

			if (tag.Attribute.Parent != null) {
				NodeTag parentTag = new NodeTag
				{
					Attribute = (PreferencePageAttribute)Attribute.GetCustomAttribute(tag.Attribute.Parent, typeof(PreferencePageAttribute)),
					Type = tag.Attribute.Parent
				};

				parentNodeCollection = FindOrCreateNode(parentTag).Nodes;

				foreach (TreeNode node in parentNodeCollection)
					if (node.Name == tag.Type.AssemblyQualifiedName)
						return node;
			} else {
				//It's a root node
				foreach (TreeNode node in tree.Nodes)
					if (node.Name == tag.Type.AssemblyQualifiedName)
						return node;

				parentNodeCollection = tree.Nodes;
			}

			TreeNode createdNode = new TreeNode(tag.Attribute.Name);
			createdNode.Tag = tag;
			createdNode.Name = tag.Type.AssemblyQualifiedName;
			parentNodeCollection.Add(createdNode);
			return createdNode;
		}

		void tree_AfterSelect(object sender, TreeViewEventArgs e) {
			NodeTag tag = null;
			TreeNode finalNode = null;

			try {
				//Get the first leaf node
				finalNode = e.Node;
				while (finalNode.Nodes.Count > 0)
					finalNode = finalNode.Nodes[0];

				tag = finalNode.Tag as NodeTag;
			} catch {
				content.Controls.Clear();
				displayedPage = null;
				displayedNode = null;
				throw;
			}

			//There were no instantiable leaf nodes!
			if (tag.Type.IsAbstract) {
				content.Controls.Clear();
				displayedNode = null;
				displayedPage = null;
				return;
			}

			if (displayedNode == finalNode)
				return;

			PreferencePage page = null;
			foreach (PreferencePage uncommittedPage in commitList) {
				if(uncommittedPage.GetType() == tag.Type) {
					page = uncommittedPage;
					break;
				}
			}

			if (page == null) {
				page = Activator.CreateInstance(tag.Type) as PreferencePage;
				page.Owner = this;
			}

			if (page == null) {
				displayedNode = null;
				displayedPage = null;
				content.Controls.Clear();
				return;
			}

			page.Dock = DockStyle.Fill;

			content.Controls.Add(new Label { 
				Text = string.Join(@"\", new List<string>(tag.Attribute.LocalisedPath).ToArray())
			});

			content.Controls.Clear();
			content.Controls.Add(page);
			displayedNode = finalNode;

			if (displayedPage != null && commitList.IndexOf(displayedPage) == -1)
				commitList.Add(displayedPage);
			displayedPage = page;
		}

		internal class NodeTag {
			public Type Type { get; set; }
			public PreferencePageAttribute Attribute { get; set; }
		}

		internal class ComparePreferencePagesByOrder : System.Collections.IComparer, System.Collections.Generic.IComparer<TreeNode> {
			public int Compare(object x, object y) {
				return ((System.Collections.Generic.IComparer<TreeNode>)this).Compare(x as System.Windows.Forms.TreeNode, y as System.Windows.Forms.TreeNode);
			}

			int System.Collections.Generic.IComparer<TreeNode>.Compare(System.Windows.Forms.TreeNode x, System.Windows.Forms.TreeNode y) {
				if (x.Tag is NodeTag && y.Tag is NodeTag) {
					return (y.Tag as NodeTag).Attribute.Importance.CompareTo((x.Tag as NodeTag).Attribute.Importance);
				} else {
					throw new ArgumentException();
				}
			}
		}

		private void cancelButton_Click(object sender, EventArgs e) {
			Close();
		}

		private void okButton_Click(object sender, EventArgs e) {
			if (commitList.IndexOf(displayedPage) == -1)
				commitList.Add(displayedPage);

			foreach (PreferencePage page in commitList)
				page.Commit();

			Configuration.Save();
			Close();
		}

		public void ClearCommitList() {
			commitList.Clear();
		}
	}
	
	internal static class ArrayExtensions {
		public static T GetFirstOrNull<T>(IEnumerable<T> enumerable) {
			var e = enumerable.GetEnumerator();
			if (e.MoveNext())
				return e.Current;
			return default(T);
		}
	}
}
