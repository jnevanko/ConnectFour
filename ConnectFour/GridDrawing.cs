namespace ConnectFour;

public class GridDrawing : IDrawable
{
    const float WIDTH_TO_HEIGHT_RATIO = (float)Game.NUM_COLS / Game.NUM_ROWS;

    public Game Game;

    bool m_SizeIsValid = false;
    float m_ViewWidth = 0;
    float m_ViewHeight = 0;
    float m_GridWidth;
    float m_GridHeight;
    float m_GridLeft;
    float m_GridTop;
    float m_HoleDelta;
    float m_FirstHoleOffset;
    float m_HoleRadius;

    public int TapToColumnIndex(Point tapLocation)
    {
        if (!m_SizeIsValid ||
            tapLocation.Y < m_GridTop ||
            tapLocation.Y > m_GridTop + m_GridHeight)
        {
            return -1;
        }

        int columnIndex = (int)Math.Floor((tapLocation.X - m_GridLeft) / m_HoleDelta);

        if (columnIndex < 0 || Game.NUM_COLS <= columnIndex)
        {
            return -1;
        }

        return columnIndex;
    }

    public void Resize(float width, float height)
    {
        m_SizeIsValid = false;

        m_ViewWidth = width;
        m_ViewHeight = height;

        float newWidth = (float)(m_ViewHeight * WIDTH_TO_HEIGHT_RATIO);

        if (newWidth <= m_ViewWidth)
        {
            m_GridWidth = newWidth;
            m_GridHeight = m_ViewHeight;
        }
        else
        {
            m_GridWidth = m_ViewWidth;
            m_GridHeight = m_ViewWidth / WIDTH_TO_HEIGHT_RATIO;
        }

        m_GridLeft = (float)((m_ViewWidth - m_GridWidth) / 2.0);
        m_GridTop = (float)((m_ViewHeight - m_GridHeight) / 2.0);

        m_HoleDelta = m_GridWidth / Game.NUM_COLS;
        m_FirstHoleOffset = m_HoleDelta / 2F;
        m_HoleRadius = 0.425F * m_HoleDelta;

        m_SizeIsValid = true;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        if (!m_SizeIsValid)
        {
            return;
        }

        canvas.FillColor = Colors.SlateGray;
        canvas.FillRectangle(0, 0, m_ViewWidth, m_ViewHeight);

        canvas.FillColor = Colors.Gold;
        canvas.FillRectangle(m_GridLeft, m_GridTop, m_GridWidth, m_GridHeight);

        for (int r = 0; r < Game.NUM_ROWS; ++r)
        {
            for (int c = 0; c < Game.NUM_COLS; ++c)
            {
                switch (Game.Hole(r, c))
                {
                    case HoleStatus.Red:
                        canvas.FillColor = Colors.Red;
                        break;

                    case HoleStatus.Black:
                        canvas.FillColor = Colors.Black;
                        break;

                    case HoleStatus.Empty:
                        canvas.FillColor = Color.FromRgb(150, 180, 210);
                        break;

                    default:
                        throw new InvalidOperationException();
                }

                canvas.FillCircle(
                    m_GridLeft + m_FirstHoleOffset + c * m_HoleDelta,
                    m_GridTop + m_FirstHoleOffset + (Game.NUM_ROWS - r - 1) * m_HoleDelta,
                    m_HoleRadius);
            }
        }
    }
}