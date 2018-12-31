/**
 *  Image.cs
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
using System.IO;
using System.Collections.Generic;
using System.IO.Compression;


namespace PDFjet.NET {
/**
 *  Used to create image objects and draw them on a page.
 *  The image type can be one of the following: ImageType.JPG, ImageType.PNG, ImageType.BMP or ImageType.PDF.
 *
 *  Please see Example_03.
 */
public class Image : IDrawable {

    internal int objNumber;

    internal float x;   // Position of the image on the page
    internal float y;
    internal float w;   // Image width
    internal float h;   // Image height

    internal long size; // The image file size
    
    internal String uri;
    internal String key;

    private float box_x;
    private float box_y;

    private bool rotate90 = false;


    /**
     *  Use of this constructor will result in reduced memory consumption and faster processing, however it currently only supports JPG images.
     *  Please see Example_24
     */
    public Image(PDF pdf, JPGImage jpg) {
        this.w = jpg.GetWidth();
        this.h = jpg.GetHeight();
        this.size = jpg.GetFileSize();
        Stream stream = jpg.GetInputStream();
        if (jpg.GetColorComponents() == 1) {
            AddImage(pdf, stream, ImageType.JPG, w, h, size, "DeviceGray", 8);
        }
        else if (jpg.GetColorComponents() == 3) {
            AddImage(pdf, stream, ImageType.JPG, w, h, size, "DeviceRGB", 8);
        }
        else if (jpg.GetColorComponents() == 4) {
            AddImage(pdf, stream, ImageType.JPG, w, h, size, "DeviceCMYK", 8);
        }
        stream.Dispose();
    }


    /**
     *  Use of this constructor will result in reduced memory consumption and faster processing.
     *  Please see Example_24
     */
    public Image(PDF pdf, PDFImage raw) {
        this.w = raw.GetWidth();
        this.h = raw.GetHeight();
        this.size = raw.GetSize();
        Stream stream = raw.GetInputStream();
        if (raw.GetColorComponents() == 1) {
            AddImage(pdf, stream, ImageType.PDF, w, h, size, "DeviceGray", 8);
        }
        else {
            AddImage(pdf, stream, ImageType.PDF, w, h, size, "DeviceRGB", 8);
        }
        stream.Dispose();
    }


    /**
     *  The main constructor for the Image class.
     *
     *  @param pdf the page to draw this image on.
     *  @param inputStream the input stream to read the image from.
     *  @param imageType ImageType.JPG, ImageType.PNG or ImageType.BMP.
     *
     */
    public Image(PDF pdf, Stream inputStream, int imageType) {
        byte[] data;
        if (imageType == ImageType.JPG) {
            JPGImage jpg = new JPGImage(inputStream);
            data = jpg.GetData();
            w = jpg.GetWidth();
            h = jpg.GetHeight();
            if (jpg.GetColorComponents() == 1) {
                AddImage(pdf, data, null, imageType, "DeviceGray", 8);
            }
            else if (jpg.GetColorComponents() == 3) {
                AddImage(pdf, data, null, imageType, "DeviceRGB", 8);
            }
            else if (jpg.GetColorComponents() == 4) {
                AddImage(pdf, data, null, imageType, "DeviceCMYK", 8);
            }
        }
        else if (imageType == ImageType.PNG) {
            PNGImage png = new PNGImage(inputStream);
            data = png.GetData();
            w = png.GetWidth();
            h = png.GetHeight();
            if (png.colorType == 0) {
                AddImage(pdf, data, null, imageType, "DeviceGray", png.bitDepth);
            }
            else {
                if (png.bitDepth == 16) {
                    AddImage(pdf, data, null, imageType, "DeviceRGB", 16);
                }
                else {
                    AddImage(pdf, data, png.GetAlpha(), imageType, "DeviceRGB", 8);
                }
            }
        }
        else if (imageType == ImageType.BMP) {
            BMPImage bmp = new BMPImage(inputStream);
            data = bmp.GetData();
            w = bmp.GetWidth();
            h = bmp.GetHeight();
            AddImage(pdf, data, null, imageType, "DeviceRGB", 8);
        }

        inputStream.Dispose();
    }


    /**
     *  Sets the position of this image on the page to (x, y).
     *
     *  @param x the x coordinate of the top left corner of the image.
     *  @param y the y coordinate of the top left corner of the image.
     */
    public void SetPosition(double x, double y) {
        SetPosition((float) x, (float) y);
    }


    /**
     *  Sets the position of this image on the page to (x, y).
     *
     *  @param x the x coordinate of the top left corner of the image.
     *  @param y the y coordinate of the top left corner of the image.
     */
    public void SetPosition(float x, float y) {
        SetLocation(x, y);
    }


    /**
     *  Sets the location of this image on the page to (x, y).
     *
     *  @param x the x coordinate of the top left corner of the image.
     *  @param y the y coordinate of the top left corner of the image.
     */
    public void SetLocation(float x, float y) {
        this.x = x;
        this.y = y;
    }


    /**
     *  Scales this image by the specified factor.
     *
     *  @param factor the factor used to scale the image.
     */
    public void ScaleBy(double factor) {
     	this.ScaleBy((float) factor, (float) factor);
    }


    /**
     *  Scales this image by the specified factor.
     *
     *  @param factor the factor used to scale the image.
     */
    public void ScaleBy(float factor) {
     	this.ScaleBy(factor, factor);
    }


    /**
     *  Scales this image by the specified width and height factor.
     *  <p><i>Author:</i> <strong>Pieter Libin</strong>, pieter@emweb.be</p>
     *
     *  @param widthFactor the factor used to scale the width of the image
     *  @param heightFactor the factor used to scale the height of the image
     */
    public void ScaleBy(float widthFactor, float heightFactor) {
        this.w *= widthFactor;
        this.h *= heightFactor;
    }


    /**
     *  Places this image in the specified box.
     *
     *  @param box the specified box.
     */
    public void PlaceIn(Box box) {
        box_x = box.x;
        box_y = box.y;
    }
    

    /**
     *  Sets the URI for the "click box" action.
     *
     *  @param uri the URI
     */
    public void SetURIAction(String uri) {
        this.uri = uri;
    }


    /**
     *  Sets the destination key for the action.
     *
     *  @param key the destination name.
     */
    public void SetGoToAction(String key) {
        this.key = key;
    }


    /**
     *  Sets the rotate90 flag.
     *  When the flag is true the image is rotated 90 degrees clockwise.
     *
     *  @param rotate90 the flag.
     */
    public void SetRotateCW90(bool rotate90) {
        this.rotate90 = rotate90;
    }


    /**
     *  Draws this image on the specified page.
     *
     *  @param page the page to draw this image on.
     */
    public void DrawOn(Page page) {
        x += box_x;
        y += box_y;
        page.Append("q\n");

        if (rotate90) {
            page.Append(h);
            page.Append(' ');
            page.Append(0f);
            page.Append(' ');
            page.Append(0f);
            page.Append(' ');
            page.Append(w);
            page.Append(' ');
            page.Append(x);
            page.Append(' ');
            page.Append(page.height - y);
            page.Append(" cm\n");

            // Rotate the image 2x45 degrees clockwise. The magic number is Math.sqrt(0.5):
            page.Append("0.7071067811 -0.7071067811 0.7071067811 0.7071067811 0.0 0.0 cm\n");
            page.Append("0.7071067811 -0.7071067811 0.7071067811 0.7071067811 0.0 0.0 cm\n");
        }
        else {
            page.Append(w);
            page.Append(' ');
            page.Append(0f);
            page.Append(' ');
            page.Append(0f);
            page.Append(' ');
            page.Append(h);
            page.Append(' ');
            page.Append(x);
            page.Append(' ');
            page.Append(page.height - (y + h));
            page.Append(" cm\n");
        }

        page.Append("/Im");
        page.Append(objNumber);
        page.Append(" Do\n");
        page.Append("Q\n");

        if (uri != null || key != null) {
            page.annots.Add(new Annotation(
                    uri,
                    key,    // The destination name
                    x,
                    page.height - y,
                    x + w,
                    page.height - (y + h)));
        }
    }


    /**
     *  Use this constructor to create an image from data found in PDFobj.
     *
     *  @param pdf the PDF we want to add this image to.
     *  @param obj the PDFobj containing the image data.
     */
    public Image(PDF pdf, PDFobj obj) {
        pdf.Newobj();
        pdf.Append("<<\n");
        pdf.Append("/Type /XObject\n");
        pdf.Append("/Subtype /Image\n");
        pdf.Append("/Filter ");
        pdf.Append(obj.GetValue(PDFobj.FILTER));
        pdf.Append('\n');
        pdf.Append("/Width ");
        pdf.Append(obj.GetValue(PDFobj.WIDTH));
        pdf.Append('\n');
        pdf.Append("/Height ");
        pdf.Append(obj.GetValue(PDFobj.HEIGHT));
        pdf.Append('\n');
        pdf.Append("/ColorSpace ");
        pdf.Append(obj.GetValue(PDFobj.COLORSPACE));
        pdf.Append('\n');
        pdf.Append("/BitsPerComponent ");
        pdf.Append(obj.GetValue(PDFobj.BITSPERCOMPONENT));
        pdf.Append('\n');
        pdf.Append("/Length ");
        pdf.Append(obj.stream.Length);
        pdf.Append('\n');
        pdf.Append(">>\n");
        pdf.Append("stream\n");
        pdf.Append(obj.stream, 0, obj.stream.Length);
        pdf.Append("\nendstream\n");
        pdf.Endobj();
        pdf.images.Add(this);
        objNumber = pdf.objNumber;
    }


    /**
     *  Returns the width of this image when drawn on the page.
     *  The scaling is take into account.
     *
     *  @return w - the width of this image.
     */
    public float GetWidth() {
        return this.w;
    }


    /**
     *  Returns the height of this image when drawn on the page.
     *  The scaling is take into account.
     *
     *  @return h - the height of this image.
     */
    public float GetHeight() {
        return this.h;
    }


    private void AddSoftMask(
            PDF pdf,
            byte[] data,
            String colorSpace,
            int bitsPerComponent) {
        pdf.Newobj();
        pdf.Append("<<\n");
        pdf.Append("/Type /XObject\n");
        pdf.Append("/Subtype /Image\n");
        pdf.Append("/Filter /FlateDecode\n");
        pdf.Append("/Width ");
        pdf.Append(( int ) w);
        pdf.Append('\n');
        pdf.Append("/Height ");
        pdf.Append(( int ) h);
        pdf.Append('\n');
        pdf.Append("/ColorSpace /");
        pdf.Append(colorSpace);
        pdf.Append('\n');
        pdf.Append("/BitsPerComponent ");
        pdf.Append(bitsPerComponent);
        pdf.Append('\n');
        pdf.Append("/Length ");
        pdf.Append(data.Length);
        pdf.Append('\n');
        pdf.Append(">>\n");
        pdf.Append("stream\n");
        pdf.Append(data, 0, data.Length);
        pdf.Append("\nendstream\n");
        pdf.Endobj();
        objNumber = pdf.objNumber;
    }


    private void AddImage(
            PDF pdf,
            byte[] data,
            byte[] alpha,
            int imageType,
            String colorSpace,
            int bitsPerComponent) {
        if (alpha != null) {
            AddSoftMask(pdf, alpha, "DeviceGray", 8);
        }
        pdf.Newobj();
        pdf.Append("<<\n");
        pdf.Append("/Type /XObject\n");
        pdf.Append("/Subtype /Image\n");
        if (imageType == ImageType.JPG) {
            pdf.Append("/Filter /DCTDecode\n");
        }
        else if (imageType == ImageType.PNG || imageType == ImageType.BMP) {
            pdf.Append("/Filter /FlateDecode\n");
            if (alpha != null) {
                pdf.Append("/SMask ");
                pdf.Append(objNumber);
                pdf.Append(" 0 R\n");
            }
        }
        pdf.Append("/Width ");
        pdf.Append(( int ) w);
        pdf.Append('\n');
        pdf.Append("/Height ");
        pdf.Append(( int ) h);
        pdf.Append('\n');
        pdf.Append("/ColorSpace /");
        pdf.Append(colorSpace);
        pdf.Append('\n');
        pdf.Append("/BitsPerComponent ");
        pdf.Append(bitsPerComponent);
        pdf.Append('\n');
        if (colorSpace.Equals("DeviceCMYK")) {
            // If the image was created with Photoshop - invert the colors:
            pdf.Append("/Decode [1.0 0.0 1.0 0.0 1.0 0.0 1.0 0.0]\n");
        }
        pdf.Append("/Length ");
        pdf.Append(data.Length);
        pdf.Append('\n');
        pdf.Append(">>\n");
        pdf.Append("stream\n");
        pdf.Append(data, 0, data.Length);
        pdf.Append("\nendstream\n");
        pdf.Endobj();
        pdf.images.Add(this);
        objNumber = pdf.objNumber;
    }


    private void AddImage(
            PDF pdf,
            Stream stream,
            int imageType,
            float w,
            float h,
            long length,
            String colorSpace,
            int bitsPerComponent) {
        pdf.Newobj();
        pdf.Append("<<\n");
        pdf.Append("/Type /XObject\n");
        pdf.Append("/Subtype /Image\n");
        if (imageType == ImageType.JPG) {
            pdf.Append("/Filter /DCTDecode\n");
        }
        else if (imageType == ImageType.PDF) {
            pdf.Append("/Filter /FlateDecode\n");
        }
        pdf.Append("/Width ");
        pdf.Append(( int ) w);
        pdf.Append('\n');
        pdf.Append("/Height ");
        pdf.Append(( int ) h);
        pdf.Append('\n');
        pdf.Append("/ColorSpace /");
        pdf.Append(colorSpace);
        pdf.Append('\n');
        pdf.Append("/BitsPerComponent ");
        pdf.Append(bitsPerComponent);
        pdf.Append('\n');
        if (colorSpace.Equals("DeviceCMYK")) {
            // If the image was created with Photoshop - invert the colors:
            pdf.Append("/Decode [1.0 0.0 1.0 0.0 1.0 0.0 1.0 0.0]\n");
        }
        pdf.Append("/Length ");
        pdf.Append((int) length);
        pdf.Append('\n');
        pdf.Append(">>\n");
        pdf.Append("stream\n");
        byte[] buf = new byte[2048];
        int count;
        while ((count = stream.Read(buf, 0, buf.Length)) != 0) {
            pdf.Append(buf, 0, count);
        }
        pdf.Append("\nendstream\n");
        pdf.Endobj();
        pdf.images.Add(this);
        objNumber = pdf.objNumber;
    }
    
}   // End of Image.cs
}   // End of namespace PDFjet.NET
