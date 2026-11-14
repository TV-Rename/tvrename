//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename.AddIns.Themes;

internal static partial class ThemeStyler
{
    private static ToolStripRenderer? themeRenderer;

    /// <summary>True when the theme changes the window background from Windows' own dark grey.</summary>
    private static bool HasOwnBackground => T.Background.ToArgb() != SystemColors.Control.ToArgb();

    /// <summary>
    /// Menus, toolbars and the status bar are drawn by Windows Forms' toolbar renderer in Windows' dark greys,
    /// whatever their background colour. When a theme has its own background colour they get a renderer that
    /// uses the theme's colours instead. With the standard Dark colours nothing changes.
    /// </summary>
    private static void UseThemeRenderer(ToolStrip toolStrip)
    {
        if (!HasOwnBackground)
        {
            return;
        }

        if (themeRenderer is null)
        {
            themeRenderer = new ToolStripProfessionalRenderer(new ThemeColourTable()) { RoundedEdges = false };
            ToolStripManager.Renderer = themeRenderer;
        }

        toolStrip.Renderer = themeRenderer;
    }

    private sealed class ThemeColourTable : ProfessionalColorTable
    {
        public ThemeColourTable() => UseSystemColors = false;

        private static Color Back => T.Background;
        private static Color Hot => T.SelectedSideTab;
        private static Color Pressed => T.HeaderPressed;
        private static Color Line => T.Border;

        public override Color ToolStripGradientBegin => Back;
        public override Color ToolStripGradientMiddle => Back;
        public override Color ToolStripGradientEnd => Back;
        public override Color ToolStripBorder => Back;
        public override Color ToolStripDropDownBackground => Back;
        public override Color ToolStripContentPanelGradientBegin => Back;
        public override Color ToolStripContentPanelGradientEnd => Back;
        public override Color ToolStripPanelGradientBegin => Back;
        public override Color ToolStripPanelGradientEnd => Back;
        public override Color MenuStripGradientBegin => Back;
        public override Color MenuStripGradientEnd => Back;
        public override Color StatusStripGradientBegin => Back;
        public override Color StatusStripGradientEnd => Back;
        public override Color ImageMarginGradientBegin => Back;
        public override Color ImageMarginGradientMiddle => Back;
        public override Color ImageMarginGradientEnd => Back;
        public override Color ImageMarginRevealedGradientBegin => Back;
        public override Color ImageMarginRevealedGradientMiddle => Back;
        public override Color ImageMarginRevealedGradientEnd => Back;
        public override Color OverflowButtonGradientBegin => Back;
        public override Color OverflowButtonGradientMiddle => Back;
        public override Color OverflowButtonGradientEnd => Back;
        public override Color RaftingContainerGradientBegin => Back;
        public override Color RaftingContainerGradientEnd => Back;

        public override Color MenuBorder => Line;
        public override Color MenuItemBorder => Line;
        public override Color SeparatorDark => Line;
        public override Color SeparatorLight => Back;
        public override Color GripDark => Line;
        public override Color GripLight => Back;

        public override Color MenuItemSelected => Hot;
        public override Color MenuItemSelectedGradientBegin => Hot;
        public override Color MenuItemSelectedGradientEnd => Hot;
        public override Color MenuItemPressedGradientBegin => Pressed;
        public override Color MenuItemPressedGradientMiddle => Pressed;
        public override Color MenuItemPressedGradientEnd => Pressed;

        public override Color ButtonSelectedHighlight => Hot;
        public override Color ButtonSelectedHighlightBorder => Line;
        public override Color ButtonSelectedBorder => Line;
        public override Color ButtonSelectedGradientBegin => Hot;
        public override Color ButtonSelectedGradientMiddle => Hot;
        public override Color ButtonSelectedGradientEnd => Hot;
        public override Color ButtonPressedHighlight => Pressed;
        public override Color ButtonPressedHighlightBorder => Line;
        public override Color ButtonPressedBorder => Line;
        public override Color ButtonPressedGradientBegin => Pressed;
        public override Color ButtonPressedGradientMiddle => Pressed;
        public override Color ButtonPressedGradientEnd => Pressed;
        public override Color ButtonCheckedHighlight => T.ActiveFilterButton;
        public override Color ButtonCheckedHighlightBorder => Line;
        public override Color ButtonCheckedGradientBegin => T.ActiveFilterButton;
        public override Color ButtonCheckedGradientMiddle => T.ActiveFilterButton;
        public override Color ButtonCheckedGradientEnd => T.ActiveFilterButton;
        public override Color CheckBackground => T.ActiveFilterButton;
        public override Color CheckSelectedBackground => Hot;
        public override Color CheckPressedBackground => Pressed;
    }
}
