/**
 *  Point.cs
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
using System.Collections.Generic;


namespace PDFjet.NET {
/**
 *  Used to create point objects with different shapes and draw them on a page.
 *  Please note: When we are mentioning (x, y) coordinates of a point - we are talking about the coordinates of the center of the point.
 *
 *  Please see Example_05.
 */
public class Point : IDrawable {

    public static int INVISIBLE = -1;
    public static int CIRCLE = 0;
    public static int DIAMOND = 1;
    public static int BOX = 2;
    public static int PLUS = 3;
    public static int H_DASH = 4;
    public static int V_DASH = 5;
    public static int MULTIPLY = 6;
    public static int STAR = 7;
    public static int X_MARK = 8;
    public static int UP_ARROW = 9;
    public static int DOWN_ARROW = 10;
    public static int LEFT_ARROW = 11;
    public static int RIGHT_ARROW = 12;

    public static bool CONTROL_POINT = true;

    internal float x;
    internal float y;
    internal float r = 2f;

    internal int shape = CIRCLE;
    internal int color = Color.black;
    internal float lineWidth = 0.3f;
    internal String linePattern = "[] 0";
    internal bool fillShape = false;

    internal bool isControlPoint = false;
    internal int align = Align.RIGHT;

    private String text;
    private String uri;

    // Don't draw a line from the previous point to this point.
    private bool drawLineTo = false;

    private float box_x;
    private float box_y;


    /**
     *  The default constructor.
     *
     */
    public Point() {
    }


    /**
     *  Constructor for creating point objects.
     *
     *  @param x the x coordinate of this point when drawn on the page.
     *  @param y the y coordinate of this point when drawn on the page.
     */
    public Point(double x, double y) : this((float) x, (float) y) {
    }


    /**
     *  Constructor for creating point objects.
     *
     *  @param x the x coordinate of this point when drawn on the page.
     *  @param y the y coordinate of this point when drawn on the page.
     */
    public Point(float x, float y) {
        this.x = x;
        this.y = y;
    }


    /**
     *  Constructor for creating point objects.
     *
     *  @param x the x coordinate of this point when drawn on the page.
     *  @param y the y coordinate of this point when drawn on the page.
     *  @param isControlPoint true if this point is one of the points specifying a curve.
     */
    public Point(double x, double y, bool isControlPoint) : this((float) x, (float) y, isControlPoint) {
    }


    /**
     *  Constructor for creating point objects.
     *
     *  @param x the x coordinate of this point when drawn on the page.
     *  @param y the y coordinate of this point when drawn on the page.
     *  @param isControlPoint true if this point is one of the points specifying a curve.
     */
    public Point(float x, float y, bool isControlPoint) {
        this.x = x;
        this.y = y;
        this.isControlPoint = isControlPoint;
    }


    /**
     *  Sets the position (x, y) of this point.
     *
     *  @param x the x coordinate of this point when drawn on the page.
     *  @param y the y coordinate of this point when drawn on the page.
     */
    public void SetPosition(double x, double y) {
        SetPosition((float) x, (float) y);
    }


    /**
     *  Sets the position (x, y) of this point.
     *
     *  @param x the x coordinate of this point when drawn on the page.
     *  @param y the y coordinate of this point when drawn on the page.
     */
    public void SetPosition(float x, float y) {
        SetLocation(x, y);
    }


    /**
     *  Sets the location (x, y) of this point.
     *
     *  @param x the x coordinate of this point when drawn on the page.
     *  @param y the y coordinate of this point when drawn on the page.
     */
    public void SetLocation(float x, float y) {
        this.x = x;
        this.y = y;
    }


    /**
     *  Sets the x coordinate of this point.
     *
     *  @param x the x coordinate of this point when drawn on the page.
     */
    public void SetX(double x) {
        this.x = (float) x;
    }


    /**
     *  Sets the x coordinate of this point.
     *
     *  @param x the x coordinate of this point when drawn on the page.
     */
    public void SetX(float x) {
        this.x = x;
    }


    /**
     *  Returns the x coordinate of this point.
     *
     *  @return the x coordinate of this point.
     */
    public float GetX() {
        return x;
    }


    /**
     *  Sets the y coordinate of this point.
     *
     *  @param y the y coordinate of this point when drawn on the page.
     */
    public void SetY(double y) {
        this.y = (float) y;
    }


    /**
     *  Sets the y coordinate of this point.
     *
     *  @param y the y coordinate of this point when drawn on the page.
     */
    public void SetY(float y) {
        this.y = y;
    }


    /**
     *  Returns the y coordinate of this point.
     *
     *  @return the y coordinate of this point.
     */
    public float GetY() {
        return y;
    }


    /**
     *  Sets the radius of this point.
     *
     *  @param r the radius.
     */
    public void SetRadius(double r) {
        this.r = (float) r;
    }


    /**
     *  Sets the radius of this point.
     *
     *  @param r the radius.
     */
    public void SetRadius(float r) {
        this.r = r;
    }


    /**
     *  Returns the radius of this point.
     *
     *  @return the radius of this point.
     */
    public float GetRadius() {
        return r;
    }


    /**
     *  Sets the shape of this point.
     *
     *  @param shape the shape of this point. Supported values:
     *  <pre>
     *  Point.INVISIBLE
     *  Point.CIRCLE
     *  Point.DIAMOND
     *  Point.BOX
     *  Point.PLUS
     *  Point.H_DASH
     *  Point.V_DASH
     *  Point.MULTIPLY
     *  Point.STAR
     *  Point.X_MARK
     *  Point.UP_ARROW
     *  Point.DOWN_ARROW
     *  Point.LEFT_ARROW
     *  Point.RIGHT_ARROW
     *  </pre>
     */
    public void SetShape(int shape) {
        this.shape = shape;
    }


    /**
     *  Returns the point shape code value.
     *
     *  @return the shape code value.
     */
    public int GetShape() {
        return shape;
    }


    /**
     *  Sets the private fillShape variable.
     *
     *  @param fill if true - fill the point with the specified brush color.
     */
    public void SetFillShape(bool fill) {
        this.fillShape = fill;
    }


    /**
     *  Returns the value of the fillShape private variable.
     *
     *  @return the value of the private fillShape variable.
     */
    public bool GetFillShape() {
        return fillShape;
    }


    /**
     *  Sets the pen color for this point.
     *
     *  @param color the color specified as an integer.
     */
    public void SetColor(int color) {
        this.color = color;
    }


    /**
     *  Returns the point color as an integer.
     *
     *  @return the color.
     */
    public int GetColor() {
        return color;
    }


    /**
     *  Sets the width of the lines of this point.
     *
     *  @param lineWidth the line width.
     */
    public void SetLineWidth(double lineWidth) {
        this.lineWidth = (float) lineWidth;
    }


    /**
     *  Sets the width of the lines of this point.
     *
     *  @param lineWidth the line width.
     */
    public void SetLineWidth(float lineWidth) {
        this.lineWidth = lineWidth;
    }


    /**
     *  Returns the width of the lines used to draw this point.
     *
     *  @return the width of the lines used to draw this point.
     */
    public float GetLineWidth() {
        return lineWidth;
    }


    /**
     *
     *  The line dash pattern controls the pattern of dashes and gaps used to stroke paths.
     *  It is specified by a dash array and a dash phase.
     *  The elements of the dash array are positive numbers that specify the lengths of
     *  alternating dashes and gaps.
     *  The dash phase specifies the distance into the dash pattern at which to start the dash.
     *  The elements of both the dash array and the dash phase are expressed in user space units.
     *  <pre>
     *  Examples of line dash patterns:
     *
     *      "[Array] Phase"     Appearance          Description
     *      _______________     _________________   ____________________________________
     *
     *      "[] 0"              -----------------   Solid line
     *      "[3] 0"             ---   ---   ---     3 units on, 3 units off, ...
     *      "[2] 1"             -  --  --  --  --   1 on, 2 off, 2 on, 2 off, ...
     *      "[2 1] 0"           -- -- -- -- -- --   2 on, 1 off, 2 on, 1 off, ...
     *      "[3 5] 6"             ---     ---       2 off, 3 on, 5 off, 3 on, 5 off, ...
     *      "[2 3] 11"          -   --   --   --    1 on, 3 off, 2 on, 3 off, 2 on, ...
     *  </pre>
     *
     *  @param linePattern the line dash pattern.
     */
    public void SetLinePattern(String linePattern) {
        this.linePattern = linePattern;
    }


    /**
     *  Returns the line dash pattern.
     *
     *  @return the line dash pattern.
     */
    public String GetLinePattern() {
        return linePattern;
    }


    /**
     *  Sets the private drawLineTo variable.
     *  If drawLineTo is true a line is drawn from the last pen position to this point.
     *
     *  @param drawLineTo the drawLineTo bool value.
     */
    public void SetDrawLineTo(bool drawLineTo) {
        this.drawLineTo = drawLineTo;
    }


    /**
     *  Returns the current value of the private drawLineTo variable.
     */
    public bool GetDrawLineTo() {
        return drawLineTo;
    }


    /**
     *  Sets the URI for the "click point" action.
     *
     *  @param uri the URI
     */
    public void SetURIAction(String uri) {
        this.uri = uri;
    }


    /**
     *  Returns the URI for the "click point" action.
     *
     *  @return the URI for the "click point" action.
     */
    public String GetURIAction() {
        return uri;
    }


    /**
     *  Sets the point text.
     *
     *  @param text the text.
     */
    public void SetText(String text) {
        this.text = text;
    }


    /**
     *  Returns the text associated with this point.
     *
     *  @return the text.
     */
    public String GetText() {
        return text;
    }


    /**
     *  Sets the point alignment.
     *
     *  @param align the alignment value.
     */
    public void SetAlignment(int align) {
        this.align = align;
    }


    /**
     *  Returns the point alignment.
     *
     *  @return align the alignment value.
     */
    public int GetAlignment() {
        return this.align;
    }


    /**
     *  Places this point in the specified box at position (0f, 0f).
     *
     *  @param box the specified box.
     */
    public void PlaceIn(Box box) {
        PlaceIn(box, 0f, 0f);
    }


    /**
     *  Places this point in the specified box.
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
     *  Places this point in the specified box.
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
     *  Draws this point on the specified page.
     *
     *  @param page the page to draw this point on.
     */
    public void DrawOn(Page page) {
        page.SetPenWidth(lineWidth);
        page.SetLinePattern(linePattern);

        if (fillShape) {
            page.SetBrushColor(color);
        }
        else {
            page.SetPenColor(color);
        }

        x += box_x;
        y += box_y;
        page.DrawPoint(this);
        x -= box_x;
        y -= box_y;
    }

}   // End of Point.cs
}   // End of namespace PDFjet.NET
