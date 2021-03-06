﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace EEditor
{
    partial class EditArea
    {
        public void Rotate90()
        {
            try
            {
                ToolMark tool = Tool as ToolMark;
                tool.RemoveBorder();
                tool.ClearR();
                int height = tool.Front.GetLength(0);
                int width = tool.Front.GetLength(1);
                string[,] Area = new string[width, height];
                string[,] Back = new string[width, height];
                string[,] Coins = new string[width, height];
                string[,] Id = new string[width, height];
                string[,] Target = new string[width, height];
                string[,] Text2 = new string[width, height];
                for (int x = 0; x < width; ++x)
                    for (int y = 0; y < height; ++y)
                    {
                        int nx = height - y - 1;
                        int ny = x;
                        int type1 = Convert.ToInt32(tool.Front[y, x]);
                        Area[ny, nx] = type1.ToString();
                        Back[ny, nx] = tool.Back[y, x];
                        Coins[ny, nx] = tool.Coins[y, x];
                        Id[ny, nx] = tool.Id1[y, x];
                        Target[ny, nx] = tool.Target1[y, x];
                        Text2[ny, nx] = tool.Text1[y, x];

                    }
                SetMarkBlock(Area, Back, Coins, Id, Target, Text2, tool.Rect.X, tool.Rect.Y);
            }
            catch
            {
            }
        }

        public void Flip()
        {
            try
            {
                ToolMark tool = Tool as ToolMark;
                tool.RemoveBorder();
                tool.ClearR();
                int height = tool.Front.GetLength(0);
                int width = tool.Front.GetLength(1);
                string[,] Area = new string[height, width];
                string[,] Back = new string[height, width];
                string[,] Coins = new string[height, width];
                string[,] Id = new string[height, width];
                string[,] Target = new string[height, width];
                string[,] Text2 = new string[height, width];
                for (int x = 0; x < width; ++x)
                    for (int y = 0; y < height; ++y)
                    {
                        int ny = height - y - 1;
                        string type = tool.Front[y, x];
                        int type1 = Convert.ToInt32(tool.Front[y, x]);
                        switch (type1)
                        {
                            case 116:
                                type1 = 117;
                                break;
                            case 117:
                                type1 = 116;
                                break;
                            
                        }
                        Area[ny, x] = Convert.ToString(type1);
                        Back[ny, x] = tool.Back[y, x];
                        Coins[ny, x] = tool.Coins[y, x];
                        Id[ny, x] = tool.Id1[y, x];
                        Target[ny, x] = tool.Target1[y, x];
                        Text2[ny, x] = tool.Text1[y, x];
                    }
                SetMarkBlock(Area, Back, Coins, Id, Target, Text2, tool.Rect.X, tool.Rect.Y);
            }
            catch
            {
            }
        }

        public void Mirror()
        {
            try {
            ToolMark tool = Tool as ToolMark;
            tool.RemoveBorder();
            tool.ClearR();
                int width = tool.Front.GetLength(1);
                int height = tool.Front.GetLength(0);
                string[,] Area = new string[height, width];
                string[,] Back = new string[height, width];
                string[,] Coins = new string[height, width];
                string[,] Id = new string[height, width];
                string[,] Target = new string[height, width];
                string[,] Text2 = new string[height, width];
                for (int x = 0; x < width; ++x)
                    for (int y = 0; y < height; ++y)
                    {
                        int nx = width - x - 1;
                        string type = tool.Front[y, x];
                        int type1 = Convert.ToInt32(tool.Front[y, x]);
                        switch (type1)
                        {
                            case 1:
                                type1 = 3;
                                break;
                            case 3:
                                type1 = 1;
                                break;
                            case 411:
                                type1 = 413;
                                break;
                            case 413:
                                type1 = 411;
                                break;
                            case 114:
                                type1 = 115;
                                break;
                            case 115:
                                type1 = 114;
                                break;
                        }
                        Area[y, nx] = Convert.ToString(type1);
                        Back[y, nx] = tool.Back[y, x];
                        Coins[y, nx] = tool.Coins[y, x];
                        Id[y, nx] = tool.Id1[y, x];
                        Target[y, nx] = tool.Target1[y, x];
                        Text2[y, nx] = tool.Text1[y, x];
                    }
                SetMarkBlock(Area, Back, Coins, Id, Target, Text2, tool.Rect.X, tool.Rect.Y);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }



    }
}
