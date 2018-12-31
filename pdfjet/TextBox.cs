/**
 *  TextBox.cs
 *
Copyright (c) 2013, Innovatics Inc.
All rights reserved.

Redistribution and use in source and binary forms, with or without modification,
are permitted provided that the following conditions are met:

    * Redistributions of source code must retain the above copyright notice,
      this list of conditions and the following disclaimer.
 
    * Redistributions in binary form must reproduce the above copyright notice,
      this list of conditions and the following disclaimer in the documentation
      and / or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;


namespace PDFjet.NET {
/**
 *  A box containing line-wrapped text.
 *  
 *  <p>Defaults:<br />
 *  x = 0f<br />
 *  y = 0f<br />
 *  width = 300f<br />
 *  height = 200f<br />
 *  alignment = Align.LEFT<br />
 *  valign = Align.TOP<br />
 *  spacing = 3f<br />
 *  margin = 1f<br />
 *  </p>
 *  
 *  This class was originally developed by Ronald Bourret.
 *  It was completely rewritten in 2013 by Eugene Dragoev.
 */
public class TextBox : IDrawable {

    protected Font font;
    protected String text;

    protected float x;
    protected float y;

    protected float width = 300f;
    protected float height = 200f;
    protected float spacing = 3f;
    protected float margin = 1f;
    private float lineWidth;

    private int background = Color.white;
    private int pen = Color.black;
    private int brush = Color.black;
    private int valign = 0;
    private Font fallbackFont;
    private Dictionary<String, Int32> colors = null;

    // TextBox properties
    // Future use:
    // bits 0 to 15
    // Border:
    // bit 16 - top
    // bit 17 - bottom
    // bit 18 - left
    // bit 19 - right
    // Text Alignment:
    // bit 20
    // bit 21
    // Text Decoration:
    // bit 22 - underline
    // bit 23 - strikeout
    // Future use:
    // bits 24 to 31
    private int properties = 0x000F0001;


    /**
     *  Creates a text box and sets the font.
     *
     *  @param font the font.
     */
    public TextBox(Font font) {
        this.font = font;
    }


    /**
     *  Creates a text box and sets the font and the text.
     *
     *  @param font the font.
     *  @param text the text.
     *  @param width the width.
     *  @param height the height.
     */
    public TextBox(Font font, String text, double width, double height) :
        this(font, text, (float) width, (float) height) {
    }


    /**
     *  Creates a text box and sets the font and the text.
     *
     *  @param font the font.
     *  @param text the text.
     *  @param width the width.
     *  @param height the height.
     */
    public TextBox(Font font, String text, float width, float height) {
        this.font = font;
        this.text = text;
        this.width = width;
        this.height = height;
    }


    /**
     *  Sets the font for this text box.
     *
     *  @param font the font.
     */
    public void SetFont(Font font) {
        this.font = font;
    }


    /**
     *  Returns the font used by this text box.
     *
     *  @return the font.
     */
    public Font GetFont() {
        return font;
    }


    /**
     *  Sets the text box text.
     *
     *  @param text the text box text.
     */
    public void SetText(String text) {
        this.text = text;
    }


    /**
     *  Returns the text box text.
     *
     *  @return the text box text.
     */
    public String GetText() {
        return text;
    }


    /**
     *  Sets the position where this text box will be drawn on the page.
     *
     *  @param x the x coordinate of the top left corner of the text box.
     *  @param y the y coordinate of the top left corner of the text box.
     */
    public void SetPosition(double x, double y) {
        SetPosition((float) x, (float) y);
    }


    /**
     *  Sets the position where this text box will be drawn on the page.
     *
     *  @param x the x coordinate of the top left corner of the text box.
     *  @param y the y coordinate of the top left corner of the text box.
     */
    public void SetPosition(float x, float y) {
        SetLocation(x, y);
    }


    /**
     *  Sets the location where this text box will be drawn on the page.
     *
     *  @param x the x coordinate of the top left corner of the text box.
     *  @param y the y coordinate of the top left corner of the text box.
     */
    public void SetLocation(float x, float y) {
        this.x = x;
        this.y = y;
    }


    /**
     *  Gets the x coordinate where this text box will be drawn on the page.
     *
     *  @return the x coordinate of the top left corner of the text box.
     */
    public float GetX() {
        return x;
    }


    /**
     *  Gets the y coordinate where this text box will be drawn on the page.
     *
     *  @return the y coordinate of the top left corner of the text box.
     */
    public float GetY() {
        return y;
    }


    /**
     *  Sets the width of this text box.
     *
     *  @param width the specified width.
     */
    public void SetWidth(double width) {
        this.width = (float) width;
    }


    /**
     *  Sets the width of this text box.
     *
     *  @param width the specified width.
     */
    public void SetWidth(float width) {
        this.width = width;
    }


    /**
     *  Returns the text box width.
     *
     *  @return the text box width.
     */
    public float GetWidth() {
        return width;
    }


    /**
     *  Sets the height of this text box.
     *
     *  @param height the specified height.
     */
    public void SetHeight(double height) {
        this.height = (float) height;
    }


    /**
     *  Sets the height of this text box.
     *
     *  @param height the specified height.
     */
    public void SetHeight(float height) {
        this.height = height;
    }


    /**
     *  Returns the text box height.
     *
     *  @return the text box height.
     */
    public float GetHeight() {
        return height;
    }


    /**
     *  Sets the margin of this text box.
     *
     *  @param margin the margin between the text and the box
     */
    public void SetMargin(double margin) {
        this.margin = (float) margin;
    }


    /**
     *  Sets the margin of this text box.
     *
     *  @param margin the margin between the text and the box
     */
    public void SetMargin(float margin) {
        this.margin = margin;
    }


    /**
     *  Returns the text box margin.
     *
     *  @return the margin between the text and the box
     */
    public float GetMargin() {
        return margin;
    }


    /**
     *  Sets the border line width.
     *
     *  @param lineWidth float
     */
    public void SetLineWidth(double lineWidth) {
        this.lineWidth = (float) lineWidth;
    }


    /**
     *  Sets the border line width.
     *
     *  @param lineWidth float
     */
    public void SetLineWidth(float lineWidth) {
        this.lineWidth = lineWidth;
    }


    /**
     *  Returns the border line width.
     *
     *  @return float the line width.
     */
    public float GetLineWidth() {
        return lineWidth;
    }


    /**
     *  Sets the spacing between lines of text.
     *
     *  @param spacing the spacing
     */
    public void SetSpacing(double spacing) {
        this.spacing = (float) spacing;
    }


    /**
     *  Sets the spacing between lines of text.
     *
     *  @param spacing the spacing
     */
    public void SetSpacing(float spacing) {
        this.spacing = spacing;
    }


    /**
     *  Returns the spacing between lines of text.
     *
     *  @return the spacing.
     */
    public float GetSpacing() {
        return spacing;
    }


    /**
     *  Sets the background to the specified color.
     *
     *  @param color the color specified as 0xRRGGBB integer.
     */
    public void SetBgColor(int color) {
        this.background = color;
    }


    /**
     *  Sets the background to the specified color.
     *
     *  @param color the color specified as array of integer values from 0x00 to 0xFF.
     */
    public void SetBgColor(int[] color) {
        this.background = color[0] << 16 | color[1] << 8 | color[2];
    }


    /**
     *  Sets the background to the specified color.
     *
     *  @param color the color specified as array of double values from 0.0 to 1.0.
     */
    public void SetBgColor(double[] color) {
        SetBgColor(new int[] { (int) color[0], (int) color[1], (int) color[2] });
    }


    /**
     *  Returns the background color.
     *
     * @return int the color as 0xRRGGBB integer.
     */
    public int GetBgColor() {
        return this.background;
    }


    /**
     *  Sets the pen and brush colors to the specified color.
     *
     *  @param color the color specified as 0xRRGGBB integer.
     */
    public void SetFgColor(int color) {
        this.pen = color;
        this.brush = color;
    }


    /**
     *  Sets the pen and brush colors to the specified color.
     *
     *  @param color the color specified as 0xRRGGBB integer.
     */
    public void SetFgColor(int[] color) {
        this.pen = color[0] << 16 | color[1] << 8 | color[2];
        this.brush = pen;
    }


    /**
     *  Sets the foreground pen and brush colors to the specified color.
     *
     *  @param color the color specified as an array of double values from 0.0 to 1.0.
     */
    public void SetFgColor(double[] color) {
        SetPenColor(new int[] { (int) color[0], (int) color[1], (int) color[2] });
        SetBrushColor(pen);
    }


    /**
     *  Sets the pen color.
     *
     *  @param color the color specified as 0xRRGGBB integer.
     */
    public void SetPenColor(int color) {
        this.pen = color;
    }


    /**
     *  Sets the pen color.
     *
     *  @param color the color specified as an array of int values from 0x00 to 0xFF.
     */
    public void SetPenColor(int[] color) {
        this.pen = color[0] << 16 | color[1] << 8 | color[2];
    }


    /**
     *  Sets the pen color.
     *
     *  @param color the color specified as an array of double values from 0.0 to 1.0.
     */
    public void SetPenColor(double[] color) {
        SetPenColor(new int[] { (int) color[0], (int) color[1], (int) color[2] });
    }


    /**
     *  Returns the pen color as 0xRRGGBB integer.
     *
     * @return int the pen color.
     */
    public int GetPenColor() {
        return this.pen;
    }


    /**
     *  Sets the brush color.
     *
     *  @param color the color specified as 0xRRGGBB integer.
     */
    public void SetBrushColor(int color) {
        this.brush = color;
    }


    /**
     *  Sets the brush color.
     *
     *  @param color the color specified as an array of int values from 0x00 to 0xFF.
     */
    public void SetBrushColor(int[] color) {
        this.brush = color[0] << 16 | color[1] << 8 | color[2];
    }


    /**
     *  Sets the brush color.
     *
     *  @param color the color specified as an array of double values from 0.0 to 1.0.
     */
    public void SetBrushColor(double[] color) {
        SetBrushColor(new int [] { (int) color[0], (int) color[1], (int) color[2] });
    }


    /**
     * Returns the brush color.
     *
     * @return int the brush color specified as 0xRRGGBB integer.
     */
    public int GetBrushColor() {
        return this.brush;
    }


    /**
     *  Sets the TextBox border object.
     *
     *  @param border the border object.
     */
    public void SetBorder(int border, bool visible) {
        if (visible) {
            this.properties |= border;
        }
        else {
            this.properties &= (~border & 0x00FFFFFF);
        }
    }


    /**
     *  Returns the text box border.
     *
     *  @return boolean the text border object.
     */
    public bool GetBorder(int border) {
        return (this.properties & border) != 0;
    }


    /**
     *  Sets all borders to be invisible.
     *  This cell will have no borders when drawn on the page.
     */
    public void SetNoBorders() {
        this.properties &= 0x00F0FFFF;
    }


    /**
     *  Sets the cell text alignment.
     *
     *  @param alignment the alignment code.
     *  Supported values: Align.LEFT, Align.RIGHT and Align.CENTER.
     */
    public void SetTextAlignment(int alignment) {
        this.properties &= 0x00CFFFFF;
        this.properties |= (alignment & 0x00300000);
    }


    /**
     *  Returns the text alignment.
     *
     *  @return alignment the alignment code. Supported values: Align.LEFT, Align.RIGHT and Align.CENTER.
     */
    public int GetTextAlignment() {
        return (this.properties & 0x00300000);
    }


    /**
     *  Sets the underline variable.
     *  If the value of the underline variable is 'true' - the text is underlined.
     *
     *  @param underline the underline flag.
     */
    public void SetUnderline(bool underline) {
        if (underline) {
            this.properties |= 0x00400000;
        }
        else {
            this.properties &= 0x00BFFFFF;
        }
    }


    /**
     *  Whether the text will be underlined.
     *
     *  @return whether the text will be underlined
     */
    public bool GetUnderline() {
        return (properties & 0x00400000) != 0;
    }


    /**
     *  Sets the srikeout flag.
     *  In the flag is true - draw strikeout line through the text.
     *
     *  @param strikeout the strikeout flag.
     */
    public void SetStrikeout(bool strikeout) {
        if (strikeout) {
            this.properties |= 0x00800000;
        }
        else {
            this.properties &= 0x007FFFFF;
        }
    }


    /**
     *  Returns the strikeout flag.
     *
     *  @return boolean the strikeout flag.
     */
    public bool GetStrikeout() {
        return (properties & 0x00800000) != 0;
    }


    public void SetFallbackFont(Font font) {
        this.fallbackFont = font;
    }


    /**
     *  Sets the vertical alignment of the text in this TextBox.
     *
     *  @param alignment - valid values are TextAlign.TOP, TextAlign.BOTTOM and TextAlign.CENTER
     */
    public void SetVerticalAlignment(int alignment) {
        this.valign = alignment;
    }


    public int GetVerticalAlignment() {
        return this.valign;
    }


    public void SetTextColors(Dictionary<String, Int32> colors) {
        this.colors = colors;
    }


    /**
     *  Draws this text box on the specified page.
     *
     */
    public void DrawOn(Page page) {
        if (GetBgColor() != Color.white) {
            DrawBackground(page);
        }
        DrawBorders(page);
        DrawText(page);
    }


    private void DrawBackground(Page page) {
        page.SetBrushColor(background);
        page.FillRect(x, y, width, height);
    }


    private void DrawBorders(Page page) {
        page.SetPenColor(pen);
        page.SetPenWidth(lineWidth);

        if (GetBorder(Border.TOP) &&
                GetBorder(Border.BOTTOM) &&
                GetBorder(Border.LEFT) &&
                GetBorder(Border.RIGHT)) {
            page.DrawRect(x, y, width, height);
        }
        else {
            if (GetBorder(Border.TOP)) {
                page.MoveTo(x, y);
                page.LineTo(x + width, y);
                page.StrokePath();
            }
            if (GetBorder(Border.BOTTOM)) {
                page.MoveTo(x, y + height);
                page.LineTo(x + width, y + height);
                page.StrokePath();
            }
            if (GetBorder(Border.LEFT)) {
                page.MoveTo(x, y);
                page.LineTo(x, y + height);
                page.StrokePath();
            }
            if (GetBorder(Border.RIGHT)) {
                page.MoveTo(x + width, y);
                page.LineTo(x + width, y + height);
                page.StrokePath();
            }
        }
    }


    private void DrawText(Page page) {
 
        page.SetPenColor(this.pen);
        page.SetBrushColor(this.brush);
        page.SetPenWidth(this.font.underlineThickness);
 
        float textAreaWidth = width - 2*margin;
        List<String> list = new List<String>();
        StringBuilder buf = new StringBuilder();
        String[] lines = Regex.Split(text, "\n");
        foreach (String line in lines) {
            if (font.StringWidth(line) < textAreaWidth) {
                list.Add(line);
            }
            else {
                buf.Length = 0;
 
                String[] tokens = Regex.Split(line, "\\s+");
                foreach (String token in tokens) {
                    if (font.StringWidth(buf.ToString() + " " + token) < textAreaWidth) {
                        buf.Append(" " + token);
                    }
                    else {
                        list.Add(buf.ToString().Trim());
                        buf.Length = 0;
                        buf.Append(token);
                    }
                }
 
                if (!buf.ToString().Trim().Equals("")) {
                    list.Add(buf.ToString().Trim());
                }
            }
        }
        lines = list.ToArray();
 
        float lineHeight = font.GetBodyHeight() + spacing;
        float x_text;
        float y_text = y + font.ascent + margin;
        if (valign == TextAlign.BOTTOM) {
            y_text += height - lines.Length*lineHeight;
        }
        else if (valign == TextAlign.CENTER) {
            y_text += (height - lines.Length*lineHeight)/2f;
        }
 
        for (int i = 0; i < lines.Length; i++) {
 
            if (GetTextAlignment() == Align.RIGHT) {
                x_text = (x + width) - (font.StringWidth(lines[i]) + margin);
            }
            else if (GetTextAlignment() == Align.CENTER) {
                x_text = x + (width - font.StringWidth(lines[i])) / 2f;
            }
            else {
                // Align.LEFT
                x_text = x + margin;
            }
 
            if (y_text + font.GetBodyHeight() + spacing + font.GetDescent() >= y + height
                    && i < (lines.Length - 1)) {
                String str = lines[i];
                int index = str.LastIndexOf(' ');
                if (index != -1) {
                    lines[i] = str.Substring(0, index) + " ...";
                }
                else {
                    lines[i] = str + " ...";
                }
            }

            if (y_text + font.GetDescent() < y + height) {
                if (fallbackFont == null) {
                    if (colors == null) {
                        page.DrawString(font, lines[i], x_text, y_text);
                    }
                    else {
                        page.DrawString(font, lines[i], x_text, y_text, colors);
                    }
                }
                else {
                    page.DrawString(font, fallbackFont, lines[i], x_text, y_text);
                }
 
                float lineLength = font.StringWidth(lines[i]);
                if (GetUnderline()) {
                    float y_adjust = font.underlinePosition;
                    page.MoveTo(x_text, y_text + y_adjust);
                    page.LineTo(x_text + lineLength, y_text + y_adjust);
                    page.StrokePath();
                }
                if (GetStrikeout()) {
                    float y_adjust = font.body_height / 4f;
                    page.MoveTo(x_text, y_text - y_adjust);
                    page.LineTo(x_text + lineLength, y_text - y_adjust);
                    page.StrokePath();
                }
 
                y_text += font.GetBodyHeight() + spacing;
            }
        }
    }

}   // End of TextBox.cs
}   // End of namespace PDFjet.NET
