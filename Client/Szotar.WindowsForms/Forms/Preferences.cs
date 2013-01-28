using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Szotar.WindowsForms.Preferences;

namespace Szotar.WindowsForms.Forms {
	public partial class Preferences : Form {
		TreeNode displayedNode;
		PreferencePage displayedPage;
		readonly List<PreferencePage> commitList = new List<PreferencePage>();

		public Preferences() {
			InitializeComponent();

			ThemeHelper.UseExplorerTheme(tree);
			tree.Nodes.Clear();

			foreach (Type type in System.Reflection.Assembly.GetExecutingAssembly().GetTypes()) {
				var attr = Attribute.GetCustomAttribute(type, typeof(PreferencePageAttribute), true);
				if (attr != null)
					FindOrCreateNode(new NodeTag { Type = type, Attribute = attr as PreferencePageAttribute });
			}

			tree.TreeViewNodeSorter = new ComparePreferencePagesByOrder();
			tree.Sort();
			tree.ExpandAll();

			tree.AfterSelect += tree_AfterSelect;
		}

		private TreeNode FindOrCreateNode(NodeTag tag) {
			TreeNodeCollection parentNodeCollection;

			if (tag.Attribute.Parent != null) {
				var parentTag = new NodeTag {
					Attribute = (PreferencePageAttribute)Attribute.GetCustomAttribute(tag.Attribute.Parent, typeof(PreferencePageAttribute)),
					Type = tag.Attribute.Parent
				};

				parentNodeCollection = FindOrCreateNode(parentTag).Nodes;

				foreach (TreeNode node in parentNodeCollection)
					if (node.Name == tag.Type.AssemblyQualifiedName)
						return node;
			} else {
				// It's a root node
				foreach (TreeNode node in tree.Nodes)
					if (node.Name == tag.Type.AssemblyQualifiedName)
						return node;

				parentNodeCollection = tree.Nodes;
			}

			var createdNode = new TreeNode(tag.Attribute.LocalisedName);
			createdNode.Tag = tag;
			createdNode.Name = tag.Type.AssemblyQualifiedName;
			parentNodeCollection.Add(createdNode);
			return createdNode;
		}

		void tree_AfterSelect(object sender, TreeViewEventArgs e) {
			NodeTag tag;
			TreeNode finalNode;

			try {
				// Get the first leaf node
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

			// There were no instantiable leaf nodes!
			if (tag.Type.IsAbstract) {
				content.Controls.Clear();
				displayedNode = null;
				displayedPage = null;
				return;
			}

			if (displayedNode == finalNode)
				return;

			PreferencePage page = commitList.FirstOrDefault(uncommittedPage => uncommittedPage.GetType() == tag.Type) 
				?? Activator.CreateInstance(tag.Type) as PreferencePage;

			if (page != null) {
				page.Owner = this;
			} else {
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

		internal class ComparePreferencePagesByOrder : System.Collections.IComparer, IComparer<TreeNode> {
			public int Compare(object x, object y) {
				return ((IComparer<TreeNode>)this).Compare(x as TreeNode, y as TreeNode);
			}

			int IComparer<TreeNode>.Compare(TreeNode x, TreeNode y) {
				if (x.Tag is NodeTag && y.Tag is NodeTag)
					return (y.Tag as NodeTag).Attribute.Importance.CompareTo((x.Tag as NodeTag).Attribute.Importance);
				throw new ArgumentException();
			}
		}

		private void cancelButton_Click(object sender, EventArgs e) {
			Close();
		}

		void Commit(bool close) {
			if (commitList.IndexOf(displayedPage) == -1)
				commitList.Add(displayedPage);

			foreach (var page in commitList)
				page.Commit();

			Configuration.Save();

			if (close)
				Close();
		}

		private void okButton_Click(object sender, EventArgs e) {
			Commit(true);
		}

		private void applyButton_Click(object sender, EventArgs e) {
			Commit(false);
			ClearCommitList();
		}

		public void ClearCommitList() {
			commitList.Clear();
			if (displayedPage != null)
				commitList.Add(displayedPage);
		}
	}
}
