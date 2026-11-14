# Themes add-in

Optional colour themes for TV Rename. Everything lives in this folder; TV
Rename itself only has two one-line hooks:

* `App/Program.cs`, `Main`, just before `Application.SetColorMode(...)`:
  `AddIns.Themes.ThemeManager.Startup(args);`
  reads the chosen theme and switches on dark mode before the first window
  exists.
* `Forms/Supporting/TVRenameSplash.cs`, constructor, after
  `InitializeComponent()`:
  `AddIns.Themes.ThemeManager.KeepStandardLook(this);`
  the splash screen runs on its own thread, so the add-in cannot see it; this
  keeps it looking the same in every theme.

Remove those two lines and this folder and TV Rename is back to stock.

## What the user sees

* **Options > Preferences > Display > Theme**: Light (default), every theme
  file found, and Same as Windows. A new choice is used the next time TV
  Rename starts. "Folder" opens the themes folder.
* The standard (Light) look is never changed: every step checks that a theme
  is in use first.

## Files

In the settings folder (the `/userfilepath` folder when that is used), sub
folder `Themes`:

* `*.theme.xml`: the themes offered in Preferences. The built in themes
  (`Dark.theme.xml`, `Cream.theme.xml`) are created if missing. Copy one to
  make a new theme.
* `Defaults\*.theme.xml`: the original built in themes, rewritten every start
  so there is always a clean copy.
* `Backups\*.theme.xml`: the last copy of each theme that loaded without a
  mistake.
* `ThemeSettings.xml`: which theme was chosen (`Light`, `Windows` or a theme
  file name).

A theme is built on Windows' dark look (`base="Dark"`) or on the standard
light look (`base="Light"`). It lists colours by name (`#RRGGBB`, `R, G, B` or
a colour name); missing or empty colours use the default for its base. A
colour with a mistake uses its default; a file that cannot be read at all
falls back to its backup, then (for a built in theme) to the built in
original, otherwise to the standard look. The user is told what went wrong
once the main window is open (not when running with /hide or /unattended),
and it is logged.

Built in themes: **Dark** (Windows' dark look, fixed) and **Cream** (light
base: warm cream colours taken from the Magnetar browser extension's glass
toolbar, github.com/ArrCee76/magnetar; only its colours, since Windows Forms
cannot draw its see-through glass, blur or gradients).

## How it works

* `ThemeManager`: start up, theme files, backups, the chosen theme. A thread
  message hook sees every window just before it first appears (after it has
  loaded, before it paints), which is where it is themed and where the Theme
  setting is added to Preferences. An idle sweep catches windows the hook
  cannot see.
* `ThemeStyler` (split by area into ThemeStyler.*.cs): the theme's colours
  for backgrounds, text, tab strips, headers, calendar, progress bars, menus
  and web pages (every theme), plus, for dark based themes only, what
  Windows' own dark mode leaves light or unreadable: tab strips, list
  headers, group titles and row colours, the calendar, progress bars,
  disabled labels, link labels, SourceGrid grids, black icons, and colours
  TV Rename hard codes for the light look (warning pink, schedule colours,
  filter buttons, grey text). When a theme changes the background colours,
  menus, toolbars and the status bar get a renderer in the theme's colours
  (ThemeStyler.Menus.cs).
* `WebPageStyler`: a small style sheet for the Information, Images and
  Summary pages (only TV Rename's own pages, never real web sites).
* `ThemeChooser`: the Theme setting in Preferences.
