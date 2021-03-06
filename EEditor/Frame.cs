﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayerIOClient;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Drawing;
using System.IO;
namespace EEditor
{
    public class Frame
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public int[,] Foreground { get; set; }
        public int[,] Background { get; set; }
        public int[,] BlockData { get; set; }
        public int[,] BlockData1 { get; set; }
        public int[,] BlockData2 { get; set; }
        public string[,] BlockData3 { get; set; }
        public static byte[] xx;
        public static byte[] yy;
        public static byte[] xx1;
        public static byte[] yy1;
        public static string[] split1;

        public Frame(int width, int height)
        {
            Width = width;
            Height = height;
            Foreground = new int[Height, Width];
            Background = new int[Height, Width];
            BlockData = new int[Height, Width];
            BlockData1 = new int[Height, Width];
            BlockData2 = new int[Height, Width];
            BlockData3 = new string[height, width];
        }
        public event EventHandler<StatusChangedArgs> StatusChanged;

        protected void OnStatusChanged(string msg, DateTime time,bool done = false,int totallines = 0,int countlines = 0)
        {
            if (StatusChanged != null) StatusChanged(this, new StatusChangedArgs(msg, time,done,totallines,countlines));
        }
        public void Reset(bool frame)
        {
            for (int i = 0; i < Height; ++i)
            {
                for (int j = 0; j < Width; ++j)
                {
                    if (i == 0 || j == 0 || i == Height - 1 || j == Width - 1)
                    {
                        if (Width == 110 && Height == 110)
                        {
                            if (!frame) { Foreground[i, j] = 182; }
                            else { Foreground[i, j] = -1; }
                        }
                        else
                        {
                            if (!frame) { Foreground[i, j] = 9; }
                            else { Foreground[i, j] = -1; }
                        }

                    }
                    else
                    {
                        if (!frame) { Foreground[i, j] = 0; }
                        else { Foreground[i, j] = -1; }
                    }
                }
            }

        }

        public static Frame FromMessage2(DatabaseObject dbo)
        {
            var w = 0;
            var h = 0;
            if (dbo.Contains("width") && dbo.Contains("height"))
            {
                return FromMessage1(dbo, dbo.GetInt("width"), dbo.GetInt("height"), 0);
            }
            else
            {
                if (dbo.Contains("type"))
                {
                    switch ((int)dbo["type"])
                    {
                        case 1:
                            w = 50;
                            h = 50;
                            break;
                        case 2:
                            w = 100;
                            h = 100;
                            break;
                        default:
                        case 3:
                            w = 200;
                            h = 200;
                            break;
                        case 4:
                            w = 400;
                            h = 50;
                            break;
                        case 5:
                            w = 400;
                            h = 200;
                            break;
                        case 6:
                            w = 100;
                            h = 400;
                            break;
                        case 7:
                            w = 636;
                            h = 50;
                            break;
                        case 8:
                            w = 110;
                            h = 110;
                            break;
                        case 11:
                            w = 300;
                            h = 300;
                            break;
                        case 12:
                            w = 250;
                            h = 150;
                            break;
                    }
                    return FromMessage1(dbo, w, h, 0);
                }
                else
                {
                    return FromMessage1(dbo, 200, 200, 0);
                }

            }
        }
        public static Frame FromMessage(PlayerIOClient.Message e, bool f)
        {
            if (bdata.ParamNumbers(e, 18, "System.Int32") && bdata.ParamNumbers(e, 19, "System.Int32"))
            {
                return FromMessage(e, e.GetInt(18), e.GetInt(19), 0, f);
            }
            else
            {
                return null;
            }
        }

        #region read from init
        public static Frame FromMessage(PlayerIOClient.Message e, int width, int height, int x1, bool f)
        {
            Frame frame;
            int width1 = width;
            int height1 = height;
            frame = new Frame(width, height);
            var chunks = EEditor.InitParse.Parse(e);
            if (MainForm.userdata.level.StartsWith("OW"))
            {
                if (e.GetBoolean(14)) { MainForm.OpenWorld = true; MainForm.OpenWorldCode = false; }
                else if (!e.GetBoolean(14)) { MainForm.OpenWorld = true; MainForm.OpenWorldCode = true; }
            }
            //int num2 = 0;
            foreach (var chunk in chunks)
            {
                /*for (int i = 0; i < chunk.Args.Length; i++)
                {
                    Console.WriteLine("Bid: " + chunk.Type + " Length: " + chunk.Args.Length + " Data: " + chunk.Args[i]);
                }
                 */
                foreach (var pos in chunk.Locations)
                {
                    if (chunk.Args.Length == 0)
                    {

                        if ((int)chunk.Layer == 1)
                        {
                            int x = pos.X;
                            int y = pos.Y;
                            frame.Background[y, x] = Convert.ToInt32(chunk.Type);

                        }
                        else
                        {

                            int x = pos.X;
                            int y = pos.Y;
                            frame.Foreground[y, x] = Convert.ToInt32(chunk.Type);
                        }
                    }
                    if (chunk.Args.Length == 1)
                    {
                        if (Convert.ToString(chunk.Args[0]) != "we")
                        {
                            if (bdata.goal.Contains((int)chunk.Type) || bdata.morphable.Contains((int)chunk.Type) || bdata.rotate.Contains((int)chunk.Type) && (int)chunk.Type != 385 && (int)chunk.Type != 374)
                            {
                                frame.Foreground[pos.Y, pos.X] = Convert.ToInt32(chunk.Type);
                                frame.BlockData[pos.Y, pos.X] = Convert.ToInt32(chunk.Args[0]);
                            }
                            else
                            {

                                if ((int)chunk.Type == 385)
                                {
                                    frame.Foreground[pos.Y, pos.X] = Convert.ToInt32(chunk.Type);
                                    frame.BlockData[pos.Y, pos.X] = Convert.ToInt32(chunk.Args[1]);
                                    frame.BlockData3[pos.Y, pos.X] = chunk.Args[0].ToString();
                                }
                                else if ((int)chunk.Type == 374)
                                {
                                    frame.Foreground[pos.Y, pos.X] = Convert.ToInt32(chunk.Type);
                                    frame.BlockData3[pos.Y, pos.X] = chunk.Args[0].ToString();
                                }
                                if ((int)chunk.Type != 374 && (int)chunk.Type != 385)
                                {

                                    if ((int)chunk.Layer == 1)
                                    {
                                        int x = pos.X;
                                        int y = pos.Y;
                                        frame.Background[y, x] = Convert.ToInt32(chunk.Type);

                                    }
                                    else
                                    {

                                        int x = pos.X;
                                        int y = pos.Y;
                                        frame.Foreground[y, x] = Convert.ToInt32(chunk.Type);
                                    }
                                }

                            }
                        }
                        else if (Convert.ToString(chunk.Args[0]) == "we")
                        {
                            if ((int)chunk.Layer == 1)
                            {
                                int x = pos.X;
                                int y = pos.Y;
                                frame.Background[y, x] = Convert.ToInt32(chunk.Type);

                            }
                            else
                            {

                                int x = pos.X;
                                int y = pos.Y;
                                frame.Foreground[y, x] = Convert.ToInt32(chunk.Type);
                            }
                        }
                    }
                    if (chunk.Args.Length == 2 && (int)chunk.Type != 1000)
                    {
                        if (Convert.ToString(chunk.Args[0]) != "we")
                        {
                            frame.Foreground[pos.Y, pos.X] = Convert.ToInt32(chunk.Type);
                            if (chunk.Type == 385)
                            {
                                frame.BlockData[pos.Y, pos.X] = Convert.ToInt32(chunk.Args[1]);
                                frame.BlockData3[pos.Y, pos.X] = chunk.Args[0].ToString();
                            }
                            else if (chunk.Type == 374)
                            {
                                frame.BlockData3[pos.Y, pos.X] = chunk.Args[0].ToString();
                            }
                            else if (bdata.goal.Contains((int)chunk.Type) || bdata.morphable.Contains((int)chunk.Type) || bdata.rotate.Contains((int)chunk.Type))
                            {
                                frame.BlockData[pos.Y, pos.X] = Convert.ToInt32(chunk.Args[0]);
                            }
                        }
                    }
                    //if (chunk.Args.Length == 2 && (int)chunk.Type == 1000) { Chunk.Args[0] Chunk.Args[1] (Colored Text) }
                    if (chunk.Args.Length == 3)
                    {

                        if ((int)chunk.Type == 242 || (int)chunk.Type == 381)
                        {
                            if (Convert.ToString(chunk.Args[0]) != "we" && Convert.ToString(chunk.Args[1]) != "we" && Convert.ToString(chunk.Args[2]) != "we")
                            {

                                frame.Foreground[pos.Y, pos.X] = Convert.ToInt32(chunk.Type);
                                frame.BlockData[pos.Y, pos.X] = Convert.ToInt32(chunk.Args[0]);
                                frame.BlockData1[pos.Y, pos.X] = Convert.ToInt32(chunk.Args[1]);
                                frame.BlockData2[pos.Y, pos.X] = Convert.ToInt32(chunk.Args[2]);
                            }
                        }
                        else if ((int)chunk.Type == 385)
                        {
                            if (Convert.ToString(chunk.Args[0]) != "we" && Convert.ToString(chunk.Args[1]) != "we")
                            {
                                frame.Foreground[pos.Y, pos.X] = Convert.ToInt32(chunk.Type);
                                frame.BlockData[pos.Y, pos.X] = Convert.ToInt32(chunk.Args[1]);
                                frame.BlockData3[pos.Y, pos.X] = Convert.ToString(chunk.Args[0]);
                            }
                        }
                    }
                    if (chunk.Args.Length == 4)
                    {
                        if ((int)chunk.Type == 242 || (int)chunk.Type == 381)
                        {
                            if (Convert.ToString(chunk.Args[0]) != "we" && Convert.ToString(chunk.Args[1]) != "we" && Convert.ToString(chunk.Args[2]) != "we")
                            {

                                frame.Foreground[pos.Y, pos.X] = Convert.ToInt32(chunk.Type);
                                frame.BlockData[pos.Y, pos.X] = Convert.ToInt32(chunk.Args[0]);
                                frame.BlockData1[pos.Y, pos.X] = Convert.ToInt32(chunk.Args[1]);
                                frame.BlockData2[pos.Y, pos.X] = Convert.ToInt32(chunk.Args[2]);
                            }
                        }
                    }
                }
            }
            return frame;
        }
        #endregion

        public static Color UIntToColor(uint color)
        {
            byte a = (byte)(color >> 24);
            byte r = (byte)(color >> 16);
            byte g = (byte)(color >> 8);
            byte b = (byte)(color >> 0);
            return Color.FromArgb(a, r, g, b);
        }

        #region read from database
        public static Frame FromMessage1(DatabaseObject worlds, int width, int height, int x1)
        {
            Frame frame = new Frame(1, 1);

            DatabaseArray worlddata = worlds.GetArray("worlddata");
            if (worlds.Contains("worlddata"))
            {
                frame = new Frame(width, height);
                int width1 = width;
                int height1 = height;
                for (int i = 0; i < worlddata.Count; i++)
                {
                    if (worlddata[i] != null)
                    {

                        DatabaseObject worldinfo = worlddata.GetObject(i);
                        xx1 = worldinfo.GetBytes("x1");
                        yy1 = worldinfo.GetBytes("y1");
                        xx = worldinfo.GetBytes("x");
                        yy = worldinfo.GetBytes("y");
                        uint bid = worldinfo.GetUInt("type");
                        int layer = worldinfo.Contains("layer") ? worldinfo.GetInt("layer") : 0;
                        if (xx != null && yy != null)
                        {
                            for (int xxx = 0; xxx < xx.Length; xxx += 2)
                            {
                                int tmpxx = xx[xxx] << 8 | xx[xxx + 1];
                                int tmpyy = yy[xxx] << 8 | yy[xxx + 1];
                                if (layer == 0)
                                {

                                    if (bdata.goal.Contains((int)bid))
                                    {
                                        if (bid == 77 || bid == 83 || bid == 1520)
                                        {

                                            uint id = worldinfo.GetUInt("id");
                                            frame.Foreground[tmpyy, tmpxx] = (int)bid;
                                            frame.BlockData[tmpyy, tmpxx] = (int)id;
                                        }
                                        else
                                        {

                                            uint rotation = worldinfo.GetUInt("goal");
                                            frame.Foreground[tmpyy, tmpxx] = (int)bid;
                                            frame.BlockData[tmpyy, tmpxx] = (int)rotation;
                                        }
                                    }
                                    else if (bdata.rotate.Contains((int)bid) || bdata.morphable.Contains((int)bid))
                                    {
                                        if (bid == 385)
                                        {
                                            if (worldinfo.Contains("text"))
                                            {
                                                string text = worldinfo.GetString("text");
                                                frame.BlockData3[tmpyy, tmpxx] = text;
                                                if (worldinfo.Contains("signtype"))
                                                {
                                                    uint rotation = worldinfo.GetUInt("signtype");
                                                    frame.BlockData[tmpyy, tmpxx] = (int)rotation;
                                                }
                                                else
                                                {
                                                    frame.BlockData[tmpyy, tmpxx] = 0;
                                                }
                                                frame.Foreground[tmpyy, tmpxx] = (int)bid;
                                                frame.BlockData3[tmpyy, tmpxx] = text;

                                            }
                                        }
                                        else if (bid == 374)
                                        {
                                            if (worldinfo.Contains("target"))
                                            {
                                                string world = worldinfo.GetString("target");
                                                frame.Foreground[tmpyy, tmpxx] = (int)bid;
                                                frame.BlockData3[tmpyy, tmpxx] = world;
                                            }
                                        }
                                        else
                                        {
                                            if (worldinfo.Contains("rotation"))
                                            {
                                                uint rotation = worldinfo.GetUInt("rotation");
                                                frame.Foreground[tmpyy, tmpxx] = (int)bid;
                                                frame.BlockData[tmpyy, tmpxx] = (int)rotation;
                                            }
                                        }
                                    }
                                    /*else if (bid == 1000)
                                    {
                                        string hexcolor = "#FFFFFF";
                                        frame.Foreground[tmpyy, tmpxx] = (int)bid;
                                        if (worldinfo.Contains("text"))
                                        {
                                            string text = worldinfo.GetString("text");
                                            frame.BlockData3[tmpyy, tmpxx] = text;
                                            if (worldinfo.Contains("text_color"))
                                            {
                                                hexcolor = worldinfo.GetString("text_color");
                                            }
                                            else
                                            {
                                                //hexcolor

                                            }
                                        }
                                    }*/
                                    else if (bdata.portals.Contains((int)bid))
                                    {
                                        uint rotation = worldinfo.GetUInt("rotation");
                                        uint sid = worldinfo.GetUInt("id");
                                        uint tid = worldinfo.GetUInt("target");
                                        frame.Foreground[tmpyy, tmpxx] = (int)bid;
                                        frame.BlockData[tmpyy, tmpxx] = (int)rotation;
                                        frame.BlockData1[tmpyy, tmpxx] = (int)sid;
                                        frame.BlockData2[tmpyy, tmpxx] = (int)tid;
                                    }
                                    else
                                    {

                                        frame.Foreground[tmpyy, tmpxx] = (int)bid;
                                        var rotation = worldinfo.Contains("rotation") ? (int)worldinfo.GetUInt("rotation") : -1;
                                        if (rotation != -1)
                                        {
                                            frame.BlockData[tmpyy, tmpxx] = (int)rotation;
                                        }
                                    }
                                }
                                else
                                {
                                    frame.Background[tmpyy, tmpxx] = (int)bid;
                                }
                            }
                        }

                        if (xx1 != null && yy1 != null)
                        {

                            for (int xxxx = 0; xxxx < xx1.Length; xxxx++)
                            {
                                int tmpxx0 = xx1[xxxx];
                                int tmpyy0 = yy1[xxxx];
                                if (layer == 0)
                                {

                                    if (bdata.goal.Contains((int)bid))
                                    {
                                        if (bid == 77 || bid == 83 || bid == 1520)
                                        {
                                            uint rotation = worldinfo.GetUInt("id");
                                            frame.Foreground[tmpyy0, tmpxx0] = (int)bid;
                                            frame.BlockData[tmpyy0, tmpxx0] = (int)rotation;
                                        }
                                        else
                                        {
                                            uint rotation = worldinfo.GetUInt("goal");
                                            frame.Foreground[tmpyy0, tmpxx0] = (int)bid;
                                            frame.BlockData[tmpyy0, tmpxx0] = (int)rotation;
                                        }
                                    }
                                    else if (bdata.rotate.Contains((int)bid) || bdata.morphable.Contains((int)bid))
                                    {
                                        if (bid == 385)
                                        {
                                            if (worldinfo.Contains("text"))
                                            {
                                                string text = worldinfo.GetString("text");
                                                if (worldinfo.Contains("signtype"))
                                                {
                                                    uint rotation = worldinfo.GetUInt("signtype");
                                                    frame.BlockData[tmpyy0, tmpxx0] = (int)rotation;
                                                }
                                                else
                                                {
                                                    frame.BlockData[tmpyy0, tmpxx0] = 0;
                                                }
                                                frame.Foreground[tmpyy0, tmpxx0] = (int)bid;
                                                frame.BlockData3[tmpyy0, tmpxx0] = text;
                                            }
                                        }
                                        else if (bid == 374)
                                        {
                                            frame.Foreground[tmpyy0, tmpxx0] = (int)bid;
                                            if (worldinfo.Contains("target"))
                                            {
                                                string target = worldinfo.GetString("target");
                                                frame.BlockData3[tmpyy0, tmpxx0] = target;
                                            }
                                        }
                                        else
                                        {
                                            frame.Foreground[tmpyy0, tmpxx0] = (int)bid;
                                            if (worldinfo.Contains("rotation"))
                                            {
                                                uint rotation = worldinfo.GetUInt("rotation");
                                                frame.BlockData[tmpyy0, tmpxx0] = (int)rotation;
                                            }
                                            else
                                            {
                                            }
                                        }
                                    }
                                    /*else if (bid == 1000)
                                    {
                                        string hexcolor = "#FFFFFF";
                                        frame.Foreground[tmpyy0, tmpxx0] = (int)bid;
                                        if (worldinfo.Contains("text"))
                                        {
                                            string text = worldinfo.GetString("text");
                                            frame.BlockData3[tmpyy0, tmpxx0] = text;
                                            if (worldinfo.Contains("text_color"))
                                            {
                                                hexcolor = worldinfo.GetString("text_color");
                                            }
                                            else
                                            {
                                                //hexcolor

                                            }
                                        }
                                    }*/
                                    else if (bdata.portals.Contains((int)bid))
                                    {
                                        uint rotation = worldinfo.GetUInt("rotation");
                                        uint sid = worldinfo.GetUInt("id");
                                        uint tid = worldinfo.GetUInt("target");
                                        frame.Foreground[tmpyy0, tmpxx0] = (int)bid;
                                        frame.BlockData[tmpyy0, tmpxx0] = (int)rotation;
                                        frame.BlockData1[tmpyy0, tmpxx0] = (int)sid;
                                        frame.BlockData2[tmpyy0, tmpxx0] = (int)tid;
                                    }
                                    else
                                    {

                                        frame.Foreground[tmpyy0, tmpxx0] = (int)bid;
                                        var rotation = worldinfo.Contains("rotation") ? (int)worldinfo.GetUInt("rotation") : -1;
                                        if (rotation != -1)
                                        {
                                            frame.BlockData[tmpyy0, tmpxx0] = (int)rotation;
                                        }
                                    }
                                }
                                else
                                {
                                    frame.Background[tmpyy0, tmpxx0] = (int)bid;
                                }
                            }
                        }
                    }
                }
            }
            //frame.Foreground[10, 10] = 9;
            //MainForm.editArea.Init(frame);
            return frame;

        }
        #endregion

        public List<string[]> Diff(Frame f)
        {
            List<string[]> res = new List<string[]>();
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {

                    if (Foreground[y, x] != f.Foreground[y, x])
                    {
                        if (bdata.morphable.Contains(Foreground[y, x]))
                        {
                            res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0", BlockData[y, x].ToString() });
                        }
                        else if (bdata.goal.Contains(Foreground[y, x]) && Foreground[y, x] != 83 && Foreground[y, x] != 77 && Foreground[y, x] != 1520)
                        {
                            res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0", BlockData[y, x].ToString() });
                        }
                        else if (bdata.rotate.Contains(Foreground[y, x]) && Foreground[y, x] != 385 && Foreground[y, x] != 374)
                        {
                            res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0", BlockData[y, x].ToString() });
                        }
                        else if (bdata.portals.Contains(Foreground[y, x]))
                        {
                            res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0", BlockData[y, x].ToString(), BlockData1[y, x].ToString(), BlockData2[y, x].ToString() });

                        }
                        else if (Foreground[y, x] == 83 || Foreground[y, x] == 77 || Foreground[y, x] == 1520)
                        {
                            res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0", BlockData[y, x].ToString() });
                        }
                        else if (Foreground[y, x] == 385)
                        {
                            res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0", BlockData[y, x].ToString(), BlockData3[y, x].ToString() });
                        }
                        else if (Foreground[y, x] == 374)
                        {
                            res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0", BlockData3[y, x].ToString() });
                        }
                        else
                        {
                            if (MainForm.userdata.level.StartsWith("OW") && !MainForm.OpenWorldCode && MainForm.OpenWorld)
                            {
                                if (y > 4)
                                {
                                    res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0" });
                                }
                            }
                            else if (MainForm.userdata.level.StartsWith("OW") && MainForm.OpenWorldCode && MainForm.OpenWorld)
                            {
                                res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0" });
                            }
                            else
                            {
                                res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0" });
                            }
                        }
                    }
                    if (Foreground[y, x] == f.Foreground[y, x])
                    {
                        if (bdata.morphable.Contains(Foreground[y, x]))
                        {
                            if (BlockData[y, x] != f.BlockData[y, x])
                            {
                                res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0", BlockData[y, x].ToString() });
                            }
                        }
                        else if (bdata.goal.Contains(Foreground[y, x]) && Foreground[y, x] != 83 && Foreground[y, x] != 77 && Foreground[y, x] != 1520)
                        {
                            if (BlockData[y, x] != f.BlockData[y, x])
                            {
                                res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0", BlockData[y, x].ToString() });
                            }
                        }
                        else if (bdata.rotate.Contains(Foreground[y, x]) && Foreground[y, x] != 385 && Foreground[y, x] != 374)
                        {
                            if (BlockData[y, x] != f.BlockData[y, x])
                            {
                                res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0", BlockData[y, x].ToString() });
                            }
                        }
                        else if (bdata.portals.Contains(Foreground[y, x]))
                        {
                            if (BlockData[y, x] != f.BlockData[y, x] || BlockData1[y, x] != f.BlockData1[y, x] || BlockData2[y, x] != f.BlockData2[y, x])
                            {
                                res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0", BlockData[y, x].ToString(), BlockData1[y, x].ToString(), BlockData2[y, x].ToString() });
                            }

                        }
                        else if (Foreground[y, x] == 83 || Foreground[y, x] == 77 || Foreground[y, x] == 1520)
                        {
                            if (BlockData[y, x] != f.BlockData[y, x])
                            {
                                res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0", BlockData[y, x].ToString() });
                            }
                        }
                        else if (Foreground[y, x] == 385)
                        {
                            if (BlockData[y, x] != f.BlockData[y, x] || BlockData3[y, x] != f.BlockData3[y, x])
                            {
                                res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0", BlockData[y, x].ToString(), BlockData3[y, x].ToString() });
                            }
                        }
                        else if (Foreground[y, x] == 374)
                        {
                            if (BlockData3[y, x] != f.BlockData3[y, x])
                            {
                                res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0", BlockData3[y, x].ToString() });
                            }
                        }
                    }
                    if (Background[y, x] != f.Background[y, x])
                    {
                        res.Add(new string[] { x.ToString(), y.ToString(), Background[y, x].ToString(), "1" });
                    }
                    /*if (Foreground[y, x] != f.Foreground[y, x])
                    {
                        Console.WriteLine(Foreground[y, x]);
                        if (bdata.goal.Contains(Foreground[y, x]) || bdata.rotate.Contains(Foreground[y, x]) || bdata.morphable.Contains(Foreground[y, x]) && Foreground[y, x] != 385 && Foreground[y, x] != 374 && BlockData[y, x] != f.BlockData[y, x])
                        {

                         res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0", BlockData[y, x].ToString() });
                            
                        }
                        if (Foreground[y, x] == 385) res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0", BlockData[y, x].ToString(), BlockData3[y, x].ToString() });
                        if (Foreground[y, x] == 374) res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0", BlockData3[y, x].ToString() });
                        
                        if (bdata.portals.Contains(Foreground[y, x]))
                        {
                            res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0", BlockData[y, x].ToString(), BlockData1[y, x].ToString(), BlockData2[y, x].ToString() });

                        }
                        else { res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0" }); }
                        
                    }
                    if (Foreground[y, x] == f.Foreground[y, x])
                    {
                        if (bdata.goal.Contains(Foreground[y, x]) || bdata.rotate.Contains(Foreground[y, x]) || bdata.morphable.Contains(Foreground[y, x]) && Foreground[y, x] != 385 && Foreground[y, x] != 374 && BlockData[y, x] != f.BlockData[y, x]) res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0", BlockData[y, x].ToString() });
                        if (Foreground[y, x] == 385 && BlockData[y, x] != f.BlockData[y, x] || BlockData3[y, x] != f.BlockData3[y, x]) res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0", BlockData[y, x].ToString(), BlockData3[y, x].ToString() });
                        if (Foreground[y, x] == 374 && BlockData3[y, x] != f.BlockData3[y, x]) res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0", BlockData3[y, x].ToString() });
                        if (bdata.portals.Contains(Foreground[y, x]) && BlockData[y, x] != f.BlockData[y, x])
                        {
                            res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0", BlockData[y, x].ToString(), BlockData1[y, x].ToString(), BlockData2[y, x].ToString() });

                        }
                    }
                    if (Background[y, x] != f.Background[y, x])
                    {
                        res.Add(new string[] { x.ToString(), y.ToString(), Background[y, x].ToString(), "1" });
                    }*/
                }
            }
            /*for (int i = 0; i < res.Count; i++)
            {
                for (int o = 0; o < res[i].Length; o++)
                {
                    Console.WriteLine("Id: " + i + "Data: " + res[i][o]);
                }
            }*/

            return res;

        }

        public void Save(System.IO.BinaryWriter writer)
        {
            writer.Write(Width);
            writer.Write(Height);
            for (int y = 0; y < Height; ++y)
                for (int x = 0; x < Width; ++x)
                {
                    int t = Foreground[y, x];
                    writer.Write((short)t);
                    writer.Write((short)Background[y, x]);
                    if (bdata.goal.Contains(t) || bdata.rotate.Contains(t) || bdata.morphable.Contains(t) && t != 385 && t != 374)
                    {
                        writer.Write((short)BlockData[y, x]);
                    }
                    if (t == 385)
                    {
                        writer.Write((short)BlockData[y, x]);
                        writer.Write(BlockData3[y, x]);
                    }
                    if (t == 374)
                    {
                        writer.Write(BlockData3[y, x]);
                    }
                    if (bdata.portals.Contains(t))
                    {
                        writer.Write(BlockData[y, x]);
                        writer.Write(BlockData1[y, x]);
                        writer.Write(BlockData2[y, x]);
                    }
                }
            writer.Close();
        }

        public static bool[] detectWorlds(string file)
        {
            bool[] corrects = new bool[10];
            using (BinaryReader reader = new BinaryReader(File.Open(file, FileMode.Open)))
            {
                
                var width = reader.ReadInt32();
                var height = reader.ReadInt32();
                if (width >= 25 && width <= 636 || height >= 25 && height <= 400) { corrects[0] = true; }
                else { corrects[0] = false; }

                if (corrects[0])
                {
                    for (int y = 0;y < height;y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            var fg = reader.ReadInt16();
                            if (fg < 500 || fg >= 1001) { corrects[1] = true; }
                            else { corrects[1] = false; }

                            if (corrects[1])
                            {
                                var bg = reader.ReadInt16();
                                if (bg >= 500 || bg <= 999) { corrects[2] = true; }
                                else { corrects[2] = false; }
                                if (bdata.goal.Contains(fg) || bdata.rotate.Contains(fg) || bdata.morphable.Contains(fg) && fg != 385 && fg != 374)
                                {
                                    var rotation = reader.ReadInt16();
                                    if (rotation >= 0 || rotation <= 999) { corrects[3] = true; }
                                    else { corrects[3] = false; }
                                }
                                if (fg == 385)
                                {
                                    var rotation = reader.ReadInt16();
                                    if (rotation >= 0 || rotation <= 5) { corrects[4] = true; }
                                    else { corrects[4] = false; }

                                    var text = reader.ReadString();
                                    if (text.Length != 0) { corrects[5] = true; }
                                    else { corrects[5] = false; }
                                }
                                if (fg == 374)
                                {
                                    var text = reader.ReadString();
                                    if (text.Length != 0) { corrects[6] = true; }
                                    else { corrects[6] = false; }
                                }
                                if (bdata.portals.Contains(fg))
                                {

                                }
                            }
                        }
                    }
                }
                reader.Close();
            }
            return corrects;
        }
        public static void LoadEEBuilder(string file, int num)
        {
            if (MainForm.editArea.Frames[0].Height >= 30 && MainForm.editArea.Frames[0].Width >= 41)
            {
                bool error = false;
                string[] lines = System.IO.File.ReadAllLines(file);
                int linee = 0;
                string[,] area = new string[29, 40];
                string[,] back = new string[29, 40];
                string[,] coins = new string[29, 40];
                string[,] id = new string[29, 40];
                string[,] target = new string[29, 40];
                string[,] text1 = new string[29, 40];
                foreach (string line in lines)
                {
                    linee += 1;
                    if (linee == 1)
                    {
                        
                        if (Regex.IsMatch(line, @"[0-9]{1,3}\ [0-9]{1,3}\ [0-9]{1,3}\ [0-9]{1,3}\ [0-9]{1,3}\ [0-9]{1,3}\ [0-9]{1,3}\ [0-9]{1,3}\ [0-9]{1,3}\ [0-9]{1,3}"))
                        {
                            split1 = line.Split(' ');
                            error = false;
                        }
                        else
                        {
                            error = true;
                        }
                    }
                    else if (linee > 1)
                    {
                        if (!error)
                        {
                            string[] split = line.Split(new char[] { ' ' });
                            for (int m = 0; m < eebuilderData.Length / 2; m++)
                            {
                                int s1 = eebuilderData[m, 0], s2 = eebuilderData[m, 1];
                                int abc = Convert.ToInt32(split[0]) - 1;

                                if (Convert.ToInt32(split1[abc]) == s1)
                                {
                                        area[Convert.ToInt32(split[2]) - 1, Convert.ToInt32(split[1]) - 1] = s2.ToString();
                                }
                            }
                        }

                    }
                }
                if (!error)
                {
                    Clipboard.SetData("EEData", new string[][,] { area, back, coins, id, target, text1 });
                    MainForm.editArea.Focus();
                    SendKeys.Send("^{v}");
                }
            }
            else
            {
                MessageBox.Show("The world is too small to handle EEBuilder files.\nWorlds should be larger or equal to width 30 and height 41", "World too small",MessageBoxButtons.OK,MessageBoxIcon.Asterisk);

            }
        }
        public static Frame Load(System.IO.BinaryReader reader, int num)
        {
            /*
             * Loading new world anti-hack (not done)
             * reader.Close();
            bool[] bol = detectWorlds(file);
            int missed = 0;
            int got = 0;
            for (int i = 0;i < bol.Length;i++)
            {
                Console.WriteLine(bol[i]);
                if (bol[i]) got += 1;
                else missed += 1;
            }
            Console.WriteLine(missed + " " + got);
            */
            if (num == 4)
            {
                int width = reader.ReadInt32();
                int height = reader.ReadInt32();
                Frame f = new Frame(width, height);
                for (int y = 0; y < height; ++y)
                {
                    for (int x = 0; x < width; ++x)
                    {
                        int t = reader.ReadInt16();
                        f.Foreground[y, x] = t;
                        f.Background[y, x] = reader.ReadInt16();
                        if (bdata.goal.Contains(t) || bdata.rotate.Contains(t) || bdata.morphable.Contains(t) && t != 385 && t != 374)
                        {
                            f.BlockData[y, x] = reader.ReadInt16();
                        }
                        if (t == 385)
                        {
                            f.BlockData[y, x] = reader.ReadInt16();
                            f.BlockData3[y, x] = reader.ReadString();
                        }
                        if (t == 374)
                        {
                            f.BlockData3[y, x] = reader.ReadString();
                        }
                        if (bdata.portals.Contains(t))
                        {
                            f.BlockData[y, x] = reader.ReadInt32();
                            f.BlockData1[y, x] = reader.ReadInt32();
                            f.BlockData2[y, x] = reader.ReadInt32();
                        }
                    }
                }
                return f;
            }
            if (num == 3)
            {
                char[] filetype = reader.ReadChars(16);
                if (new string(filetype) == "ANIMATOR SAV V05")
                {
                    reader.ReadInt16();
                    int LayerCount = Convert.ToInt16(reader.ReadInt16());
                    int width = Convert.ToInt16(reader.ReadInt16());
                    int height = Convert.ToInt16(reader.ReadInt16());
                    Frame f = new Frame(width, height);
                    for (int z = 1; z >= 0; z += -1)
                    {

                        for (int y = 0; y <= height - 1; y++)
                        {

                            for (int x = 0; x <= width - 1; x++)
                            {

                                int bid = eeanimator2blocks(Convert.ToInt16(reader.ReadInt16()));
                                if (bid >= 500 && bid <= 900)
                                {
                                    f.Background[y, x] = bid;
                                }
                                else
                                {
                                    f.Foreground[y, x] = bid;
                                }
                            }

                        }

                    }
                    return f;

                }
                else
                {
                    return null;
                }
            }
            if (num >= 0 && num <= 2)
            {
                int width = reader.ReadInt32();
                int height = reader.ReadInt32();
                Frame f = new Frame(width, height);
                for (int y = 0; y < height; ++y)
                    for (int x = 0; x < width; ++x)
                    {
                        if (num == 0)
                        {
                            int t = reader.ReadByte();
                            f.Foreground[y, x] = t;
                            f.Background[y, x] = 0;
                            if (bdata.goal.Contains(t) || bdata.rotate.Contains(t) || bdata.morphable.Contains(t))
                            {
                                f.BlockData[y, x] = reader.ReadInt16();
                            }
                            else if (bdata.portals.Contains(t))
                            {
                                var r = reader.ReadInt32();
                                var a = r >> 16;
                                var b = ((r >> 8) & 0xFF);
                                var c = (r & 0xFF);
                                f.BlockData[y, x] = a;
                                f.BlockData1[y, x] = b;
                                f.BlockData2[y, x] = c;
                            }
                        }
                        else if (num == 1)
                        {
                            int t = reader.ReadInt16();
                            f.Foreground[y, x] = t;
                            f.Background[y, x] = reader.ReadInt16();
                            if (bdata.goal.Contains(t) || bdata.rotate.Contains(t) || bdata.morphable.Contains(t))
                            {
                                f.BlockData[y, x] = reader.ReadInt16();
                            }
                            else if (bdata.portals.Contains(t))
                            {
                                var r = reader.ReadInt32();
                                var a = r >> 16;
                                var b = ((r >> 8) & 0xFF);
                                var c = (r & 0xFF);
                                f.BlockData[y, x] = a;
                                f.BlockData1[y, x] = b;
                                f.BlockData2[y, x] = c;
                            }
                        }
                        else if (num == 2)
                        {
                            int t = reader.ReadInt16();
                            f.Foreground[y, x] = t;
                            f.Background[y, x] = reader.ReadInt16();
                            if (bdata.goal.Contains(t) || bdata.rotate.Contains(t) || bdata.morphable.Contains(t))
                            {
                                f.BlockData[y, x] = reader.ReadInt16();
                            }
                            else if (t == 374)
                            {
                                f.BlockData[y, x] = 0;
                                f.BlockData3[y, x] = reader.ReadString();
                            }
                            else if (t == 385)
                            {
                                f.BlockData3[y, x] = reader.ReadString();
                            }
                            else if (bdata.portals.Contains(t))
                            {
                                f.BlockData[y, x] = reader.ReadInt32();
                                f.BlockData1[y, x] = reader.ReadInt32();
                                f.BlockData2[y, x] = reader.ReadInt32();
                            }
                        }
                    }
                return f;
            }
            else
            {
                return null;
            }
        }
        static int eeanimator2blocks(int id)
        {
            if (id == 127)
            {
                return 0;
            }
            else if (id - 128 >= 0 && id - 128 <= 63)
            {
                return id - 128;
            }
            else if (id + 256 >= 500 && id + 256 <= 600)
            {
                return id + 256;
            }
            else
            {
                return id - 1024;
            }
        }
        static int[,] eebuilderData = new int[,]
    {
                { 1, 9 }, { 2, 10 }, { 3, 11 }, { 4, 12 }, { 5, 13 }, { 6, 14 }, { 7, 15 },
                { 8, 37 }, { 9, 38 }, { 10, 39 }, { 11, 40 }, { 12, 41 }, { 13, 42 },
                { 14, 16 }, { 15, 17 }, { 16, 18 }, { 17, 19 }, { 18, 20 }, { 19, 21 },
                { 20, 29 }, { 21, 30 }, { 22, 31 }, { 23, 34 }, { 24, 35 }, { 25, 36 },
                { 26, 22 }, { 27, 32 }, { 28, 33 }, { 29, 44 },
                { 30, 6 }, { 31, 7 }, { 32, 8 }, { 33, 23 }, { 34, 24 }, { 35, 25 },
                { 36, 0 }, { 37, 26 }, { 38, 27 }, { 39, 28 },
                { 40, 0 }, { 41, 1 }, { 42, 2 }, { 43, 3 }, { 44, 4 }, { 45, 100 }, { 46, 101 },
                { 47, 5 }, { 48, 255 },
                { 49, 0 }, { 50, 0 }, { 51, 0 }, { 52, 0 }, { 53, 0 }, { 54, 0 },
                { 55, 0 }, { 56, 0 }, { 57, 0 }, { 58, 0 }, { 59, 0 },
                { 60, 45 }, { 61, 46 }, { 62, 47 }, { 63, 48 }, { 64, 49 },
                { 65, 50 }, { 66, 243 },
                { 67, 51 }, { 68, 52 }, { 69, 53 }, { 70, 54 }, { 71, 55 }, { 72, 56 }, { 73, 57 }, { 74, 58 },
                { 75, 233 }, { 76, 234 }, { 77, 235 }, { 78, 236 }, { 79, 237 }, { 80, 238 }, { 81, 239 }, { 82, 240 },
    };
    }
}
