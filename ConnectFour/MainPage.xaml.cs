namespace ConnectFour;

public partial class MainPage : ContentPage
{
    readonly Game m_Game = new Game();

	public MainPage()
	{
		InitializeComponent();

        GridDrawing drawing = Grid.Drawable as GridDrawing;

        drawing.Game = m_Game;
	}

    protected override void OnSizeAllocated(double width, double height)
    {
        GridDrawing drawing = Grid.Drawable as GridDrawing;

        drawing.Resize((float)width, (float)height);

        base.OnSizeAllocated(width, height);
    }

    private void HandleGridTapped(object sender, TappedEventArgs e)
	{
        Point? point = e.GetPosition(Grid);

        if (point.HasValue)
        {
            GridDrawing drawing = Grid.Drawable as GridDrawing;

            int columnIndex = drawing.TapToColumnIndex(point.Value);

            if (m_Game.InsertChecker(columnIndex))
            {
                Grid.Invalidate();
            }
        }
    }
}