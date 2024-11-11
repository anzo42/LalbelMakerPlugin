// Name: Label Maker
// Submenu: Text Formations
// Author: BoltBait
// Title: BoltBait's Label Maker v1.2
// Version: 1.2
// Desc: Render text on a colored background
// Keywords: text|render|labels|maker
// URL: https://BoltBait.com/pdn
#region UICode
TextboxControl Amount1 = ""; // [255] Text
FontFamily Amount2 = "Courier New"; // Font
IntSliderControl Amount6 = 72; // [10,200] Size
ColorWheelControl Amount3 = ColorBgra.FromBgr(0, 0, 0); // [PrimaryColor!] Text Color
ColorWheelControl Amount4 = ColorBgra.FromBgr(255, 255, 255); // [SecondaryColor!] Background Color
PanSliderControl Amount5 = new Vector2Double(0.000, 0.980); // Location
CheckboxControl Amount7 = false; // Add box outline
ColorWheelControl Amount8 = ColorBgra.FromBgr(0, 0, 0); // [Black] {Amount7} Box outline
IntSliderControl Amount9 = 2; // [1,20] Outline size
CheckboxControl AddLeftArrow = false; // Add left arrow
CheckboxControl AddRightArrow = false; // Add right arrow
CheckboxControl AddTopArrow = false; // Add top arrow
CheckboxControl AddBottomArrow = false; // Add botom arrow
DoubleSliderControl ArrowSize = -2.5; // [-5,-0.5] Arrow size
#endregion
protected override unsafe void OnDraw(IDeviceContext deviceContext)
{
    string myText = " " + Amount1.Trim() + " ";
  
    // Maintain background
    deviceContext.DrawImage(Environment.SourceImage);

    // Get the size of our canvas
    RectFloat sourceBounds = new RectFloat(Point2Float.Zero, Environment.Document.Size);

    // Create a brush for text and a brush for the background
  
    // Whenever using the color out of a color wheel control, you must double cast it otherwise you'll get
    // the wrong shade of color.
    ISolidColorBrush solidFillBrush = deviceContext.CreateSolidColorBrush((LinearColorA)(SrgbColorA)Amount3);
    ISolidColorBrush backgroundBrush = deviceContext.CreateSolidColorBrush((LinearColorA)(SrgbColorA)Amount4);
    ISolidColorBrush outlineBrush = deviceContext.CreateSolidColorBrush((LinearColorA)(SrgbColorA)Amount8);

    // Set the antialiasing modes
    deviceContext.AntialiasMode = AntialiasMode.PerPrimitive;
    deviceContext.TextAntialiasMode = TextAntialiasMode.Grayscale;

    // Set the text rendering mode
    deviceContext.UseTextRenderingMode(TextRenderingMode.Outline);

    // Prepare for rendering text
    IDirectWriteFactory textFactory = this.Services.GetService<IDirectWriteFactory>();
    IGdiFontMap fm = textFactory.GetGdiFontMap();

    // Select your font to look at the font properties
    FontProperties fp = fm.TryGetFontProperties(Amount2);

    // Use your font properties to create an actual font
    ITextFormat textFormat = textFactory.CreateTextFormat(
        fp.FontFamilyName, // font family name
        null,
        FontWeight.Bold, // font weight
        FontStyle.Normal, // font style
        fp.Stretch, // how to stretch the font
        Amount6); // size in points

    // Combine your text with your text format to create a layout
    ITextLayout textLayout = textFactory.CreateTextLayout(
        myText, 
        textFormat, 
        TextLayoutConstants.DefaultMaxWidth,
        TextLayoutConstants.DefaultMaxHeight);

    // Measure the text we're going to render
    TextMeasurement tm = textLayout.Measure();

    // Make a rectangle of the background strip we're going to color in
    RectFloat r = new RectFloat(sourceBounds.Center.X - (tm.Width/2), sourceBounds.Center.Y - (tm.Height/2), tm.Width, tm.Height);
    
    // Determine the width of the vertical arrows so that they are no wider than the blank text label
    var verticalArrowWidth = tm.Height/2;
    if (tm.Width < tm.Height)
    {
        verticalArrowWidth = tm.Width/2;
    }

    // Define the points for the final shape based on the dimensions of the rectangle above
    Point2Float[] points = new Point2Float[]
    {
        new Point2Float(r.Left, r.Top),
        new Point2Float(r.Center.X - verticalArrowWidth, r.Top),
        new Point2Float(r.Center.X, r.Top - (tm.Height / (float)Math.Abs(ArrowSize))),
        new Point2Float(r.Center.X + verticalArrowWidth, r.Top),
        new Point2Float(r.Right, r.Top),
        new Point2Float(r.Right + (tm.Height / (float)Math.Abs(ArrowSize)), r.Center.Y),
        new Point2Float(r.Right, r.Bottom),
        new Point2Float(r.Center.X + verticalArrowWidth, r.Bottom),
        new Point2Float(r.Center.X, r.Bottom + (tm.Height / (float)Math.Abs(ArrowSize))),
        new Point2Float(r.Center.X - verticalArrowWidth, r.Bottom),
        new Point2Float(r.Left, r.Bottom),
        new Point2Float(r.Left - (tm.Height / (float)Math.Abs(ArrowSize)), r.Center.Y),
    };

    // Flattens the arrow heads if it is not enabled
    if (!AddRightArrow)
    {
        points[5] = new Point2Float(r.Right, r.Center.Y);
    }

    if (!AddLeftArrow)
    {
        points[11] = new Point2Float(r.Left, r.Center.Y);
    }

    if (!AddTopArrow)
    {
        points[2] = new Point2Float(r.Center.X, r.Top);
    }

    if (!AddBottomArrow)
    {
        points[8] = new Point2Float(r.Center.X, r.Bottom);
    }

    // Calculate the difference in location based on the [+] control
    float xoffset = r.X * (float)Amount5.X;
    float yoffset = r.Y * (float)Amount5.Y;

    // Here we always offset all drawing by 0.5f (half a pixel) to get cleaner renders
    // as this draws on pixel boundaries instead of between them.
    using (deviceContext.UseTranslateTransform(xoffset + 0.5f, yoffset + 0.5f))
    {
        if (Amount7)
        {
            // Draw the background shape that acts as an outline
            deviceContext.DrawPolygon(points, outlineBrush, Amount9 * 2);
        }

        // Draw the background shape
        deviceContext.FillPolygon(points, backgroundBrush);
        // Draw the text
        deviceContext.DrawText(
            myText, // the actual text to render
            textFormat, // format of text defined above
            r, // text location
            solidFillBrush, // our brush, from above
            DrawTextOptions.None); // Options
    }
}
 