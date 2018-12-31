/**
 *  PDFImage.cs
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


/**
 * Used to create images from pre-processed raw image data files.
 * Please see Example_24.
 *
 * To create raw PDF image data file you can use the script convert-png-to-raw-image.sh
 * On Windows use convert-png-to-raw-image.cmd
 */
namespace PDFjet.NET {
public class PDFImage {

    Stream stream;
    long size;  // The file size
    int colorComponents;
    int w;      // width
    int h;      // height
    int bitsPerComponent;


    /**
     * Used to construct images from pre-processed raw image data files.
     * 
     * @param path the path to the image file.
     * @throws Exception
     */
    public PDFImage(String path, Stream stream, long size) {
        // The path has the following format:
        // images/mt-map.rbg.640x480x8.raw
        String fileName = path.Substring(path.LastIndexOf("/") + 1);
        String[] tokens = fileName.Split(new Char[] { '.' } );
        String[] dim = tokens[2].Split(new Char[] { 'x' } );
        this.stream = stream;
        this.size = size;
        this.colorComponents = tokens[1].Equals("rgb") ? 3 : 1;
        this.w = Convert.ToInt32(dim[0]);
        this.h = Convert.ToInt32(dim[1]);
        this.bitsPerComponent = Convert.ToInt32(dim[2]);
    }


    internal Stream GetInputStream() {
        return stream;
    }


    internal int GetWidth() {
        return this.w;
    }


    internal int GetHeight() {
        return this.h;
    }


    internal long GetSize() {
        return this.size;
    }


    internal int GetColorComponents() {
        return this.colorComponents;
    }


    internal int GetBitsPerComponent() {
        return this.bitsPerComponent;
    }

}   // End of PDFImage.cs
}   // End of namespace PDFjet.NET
