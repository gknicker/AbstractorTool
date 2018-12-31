/**
 *  CheckBox.cs
 *
Copyright (c) 2013, Innovatics Inc.
All rights reserved.

Portions provided by Shirley C. Christenson
Shirley Christenson Consulting

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
 *  Create a CheckBox, which can be set checked or unchecked.
 *  Default is checked, with a blue check mark.
 *  Default box is black, default size is 14.0.
 */
public class CheckBox {

    private bool boxChecked = true;
    private float x;
    private float y;
    private float w = 12.0f;
    private float h = 12.0f;
    private int checkColor = Color.blue;
    private int boxColor = Color.black;
    private float penWidth = 0.3f;
    private float checkWidth = 3.0f;
    private int mark = 1;
    private Font font = null;
    private String text = null;


    /**
     *  Creates a CheckBox with blue check mark.
     *
     */
    public CheckBox() {
    }


    public CheckBox(Font font, String text) {
        this.font = font;
        this.text = text;
        this.boxChecked = false;
    }


    /**
     *  Creates a CheckBox.
     *
     *  @param boxChecked boolean - true or false. Default is true. 
     *  @param checkColor int - color of the check mark. Default is blue.
     */
    public CheckBox(bool boxChecked, int checkColor) {
        this.boxChecked = boxChecked;
        this.checkColor = checkColor;
    }


    /**
     *  Creates a CheckBox.
     *
     *  @param boxChecked boolean - If true box is checked. If false no check mark.
     *  Use default green check mark. 
     */
    public CheckBox(bool boxChecked) {
        this.boxChecked = boxChecked;
    }


    /**
     *  Sets the color of the check mark.
     *
     *  @param checkColor the check mark color specified as an 0xRRGGBB integer.
     */
    public void SetCheckColor(int checkColor) {
        this.checkColor = checkColor;
    }


    /**
     *  Sets the color of the check box.
     *
     *  @param boxColor the check box color specified as an 0xRRGGBB integer.
     */
    public void SetBoxColor(int boxColor) {
        this.boxColor = boxColor;
    }


    /**
     *  Set the x,y position on the Page.
     *
     *  @param x the x coordinate on the Page.
     *  @param y the y coordinate on the Page.
     */
    public void SetPosition(double x, double y) {
        SetPosition((float) x, (float) y);
    }


    /**
     *  Set the x,y position on the Page.
     *
     *  @param x the x coordinate on the Page.
     *  @param y the y coordinate on the Page.
     */
    public void SetPosition(float x, float y) {
        SetLocation(x, y);
    }


    /**
     *  Sets the x,y location on the Page.
     *
     *  @param x the x coordinate on the Page.
     *  @param y the y coordinate on the Page.
     */
    public void SetLocation(float x, float y) {
        this.x = x;
        this.y = y;
    }


    /**
     *  Set the size of the CheckBox.
     *
     *  @param size size of the CheckBox.
     */
    public void SetSize(double size) {
        SetSize((float) size);
    }


    /**
     *  Set the size of the CheckBox.
     *
     *  @param size size of the CheckBox.
     */
    public void SetSize(float size) {
        this.h = size;
        this.w = size;
        this.checkWidth = size / 4.0f;
        this.penWidth = size / 40.0f;
    }


    /**
     *  Sets the type of check mark.
     *
     *  @param mark int - The type of check mark.
     *  1 = check (the default),
     *  2 = X
     *  
     */
    public void SetMarkType(int mark) {
    	if (mark > 0 && mark < 3) {
    		this.mark = mark;
    	}
    }


    /**
     *  Gets the height of the CheckBox.
     *
     */
    public float GetHeight() {
        return this.h;
    }


    /**
     *  Gets the width of the CheckBox.
     *
     */
    public float GetWidth() {
        return this.w;
    }


    /**
     *  Get the x coordinate of the upper left corner.
     *
     */
    public float GetXPosition() {
        return this.x;
    }


    /**
     *  Get the y coordinate of the upper left corner.
     *
     */
    public float GetYPosition() {
        return this.y;
    }


    public void SetChecked(bool boxChecked) {
        this.boxChecked = boxChecked;
    }


    /**
     *  Draws this CheckBox on the specified Page.
     *
     *  @param page the Page where the CheckBox is to be drawn.
     */
    public void DrawOn(Page page) {
        page.SetPenWidth(penWidth);
        page.MoveTo(x, y);
        page.LineTo(x + w, y);
        page.LineTo(x + w, y + h);
        page.LineTo(x, y + h);
        page.ClosePath();
        page.SetPenColor(boxColor);
        page.StrokePath();
        
        if (this.boxChecked) {
        	page.SetPenWidth(checkWidth);
        	if (mark == 1) {
        		page.MoveTo(x + checkWidth/2, y + h/2);
        		page.LineTo(x + w/3, (y + h) - checkWidth/2);
        		page.LineTo((x + w) - checkWidth/2, y + checkWidth/2);
        	}
        	else {
        		page.MoveTo(x + checkWidth/2, y + checkWidth/2);
        		page.LineTo((x + w) - checkWidth/2, (y + h) - checkWidth/2);
        		page.MoveTo((x + w) - checkWidth/2, y + checkWidth/2);
        		page.LineTo(x + checkWidth/2, (y + h) - checkWidth/2);
        	}
        	page.SetPenColor(checkColor);
        	page.SetLineCapStyle(Cap.ROUND);
        	page.StrokePath();
        }

        if (font != null && text != null) {
            page.DrawString(font, text, x + 5f*w/4f, y + h); 
        }

        page.SetPenWidth(0f);
        page.SetPenColor(Color.black);
    }

}   // End of CheckBox.cs
}   // End of namespace PDFjet.NET
