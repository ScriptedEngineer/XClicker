﻿using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using System.Drawing;
using System.Globalization;

namespace UltimateUserInput
{
    class ScriptLanguage
    {
        static Random rnd = new Random();
        static Dictionary<string, int> IntVars = new Dictionary<string, int>();
        static public void RunScript(string script)
        {
            IntVars.Clear();
            IntVars.Add("N", 0);
            string[] functions = script.Split('\n');
            int len = functions.Length;
            for (int index = 0; index < len; index++)
            {
                index = RunCommand(functions[index].Trim(), index, len);
            }
        }

        static public int RunCommand(string command, int index, int max)
        {
            try
            {
                string[] param = command.Split(' ');
                if (param.Count() < 2) return index;
                if (int.TryParse(param[0], out int E))
                {
                    Thread.Sleep(E);
                }
                else if (param[0] == "N")
                {
                    Thread.Sleep(IntVars["N"]*2);
                }
                IntVars["N"] = rnd.Next(50, 150);
                switch (param[1])
                {
                    case "Mouse":
                        switch (param[2])
                        {
                            case "Down":
                                UserInput.MouseButton ClickR;
                                Enum.TryParse(param[3], out ClickR);
                                UserInput.MouseButtonEvent(ClickR, UserInput.ButtonEvents.Down);
                                break;
                            case "Click":
                                UserInput.MouseButton Click;
                                Enum.TryParse(param[3], out Click);
                                UserInput.MouseClick(Click);
                                break;
                            case "Up":
                                UserInput.MouseButton ClickN;
                                Enum.TryParse(param[3], out ClickN);
                                UserInput.MouseButtonEvent(ClickN, UserInput.ButtonEvents.Up);
                                break;
                            case "Scroll":
                                if (!int.TryParse(param[3], out int Yg)) Yg = IntVars[param[3]];
                                WinApi.MouseEvent(WinApi.MouseEventFlags.Wheel, Yg);
                                break;
                            case "Move":
                                if (!int.TryParse(param[3], out int X)) X = IntVars[param[3]];
                                if (!int.TryParse(param[4], out int Y)) Y = IntVars[param[4]];
                                UserInput.MouseMove(X, Y);
                                break;
                            case "Set":
                                if(!int.TryParse(param[3], out int Xa)) Xa = IntVars[param[3]];
                                if(!int.TryParse(param[4], out int Ya)) Ya = IntVars[param[4]];
                                UserInput.SetMouse(Xa, Ya);
                                break;
                            case "Get":
                                var Mouse = WinApi.GetCursorPosition();
                                if (IntVars.ContainsKey(param[3]) && IntVars.ContainsKey(param[4]))
                                {
                                    IntVars[param[3]] = Mouse.X;
                                    IntVars[param[4]] = Mouse.Y;
                                }
                                break;
                        }
                        break;
                    case "Button":
                        switch (param[2])
                        {
                            case "Down":
                                Enum.TryParse("VK_" + param[3], out WinApi.Vk X);
                                UserInput.ButtonEvent(X, UserInput.ButtonEvents.Down);
                                break;
                            case "Click":
                                Enum.TryParse("VK_" + param[3], out WinApi.Vk C);
                                UserInput.ButtonEvent(C, UserInput.ButtonEvents.Down);
                                UserInput.ButtonEvent(C, UserInput.ButtonEvents.Up);
                                break;
                            case "Up":
                                Enum.TryParse("VK_" + param[3], out WinApi.Vk Y);
                                UserInput.ButtonEvent(Y, UserInput.ButtonEvents.Up);
                                break;
                        }
                        break;
                    case "Screen":
                        switch (param[2])
                        {
                            case "On":
                                WinApi.SetEnableScreen(true);
                                break;
                            case "Off":
                                WinApi.SetEnableScreen(false);
                                break;
                        }
                        break;
                    case "If":
                        bool YesOrNot = false;
                        switch (param[2])
                        {
                            case "ScreenPixel":
                                if (!int.TryParse(param[3], out int ScX)) ScX = IntVars[param[3]];
                                if (!int.TryParse(param[4], out int ScY)) ScY = IntVars[param[4]];
                                Color Pixel = UserInput.GetScreenColorAt(ScX, ScY);
                                int.TryParse(param[5], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out int color);
                                YesOrNot = Pixel == Color.FromArgb(color);
                                break;
                            case "MousePos":
                                var Posit = WinApi.GetCursorPosition();
                                if (!int.TryParse(param[4], out int EqvaterX)) EqvaterX = IntVars[param[4]];
                                if (!int.TryParse(param[6], out int EqvaterY)) EqvaterY = IntVars[param[6]];
                                switch (param[3])
                                {
                                    case "==":
                                        YesOrNot = Posit.X == EqvaterX;
                                        break;
                                    case ">":
                                        YesOrNot = Posit.X > EqvaterX;
                                        break;
                                    case "<":
                                        YesOrNot = Posit.X < EqvaterX;
                                        break;
                                }
                                switch (param[5])
                                {
                                    case "==":
                                        YesOrNot &= Posit.Y == EqvaterY;
                                        break;
                                    case ">":
                                        YesOrNot &= Posit.Y > EqvaterY;
                                        break;
                                    case "<":
                                        YesOrNot &= Posit.Y < EqvaterY;
                                        break;
                                }
                                break;
                            case "IntVar":
                                if (!int.TryParse(param[5], out int Eqvater)) Eqvater = IntVars[param[5]];
                                switch (param[4])
                                {
                                    case "==":
                                        YesOrNot = IntVars[param[3]] == Eqvater;
                                        break;
                                    case ">":
                                        YesOrNot = IntVars[param[3]] > Eqvater;
                                        break;
                                    case "<":
                                        YesOrNot = IntVars[param[3]] < Eqvater;
                                        break;
                                    case ">=":
                                        YesOrNot = IntVars[param[3]] >= Eqvater;
                                        break;
                                    case "<=":
                                        YesOrNot = IntVars[param[3]] <= Eqvater;
                                        break;
                                }
                                break;
                        }
                        if(!YesOrNot)
                            return index + 1;
                        break;
                    case "IntVar":
                        switch (param[2])
                        {
                            case "Add":
                                if (!int.TryParse(param[4], out int IntVarAdd)) IntVarAdd = IntVars[param[4]];
                                IntVars[param[3]] += IntVarAdd;
                                break;
                            case "Sub":
                                if (!int.TryParse(param[4], out int IntVarSub)) IntVarSub = IntVars[param[4]];
                                IntVars[param[3]] -= IntVarSub;
                                break;
                            case "Init":
                                int.TryParse(param[4], out int IntVar);
                                if (IntVars.ContainsKey(param[3]))
                                    IntVars[param[3]] = IntVar;
                                else
                                    IntVars.Add(param[3], IntVar);
                                break;
                        }
                        break;
                    case "GoTo":
                        if (!int.TryParse(param[2], out int Pos)) Pos = IntVars[param[2]];
                        if (Pos-1 < max && Pos-1 >= 0)
                            return Pos-2;
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return index;
        }
    }
}
