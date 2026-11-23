using System.Runtime.InteropServices;

namespace TVRename.Forms;
public class ThemeableMessageBox : Form
{
    // Windows API to force dark title bar (Windows 10 build 18985+ and Windows 11)
    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);
    private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

    // Wording for the Yes and No buttons, when the standard words do not fit the question
    private readonly string yesText;
    private readonly string noText;

    // No is the default (Enter, and where the focus starts) when Yes would lose work
    private readonly bool noIsDefault;

    private ThemeableMessageBox(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, string yesText = "Yes", string noText = "No", bool noIsDefault = false)
    {
        this.yesText = yesText;
        this.noText = noText;
        this.noIsDefault = noIsDefault;

        // 1. Form Styling
        this.Text = caption;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.ShowInTaskbar = false;
        this.StartPosition = FormStartPosition.CenterParent;
        this.AutoScaleMode = AutoScaleMode.Font;

        // 2. Layout Panels
        Panel topPanel = new()
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(20, 20, 20, 15)
        };

        Panel bottomPanel = new()
        {
            Dock = DockStyle.Bottom,
            Height = 52,
            Padding = new Padding(10)
        };

        // 3. Icon Handling
        Icon? sysIcon = GetSystemIcon(icon);
        int textLeftOffset = 20;

        if (sysIcon != null)
        {
            PictureBox picIcon = new()
            {
                Image = sysIcon.ToBitmap(),
                SizeMode = PictureBoxSizeMode.AutoSize,
                Location = new Point(20, 22)
            };
            topPanel.Controls.Add(picIcon);
            textLeftOffset = picIcon.Right + 15;
        }

        // 4. Message Label
        Label lblMessage = new()
        {
            Text = text,
            AutoSize = true,
            MaximumSize = new Size(360, 0),
            Location = new Point(textLeftOffset, 22),
        };
        topPanel.Controls.Add(lblMessage);

        // 5. Dynamic Sizing
        int contentWidth = Math.Max(280, lblMessage.Right + 25);
        int contentHeight = Math.Max(65, lblMessage.Bottom + 20);
        ClientSize = new Size(contentWidth, contentHeight + bottomPanel.Height);

        // 6. Buttons
        AddButtons(bottomPanel, buttons);

        Controls.Add(topPanel);
        Controls.Add(bottomPanel);

        this.UpdateColorControls();
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        // Apply dark mode to the native window title bar if needed
        if (ThemeManager.IsDark)
        { 
            int useDarkMode = 1;
            DwmSetWindowAttribute(Handle, DWMWA_USE_IMMERSIVE_DARK_MODE, ref useDarkMode, sizeof(int));
        }
    }

    private void AddButtons(Panel container, MessageBoxButtons buttons)
    {
        FlowLayoutPanel flow = new()
        {
            Dock = DockStyle.Right,
            FlowDirection = FlowDirection.RightToLeft,
            AutoSize = true,
            WrapContents = false
        };

        switch (buttons)
        {
            case MessageBoxButtons.OK:
                flow.Controls.Add(CreateButton("OK", DialogResult.OK, true));
                break;
            case MessageBoxButtons.OKCancel:
                flow.Controls.Add(CreateButton("Cancel", DialogResult.Cancel, false));
                flow.Controls.Add(CreateButton("OK", DialogResult.OK, true));
                break;
            case MessageBoxButtons.YesNo:
                flow.Controls.Add(CreateButton(noText, DialogResult.No, noIsDefault));
                flow.Controls.Add(CreateButton(yesText, DialogResult.Yes, !noIsDefault));
                break;
            case MessageBoxButtons.YesNoCancel:
                flow.Controls.Add(CreateButton("Cancel", DialogResult.Cancel, false));
                flow.Controls.Add(CreateButton("No", DialogResult.No, false));
                flow.Controls.Add(CreateButton("Yes", DialogResult.Yes, true));
                break;
            case MessageBoxButtons.RetryCancel:
                flow.Controls.Add(CreateButton("Cancel", DialogResult.Cancel, false));
                flow.Controls.Add(CreateButton("Retry", DialogResult.Retry, true));
                break;
            case MessageBoxButtons.AbortRetryIgnore:
                flow.Controls.Add(CreateButton("Ignore", DialogResult.Ignore, false));
                flow.Controls.Add(CreateButton("Retry", DialogResult.Retry, false));
                flow.Controls.Add(CreateButton("Abort", DialogResult.Abort, true));
                break;
        }

        container.Controls.Add(flow);
    }

    private Button CreateButton(string text, DialogResult result, bool isDefault)
    {
        Button btn = new()
        {
            Text = text,
            DialogResult = result,
            Width = Math.Max(80, TextRenderer.MeasureText(text, Font).Width + 24),
            Height = 28,
            Margin = new Padding(6, 2, 0, 0),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
        };

        btn.FlatAppearance.BorderSize = 1;

        if (isDefault) this.AcceptButton = btn;
        if (result == DialogResult.Cancel) this.CancelButton = btn;

        return btn;
    }

    private static Icon? GetSystemIcon(MessageBoxIcon icon) => icon switch
    {
        MessageBoxIcon.Error => SystemIcons.Error,
        MessageBoxIcon.Warning => SystemIcons.Warning,
        MessageBoxIcon.Information => SystemIcons.Information,
        MessageBoxIcon.Question => SystemIcons.Question,
        _ => null
    };

    // Public Static Show Overloads
    public static DialogResult Show(string text, string caption = "", MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None)
    {
        using var box = new ThemeableMessageBox(text, caption, buttons, icon);
        return box.ShowDialog();
    }

    public static DialogResult Show(IWin32Window owner, string text, string caption = "", MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None)
    {
        using var box = new ThemeableMessageBox(text, caption, buttons, icon);
        return box.ShowDialog(owner);
    }

    // A Yes/No question with its own button wording, such as "Restart now" and "Later"
    public static DialogResult Show(IWin32Window owner, string text, string caption, string yesText, string noText, MessageBoxIcon icon, bool noIsDefault)
    {
        using var box = new ThemeableMessageBox(text, caption, MessageBoxButtons.YesNo, icon, yesText, noText, noIsDefault);

        // Start on the default button, so Enter and the focus agree
        box.Shown += (_, _) => (box.AcceptButton as Button)?.Focus();
        return box.ShowDialog(owner);
    }
}
