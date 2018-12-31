/**
 *  Line.cs
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


namespace PDFjet.NET {
/**
 *  Used to create line objects.
 *
 *  Please see Example_01.
 */
public class Line : IDrawable {

    private float x1;
    private float y1;
    private float x2;
    private float y2;

    private float box_x;
    private float box_y;

    private int color = Color.black;
    private float width = 0.3f;
    private String pattern = "[] 0";
    private int capStyle = 0;


    /**
     *  The default constructor.
     *
     */
    public Line() {
    }


    /**
     *  Create a line object.
     *
     *  @param x1 the x coordinate of the start point.
     *  @param y1 the y coordinate of the start point.
     *  @param x2 the x coordinate of the end point.
     *  @param y2 the y coordinate of the end point.     
     */
    public Line(double x1, double y1, double x2, double y2) : this((float) x1, (float) y1, (float) x2, (float) y2) {
    }


    /**
     *  Create a line object.
     *
     *  @param x1 the x coordinate of the start point.
     *  @param y1 the y coordinate of the start point.
     *  @param x2 the x coordinate of the end point.
     *  @param y2 the y coordinate of the end point.     
     */
    public Line(float x1, float y1, float x2, float y2) {
        this.x1 = x1;
        this.y1 = y1;
        this.x2 = x2;
        this.y2 = y2;
    }


    /**
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
     *  @param pattern the line dash pattern.
     */
    public void SetPattern(String pattern) {
        this.pattern = pattern;
    }


    /**
     *  Sets the x and y coordinates of the start point.
     *
     *  @param x the x coordinate of the start point.
     *  @param y the t coordinate of the start point.
     */
    public void SetStartPoint(double x, double y) {
        this.x1 = (float) x;
        this.y1 = (float) y;
    }


    /**
     *  Sets the x and y coordinates of the start point.
     *
     *  @param x the x coordinate of the start point.
     *  @param y the y coordinate of the start point.
     */
    public void SetStartPoint(float x, float y) {
        this.x1 = x;
        this.y1 = y;
    }


    /**
     *  Sets the x and y coordinates of the start point.
     *
     *  @param x the x coordinate of the start point.
     *  @param y the y coordinate of the start point.
     */
    public void SetPointA(float x, float y) {
        this.x1 = x;
        this.y1 = y;
    }


    /**
     *  Returns the start point of this line.
     *
     *  @return Point the point.
     */
    public Point GetStartPoint() {
        return new Point(x1, y1);
    }


    /**
     *  Sets the x and y coordinates of the end point.
     *
     *  @param x the x coordinate of the end point.
     *  @param y the y coordinate of the end point.
     */
    public void SetEndPoint(double x, double y) {
        this.x2 = (float) x;
        this.y2 = (float) y;
    }


    /**
     *  Sets the x and y coordinates of the end point.
     *
     *  @param x the x coordinate of the end point.
     *  @param y the y coordinate of the end point.
     */
    public void SetEndPoint(float x, float y) {
        this.x2 = x;
        this.y2 = y;
    }


    /**
     *  Sets the x and y coordinates of the end point.
     *
     *  @param x the x coordinate of the end point.
     *  @param y the y coordinate of the end point.
     */
    public void SetPointB(float x, float y) {
        this.x2 = x;
        this.y2 = y;
    }

    
    /**
     *  Returns the end point of this line.
     *
     *  @return Point the point.
     */
    public Point GetEndPoint() {
        return new Point(x2, y2);
    }


    /**
     *  Sets the width of this line.
     *
     *  @param width the width.
     */
    public void SetWidth(double width) {
        this.width = (float) width;
    }


    /**
     *  Sets the width of this line.
     *
     *  @param width the width.
     */
    public void SetWidth(float width) {
        this.width = width;
    }


    /**
     *  Sets the color for this line.
     *
     *  @param color the color specified as an integer.
     */
    public void SetColor(int color) {
        this.color = color;
    }


    /**
     *  Sets the line cap style.
     *
     *  @param style the cap style of the current line. Supported values: Cap.BUTT, Cap.ROUND and Cap.PROJECTING_SQUARE
     */
    public void SetCapStyle(int style) {
        this.capStyle = style;
    }


    /**
     *  Returns the line cap style.
     *
     *  @return the cap style.
     */
    public int getCapStyle() {
        return capStyle;
    }


    /**
     *  Places this line in the specified box at position (0.0f, 0.0f).
     *
     *  @param box the specified box.
     */
    public void PlaceIn(Box box) {
        PlaceIn(box, 0.0f, 0.0f);
    }


    /**
     *  Places this line in the specified box.
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
     *  Places this line in the specified box.
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
     *  Scales this line by the spacified factor.
     *
     *  @param factor the factor used to scale the line.
     */
    public void ScaleBy(double factor) {
        ScaleBy((float) factor);
    }


    /**
     *  Scales this line by the spacified factor.
     *
     *  @param factor the factor used to scale the line.
     */
    public void ScaleBy(float factor) {
        this.x1 *= factor;
        this.x2 *= factor;
        this.y1 *= factor;
        this.y2 *= factor;
    }


    /**
     *  Draws this line on the specified page.
     *
     *  @param page the page to draw this line on.
     */
    public void DrawOn(Page page) {
        page.SetPenColor(color);
        page.SetPenWidth(width);
        page.SetLineCapStyle(capStyle);
        page.SetLinePattern(pattern);
        page.DrawLine(
                x1 + box_x,
                y1 + box_y,
                x2 + box_x,
                y2 + box_y);
    }

}   // End of Line.cs
}   // End of namespace PDFjet.NET
