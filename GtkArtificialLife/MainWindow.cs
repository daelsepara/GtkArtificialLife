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
    FileChooserDialog ImageLoader;
    Stopwatch timer = new Stopwatch();
    List<Colony> Colonies = new List<Colony>();
    List<ColonyTypes.Type> ColoniesType = new List<ColonyTypes.Type>();
    List<Parameter> ColonyParameters = new List<Parameter>();
    const int PAGE_WORLD = 0;
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

        ImageLoader = new FileChooserDialog(
            "Load Image",
            this,
            FileChooserAction.Save,
            "Cancel", ResponseType.Cancel,
            "Load", ResponseType.Accept
        );

        worldNotebook.CurrentPage = PAGE_WORLD;

        Paused = true;

        StopButton.Sensitive = false;
        RunButton.Sensitive = true;
        SaveButton.Sensitive = true;
        ShowColonies.Sensitive = true;
        ClearButton.Sensitive = true;

        worldPixbuf = InitWorld(worldImage.WidthRequest, worldImage.HeightRequest);
        worldImage.Pixbuf = InitWorld(worldImage.WidthRequest, worldImage.HeightRequest);
        LoadedImage.Pixbuf = InitWorld(LoadedImage.WidthRequest, LoadedImage.HeightRequest);

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

    protected int RenderArtificialLife(Pixbuf pixbuf, ArtificialLife artificialLife, int x, int y, bool Clear = true)
    {
        var Updates = 0;

        if (pixbuf != null && artificialLife != null)
        {
            var writeBuffer = artificialLife.GetPixelWriteBuffer();

            Updates += writeBuffer.Count;

            if (writeBuffer.Count > 0)
            {
                Parallel.ForEach(writeBuffer, (pixel) =>
                {
                    pixel.Write(pixbuf, x, y);
                });

                if (Clear)
                    artificialLife.ClearPixelWriteBuffer();
            }
        }

        return Updates;
    }

    protected void RenderColonies(Pixbuf pixbuf)
    {
        var Updates = 0;

        pixbuf.Fill(0);

        Parallel.ForEach(Colonies, (colony) =>
        {
            Updates += RenderArtificialLife(pixbuf, colony.ArtificialLife, colony.X, colony.Y, true);
        });

        if (Updates == 0)
            Pause();

        System.GC.Collect();
        System.GC.WaitForPendingFinalizers();
    }

    void RefreshColonies()
    {
        for (int i = 0; i < Colonies.Count; i++)
        {
            Colonies[i].ArtificialLife.Refresh();
        }
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

    void Refresh()
    {
        RefreshColonies();
        RenderColonies(worldPixbuf);
        RenderWorld(worldPixbuf);
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
                ColonyParameters.AddRange(Colonies[colony].ArtificialLife.Parameters());

                CopyNeighborhood(Colonies[colony].ArtificialLife.GetNeighborhood());

                ColonyColor.Color = Colonies[colony].ArtificialLife.Color();

                for (int i = 0; i < ColoniesType.Count; i++)
                {
                    if (Colonies[colony].ArtificialLife is Life && ColoniesType[i] == ColonyTypes.Type.Life)
                    {
                        ColonyTypeList.Active = i;
                    }

                    if (Colonies[colony].ArtificialLife is Zhabotinsky && ColoniesType[i] == ColonyTypes.Type.Zhabotinsky)
                    {
                        ColonyTypeList.Active = i;
                    }

                    if (Colonies[colony].ArtificialLife is LangtonAnt && ColoniesType[i] == ColonyTypes.Type.LangtonAnt)
                    {
                        ColonyTypeList.Active = i;
                    }

                    if (Colonies[colony].ArtificialLife is YinYangFire && ColoniesType[i] == ColonyTypes.Type.YinYangFire)
                    {
                        ColonyTypeList.Active = i;
                    }

                    if (Colonies[colony].ArtificialLife is ForestFire && ColoniesType[i] == ColonyTypes.Type.ForestFire)
                    {
                        ColonyTypeList.Active = i;
                    }

                    if (Colonies[colony].ArtificialLife is ElementaryCA && ColoniesType[i] == ColonyTypes.Type.ElementaryCA)
                    {
                        ColonyTypeList.Active = i;
                    }

                    if (Colonies[colony].ArtificialLife is Snowflake && ColoniesType[i] == ColonyTypes.Type.Snowflake)
                    {
                        ColonyTypeList.Active = i;
                    }

                    if (Colonies[colony].ArtificialLife is Ice && ColoniesType[i] == ColonyTypes.Type.Ice)
                    {
                        ColonyTypeList.Active = i;
                    }

                    if (Colonies[colony].ArtificialLife is Cyclic && ColoniesType[i] == ColonyTypes.Type.Cyclic)
                    {
                        ColonyTypeList.Active = i;
                    }
                }

                CopyParameterValues(ParameterList, ColonyParameters);

                Enable();
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
                if (i == GtkSelection.Selected - 1 || ShowColonies.Active)
                {
                    var box = boxes[i];

                    var w = Math.Abs(box.X1 - box.X0);
                    var h = Math.Abs(box.Y1 - box.Y0);

                    gc.RgbFgColor = i != GtkSelection.Selected - 1 ? GtkSelection.SelectedColor : Colonies[i].ArtificialLife.ColonyColor;
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
                var colony = added - 1;

                RefreshColonies();

                var box = GtkSelection.Selection.BoundingBoxes()[colony];

                var w = Math.Abs(box.X1 - box.X0);
                var h = Math.Abs(box.Y1 - box.Y0);
                var x = Math.Min(box.X0, box.X1);
                var y = Math.Min(box.Y0, box.Y1);

                var neighborhood = SetNeighborhood();

                if (ColoniesType[type] == ColonyTypes.Type.Life)
                {
                    var density = GetNumeric(ColonyParameters, "Density");
                    var Birth = GetString(ColonyParameters, "Birth");
                    var Survival = GetString(ColonyParameters, "Survival");

                    World.AddLifeColony(Colonies, w, h, x, y, density, Birth, Survival, ColonyColor.Color, neighborhood);
                }

                if (ColoniesType[type] == ColonyTypes.Type.LangtonAnt)
                {
                    var ants = (int)GetNumeric(ColonyParameters, "Ants");
                    var rule = GetString(ColonyParameters, "Rule");

                    World.AddLangtonAntColony(Colonies, w, h, x, y, ants, rule, ColonyColor.Color, Gradient.Active);
                }

                if (ColoniesType[type] == ColonyTypes.Type.Zhabotinsky)
                {
                    var density = GetNumeric(ColonyParameters, "Density");
                    var g = GetNumeric(ColonyParameters, "g");
                    var k1 = GetNumeric(ColonyParameters, "k1");
                    var k2 = GetNumeric(ColonyParameters, "k2");

                    World.AddZhabotinskyColony(Colonies, w, h, x, y, density, k1, k2, g, ColonyColor.Color, neighborhood, Gradient.Active);
                }

                if (ColoniesType[type] == ColonyTypes.Type.YinYangFire)
                {
                    var density = GetNumeric(ColonyParameters, "Density");
                    var maxstates = (int)GetNumeric(ColonyParameters, "MaxStates");

                    World.AddYinYangFireColony(Colonies, w, h, x, y, density, maxstates, ColonyColor.Color, neighborhood, Gradient.Active);
                }

                if (ColoniesType[type] == ColonyTypes.Type.ForestFire)
                {
                    var density = GetNumeric(ColonyParameters, "Density");
                    var F = GetNumeric(ColonyParameters, "F");
                    var P = GetNumeric(ColonyParameters, "P");

                    World.AddForestFireColony(Colonies, w, h, x, y, density, F, P, ColonyColor.Color, neighborhood);
                }

                if (ColoniesType[type] == ColonyTypes.Type.ElementaryCA)
                {
                    var rule = (int)GetNumeric(ColonyParameters, "Rule");
                    var height = w / 2;

                    GtkSelection.Selection.BoundingBoxes()[colony].Y0 = y;
                    GtkSelection.Selection.BoundingBoxes()[colony].Y1 = y + height - 1;

                    World.AddElementaryCA(Colonies, w, height, x, y, rule, ColonyColor.Color);
                }

                if (ColoniesType[type] == ColonyTypes.Type.Snowflake)
                {
                    var MaxStates = (int)GetNumeric(ColonyParameters, "MaxStates");
                    var Growth = GetString(ColonyParameters, "Growth");

                    World.AddSnowflakeColony(Colonies, w, h, x, y, MaxStates, Growth, ColonyColor.Color, neighborhood, Gradient.Active);
                }

                if (ColoniesType[type] == ColonyTypes.Type.Ice)
                {
                    var density = GetNumeric(ColonyParameters, "Density");
                    var Freeze = GetNumeric(ColonyParameters, "Freeze");

                    World.AddIceColony(Colonies, w, h, x, y, density, Freeze, ColonyColor.Color, neighborhood);
                }

                if (ColoniesType[type] == ColonyTypes.Type.Cyclic)
                {
                    var maxstates = (int)GetNumeric(ColonyParameters, "MaxStates");

                    World.AddCyclicColony(Colonies, w, h, x, y, maxstates, ColonyColor.Color, neighborhood, Gradient.Active);
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

                if (GtkSelection.Selected > 0 && Colonies.Count > 0 && (GtkSelection.Selected - 1) < Colonies.Count)
                {
                    var num = GtkSelection.Selected - 1;
                    var colony = Colonies[num];

                    var temp = InitWorld(colony.ArtificialLife.Width, colony.ArtificialLife.Height);

                    if (temp != null)
                    {
                        // Populate Write Buffer
                        colony.ArtificialLife.Refresh();

                        temp.Fill(0);

                        // Render to pixbuf but do not clear the write buffer
                        RenderArtificialLife(temp, colony.ArtificialLife, 0, 0, false);

                        temp.Save(FileName, "png");

                        temp.Dispose();
                    }
                }
                else
                {
                    if (worldPixbuf != null)
                    {
                        worldPixbuf.Save(FileName, "png");
                    }
                }
            }
        }

        ImageChooser.Hide();
    }

    protected void LoadImageFile()
    {
        // Add most recent directory
        if (!string.IsNullOrEmpty(ImageLoader.Filename))
        {
            var directory = System.IO.Path.GetDirectoryName(ImageLoader.Filename);

            if (Directory.Exists(directory))
            {
                ImageLoader.SetCurrentFolder(directory);
            }
        }

        if (ImageLoader.Run() == (int)ResponseType.Accept)
        {
            if (!string.IsNullOrEmpty(ImageLoader.Filename))
            {
                var FileName = ImageLoader.Filename;

                try
                {
                    var temp = new Pixbuf(FileName);

                    if (LoadedImage.Pixbuf != null && temp != null)
                    {
                        LoadedImage.Pixbuf.Dispose();
                        LoadedImage.Pixbuf = temp;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: {0}", ex.Message);
                }
            }
        }

        ImageLoader.Hide();
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

    protected void ClearNeighborhood()
    {
        TL.Active = false;
        TM.Active = false;
        TR.Active = false;
        ML.Active = false;
        MR.Active = false;
        BL.Active = false;
        BM.Active = false;
        BR.Active = false;
    }

    protected void CopyNeighborhood(List<Cell> neighborhood)
    {
        ClearNeighborhood();

        foreach (var neighbor in neighborhood)
        {
            if (neighbor.X == -1 && neighbor.Y == -1)
                TL.Active = true;

            if (neighbor.X == 0 && neighbor.Y == -1)
                TM.Active = true;

            if (neighbor.X == 1 && neighbor.Y == -1)
                TR.Active = true;

            if (neighbor.X == -1 && neighbor.Y == 0)
                ML.Active = true;

            if (neighbor.X == 1 && neighbor.Y == 0)
                MR.Active = true;

            if (neighbor.X == -1 && neighbor.Y == 1)
                BL.Active = true;

            if (neighbor.X == 0 && neighbor.Y == 1)
                BM.Active = true;

            if (neighbor.X == 1 && neighbor.Y == 1)
                BR.Active = true;
        }
    }

    protected List<Cell> SetNeighborhood()
    {
        var neighborhood = new List<Cell>();

        if (TL.Active)
            ParameterSets.AddNeighbor(neighborhood, new Cell(-1, -1));

        if (TM.Active)
            ParameterSets.AddNeighbor(neighborhood, new Cell(0, -1));

        if (TR.Active)
            ParameterSets.AddNeighbor(neighborhood, new Cell(1, -1));

        if (ML.Active)
            ParameterSets.AddNeighbor(neighborhood, new Cell(-1, 0));

        if (MR.Active)
            ParameterSets.AddNeighbor(neighborhood, new Cell(1, 0));

        if (BL.Active)
            ParameterSets.AddNeighbor(neighborhood, new Cell(-1, 1));

        if (BM.Active)
            ParameterSets.AddNeighbor(neighborhood, new Cell(0, 1));

        if (BR.Active)
            ParameterSets.AddNeighbor(neighborhood, new Cell(1, 1));

        return neighborhood;
    }

    protected void Run()
    {
        Paused = false;

        RunButton.Sensitive = false;
        StopButton.Sensitive = true;
        SaveButton.Sensitive = false;
        ShowColonies.Sensitive = false;
        ClearButton.Sensitive = false;
    }

    protected void Pause()
    {
        Paused = true;

        RunButton.Sensitive = true;
        StopButton.Sensitive = false;
        SaveButton.Sensitive = true;
        ShowColonies.Sensitive = true;
        ClearButton.Sensitive = true;
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

        if (LoadedImage.Pixbuf != null)
        {
            LoadedImage.Pixbuf.Dispose();
        }

        if (LoadedImage != null)
        {
            LoadedImage.Dispose();
        }

        Colonies.Clear();

        Application.Quit();

        a.RetVal = true;
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
                    colony.ArtificialLife.Update();
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
        if (Colonies.Count > 0)
        {
            Run();
        }
    }

    protected void OnStopButtonClicked(object sender, EventArgs e)
    {
        Pause();
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
            if (Paused)
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

                        Refresh();

                        System.GC.Collect();
                        System.GC.WaitForPendingFinalizers();

                        break;
                    }
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
                    IsSelecting |= Paused;
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
                Refresh();
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
                        CopyNeighborhood(ParameterSets.MooreNeighborhood());
                        break;
                    case ColonyTypes.Type.LangtonAnt:
                        ColonyParameters.AddRange(ParameterSets.LangtonAnt());
                        CopyNeighborhood(ParameterSets.EmptyNeighborhood());
                        break;
                    case ColonyTypes.Type.Zhabotinsky:
                        ColonyParameters.AddRange(ParameterSets.Zhabotinsky());
                        CopyNeighborhood(ParameterSets.MooreNeighborhood());
                        break;
                    case ColonyTypes.Type.YinYangFire:
                        ColonyParameters.AddRange(ParameterSets.YinYangFire());
                        CopyNeighborhood(ParameterSets.MooreNeighborhood());
                        break;
                    case ColonyTypes.Type.ForestFire:
                        ColonyParameters.AddRange(ParameterSets.ForestFire());
                        CopyNeighborhood(ParameterSets.MooreNeighborhood());
                        break;
                    case ColonyTypes.Type.ElementaryCA:
                        ColonyParameters.AddRange(ParameterSets.ElementaryCA());
                        CopyNeighborhood(ParameterSets.EmptyNeighborhood());
                        break;
                    case ColonyTypes.Type.Snowflake:
                        ColonyParameters.AddRange(ParameterSets.Snowflake());
                        CopyNeighborhood(ParameterSets.HexNeighborhood());
                        break;
                    case ColonyTypes.Type.Ice:
                        ColonyParameters.AddRange(ParameterSets.Ice());
                        CopyNeighborhood(ParameterSets.VonNeumannNeighborhood());
                        break;
                    case ColonyTypes.Type.Cyclic:
                        ColonyParameters.AddRange(ParameterSets.Cyclic());
                        CopyNeighborhood(ParameterSets.MooreNeighborhood());
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

    protected void OnAddImageButtonClicked(object sender, EventArgs e)
    {
        if (Paused)
        {
            var type = ColonyTypeList.Active;

            if (type >= 0)
            {
                GtkSelection.Selected = 0;

                var Width = Math.Min(worldImage.WidthRequest, LoadedImage.Pixbuf.Width);
                var Height = Math.Min(worldImage.HeightRequest, LoadedImage.Pixbuf.Height);

                var neighborhood = SetNeighborhood();
                var colony = ConvertImage.Convert(ColoniesType[type], LoadedImage, Width, Height, ColonyParameters, ColonyColor.Color, neighborhood, Gradient.Active);

                // Cannot handle Add Image for Elementary CA (1D)
                if (!(colony is EmptyArtificialLife || colony is ElementaryCA))
                {
                    var count = GtkSelection.Selection.Count();

                    GtkSelection.Selection.Add(0, 0, colony.Width, colony.Height);

                    if (GtkSelection.Selection.Count() > count)
                    {
                        Colonies.Add(new Colony(0, 0, colony));
                    }
                }

                Refresh();
            }
        }
    }

    protected void OnLoadImageButtonClicked(object sender, EventArgs e)
    {
        if (Paused)
        {
            LoadImageFile();
        }
    }

    protected void OnClearButtonClicked(object sender, EventArgs e)
    {
        if (Paused)
        {
            Colonies.Clear();
            GtkSelection.Selection.Clear();
            GtkSelection.Selected = 0;

            worldPixbuf.Fill(0);
            RenderWorld(worldPixbuf);

            Pause();
        }
    }

    protected void OnTestButtonClicked(object sender, EventArgs e)
    {
        if (Paused)
        {
            var type = ColonyTypeList.Active;

            if (type >= 0)
            {
                if (ColoniesType[type] == ColonyTypes.Type.Life)
                {
                    Tests.LifeTest(Colonies);
                }

                if (ColoniesType[type] == ColonyTypes.Type.LangtonAnt)
                {
                    Tests.LangtonAntTest(Colonies);
                }

                if (ColoniesType[type] == ColonyTypes.Type.Zhabotinsky)
                {
                    Tests.ZhabotinskyTest(Colonies);
                }

                if (ColoniesType[type] == ColonyTypes.Type.YinYangFire)
                {
                    Tests.YinYangFireTest(Colonies);
                }

                if (ColoniesType[type] == ColonyTypes.Type.ForestFire)
                {
                    Tests.ForestFireTest(Colonies);
                }

                if (ColoniesType[type] == ColonyTypes.Type.ElementaryCA)
                {
                    Tests.ElementaryCATest(Colonies);
                }

                if (ColoniesType[type] == ColonyTypes.Type.Snowflake)
                {
                    Tests.SnowflakeTest(Colonies);
                }

                if (ColoniesType[type] == ColonyTypes.Type.Ice)
                {
                    Tests.IceTest(Colonies);
                }

                if (ColoniesType[type] == ColonyTypes.Type.Cyclic)
                {
                    Tests.CyclicTest(Colonies);
                }

                RenderColonies(worldPixbuf);
                RenderWorld(worldPixbuf);
            }
        }
    }
}
