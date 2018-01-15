using Gdk;
using GLib;
using Gtk;
using System.Collections.Generic;
using System.Diagnostics;

public partial class MainWindow : Gtk.Window
{
    FileChooserDialog ImageChooser;

    Stopwatch timer = new Stopwatch();

    List<Colony> Colonies = new List<Colony>();

    const int PAGE_WORLD = 0;
    const int PAGE_WORLD_PARAMS = 1;

    Pixbuf worldPixbuf;
    bool Paused;

    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        Build();

        InitControls();

        //Tests.LifeTest(Colonies);

		//Tests.ZhabotinskyTest(Colonies);

		//Tests.LifeZhabotinskyTest(Colonies);

		//Tests.LangtonAntTest(Colonies);

        Tests.YinYangFireTest(Colonies);

        RenderColonies(worldPixbuf);

        RenderWorld(worldPixbuf);

        Tic();

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
            worldImage.Pixbuf = pixbuf;
    }


    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        if (worldPixbuf != null)
            worldPixbuf.Dispose();

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
        System.Threading.Tasks.Parallel.ForEach(Colonies, (colony) =>
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
                System.Threading.Tasks.Parallel.ForEach(writeBuffer, (pixel) =>
                {
                    pixel.Write(pixbuf, x, y);
                });

                artificialLife.ClearPixelWriteBuffer();
            }
        }
    }

    bool OnIdle()
    {
        if (!Paused)
        {
            if (Ticks() > 30)
            {
                Toc();

                var start = Ticks();

                System.Threading.Tasks.Parallel.ForEach(Colonies, (colony) =>
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

                System.Console.WriteLine("Colonies Rendered in {0} ms", Ticks() - start);
            }
        }

        return true;
    }

    protected void OnRunButtonClicked(object sender, System.EventArgs e)
    {
        Paused = false;

        RunButton.Sensitive = false;
        StopButton.Sensitive = true;
        SaveButton.Sensitive = false;
    }

    protected void OnStopButtonClicked(object sender, System.EventArgs e)
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

			if (System.IO.Directory.Exists(directory))
			{
				ImageChooser.SetCurrentFolder(directory);
			}
		}

        if (ImageChooser.Run() == (int)ResponseType.Accept)
        {
            if (!string.IsNullOrEmpty(ImageChooser.Filename))
            {
				var FileName = ImageChooser.Filename;

				if (!FileName.EndsWith(".png", System.StringComparison.OrdinalIgnoreCase))
					FileName += ".png";
                
                worldPixbuf.Save(FileName, "png");
            }
        }

        ImageChooser.Hide();
    }

    protected void OnSaveButtonClicked(object sender, System.EventArgs e)
    {
        if (Paused)
        {
            SaveImageFile();
        }
    }
}
