using Gdk;
using GLib;
using Gtk;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public partial class MainWindow : Gtk.Window
{
    bool IsSelecting;
    bool IsDragging;
    int X0, Y0, X1, Y1;
    int prevX, prevY;
    Mutex Rendering = new Mutex();
    FileChooserDialog ImageChooser;
    Stopwatch timer = new Stopwatch();
    List<Colony> Colonies = new List<Colony>();
    List<ColonyTypes.Type> ColoniesType = new List<ColonyTypes.Type>();
    List<Parameter> ColonyParameters = new List<Parameter>();
    const int PAGE_WORLD = 0;
    const int PAGE_WORLD_PARAMS = 1;
    Pixbuf worldPixbuf;
    bool Paused;
    bool Disabled;

    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        Build();

        InitControls();

        Tic();

        GtkSelection.Selection.EllipseMode = false;

        Idle.Add(new IdleHandler(OnIdle));
    }

    protected void InitControls()
    {
        Title = "Artificial Life Soup";

        ImageChooser = new FileChooserDialog(
            "Save world snapshot",
            this,
            FileChooserAction.Save,
            "Cancel", ResponseType.Cancel,
            "Save", ResponseType.Accept
        );

        worldNotebook.CurrentPage = PAGE_WORLD;

        Paused = true;

        StopButton.Sensitive = false;
        RunButton.Sensitive = true;
        SaveButton.Sensitive = true;

        worldPixbuf = InitWorld(worldImage.WidthRequest, worldImage.HeightRequest);
        worldImage.Pixbuf = InitWorld(worldImage.WidthRequest, worldImage.HeightRequest);

        foreach (ColonyTypes.Type type in Enum.GetValues(typeof(ColonyTypes.Type)))
        {
            ColoniesType.Add(type);
            ColonyTypeList.AppendText(type.ToString());
        }

        ColonyColor.Color = new Color(255, 0, 255);
    }

    protected Pixbuf InitWorld(int width, int height)
    {
        var pixbuf = new Pixbuf(Colorspace.Rgb, false, 8, width, height);

        pixbuf.Fill(0);

        return pixbuf;
    }

    protected void Disable()
    {
        Disabled = true;
    }

    protected void Enable()
    {
        Disabled = false;
    }

    protected void RenderWorld(Pixbuf pixbuf)
    {
        if (pixbuf != null)
        {
            var dest = worldImage.GdkWindow;
            var gc = new Gdk.GC(dest);

            dest.DrawPixbuf(gc, pixbuf, 0, 0, 0, 0, pixbuf.Width, pixbuf.Height, RgbDither.None, 0, 0);
        }
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        if (worldPixbuf != null)
        {
            worldPixbuf.Dispose();
        }

        if (worldImage.Pixbuf != null)
        {
            worldImage.Pixbuf.Dispose();
        }

        if (worldImage != null)
        {
            worldImage.Dispose();
        }

        Colonies.Clear();

        Application.Quit();

        a.RetVal = true;
    }

    protected void Tic()
    {
        timer.Restart();
    }

    protected long Ticks()
    {
        return timer.ElapsedMilliseconds;
    }

    protected long Toc()
    {
        var elapsed = Ticks();

        timer.Restart();

        return elapsed;
    }

    protected void RenderColonies(Pixbuf pixbuf)
    {
        pixbuf.Fill(0);

        Parallel.ForEach(Colonies, (colony) =>
        {
            RenderArtificialLife(pixbuf, colony.ArtificialLife, colony.X, colony.Y);
        });

        System.GC.Collect();
        System.GC.WaitForPendingFinalizers();
    }

    protected void RenderArtificialLife(Pixbuf pixbuf, ArtificialLife artificialLife, int x, int y)
    {
        if (pixbuf != null && artificialLife != null)
        {
            var writeBuffer = artificialLife.GetPixelWriteBuffer();

            if (writeBuffer.Count > 0)
            {
                Parallel.ForEach(writeBuffer, (pixel) =>
                {
                    pixel.Write(pixbuf, x, y);
                });

                artificialLife.ClearPixelWriteBuffer();
            }
        }
    }

    protected void DrawBoxes()
    {
        if (!Paused)
        {
            return;
        }

        if (GtkSelection.Selection.Count() > 0)
        {
            var dest = worldImage.GdkWindow;
            var gc = new Gdk.GC(dest);
            var boxes = GtkSelection.Selection.BoundingBoxes();

            for (int i = 0; i < boxes.Count; i++)
            {
                if (i == GtkSelection.Selected - 1)
                {
                    var box = boxes[i];

                    var w = Math.Abs(box.X1 - box.X0);
                    var h = Math.Abs(box.Y1 - box.Y0);

                    gc.RgbFgColor = i != GtkSelection.Selected - 1 ? GtkSelection.SelectedColor : GtkSelection.MarkerColor;
                    gc.SetLineAttributes(GtkSelection.MarkerSize, LineStyle.Solid, CapStyle.Round, JoinStyle.Round);

                    GtkSelection.DrawBox(gc, dest, box.X0, box.Y0, box.X1, box.Y1, w, h, false);
                }
            }
        }
    }

    void DrawSelection()
    {
        if (Paused)
        {
            if (worldPixbuf == null)
            {
                return;
            }

            var dest = worldImage.GdkWindow;
            var gc = new Gdk.GC(dest);

            dest.DrawPixbuf(gc, worldPixbuf, 0, 0, 0, 0, worldPixbuf.Width, worldPixbuf.Height, RgbDither.None, 0, 0);

            if (IsSelecting)
            {
                GtkSelection.Draw(gc, dest, X0, Y0, X1, Y1);
            }
        }
    }

    void RefreshColonies()
    {
        for (int i = 0; i < Colonies.Count; i++)
        {
            if (Colonies[i].ArtificialLife is Life)
            {
                (Colonies[i].ArtificialLife as Life).Refresh();
            }

            if (Colonies[i].ArtificialLife is LangtonAnt)
            {
                (Colonies[i].ArtificialLife as LangtonAnt).Refresh();
            }

            if (Colonies[i].ArtificialLife is Zhabotinsky)
            {
                (Colonies[i].ArtificialLife as Zhabotinsky).Refresh();
            }

            if (Colonies[i].ArtificialLife is YinYangFire)
            {
                (Colonies[i].ArtificialLife as YinYangFire).Refresh();
            }

            if (Colonies[i].ArtificialLife is ForestFire)
            {
                (Colonies[i].ArtificialLife as ForestFire).Refresh();
            }
        }
    }

    void InitializeSelected()
    {
        if (GtkSelection.Selected > 0)
        {
            var colony = GtkSelection.Selected - 1;

            if (colony < Colonies.Count)
            {
                Disable();

                ColonyParameters.Clear();

                for (int i = 0; i < ColoniesType.Count; i++)
                {
                    if (Colonies[colony].ArtificialLife is Life && ColoniesType[i] == ColonyTypes.Type.Life)
                    {
                        ColonyTypeList.Active = i;
                        ColonyParameters.AddRange((Colonies[colony].ArtificialLife as Life).Parameters());
                        var color = (Colonies[colony].ArtificialLife as Life).Color();
                        ColonyColor.Color = new Color((byte)color.Red, (byte)color.Green, (byte)color.Blue);
                    }

                    if (Colonies[colony].ArtificialLife is Zhabotinsky && ColoniesType[i] == ColonyTypes.Type.Zhabotinsky)
                    {
                        ColonyTypeList.Active = i;
                        ColonyParameters.AddRange((Colonies[colony].ArtificialLife as Zhabotinsky).Parameters());
                        var color = (Colonies[colony].ArtificialLife as Zhabotinsky).Color();
                        ColonyColor.Color = new Color((byte)color.Red, (byte)color.Green, (byte)color.Blue);
                    }

                    if (Colonies[colony].ArtificialLife is LangtonAnt && ColoniesType[i] == ColonyTypes.Type.LangtonAnt)
                    {
                        ColonyTypeList.Active = i;
                        ColonyParameters.AddRange((Colonies[colony].ArtificialLife as LangtonAnt).Parameters());
                        var color = (Colonies[colony].ArtificialLife as LangtonAnt).Color();
                        ColonyColor.Color = new Color((byte)color.Red, (byte)color.Green, (byte)color.Blue);
                    }

                    if (Colonies[colony].ArtificialLife is YinYangFire && ColoniesType[i] == ColonyTypes.Type.YinYangFire)
                    {
                        ColonyTypeList.Active = i;
                        ColonyParameters.AddRange((Colonies[colony].ArtificialLife as YinYangFire).Parameters());
                        var color = (Colonies[colony].ArtificialLife as YinYangFire).Color();
                        ColonyColor.Color = new Color((byte)color.Red, (byte)color.Green, (byte)color.Blue);
                    }

                    if (Colonies[colony].ArtificialLife is ForestFire && ColoniesType[i] == ColonyTypes.Type.ForestFire)
                    {
                        ColonyTypeList.Active = i;
                        ColonyParameters.AddRange((Colonies[colony].ArtificialLife as ForestFire).Parameters());
                        var color = (Colonies[colony].ArtificialLife as ForestFire).Color();
                        ColonyColor.Color = new Color((byte)color.Red, (byte)color.Green, (byte)color.Blue);
                    }
                }

                ColonyColor.ModifyCursor(ColonyColor.Color, ColonyColor.Color);
                CopyParameterValues(ParameterList, ColonyParameters);

                Enable();
            }
        }
    }

    protected double GetNumeric(List<Parameter> parameters, String name)
    {
        var item = parameters.Find(parameter => parameter.Name == name);

        return item.NumericValue;
    }

    protected string GetString(List<Parameter> parameters, String name)
    {
        var item = parameters.Find(parameter => parameter.Name == name);

        return item.Value;
    }

    protected void AddColony()
    {
        var type = ColonyTypeList.Active;

        if (type >= 0)
        {
            var count = GtkSelection.Selection.Count();

            GtkSelection.Selection.Add(X0, Y0, X1, Y1);

            var added = GtkSelection.Selection.Count();

            if (added > count)
            {
                RefreshColonies();

                var box = GtkSelection.Selection.BoundingBoxes()[added - 1];

                var w = Math.Abs(box.X1 - box.X0);
                var h = Math.Abs(box.Y1 - box.Y0);
                var x = Math.Min(box.X0, box.X1);
                var y = Math.Min(box.Y0, box.Y1);

                if (ColoniesType[type] == ColonyTypes.Type.Life)
                {
                    var density = GetNumeric(ColonyParameters, "Density");

                    World.AddLifeColony(Colonies, w, h, x, y, density, ColonyColor.Color);
                }

                if (ColoniesType[type] == ColonyTypes.Type.LangtonAnt)
                {
                    var ants = (int)GetNumeric(ColonyParameters, "Ants");
                    var rule = GetString(ColonyParameters, "Rule");

                    World.AddLangtonAntColony(Colonies, w, h, x, y, ants, rule, ColonyColor.Color);
                }

                if (ColoniesType[type] == ColonyTypes.Type.Zhabotinsky)
                {
                    var density = GetNumeric(ColonyParameters, "Density");
                    var g = GetNumeric(ColonyParameters, "g");
                    var k1 = GetNumeric(ColonyParameters, "k1");
                    var k2 = GetNumeric(ColonyParameters, "k2");

                    World.AddZhabotinskyColony(Colonies, w, h, x, y, density, k1, k2, g, ColonyColor.Color);
                }

                if (ColoniesType[type] == ColonyTypes.Type.YinYangFire)
                {
                    var density = GetNumeric(ColonyParameters, "Density");
                    var maxstates = (int)GetNumeric(ColonyParameters, "MaxStates");

                    World.AddYinYangFireColony(Colonies, w, h, x, y, density, maxstates, ColonyColor.Color);
                }

                if (ColoniesType[type] == ColonyTypes.Type.ForestFire)
                {
                    var density = GetNumeric(ColonyParameters, "Density");
                    var F = GetNumeric(ColonyParameters, "F");
                    var P = GetNumeric(ColonyParameters, "P");

                    World.AddForestFireColony(Colonies, w, h, x, y, density, F, P, ColonyColor.Color);
                }

                RenderColonies(worldPixbuf);
                RenderWorld(worldPixbuf);
            }
        }
    }

    protected void MoveColony()
    {
        var dx = X1 - prevX;
        var dy = Y1 - prevY;

        prevX = X1;
        prevY = Y1;

        GtkSelection.Selection.Move(dx, dy, GtkSelection.Selected);

        // move colony
        if (GtkSelection.Selected > 0)
        {
            Colonies[GtkSelection.Selected - 1].X += dx;
            Colonies[GtkSelection.Selected - 1].Y += dy;
        }
    }

    protected void SaveImageFile()
    {
        // Add most recent directory
        if (!string.IsNullOrEmpty(ImageChooser.Filename))
        {
            var directory = System.IO.Path.GetDirectoryName(ImageChooser.Filename);

            if (Directory.Exists(directory))
            {
                ImageChooser.SetCurrentFolder(directory);
            }
        }

        if (ImageChooser.Run() == (int)ResponseType.Accept)
        {
            if (!string.IsNullOrEmpty(ImageChooser.Filename))
            {
                var FileName = ImageChooser.Filename;

                if (!FileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                {
                    FileName += ".png";
                }

                worldPixbuf.Save(FileName, "png");
            }
        }

        ImageChooser.Hide();
    }

    protected void CopyParameterValues(ComboBox combo, List<Parameter> parameters)
    {
        combo.Clear();

        var cell = new CellRendererText();
        combo.PackStart(cell, false);
        combo.AddAttribute(cell, "text", 0);
        var store = new ListStore(typeof(string));
        combo.Model = store;

        foreach (var parameter in parameters)
        {
            store.AppendValues(parameter.Name);
        }

        ParameterList.Active = -1;
        NumericValue.Sensitive = false;
        StringValue.Sensitive = false;
    }

    bool OnIdle()
    {
        Rendering.WaitOne();

        if (!Paused)
        {
            if (Ticks() > 30)
            {
                Toc();

                var start = Ticks();

                Parallel.ForEach(Colonies, (colony) =>
                {
                    if (colony.ArtificialLife is Life)
                    {
                        (colony.ArtificialLife as Life).Update();
                    }

                    if (colony.ArtificialLife is Zhabotinsky)
                    {
                        (colony.ArtificialLife as Zhabotinsky).Update();
                    }

                    if (colony.ArtificialLife is LangtonAnt)
                    {
                        (colony.ArtificialLife as LangtonAnt).Update();
                    }

                    if (colony.ArtificialLife is YinYangFire)
                    {
                        (colony.ArtificialLife as YinYangFire).Update();
                    }

                    if (colony.ArtificialLife is ForestFire)
                    {
                        (colony.ArtificialLife as ForestFire).Update();
                    }
                });

                RenderColonies(worldPixbuf);
                RenderWorld(worldPixbuf);

                Console.WriteLine("Colonies Rendered in {0} ms", Ticks() - start);
            }
        }

        Rendering.ReleaseMutex();

        DrawSelection();
        DrawBoxes();

        return true;
    }

    protected void OnRunButtonClicked(object sender, EventArgs e)
    {
        Paused = false;

        RunButton.Sensitive = false;
        StopButton.Sensitive = true;
        SaveButton.Sensitive = false;
    }

    protected void OnStopButtonClicked(object sender, EventArgs e)
    {
        Paused = true;

        RunButton.Sensitive = true;
        StopButton.Sensitive = false;
        SaveButton.Sensitive = true;
    }

    protected void OnSaveButtonClicked(object sender, EventArgs e)
    {
        if (Paused)
        {
            SaveImageFile();
        }
    }

    protected void OnWorldImageEventBoxButtonPressEvent(object o, ButtonPressEventArgs args)
    {
        X0 = Convert.ToInt32(args.Event.X);
        Y0 = Convert.ToInt32(args.Event.Y);

        X1 = X0;
        Y1 = Y0;

        if (args.Event.Button == 3)
        {
            IsSelecting = false;
            IsDragging = false;

            var box = GtkSelection.Selection.BoundingBoxes();

            for (int i = 0; i < GtkSelection.Selection.Count(); i++)
            {
                if (GtkSelection.Selection.InBox(X1, Y1, box[i]))
                {
                    Colonies.RemoveAt(i);
                    GtkSelection.Selection.Boxes.RemoveAt(i);

                    RefreshColonies();
                    RenderColonies(worldPixbuf);
                    RenderWorld(worldPixbuf);

                    System.GC.Collect();
                    System.GC.WaitForPendingFinalizers();

                    break;
                }
            }
        }
        else
        {
            if (args.Event.Button == 1)
            {
                GtkSelection.Selected = GtkSelection.Selection.Find(X0, Y0);

                if (GtkSelection.Selected > 0)
                {
                    IsDragging = true;

                    InitializeSelected();

                    prevX = X0;
                    prevY = Y0;
                }
                else
                {
                    IsSelecting = true;
                }
            }
        }
    }

    protected void OnWorldImageEventBoxButtonReleaseEvent(object o, ButtonReleaseEventArgs args)
    {
        if (IsSelecting)
        {
            IsSelecting = false;

            AddColony();
        }

        if (IsDragging)
        {
            IsDragging = false;

            if (GtkSelection.Selected > 0)
            {
                RefreshColonies();
                RenderColonies(worldPixbuf);
                RenderWorld(worldPixbuf);
            }
        }
    }

    protected void OnWorldImageEventBoxMotionNotifyEvent(object o, MotionNotifyEventArgs args)
    {
        if (!IsSelecting && !IsDragging)
        {
            return;
        }

        X1 = Convert.ToInt32(args.Event.X);
        Y1 = Convert.ToInt32(args.Event.Y);

        if (IsDragging)
        {
            MoveColony();
        }
    }

    protected void OnColonyTypeListChanged(object sender, EventArgs e)
    {
        if (!Disabled)
        {
            var type = ColonyTypeList.Active;

            if (type >= 0)
            {
                ColonyParameters.Clear();
                switch (ColoniesType[type])
                {
                    case ColonyTypes.Type.Life:
                        ColonyParameters.AddRange(ParameterSets.Life());
                        break;
                    case ColonyTypes.Type.LangtonAnt:
                        ColonyParameters.AddRange(ParameterSets.LangtonAnt());
                        break;
                    case ColonyTypes.Type.Zhabotinsky:
                        ColonyParameters.AddRange(ParameterSets.Zhabotinsky());
                        break;
                    case ColonyTypes.Type.YinYangFire:
                        ColonyParameters.AddRange(ParameterSets.YinYangFire());
                        break;
                    case ColonyTypes.Type.ForestFire:
                        ColonyParameters.AddRange(ParameterSets.ForestFire());
                        break;
                }

                Disable();

                CopyParameterValues(ParameterList, ColonyParameters);

                Enable();

                GtkSelection.Selected = 0;
            }
        }
    }

    protected void OnParameterListChanged(object sender, EventArgs e)
    {
        if (!Disabled)
        {
            var param = ParameterList.Active;

            if (param >= 0 && param < ColonyParameters.Count)
            {
                if (!ColonyParameters[param].IsNumeric)
                {
                    StringValue.Text = ColonyParameters[param].Value;
                    StringValue.Sensitive = true;
                    NumericValue.Sensitive = false;
                    NumericValue.Value = 0;
                }
                else
                {
                    StringValue.Text = "";
                    StringValue.Sensitive = false;
                    NumericValue.Sensitive = true;

                    var page = Math.Abs(ColonyParameters[param].Max - ColonyParameters[param].Min) / 1000;

                    NumericValue.Adjustment.SetBounds(ColonyParameters[param].Min, ColonyParameters[param].Max, page, page, page);
                    NumericValue.Value = ColonyParameters[param].NumericValue;
                }
            }
        }
    }

    protected void OnNumericValueValueChanged(object sender, EventArgs e)
    {
        if (!Disabled)
        {
            var param = ParameterList.Active;

            if (param >= 0 && param < ColonyParameters.Count && ColonyParameters[param].IsNumeric)
            {
                ColonyParameters[param].NumericValue = NumericValue.Value;
            }
        }
    }

    protected void OnStringValueChanged(object sender, EventArgs e)
    {
        if (!Disabled)
        {
            var param = ParameterList.Active;

            if (param >= 0 && param < ColonyParameters.Count)
            {
                if (!ColonyParameters[param].IsNumeric)
                {
                    ColonyParameters[param].Value = StringValue.Text;
                }
            }
        }
    }
}
