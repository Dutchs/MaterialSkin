using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MaterialSkin.Controls
{
    public class MaterialToolTip : ToolTip, IMaterialControl
    {
        [Browsable(false)]
        public int Depth { get; set; }
        [Browsable(false)]
        public MaterialSkinManager SkinManager { get { return MaterialSkinManager.Instance; } }
        [Browsable(false)]
        public MouseState MouseState { get; set; }

        public int TextPadding { get; set; }

        public MaterialToolTip() : base()
        {
            TextPadding = 5;
            IsBalloon = false;
            OwnerDraw = true;
            Draw += new DrawToolTipEventHandler(DrawToolTipEvent);
            Popup += new PopupEventHandler(PopupEvent);
        }

        public MaterialToolTip(IContainer container) : this()
        {
            container.Add(this);
        }

        public void RemoveToolTip(Control control)
        {
            SetToolTip(control, null);
        }

        private void DrawToolTipEvent(object sender, DrawToolTipEventArgs e)
        {
            Graphics g = e.Graphics;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            //Background
            g.Clear(SkinManager.GetApplicationBackgroundColor());

            //Text
            using (StringFormat sf = new StringFormat(StringFormatFlags.NoWrap) { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center })
                g.DrawString(e.ToolTipText.ToUpper(), SkinManager.ROBOTO_MEDIUM_10, SkinManager.GetPrimaryTextBrush(), e.Bounds, sf);

            //Border
            Rectangle BorderRect = new Rectangle(e.Bounds.Location, new Size(e.Bounds.Width - 1, e.Bounds.Height - 1));
            using (GraphicsPath borderRegion = DrawHelper.CreateRoundRect(BorderRect, 1f))
                g.DrawPath(SkinManager.ColorScheme.AccentPen, borderRegion);
        }

        private void PopupEvent(object sender, PopupEventArgs e)
        {
            // measure new tooltip size
            using (Graphics g = Graphics.FromHwnd(e.AssociatedWindow.Handle))
            using (StringFormat sf = new StringFormat(StringFormatFlags.NoWrap) { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center })
            {
                SizeF gMeasureSize = g.MeasureString(GetToolTip(e.AssociatedControl).ToUpper(), SkinManager.ROBOTO_MEDIUM_10, new SizeF(), sf);
                Size gNewSize = Size.Ceiling(gMeasureSize);
                e.ToolTipSize = new Size(gNewSize.Width + (TextPadding * 2), gNewSize.Height + (TextPadding * 2));
            }
        }
    }
}
