/**
 *  TextLine.cs
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


namespace PDFjet.NET {
/**
 *  Used to create text line objects.
 *
 *
 */
public class TextLine : IDrawable {

    internal float x;
    internal float y;

    internal Font font;
    internal Font fallbackFont;
    internal String str;

    private String uri;
    private String key;

    private bool underline = false;
    private bool strikeout = false;
    private int degrees = 0;
    private int color = Color.black;

    private float box_x;
    private float box_y;
    
    private int textEffect = Effect.NORMAL;


    /**
     *  Constructor for creating text line objects.
     *
     *  @param font the font to use.
     */
    public TextLine(Font font) {
        this.font = font;
    }


    /**
     *  Constructor for creating text line objects.
     *
     *  @param font the font to use.
     *  @param text the text.
     */
    public TextLine(Font font, String text) {
        this.font = font;
        this.str = text;
    }


    /**
     *  Sets the position where this text line will be drawn on the page.
     *
     *  @param x the x coordinate of the top left corner of the text line.
     *  @param y the y coordinate of the top left corner of the text line.
     */
    public void SetPosition(double x, double y) {
        SetPosition((float) x, (float) y);
    }


    /**
     *  Sets the position where this text line will be drawn on the page.
     *
     *  @param x the x coordinate of the top left corner of the text line.
     *  @param y the y coordinate of the top left corner of the text line.
     */
    public void SetPosition(float x, float y) {
        SetLocation(x, y);
    }


    /**
     *  Sets the location where this text line will be drawn on the page.
     *
     *  @param x the x coordinate of the top left corner of the text line.
     *  @param y the y coordinate of the top left corner of the text line.
     */
    public void SetLocation(float x, float y) {
        this.x = x;
        this.y = y;
    }


    /**
     *  Sets the text.
     *
     *  @param text the text.
     */
    public void SetText(String text) {
        this.str = text;
    }


    /**
     *  Sets the font to use for this text line.
     *
     *  @param font the font to use.
     */
    public void SetFont(Font font) {
        this.font = font;
    }


    /**
     *  Gets the font to use for this text line.
     *
     *  @return font the font to use.
     */
    public Font GetFont() {
        return font;
    }


    /**
     *  Sets the fallback font.
     *
     *  @param fallbackFont the fallback font.
     */
    public void SetFallbackFont(Font fallbackFont) {
        this.fallbackFont = fallbackFont;
    }


    /**
     *  Sets the color for this text line.
     *
     *  @param color the color specified as an integer.
     */
    public void SetColor(int color) {
        this.color = color;
    }


    /**
     *  Returns the text.
     *
     *  @return the text.
     */
    public String GetText() {
        return str;
    }


    public float GetDestinationY() {
        return y - font.GetSize();
    }


    /**
     *  Returns the width of this TextLine.
     *
     *  @return the width.
     */
    public float GetWidth() {
        if (fallbackFont == null) {
            return font.StringWidth(str);
        }
        return font.StringWidth(fallbackFont, str);
    }


    /**
     *  Returns the height of this TextLine.
     *
     *  @return the height.
     */
    public double GetHeight() {
        return font.GetHeight();
    }


    /**
     *  Returns the text line color.
     *
     *  @return the text line color.
     */
    public int GetColor() {
        return color;
    }


    /**
     *  Sets the URI for the "click text line" action.
     *
     *  @param uri the URI
     */
    public void SetURIAction(String uri) {
        this.uri = uri;
    }


    public String GetURIAction() {
        return this.uri;
    }


    /**
     *  Sets the destination key for the action.
     *
     *  @param key the destination name.
     */
    public void SetGoToAction(String key) {
        this.key = key;
    }


    public String GetGoToAction() {
        return this.key;
    }


    /**
     *  Sets the underline variable.
     *  If the value of the underline variable is 'true' - the text is underlined.
     *
     *  @param underline the underline flag.
     */
    public void SetUnderline(bool underline) {
        this.underline = underline;
    }


    public bool GetUnderline() {
        return this.underline;
    }


    /**
     *  Sets the strike variable.
     *  If the value of the strike variable is 'true' - a strike line is drawn through the text.
     *
     *  @param strike the strike value.
     */
    public void SetStrikeLine(bool strike) {
        this.strikeout = strike;
    }


    /**
     *  Sets the strike variable.
     *  If the value of the strike variable is 'true' - a strike line is drawn through the text.
     *
     *  @param strike the strike value.
     */
    public void SetStrikeout(bool strike) {
        this.strikeout = strike;
    }


    public bool GetStrikeout() {
        return this.strikeout;
    }


    /**
     *  Sets the direction in which to draw the text.
     *
     *  @param degrees the number of degrees.
     */
    public void SetTextDirection(int degrees) {
        this.degrees = degrees;
    }


    public int GetTextEffect() {
        return textEffect;
    }
    

    public void SetTextEffect(int textEffect) {
        this.textEffect = textEffect;
    }


    /**
     *  Places this text line in the specified box at position (0.0, 0.0).
     *
     *  @param box the specified box.
     */
    public void PlaceIn(Box box) {
        PlaceIn(box, 0.0, 0.0);
    }


    /**
     *  Places this text line in the box at the specified offset.
     *
     *  @param box the specified box.
     *  @param x_offset the x offset from the top left corner of the box.
     *  @param y_offset the y offset from the top left corner of the box.
     */
    public void PlaceIn(
            Box box,
            double x_offset,
            double y_offset) {
        PlaceIn(box, (float) x_offset, (float) y_offset);
    }


    /**
     *  Places this text line in the box at the specified offset.
     *
     *  @param box the specified box.
     *  @param x_offset the x offset from the top left corner of the box.
     *  @param y_offset the y offset from the top left corner of the box.
     */
    public void PlaceIn(
            Box box,
            float x_offset,
            float y_offset) {
        box_x = box.x + x_offset;
        box_y = box.y + y_offset;
    }


    /**
     *  Draws this text line on the specified page.
     *
     *  @param page the page to draw this text line on.
     */
    public void DrawOn(Page page) {
        DrawOn(page, true);
    }


    /**
     *  Draws this text line on the specified page if the draw parameter is true.
     *
     *  @param page the page to draw this text line on.
     *  @param draw if draw is false - no action is performed.
     */
    internal void DrawOn(Page page, bool draw) {
        if (page == null || !draw) return;

        page.SetTextDirection(degrees);
        x += box_x;
        y += box_y;
        if (uri != null || key != null) {
            page.annots.Add(new Annotation(
                    uri,
                    key,    // The destination name
                    x,
                    page.height - (y - font.ascent),
                    x + font.StringWidth(str),
                    page.height - (y - font.descent)));
        }

        if (str != null) {
            page.SetBrushColor(color);
            if (fallbackFont == null) {
                page.DrawString(font, str, x, y);
            }
            else {
                page.DrawString(font, fallbackFont, str, x, y);
            }
        }

        if (underline) {
            page.SetPenWidth(font.underlineThickness);
            page.SetPenColor(color);
            double lineLength = font.StringWidth(str);
            double radians = Math.PI * degrees / 180.0;
            double x_adjust = font.underlinePosition * Math.Sin(radians);
            double y_adjust = font.underlinePosition * Math.Cos(radians);
            double x2 = x + lineLength * Math.Cos(radians);
            double y2 = y - lineLength * Math.Sin(radians);
            page.MoveTo(x + x_adjust, y + y_adjust);
            page.LineTo(x2 + x_adjust, y2 + y_adjust);
            page.StrokePath();
        }

        if (strikeout) {
            page.SetPenWidth(font.underlineThickness);
            page.SetPenColor(color);
            double lineLength = font.StringWidth(str);
            double radians = Math.PI * degrees / 180.0;
            double x_adjust = ( font.body_height / 4.0 ) * Math.Sin(radians);
            double y_adjust = ( font.body_height / 4.0 ) * Math.Cos(radians);
            double x2 = x + lineLength * Math.Cos(radians);
            double y2 = y - lineLength * Math.Sin(radians);
            page.MoveTo(x - x_adjust, y - y_adjust);
            page.LineTo(x2 - x_adjust, y2 - y_adjust);
            page.StrokePath();
        }

        page.SetTextDirection(0);
    }

}   // End of TextLine.cs
}   // End of namespace PDFjet.NET
