﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace EEditor
{
    class ToolPen : Tool
    {
        public bool mouseMove = false;
        public string incfg = null;
        public ToolPen(EditArea editArea)
            : base(editArea)
        {
        }
        public static Dictionary<int, int> rotation = new Dictionary<int, int>();
        public static Dictionary<int, int> id = new Dictionary<int, int>();
        public static Dictionary<int, int> target = new Dictionary<int, int>();
        public static Dictionary<int, string> text = new Dictionary<int, string>();
        public static Stack<string> undolist = new Stack<string>();
        public static Stack<string> redolist = new Stack<string>();
        public int lockid = 0;
        private void PlaceBrick(int x, int y, bool button, bool useincfg, bool mousedown)
        {
            int width = editArea.BlockWidth;
            int height = editArea.BlockHeight;
            x = Math.Min(x, width - 1);
            y = Math.Min(y, height - 1);
            x = Math.Max(0, x);
            y = Math.Max(0, y);
            if (!IsPaintable(x, y, PenID, true) && !IsPaintable(x, y, PenID, false)) { editArea.mouseDown = false; }
            else
            {
                if (!button)
                {
                    //if (editArea.IsBackground)
                    //{
                    if (editArea.CtrlDown)
                    {
                        int bfid = editArea.CurFrame.Foreground[y, x];
                        if (mousedown)
                        {
                            if (bfid != 0)
                                editArea.CurFrame.Background[y, x] = 0;
                        }
                        else
                        {
                            editArea.CurFrame.Background[y, x] = 0;
                        }
                        editArea.mouseDown = false;
                    }
                    else if (editArea.ShiftDown)
                    {
                        int bfid = editArea.CurFrame.Foreground[y, x];
                        int loc9 = 0;
                        int loc8 = PenID >= 500 && PenID <= 999 ? 1 : 0;
                        if (mousedown)
                        {

                            loc9 = loc8;
                            if (bfid != 0)
                            {
                                loc9 = 0;

                                if (IsPaintable(x, y, 0, true))
                                {
                                    editArea.CurFrame.Foreground[y, x] = 0;
                                }
                            }
                            else
                            {
                                loc9 = 1;
                                editArea.CurFrame.Background[y, x] = 0;
                            }
                            lockid = loc9;

                        }
                        else
                        {
                            loc9 = lockid;
                            if (loc9 == 0 && IsPaintable(x, y, 0, true))
                            {
                                editArea.CurFrame.Foreground[y, x] = 0;
                            }
                            if (loc9 == 1) editArea.CurFrame.Background[y, x] = 0;

                        }
                        editArea.mouseDown = false;
                    }
                    else
                    {
                        if (PenID < 500 || PenID >= 1001)
                        {
                            if (PenID != editArea.CurFrame.Foreground[y, x])
                            {
                                if (mouseMove || useincfg)
                                {
                                    incfg += PenID + ":" + editArea.CurFrame.Foreground[y, x] + ":" + x + ":" + y + ":";
                                }
                                else
                                {
                                    undolist.Push(PenID + ":" + editArea.CurFrame.Foreground[y, x] + ":" + x + ":" + y);
                                }
                            }
                            if (editArea.CurFrame.Foreground[y, x] != 0)
                            {
                                if (!MainForm.userdata.dontReplaceBlocks)
                                {
                                    if (PenID != 77 && PenID != 83 && PenID != 1520)
                                    {
                                        if (IsPaintable(x, y, PenID, true) && IsPaintable(x, y, PenID, false))
                                        {
                                            editArea.CurFrame.Foreground[y, x] = PenID;
                                        }
                                        if (rotation.ContainsKey(PenID))
                                        {
                                            editArea.CurFrame.BlockData[y, x] = rotation[PenID];
                                        }
                                        if (text.ContainsKey(PenID))
                                        {
                                            editArea.CurFrame.BlockData3[y, x] = text[PenID];
                                        }
                                    }
                                }
                                if (MainForm.userdata.dontReplaceBlocks) editArea.mouseDown = false;

                            }
                            else
                            {
                                if (PenID != 77 && PenID != 83 && PenID != 1520)
                                {
                                    if (IsPaintable(x, y, PenID, true) && IsPaintable(x, y, PenID, false))
                                    {
                                        editArea.CurFrame.Foreground[y, x] = PenID;
                                    }
                                    if (rotation.ContainsKey(PenID))
                                    {
                                        editArea.CurFrame.BlockData[y, x] = rotation[PenID];
                                    }
                                    if (text.ContainsKey(PenID))
                                    {
                                        editArea.CurFrame.BlockData3[y, x] = text[PenID];
                                    }
                                }
                            }

                        }
                        else if (PenID >= 500 && PenID <= 999)
                        {
                            if (PenID != editArea.CurFrame.Background[y, x])
                            {
                                if (mouseMove || useincfg)
                                {
                                    incfg += PenID + ":" + editArea.CurFrame.Background[y, x] + ":" + x + ":" + y + ":";
                                }
                                else
                                {
                                    undolist.Push(PenID + ":" + editArea.CurFrame.Background[y, x] + ":" + x + ":" + y);
                                }
                            }
                            if (editArea.CurFrame.Background[y, x] != 0)
                            {
                                if (!MainForm.userdata.dontReplaceBlocks)
                                {
                                    editArea.CurFrame.Background[y, x] = PenID;
                                }
                                if (MainForm.userdata.dontReplaceBlocks) editArea.mouseDown = false;
                            }
                            else
                            {
                                editArea.CurFrame.Background[y, x] = PenID;
                            }
                        }
                    }
                }
                if (editArea.ChangeBlock)
                {
                    editArea.ChangeBlock = false;
                    int fid = editArea.CurFrame.Foreground[y, x];
                    if (fid != 0)
                    {
                        editArea.MainForm.setBrick(editArea.CurFrame.Foreground[y, x], false);
                    }
                    else editArea.MainForm.setBrick(editArea.CurFrame.Background[y, x], false);

                }
                else
                {
                    if (PenID < 500 || PenID >= 1001)
                    {
                        var bid = editArea.CurFrame.Foreground[y, x];
                        if (bdata.increase1.Contains(bid))
                        {
                            if (!mouseMove)
                            {
                                if (button)
                                {
                                    editArea.CurFrame.BlockData[y, x] += 1;
                                    if (editArea.CurFrame.BlockData[y, x] > 1) editArea.CurFrame.BlockData[y, x] = 0;
                                    editArea.CurFrame.BlockData1[y, x] = 0;
                                    editArea.CurFrame.BlockData2[y, x] = 0;
                                    if (rotation.ContainsKey(bid)) { rotation[bid] = editArea.CurFrame.BlockData[y, x]; }
                                    else { rotation.Add(bid, editArea.CurFrame.BlockData[y, x]); }
                                }
                            }
                            else
                            {
                                if (rotation.ContainsKey(bid)) { rotation[bid] = editArea.CurFrame.BlockData[y, x]; }
                                else { rotation.Add(bid, editArea.CurFrame.BlockData[y, x]); }
                            }
                        }
                        else if (bdata.increase3.Contains(bid))
                        {
                            if (!mouseMove)
                            {
                                if (button)
                                {
                                    editArea.CurFrame.BlockData[y, x] += 1;
                                    if (editArea.CurFrame.BlockData[y, x] > 3) editArea.CurFrame.BlockData[y, x] = 0;
                                    editArea.CurFrame.BlockData1[y, x] = 0;
                                    editArea.CurFrame.BlockData2[y, x] = 0;
                                    if (rotation.ContainsKey(bid)) { rotation[bid] = editArea.CurFrame.BlockData[y, x]; }
                                    else { rotation.Add(bid, editArea.CurFrame.BlockData[y, x]); }
                                }
                            }
                            else
                            {
                                if (rotation.ContainsKey(bid)) { rotation[bid] = editArea.CurFrame.BlockData[y, x]; }
                                else { rotation.Add(bid, editArea.CurFrame.BlockData[y, x]); }
                            }
                        }
                        else if (bdata.increase2.Contains(bid))
                        {
                            if (!mouseMove)
                            {
                                if (button)
                                {
                                    editArea.CurFrame.BlockData[y, x] += 1;
                                    if (editArea.CurFrame.BlockData[y, x] > 2) editArea.CurFrame.BlockData[y, x] = 0;
                                    editArea.CurFrame.BlockData1[y, x] = 0;
                                    editArea.CurFrame.BlockData2[y, x] = 0;
                                    if (rotation.ContainsKey(bid)) { rotation[bid] = editArea.CurFrame.BlockData[y, x]; }
                                    else { rotation.Add(bid, editArea.CurFrame.BlockData[y, x]); }
                                }

                            }
                            else
                            {
                                if (rotation.ContainsKey(bid)) { rotation[bid] = editArea.CurFrame.BlockData[y, x]; }
                                else { rotation.Add(bid, editArea.CurFrame.BlockData[y, x]); }
                            }
                        }
                        else if (bdata.increase5.Contains(bid))
                        {
                            if (!mouseMove)
                            {
                                if (button)
                                {
                                    editArea.CurFrame.BlockData[y, x] += 1;
                                    if (editArea.CurFrame.BlockData[y, x] > 5) editArea.CurFrame.BlockData[y, x] = 0;
                                    editArea.CurFrame.BlockData1[y, x] = 0;
                                    editArea.CurFrame.BlockData2[y, x] = 0;
                                    if (rotation.ContainsKey(bid)) { rotation[bid] = editArea.CurFrame.BlockData[y, x]; }
                                    else { rotation.Add(bid, editArea.CurFrame.BlockData[y, x]); }
                                }
                            }
                            else
                            {
                                if (rotation.ContainsKey(bid)) { rotation[bid] = editArea.CurFrame.BlockData[y, x]; }
                                else { rotation.Add(bid, editArea.CurFrame.BlockData[y, x]); }
                            }
                        }
                        else if (bdata.increase4.Contains(bid))
                        {
                            if (!mouseMove)
                            {
                                if (button)
                                {
                                    editArea.CurFrame.BlockData[y, x] += 1;
                                    if (editArea.CurFrame.BlockData[y, x] > 4) editArea.CurFrame.BlockData[y, x] = 0;
                                    editArea.CurFrame.BlockData1[y, x] = 0;
                                    editArea.CurFrame.BlockData2[y, x] = 0;
                                    if (rotation.ContainsKey(bid)) { rotation[bid] = editArea.CurFrame.BlockData[y, x]; }
                                    else { rotation.Add(bid, editArea.CurFrame.BlockData[y, x]); }
                                }
                            }
                            else
                            {
                                if (rotation.ContainsKey(bid)) { rotation[bid] = editArea.CurFrame.BlockData[y, x]; }
                                else { rotation.Add(bid, editArea.CurFrame.BlockData[y, x]); }
                            }
                        }
                        else if (bid != 77 && bid != 83 && bid != 1520 && bdata.goal.Contains(bid) && bid != 423 && bid != 1027 && bid != 1028)
                        {
                            if (button)
                            {

                                using (NumberChanger co = new NumberChanger())
                                {
                                    co.NumberChangerNumeric.Value = editArea.CurFrame.BlockData[y, x];
                                    if (co.ShowDialog() == DialogResult.OK)
                                    {
                                        var value = (int)co.NumberChangerNumeric.Value == -1 ? 1000 : (int)co.NumberChangerNumeric.Value;
                                        editArea.CurFrame.BlockData[y, x] = value;
                                        if (rotation.ContainsKey(bid)) { rotation[bid] = value; }
                                        else { rotation.Add(bid, value); }
                                    }
                                    editArea.mouseDown = false;

                                }
                            }
                            else
                            {
                                if (rotation.ContainsKey(bid))
                                {
                                    editArea.CurFrame.BlockData[y, x] = rotation[bid];
                                }
                            }


                        }
                        else if (bid == 461)
                        {
                            if (!button)
                            {
                                if (rotation.ContainsKey(bid))
                                {
                                    editArea.CurFrame.BlockData[y, x] = rotation[bid];
                                }
                            }
                        }
                        else if (bid >= 417 && bid <= 423 || bid == 1027 || bid == 1028)
                        {
                            if (button)
                            {
                                if (bid == 421 || bid == 422)
                                {
                                    using (NumberChanger co = new NumberChanger())
                                    {
                                        co.NumberChangerNumeric.Value = editArea.CurFrame.BlockData[y, x];
                                        if (co.ShowDialog() == DialogResult.OK)
                                        {
                                            editArea.CurFrame.BlockData[y, x] = (int)co.NumberChangerNumeric.Value;
                                            if (rotation.ContainsKey(bid)) { rotation[bid] = (int)co.NumberChangerNumeric.Value; }
                                            else { rotation.Add(bid, (int)co.NumberChangerNumeric.Value); }
                                        }
                                        else
                                        {
                                            if (bid == 422 || bid == 421)
                                            {
                                                editArea.CurFrame.BlockData[y, x] = 10;
                                            }
                                            else
                                            {
                                                editArea.CurFrame.BlockData[y, x] = 0;
                                            }
                                        }
                                        editArea.mouseDown = false;

                                    }
                                }
                                else if (bid == 423 || bid == 1027 || bid == 1028)
                                {
                                    using (TeamColorChanger co = new TeamColorChanger())
                                    {
                                        co.SetColorId = editArea.CurFrame.BlockData[y, x];
                                        //co.NumberChangerNumeric.Value = editArea.CurFrame.BlockData[y, x];
                                        if (co.ShowDialog() == DialogResult.OK)
                                        {
                                            editArea.CurFrame.BlockData[y, x] = co.SetColorId;
                                            if (rotation.ContainsKey(bid)) { rotation[PenID] = co.SetColorId; }
                                            else { rotation.Add(PenID, co.SetColorId); }
                                        }
                                        editArea.mouseDown = false;
                                    }
                                }
                            }
                            else
                            {
                                /*if (bid >= 417 && bid <= 420)
                                {
                                    editArea.CurFrame.BlockData[y, x] += 1;
                                    if (editArea.CurFrame.BlockData[y, x] > 1) editArea.CurFrame.BlockData[y, x] = 0;
                                    editArea.CurFrame.BlockData1[y, x] = 0;
                                    editArea.CurFrame.BlockData2[y, x] = 0;
                                }
                                 */
                                if (bid == 423 || bid == 1027 || bid == 1028)
                                {
                                    if (rotation.ContainsKey(bid))
                                    {
                                        editArea.CurFrame.BlockData[y, x] = rotation[bid];
                                    }
                                    else
                                    {
                                        editArea.CurFrame.BlockData[y, x] = 0;
                                    }

                                }
                                if (bid == 422 || bid == 421)
                                {
                                    if (rotation.ContainsKey(bid))
                                    {
                                        editArea.CurFrame.BlockData[y, x] = rotation[bid];
                                    }
                                    else
                                    {
                                        editArea.CurFrame.BlockData[y, x] = 10;
                                    }
                                }
                            }
                        }
                        else if (bid == 83 || bid == 77 || bid == 1520)
                        {
                            string message = "Piano";
                            if (button)
                            {
                                switch (bid)
                                {
                                    case 83:
                                        message = "Drums";
                                        break;
                                    case 77:
                                        message = "Piano";
                                        break;
                                    case 1520:
                                        message = "Guitar";
                                        break;
                                }
                                MessageBox.Show("EEditor doesn't support the new " + message + " blocks yet.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                editArea.mouseDown = false;
                            }
                        }
                        else if (bid == 374 || bid == 385)
                        {

                            if (button)
                            {
                                using (Text co = new Text())
                                {
                                    co.textbox.Text = editArea.CurFrame.BlockData3[y, x];
                                    co.id1 = bid;
                                    if (bid == 385) co.cm1 = editArea.CurFrame.BlockData[y, x];
                                    if (co.ShowDialog() == DialogResult.OK)
                                    {
                                        if (text.ContainsKey(bid)) { text[bid] = co.textbox.Text; }
                                        else { text.Add(bid, co.textbox.Text); }
                                        editArea.CurFrame.BlockData3[y, x] = co.textbox.Text;

                                        if (bid == 385)
                                        {
                                            if (rotation.ContainsKey(bid)) { rotation[bid] = co.cm1; }
                                            else { rotation.Add(bid, co.cm1); }
                                            editArea.CurFrame.BlockData[y, x] = co.cm1;
                                        }
                                    }

                                }
                                editArea.mouseDown = false;

                            }
                            else
                            {
                                if (bid == 385)
                                {
                                    if (rotation.ContainsKey(bid)) { editArea.CurFrame.BlockData[y, x] = rotation[bid]; }
                                    else { editArea.CurFrame.BlockData[y, x] = 0; }
                                }
                                if (text.ContainsKey(bid)) { editArea.CurFrame.BlockData3[y, x] = text[bid]; }
                                else { editArea.CurFrame.BlockData3[y, x] = "Unknown"; }
                            }
                        }
                        else if (bid == 242 || bid == 381)
                        {
                            if (button)
                            {
                                using (CoinsOptions co = new CoinsOptions())
                                {
                                    co.NumericUpdown1.Value = editArea.CurFrame.BlockData[y, x];
                                    co.NumericUpdown2.Value = editArea.CurFrame.BlockData1[y, x];
                                    co.NumericUpdown3.Value = editArea.CurFrame.BlockData2[y, x];
                                    if (co.ShowDialog() == DialogResult.OK)
                                    {
                                        if (co.NumericUpdown1.Value >= 0 && co.NumericUpdown1.Value <= 3)
                                        {
                                            if (rotation.ContainsKey(bid)) { rotation[bid] = (int)co.NumericUpdown1.Value; }
                                            else { rotation.Add(bid, (int)co.NumericUpdown1.Value); }
                                            if (id.ContainsKey(bid)) { id[bid] = (int)co.NumericUpdown2.Value; }
                                            else { id.Add(bid, (int)co.NumericUpdown2.Value); }
                                            if (target.ContainsKey(bid)) { target[bid] = (int)co.NumericUpdown3.Value; }
                                            else { target.Add(bid, (int)co.NumericUpdown3.Value); }
                                            editArea.CurFrame.BlockData[y, x] = (int)co.NumericUpdown1.Value;
                                            editArea.CurFrame.BlockData1[y, x] = (int)co.NumericUpdown2.Value;
                                            editArea.CurFrame.BlockData2[y, x] = (int)co.NumericUpdown3.Value;
                                        }
                                        else
                                        {
                                            MessageBox.Show("The rotation value is too high. ", "Rotation too high", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                                            var num2 = 0;
                                            editArea.CurFrame.BlockData[y, x] = num2;
                                            editArea.CurFrame.BlockData1[y, x] = num2;
                                            editArea.CurFrame.BlockData2[y, x] = num2;
                                        }
                                    }
                                    editArea.mouseDown = false;

                                }
                            }
                            else
                            {
                                if (rotation.ContainsKey(bid)) { editArea.CurFrame.BlockData[y, x] = rotation[bid]; }
                                else { editArea.CurFrame.BlockData[y, x] = 0; }
                                if (id.ContainsKey(bid)) { editArea.CurFrame.BlockData1[y, x] = id[bid]; }
                                else { editArea.CurFrame.BlockData1[y, x] = 0; }
                                if (target.ContainsKey(bid)) { editArea.CurFrame.BlockData2[y, x] = target[bid]; }
                                else { editArea.CurFrame.BlockData2[y, x] = 0; }
                            }
                        }
                    }
                }
                if (!editArea.ShowLines)
                {
                    Point p = new Point(x * 16 - Math.Abs(editArea.AutoScrollPosition.X), y * 16 - Math.Abs(editArea.AutoScrollPosition.Y));
                    Graphics g = Graphics.FromImage(editArea.Back);
                    editArea.Draw(x, y, g, MainForm.userdata.thisColor);
                    g.Save();
                    editArea.Invalidate(new Rectangle(p, new Size(16, 16)));
                }
                //}
            }
        }

        public override void MouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (!ToolFill.filling)
                {
                    Point p = GetLocation(e);
                    if (IsPaintable(p.X, p.Y, PenID, true) && IsPaintable(p.X, p.Y, PenID, false))
                    {
                        mouseMove = false;
                        if (PenSize == 1)
                        {
                            if (Clipboard.ContainsData("EEBrush"))
                            {
                                string[][,] data = (string[][,])Clipboard.GetData("EEBrush");
                                if (data != null && data.Length == 6)
                                {
                                    for (int y = 0; y < data[0].GetLength(0); y++)
                                    {
                                        for (int x = 0; x < data[0].GetLength(1); x++)
                                        {
                                            PenID = Convert.ToInt32(data[0][y, x]);
                                            PlaceBrick(p.X + x, p.Y + y, false, true, false);
                                        }
                                    }
                                    //Clipboard.Clear();
                                    //Console.WriteLine(data[0]);
                                }
                            }
                            else
                            {
                                PlaceBrick(p.X, p.Y, false, false, true);
                            }
                        }
                        else if (PenSize >= 2 && PenSize <= 10)
                        {
                            for (int yy = 0; yy < PenSize; yy++)
                            {
                                for (int xx = 0; xx < PenSize; xx++)
                                {
                                    PlaceBrick(p.X + xx, p.Y + yy, false, true, false);
                                }
                            }
                        }
                    }
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (!ToolFill.filling)
                {
                    Point p = GetLocation(e);
                    if (IsPaintable(p.X, p.Y, PenID, true))
                    {
                        mouseMove = false;
                        PlaceBrick(p.X, p.Y, true, false, true);
                    }
                }
            }
        }
        public override void MouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (incfg != null)
                {
                    ToolPen.undolist.Push(incfg);
                    incfg = null;
                }
            }
        }
        public override void MouseMoveHover(MouseEventArgs e)
        {
        }
        public override void MouseMove(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (!ToolFill.filling)
                {
                    Point p = GetLocation(e);
                    if (IsPaintable(p.X, p.Y, PenID, true) && IsPaintable(p.X, p.Y, PenID, false))
                    {
                        mouseMove = true;
                        if (PenSize == 1)
                        {
                                PlaceBrick(p.X, p.Y, false, false, true);
                            
                        }
                        else if (PenSize >= 2 && PenSize <= 10)
                        {
                            for (int yy = 0; yy < PenSize; yy++)
                            {
                                for (int xx = 0; xx < PenSize; xx++)
                                {
                                    PlaceBrick(p.X + xx, p.Y + yy, false, true, false);
                                }
                            }
                        }
                    }
                }

            }
            else if (e.Button == MouseButtons.Right)
            {
                Point p = GetLocation(e);
                if (IsPaintable(p.X, p.Y, PenID, true))
                {
                    mouseMove = true;
                    PlaceBrick(p.X, p.Y, true, false, false);
                }
            }
        }

        public override void KeyDown(KeyEventArgs e)
        {
            //removed +/- hotkey
            int delta = 0;
            if (e.KeyCode == Keys.Add) delta = 1;
            else if (e.KeyCode == Keys.Subtract) delta = -1;
            if (delta != 0)
            {
                Point pos = GetLocation(editArea.GetMousePosition());
                int x = pos.X;
                int y = pos.Y;
                Frame f = editArea.CurFrame;
                if (f.Foreground[y, x] == 43 || f.Foreground[y, x] == 165 || f.Foreground[y, x] == 214 || f.Foreground[y, x] == 213 || f.Foreground[y, x] == 467 || f.Foreground[y, x] == 1079 || f.Foreground[y, x] == 1080 || f.Foreground[y, x] == 113 || f.Foreground[y, x] == 185 || f.Foreground[y, x] == 184)
                {
                    Point p = new Point(x * 16 - Math.Abs(editArea.AutoScrollPosition.X), y * 16 - Math.Abs(editArea.AutoScrollPosition.Y));
                    f.BlockData[y, x] = Math.Max(1, Math.Min(f.BlockData[y, x] + delta, 999));
                    Graphics g = Graphics.FromImage(editArea.Back);
                    editArea.Draw(x, y, g, Color.Transparent);
                    g.Save();
                    editArea.Invalidate(new Rectangle(p, new Size(16, 16)));
                }

            }
        }
    }
}
