using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Szotar.WindowsForms {
	public enum ToolbarTheme {
		Toolbar,
		MediaToolbar,
		CommunicationsToolbar,
		BrowserTabBar
	}
	
	/// <summary>
	/// Renders a toolstrip using the UxTheme API via VisualStyleRenderer. Visual styles must be supported for this to work; if you need to support other operating systems use NativeToolStripRenderer.
	/// </summary>
	class UXThemeToolStripRenderer : ToolStripSystemRenderer {
		/// <summary>
		/// It shouldn't be necessary to P/Invoke like this, however VisualStyleRenderer.GetMargins
		/// misses out a parameter in its own P/Invoke.
		/// </summary>
		static internal class NativeMethods {
			[StructLayout(LayoutKind.Sequential, Pack = 1)]
			public struct MARGINS {
				public int cxLeftWidth;
				public int cxRightWidth;
				public int cyTopHeight;
				public int cyBottomHeight;
			}

			[DllImport("uxtheme.dll", ExactSpelling = true)]
			public extern static Int32 GetThemeMargins(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, int iPropId, IntPtr rect, out MARGINS pMargins);
		}

		//See http://msdn2.microsoft.com/en-us/library/bb773210.aspx - "Parts and States"
		#region Parts and States 
		enum MenuParts : int {
			ItemTMSchema = 1,
			DropDownTMSchema = 2,
			BarItemTMSchema = 3,
			BarDropDownTMSchema = 4,
			ChevronTMSchema = 5,
			SeparatorTMSchema = 6,
			BarBackground = 7,
			BarItem = 8,
			PopupBackground = 9,
			PopupBorders = 10,
			PopupCheck = 11,
			PopupCheckBackground = 12,
			PopupGutter = 13,
			PopupItem = 14,
			PopupSeparator = 15,
			PopupSubmenu = 16,
			SystemClose = 17,
			SystemMaximize = 18,
			SystemMinimize = 19,
			SystemRestore = 20
		}

		enum MenuBarStates : int {
			Active = 1,
			Inactive = 2
		}

		enum MenuBarItemStates : int {
			Normal = 1,
			Hover = 2,
			Pushed = 3,
			Disabled = 4,
			DisabledHover = 5,
			DisabledPushed = 6
		}

		enum MenuPopupItemStates : int {
			Normal = 1,
			Hover = 2,
			Disabled = 3,
			DisabledHover = 4
		}

		enum MenuPopupCheckStates : int {
			CheckmarkNormal = 1,
			CheckmarkDisabled = 2,
			BulletNormal = 3,
			BulletDisabled = 4
		}

		enum MenuPopupCheckBackgroundStates : int {
			Disabled = 1,
			Normal = 2,
			Bitmap = 3
		}

		enum MenuPopupSubMenuStates : int {
			Normal = 1,
			Disabled = 2
		}

		enum MarginTypes : int {
			Sizing = 3601,
			Content = 3602,
			Caption = 3603
		}

		const int RP_BACKGROUND = 6;
		#endregion

		#region Theme helpers
		Padding GetThemeMargins(IDeviceContext dc, MarginTypes marginType) {
			NativeMethods.MARGINS margins;
			try {
				IntPtr hDC = dc.GetHdc();
				if (0 == NativeMethods.GetThemeMargins(renderer.Handle, hDC, renderer.Part, renderer.State, (int)marginType, IntPtr.Zero, out margins))
					return new Padding(margins.cxLeftWidth, margins.cyTopHeight, margins.cxRightWidth, margins.cyBottomHeight);
				return new Padding(-1);
			} finally {
				dc.ReleaseHdc();
			}
		}

		private static int GetItemState(ToolStripItem item) {
			bool pressed = item.Pressed;
			bool hot = item.Selected;

			if (item.Owner.IsDropDown) {
				if (item.Enabled)
					return hot ? (int)MenuPopupItemStates.Hover : (int)MenuPopupItemStates.Normal;
				return hot ? (int)MenuPopupItemStates.DisabledHover : (int)MenuPopupItemStates.Disabled;
			} else {
				if (pressed)
					return item.Enabled ? (int)MenuBarItemStates.Pushed : (int)MenuBarItemStates.DisabledPushed;
				if (item.Enabled)
					return hot ? (int)MenuBarItemStates.Hover : (int)MenuBarItemStates.Normal;
				return hot ? (int)MenuBarItemStates.DisabledHover : (int)MenuBarItemStates.Disabled;
			}
		}
		#endregion

		#region Theme subclasses
		public ToolbarTheme Theme {
			get; set;
		}

		private string RebarClass {
			get {
				return SubclassPrefix + "Rebar";
			}
		}

		private string ToolbarClass {
			get {
				return SubclassPrefix + "ToolBar";
			}
		}

		private string MenuClass {
			get {
				return SubclassPrefix + "Menu";
			}
		}

		private string SubclassPrefix {
			get {
				switch (Theme) {
					case ToolbarTheme.MediaToolbar: return "Media::";
					case ToolbarTheme.CommunicationsToolbar: return "Communications::";
					case ToolbarTheme.BrowserTabBar: return "BrowserTabBar::";
					default: return string.Empty;
				}
			}
		}

		private VisualStyleElement Subclass(VisualStyleElement element) {
			return VisualStyleElement.CreateElement(SubclassPrefix + element.ClassName,
				element.Part, element.State);
		}
		#endregion

		VisualStyleRenderer renderer;

		public UXThemeToolStripRenderer(ToolbarTheme theme) {
			Theme = theme;
			renderer = new VisualStyleRenderer(VisualStyleElement.Button.PushButton.Normal);
		}

		#region Borders
		protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e) {
			renderer.SetParameters(MenuClass, (int)MenuParts.PopupBorders, 0);
			if (e.ToolStrip.IsDropDown) {
				Region oldClip = e.Graphics.Clip;

				//Tool strip borders are rendered *after* the content, for some reason.
				//So we have to exclude the inside of the popup otherwise we'll draw over it.
				Rectangle insideRect = e.ToolStrip.ClientRectangle;
				insideRect.Inflate(-1, -1);
				e.Graphics.ExcludeClip(insideRect);

				renderer.DrawBackground(e.Graphics, e.ToolStrip.ClientRectangle, e.AffectedBounds);

				//Restore the old clip in case the Graphics is used again (does that ever happen?)
				e.Graphics.Clip = oldClip;
			}
		}
		#endregion

		protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e) {
			int partID = e.Item.Owner.IsDropDown ? (int)MenuParts.PopupItem : (int)MenuParts.BarItem;
			renderer.SetParameters(MenuClass, partID, GetItemState(e.Item));
			
			Rectangle bgRect = e.Item.ContentRectangle;

			Padding content = GetThemeMargins(e.Graphics, MarginTypes.Content),
					sizing = GetThemeMargins(e.Graphics, MarginTypes.Sizing),
					caption = GetThemeMargins(e.Graphics, MarginTypes.Caption);

			if (!e.Item.Owner.IsDropDown) {
				bgRect.Y = 0;
				bgRect.Height = e.ToolStrip.Height;
				bgRect.Inflate(-1, -1); //GetMargins here perhaps?
			}

			renderer.DrawBackground(e.Graphics, bgRect, bgRect);
		}

		protected override void OnRenderToolStripPanelBackground(ToolStripPanelRenderEventArgs e) {
			//Draw the background using Rebar & RP_BACKGROUND (or, if that is not available, fall back to
			//Rebar.Band.Normal)
			if (VisualStyleRenderer.IsElementDefined(VisualStyleElement.CreateElement(RebarClass, RP_BACKGROUND, 0))) {
				renderer.SetParameters(RebarClass, RP_BACKGROUND, 0);
			} else {
				renderer.SetParameters(RebarClass, 0, 0);
				//renderer.SetParameters(VisualStyleElement.Taskbar.BackgroundBottom.Normal);
				//renderer.SetParameters(Subclass(VisualStyleElement.Rebar.Band.Normal));
			}

			if (renderer.IsBackgroundPartiallyTransparent())
				renderer.DrawParentBackground(e.Graphics, e.ToolStripPanel.ClientRectangle, e.ToolStripPanel);

			renderer.DrawBackground(e.Graphics, e.ToolStripPanel.ClientRectangle);

			//Draw the etched edges of each row.
			//renderer.SetParameters(Subclass(VisualStyleElement.Rebar.Band.Normal));
			//foreach (ToolStripPanelRow row in e.ToolStripPanel.Rows) {
			//    Rectangle rowBounds = row.Bounds;
			//    rowBounds.Offset(0, -1);
			//    renderer.DrawEdge(e.Graphics, rowBounds, Edges.Top, EdgeStyle.Etched, EdgeEffects.None);
			//}

			e.Handled = true;
		}

		//Render the background of an actual menu bar, dropdown menu or toolbar.
		protected override void OnRenderToolStripBackground(System.Windows.Forms.ToolStripRenderEventArgs e) {
			if (e.ToolStrip.IsDropDown) {
				renderer.SetParameters(MenuClass, (int)MenuParts.PopupBackground, 0);
			} else {
				//It's a MenuStrip or a ToolStrip. If it's contained inside a larger panel, it should have a
				//transparent background, showing the panel's background.

				if (e.ToolStrip.Parent is ToolStripPanel) {
					//The background should be transparent, because the ToolStripPanel's background will be visible.
					//(Of course, we assume the ToolStripPanel is drawn using the same theme, but it's not my fault
					//if someone does that.)
					return;
				} else {
					//A lone toolbar/menubar should act like it's inside a toolbox, I guess.
					//Maybe I should use the MenuClass in the case of a MenuStrip, although that would break
					//the other themes...
					if(VisualStyleRenderer.IsElementDefined(VisualStyleElement.CreateElement(RebarClass, RP_BACKGROUND, 0)))
						renderer.SetParameters(RebarClass, RP_BACKGROUND, 0);
					else
						renderer.SetParameters(RebarClass, 0, 0);
				}
			}

			if (renderer.IsBackgroundPartiallyTransparent())
				renderer.DrawParentBackground(e.Graphics, e.ToolStrip.ClientRectangle, e.ToolStrip);

			renderer.DrawBackground(e.Graphics, e.ToolStrip.ClientRectangle, e.AffectedBounds);
		}

		//The only purpose of this override is to change the arrow colour.
		//It's OK to just draw over the default arrow since we also pass down arrow drawing to the system renderer.
		protected override void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e) {
			ToolStripSplitButton sb = (ToolStripSplitButton)e.Item;
			base.OnRenderSplitButtonBackground(e);
			OnRenderArrow(new ToolStripArrowRenderEventArgs(e.Graphics, sb, sb.DropDownButtonBounds, Color.Red, ArrowDirection.Down));
		}

		Color GetItemTextColor(ToolStripItem item) {
			int partId = item.IsOnDropDown ? (int)MenuParts.PopupItem : (int)MenuParts.BarItem;
			renderer.SetParameters(MenuClass, partId, GetItemState(item));
			return renderer.GetColor(ColorProperty.TextColor);
		}

		protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e) {
			e.TextColor = GetItemTextColor(e.Item);

			base.OnRenderItemText(e);
		}

		//This is ugly. Need to get the actual grip bounds.
		//protected override void OnRenderGrip(ToolStripGripRenderEventArgs e) {
		//    if (e.GripStyle == ToolStripGripStyle.Visible) {
		//        renderer.SetParameters(VisualStyleElement.Rebar.Gripper.Normal);
		//        renderer.DrawBackground(e.Graphics, e.GripBounds, e.AffectedBounds);
		//    }
		//}

		protected override void OnRenderImageMargin(ToolStripRenderEventArgs e) {
			if (e.ToolStrip.IsDropDown) {
				renderer.SetParameters(MenuClass, (int)MenuParts.PopupGutter, 0);
				//The AffectedBounds is usually too small, way too small to look right. Instead of using that,
				//use the AffectedBounds but with the right width. Then narrow the rectangle to the correct edge
				//based on whether or not it's RTL. (It doesn't need to be narrowed to an edge in LTR mode, but let's
				//do that anyway.)
				//Using the DisplayRectangle gets roughly the right size so that the separator is closer to the text.
				int extraWidth = (e.ToolStrip.Width - e.ToolStrip.DisplayRectangle.Width) - e.AffectedBounds.Width;
				Rectangle rect = e.AffectedBounds;
				rect.Y += 2;
				rect.Height -= 4;
				int sepWidth = renderer.GetPartSize(e.Graphics, ThemeSizeType.True).Width;
				if (e.ToolStrip.RightToLeft == RightToLeft.Yes) {
					rect = new Rectangle(rect.X - extraWidth, rect.Y, sepWidth, rect.Height);
					rect.X += sepWidth;
				} else {
					rect = new Rectangle(rect.Width + extraWidth - sepWidth, rect.Y, sepWidth, rect.Height);
				}
				renderer.DrawBackground(e.Graphics, rect);
			}
		}

		protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e) {
			if (e.ToolStrip.IsDropDown) {
				renderer.SetParameters(MenuClass, (int)MenuParts.PopupSeparator, 0);
				Rectangle rect = new Rectangle(e.ToolStrip.DisplayRectangle.Left, 0, e.ToolStrip.DisplayRectangle.Width, e.Item.Height);
				renderer.DrawBackground(e.Graphics, rect, rect);
			} else {
				base.OnRenderSeparator(e);
			}
		}

		//Currently the check mark looks a bit odd in most configurations because the icon isn't scaled very well.
		//ToolStrip gets the wrong sizes for most of its items, so it could be difficult to get it to draw correctly.
		protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e) {
			Rectangle rect = e.Item.ContentRectangle;
			rect.Width = rect.Height;

			//Center the checkmark horizontally in the gutter (looks ugly, though)
			//rect.X = (e.ToolStrip.DisplayRectangle.Left - rect.Width) / 2;

			//Now, mirror its position if the menu item is RTL.
			if (e.Item.RightToLeft == RightToLeft.Yes)
				rect = new Rectangle(e.ToolStrip.ClientSize.Width - rect.X - rect.Width, rect.Y, rect.Width, rect.Height);

			renderer.SetParameters(MenuClass, (int)MenuParts.PopupCheckBackground, e.Item.Enabled ? (int)MenuPopupCheckBackgroundStates.Normal : (int)MenuPopupCheckBackgroundStates.Disabled);
			renderer.DrawBackground(e.Graphics, rect);
			
			Padding margins = GetThemeMargins(e.Graphics, MarginTypes.Sizing);

			rect = new Rectangle(rect.X + margins.Left, rect.Y + margins.Top,
				rect.Width - margins.Horizontal,
				rect.Height - margins.Vertical);

			//I don't think ToolStrip even supports radio box items, so no need to render them.
			renderer.SetParameters(MenuClass, (int)MenuParts.PopupCheck, e.Item.Enabled ? (int)MenuPopupCheckStates.CheckmarkNormal : (int)MenuPopupCheckStates.CheckmarkDisabled);

			renderer.DrawBackground(e.Graphics, rect);
		}

		//This is broken for RTL
		protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e) {
			//The default renderer will draw an arrow for us (the UXTheme API seems not to have one for all directions),
			//but it will get the colour wrong in many cases. The text colour is probably the best colour to use.
			e.ArrowColor = GetItemTextColor(e.Item);
			base.OnRenderArrow(e);
		}

		protected override void OnRenderOverflowButtonBackground(ToolStripItemRenderEventArgs e) {
			//BrowserTabBar::Rebar draws the chevron using the default background. Odd.
			string rebarClass = RebarClass;
			if(Theme == ToolbarTheme.BrowserTabBar)
				rebarClass = "Rebar";

			int state = VisualStyleElement.Rebar.Chevron.Normal.State;
			if (e.Item.Pressed) {
				state = VisualStyleElement.Rebar.Chevron.Pressed.State;
			} else if (e.Item.Selected) {
				state = VisualStyleElement.Rebar.Chevron.Hot.State;
			}

			renderer.SetParameters(rebarClass, VisualStyleElement.Rebar.Chevron.Normal.Part, state);
			renderer.DrawBackground(e.Graphics, new Rectangle(Point.Empty, e.Item.Size));
		}

		public static bool IsSupported {
			get {
				if (!VisualStyleRenderer.IsSupported)
					return false;

				return VisualStyleRenderer.IsElementDefined(VisualStyleElement.CreateElement("MENU", (int)MenuParts.BarBackground, (int)MenuBarStates.Active));
			} 
		}
	}

	/// <summary>
	/// Renders a toolstrip using UXTheme if possible, and switches back to the default
	/// ToolStripRenderer when UXTheme-based rendering is not available.
	/// Designed for menu bars and context menus - it is not guaranteed to work with anything else.
	/// </summary>
	/// <example>
	/// NativeToolStripRenderer.SetToolStripRenderer(toolStrip1, toolStrip2, contextMenuStrip1);
	/// </example>
	/// <example>
	/// toolStrip1.Renderer = new NativeToolStripRenderer();
	/// </example>
	public class NativeToolStripRenderer : ToolStripRenderer {
		UXThemeToolStripRenderer nativeRenderer;
		ToolStripRenderer defaultRenderer;
		ToolStrip toolStrip;

		//NativeToolStripRenderer looks best with no padding - but keep the old padding in case the
		//visual styles become unsupported again (e.g. user changes to windows classic skin)
		Padding defaultPadding;

		#region Constructors
		/// <summary>
		/// Creates a NativeToolStripRenderer for a particular ToolStrip. NativeToolStripRenderer  will subscribe to some events
		/// of this ToolStrip.
		/// </summary>
		/// <param name="toolStrip">The toolstrip for this NativeToolStripRenderer. NativeToolStripRenderer  will subscribe to some events
		/// of this ToolStrip.</param>
		public NativeToolStripRenderer(ToolStrip toolStrip, ToolbarTheme theme) {
			if (toolStrip == null)
				throw new ArgumentNullException("toolStrip", "ToolStrip cannot be null.");

			Theme = theme;

			this.toolStrip = toolStrip;
			defaultRenderer = toolStrip.Renderer;

			defaultPadding = toolStrip.Padding;
			toolStrip.SystemColorsChanged += new EventHandler(toolStrip_SystemColorsChanged);

			//Can't initialize here - constructor throws if visual styles not enabled
			//nativeRenderer = new NativeToolStripRenderer();
		}

		public NativeToolStripRenderer(ToolStripPanel panel, ToolbarTheme theme) {
			if (panel == null)
				throw new ArgumentNullException("panel", "Panel cannot be null.");

			Theme = theme;

			this.toolStrip = null;
			defaultRenderer = panel.Renderer;
		}
		#endregion

		public ToolbarTheme Theme { get; set; }

		void toolStrip_SystemColorsChanged(object sender, EventArgs e) {
			if (toolStrip == null)
				return;

			if (UXThemeToolStripRenderer.IsSupported)
				toolStrip.Padding = Padding.Empty;
			else
				toolStrip.Padding = defaultPadding;
		}

		//This is indeed called every time a menu part is rendered, but I can't
		//find a way of caching it that I can be sure has no race conditions.
		//The check is no longer very costly, anyway.
		protected ToolStripRenderer ActualRenderer {
			get {
				bool nativeSupported = UXThemeToolStripRenderer.IsSupported;
				
				if (nativeSupported) {
					if (nativeRenderer == null)
						nativeRenderer = new UXThemeToolStripRenderer(Theme);
					return nativeRenderer;
				}

				return defaultRenderer;
			}
		}

		#region InitializeXXX
		protected override void Initialize(ToolStrip toolStrip) {
			base.Initialize(toolStrip);

			toolStrip.Padding = Padding.Empty;

			if (/*!(toolStrip is MenuStrip) &&*/ toolStrip.Parent is ToolStripPanel) {
				toolStrip.BackColor = Color.Transparent;
			}
		}

		protected override void InitializePanel(ToolStripPanel toolStripPanel) {
			base.InitializePanel(toolStripPanel);
		}

		protected override void InitializeItem(ToolStripItem item) {
			base.InitializeItem(item);
		}
		#endregion

		#region SetToolStripRenderer
		/// <summary>
		/// Sets the renderer of each ToolStrip to a NativeToolStripRenderer. A convenience method.
		/// </summary>
		/// <param name="toolStrips">A parameter list of ToolStrips.</param>
		[SuppressMessage("Microsoft.Design", "CA1062")] //The parameter array is actually checked.
		public static void SetToolStripRenderer(ToolbarTheme theme, params Control[] toolStrips) {
			foreach (Control ts in toolStrips) {
				if (ts == null)
					throw new ArgumentNullException("toolStrips", "ToolStrips cannot contain a null reference.");
			}

			foreach (Control ts in toolStrips) {
				if (ts is ToolStrip) {
					ToolStrip t = (ToolStrip)ts;
					t.Renderer = new NativeToolStripRenderer(t, theme);
				} else if (ts is ToolStripPanel) {
					ToolStripPanel t = (ToolStripPanel)ts;
					t.Renderer = new NativeToolStripRenderer(t, theme);
				}  else
					throw new ArgumentException("Can't set the renderer for a " + ts.GetType().Name);
			}
		}

		public static void SetToolStripRenderer(params Control[] toolStrips) {
			SetToolStripRenderer(ToolbarTheme.Toolbar, toolStrips);
		}
		#endregion

		#region Overridden Methods - Deferred to actual renderer
		protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e) {
			ActualRenderer.DrawArrow(e);
		}

		protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e) {
			ActualRenderer.DrawButtonBackground(e);
		}

		protected override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e) {
			ActualRenderer.DrawDropDownButtonBackground(e);
		}

		protected override void OnRenderGrip(ToolStripGripRenderEventArgs e) {
			ActualRenderer.DrawGrip(e);
		}

		protected override void OnRenderImageMargin(ToolStripRenderEventArgs e) {
			ActualRenderer.DrawImageMargin(e);
		}

		protected override void OnRenderItemBackground(ToolStripItemRenderEventArgs e) {
			ActualRenderer.DrawItemBackground(e);
		}

		protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e) {
			ActualRenderer.DrawItemCheck(e);
		}

		protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e) {
			ActualRenderer.DrawItemImage(e);
		}

		protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e) {
			ActualRenderer.DrawItemText(e);
		}

		protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e) {
			ActualRenderer.DrawMenuItemBackground(e);
		}

		protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e) {
			ActualRenderer.DrawSeparator(e);
		}

		protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e) {
			ActualRenderer.DrawToolStripBackground(e);
		}

		protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e) {
			ActualRenderer.DrawToolStripBorder(e);
		}

		protected override void OnRenderToolStripContentPanelBackground(ToolStripContentPanelRenderEventArgs e) {
			ActualRenderer.DrawToolStripContentPanelBackground(e);
		}

		protected override void OnRenderToolStripPanelBackground(ToolStripPanelRenderEventArgs e) {
			ActualRenderer.DrawToolStripPanelBackground(e);
		}

		protected override void OnRenderLabelBackground(ToolStripItemRenderEventArgs e) {
			ActualRenderer.DrawLabelBackground(e);
		}

		protected override void OnRenderOverflowButtonBackground(ToolStripItemRenderEventArgs e) {
			ActualRenderer.DrawOverflowButtonBackground(e);
		}

		protected override void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e) {
			ActualRenderer.DrawSplitButton(e);
		}

		protected override void OnRenderStatusStripSizingGrip(ToolStripRenderEventArgs e) {
			ActualRenderer.DrawStatusStripSizingGrip(e);
		}

		protected override void OnRenderToolStripStatusLabelBackground(ToolStripItemRenderEventArgs e) {
			ActualRenderer.DrawToolStripStatusLabelBackground(e);
		}
		#endregion
	}
}
