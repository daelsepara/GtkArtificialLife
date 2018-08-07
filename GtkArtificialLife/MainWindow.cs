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

    const int PAGE_WORLD = 0;
    const int PAGE_WORLD_PARAMS = 1;

    Pixbuf worldPixbuf;
    bool Paused;

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

        foreach(ColonyTypes.Type type in Enum.GetValues(typeof(ColonyTypes.Type)))
        {
            ColoniesType.Add(type);
            ColonyTypeList.AppendText(type.ToString());
        }
    }

    protected Pixbuf InitWorld(int width, int height)
    {
        var pixbuf = new Pixbuf(Colorspace.Rgb, false, 8, width, height);

        pixbuf.Fill(0);

        return pixbuf;
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
            worldPixbuf.Dispose();

        if (worldImage.Pixbuf != null)
            worldImage.Pixbuf.Dispose();

        if (worldImage != null)
            worldImage.Dispose();

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
            return;
        
        if (GtkSelection.Selection.Count() > 0)
        {
            var dest = worldImage.GdkWindow;
            var gc = new Gdk.GC(dest);
            var boxes = GtkSelection.Selection.BoundingBoxes();

            for (int i = 0; i < boxes.Count; i++)
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

    void DrawSelection()
    {
        if (Paused)
        {
            if (worldPixbuf == null)
                return;

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
        }
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
					FileName += ".png";
                
                worldPixbuf.Save(FileName, "png");
            }
        }

        ImageChooser.Hide();
    }

    protected void OnSaveButtonClicked(object sender, EventArgs e)
    {
        if (Paused)
        {
            SaveImageFile();
        }
    }

    void InitializeSelected()
    {
        if (GtkSelection.Selected > 0)
        {
        }
    }

    protected void Move()
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
                        Tests.AddLifeColony(Colonies, w, h, x, y, 0.40, new Color(255, 0, 255));

                    if (ColoniesType[type] == ColonyTypes.Type.LangtonAnt)
                        Tests.AddLangtonAntColony(Colonies, w, h, x, y, 50, "RRLLLLRRRLLL", new Color(255, 0, 0));

                    if (ColoniesType[type] == ColonyTypes.Type.Zhabotinsky)
                        Tests.AddZhabotinskyColony(Colonies, w, h, x, y, 0.40, 1, 1, 10, new Color(0, 255, 64));

                    if (ColoniesType[type] == ColonyTypes.Type.YinYangFire)
                        Tests.AddYinYangFireColony(Colonies, w, h, x, y, 0.40, 64, new Color(255, 255, 0));

                    RenderColonies(worldPixbuf);
                    RenderWorld(worldPixbuf);
                }
            }
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
        if (!IsSelecting && !IsDragging) return;

        X1 = Convert.ToInt32(args.Event.X);
        Y1 = Convert.ToInt32(args.Event.Y);

        if (IsDragging)
        {
            Move();
        }
    }
}
