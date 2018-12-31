/**
 * 
 *  Copyright (c) 2013, Jonas Krogsb�ll

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
public class BMPImage {

    int w = 0;              // Image width in pixels
    int h = 0;              // Image height in pixels

    byte[] image;           // The reconstructed image data
    byte[] deflated;        // The deflated reconstructed image data

    private int bpp;
    private byte[][] palette = null;
    private bool r5g6b5;    // If 16 bit image two encodings can occur

    private const int m10000000 = 0x80;
    private const int m01000000 = 0x40;
    private const int m00100000 = 0x20;
    private const int m00010000 = 0x10;
    private const int m00001000 = 0x08;
    private const int m00000100 = 0x04;
    private const int m00000010 = 0x02;
    private const int m00000001 = 0x01;
    private const int m11110000 = 0xF0;
    private const int m00001111 = 0x0F;

    /* Tested with images created from GIMP */
    public BMPImage( System.IO.Stream stream ) {

        byte[] bm = getBytes(stream, 2);
        // From Wikipedia
        if((bm[0] == 'B' && bm[1] == 'M')||
           (bm[0] == 'B' && bm[1] == 'A')||
           (bm[0] == 'C' && bm[1] == 'I')||
           (bm[0] == 'C' && bm[1] == 'P')||
           (bm[0] == 'I' && bm[1] == 'C')||
           (bm[0] == 'P' && bm[1] == 'T')) {
            skipNBytes(stream, 8);
            int offset = readSignedInt(stream);
            int sizeOfHeader = readSignedInt(stream);
            w = readSignedInt(stream);
            h = readSignedInt(stream);
            skipNBytes(stream, 2);
            bpp = read2BytesLE(stream);
            int compression = readSignedInt(stream);
            if (bpp > 8) {
                r5g6b5 = (compression == 3);
                skipNBytes(stream, 20);
                if (offset > 54) {
                    skipNBytes(stream, offset-54);
                }
            } else {
                skipNBytes(stream, 12);
                int numpalcol = readSignedInt(stream);
                if (numpalcol == 0) {
                    numpalcol = (int) Math.Pow(2, bpp);
                }
                skipNBytes(stream, 4);
                parsePalette(stream, numpalcol);
            }
            parseData(stream);
        } else {
            throw new Exception("BMP data could not be parsed!");
        }

    }

    private void parseData(System.IO.Stream stream) {
        // rowsize is 4 * ceil (bpp*width/32.0)
        image = new byte[w * h * 3];

        int rowsize = 4 * (int)Math.Ceiling(bpp*w/32.0);// 4 byte alignment
        // hiv hver r�kke ud:
        byte[] row;
        int index;
        try {
            for (int i = 0; i < h; i++) {
                row = getBytes(stream, rowsize);
                switch (bpp) {
                case  1: row = bit1to8(row, w); break;      // opslag i palette
                case  4: row = bit4to8(row, w); break;      // opslag i palette
                case  8: break;                             // opslag i palette
                case 16: 
                    if(r5g6b5)
                        row = bit16to24(row, w);            // 5,6,5 bit
                    else 
                        row = bit16to24b(row, w);
                    break;
                case 24: break;                             // bytes are correct
                case 32: row = bit32to24(row, w); break;
                default:
                    throw new Exception(
                            "Can only parse 1 bit, 4bit, 8bit, 16bit, 24bit and 32bit images");
                }

                index = w*(h-i-1)*3;
                if (palette != null) {  // indexed
                    for (int j = 0; j < w; j++) {
                        image[index++] = palette[(row[j]<0)?row[j]+256:row[j]][2];
                        image[index++] = palette[(row[j]<0)?row[j]+256:row[j]][1];
                        image[index++] = palette[(row[j]<0)?row[j]+256:row[j]][0];
                    }
                } else {                // not indexed
                    for (int j = 0; j < w*3; j+=3) {
                        image[index++] = row[j+2];
                        image[index++] = row[j+1];
                        image[index++] = row[j];
                    }
                }
            }
        } catch (Exception e) {
            throw new Exception(e.ToString() +
                    " : BMP parse error: imagedata not correct");
        }

        MemoryStream data2 = new MemoryStream(32768);
        DeflaterOutputStream dos = new DeflaterOutputStream(data2);
        dos.Write(image, 0, image.Length);
        dos.Finish();
        deflated = data2.ToArray();
    }

    // 5 + 6 + 5 in B G R format 2 bytes to 3 bytes
    private static byte[] bit16to24(byte[] row, int width) {
        byte[] ret = new byte[width * 3];
        int j = 0;
        for (int i = 0; i < width*2; i+=2) {
            // Console.WriteLine("B1: " + row[i] + ", B2: " + row[i+1]);
            ret[j++] = (byte)((row[i] & 0x1F)<<3);
            ret[j++] = (byte)(((row[i+1] & 0x07)<<5)+((row[i] & 0xE0)>>3));
            ret[j++] = (byte)((row[i+1] & 0xF8));
            // Console.WriteLine("green: " + ret[j-1]);
        }
        return ret;
    }

    // 5 + 5 + 5 in B G R format 2 bytes to 3 bytes
    private static byte[] bit16to24b(byte[] row, int width) {
        byte[] ret = new byte[width * 3];
        int j = 0;
        for (int i = 0; i < width*2; i+=2) {
            // Console.WriteLine("B1: " + row[i] + ", B2: " + row[i+1]);
            ret[j++] = (byte)((row[i] & 0x1F)<<3);
            ret[j++] = (byte)(((row[i+1] & 0x03)<<6)+((row[i] & 0xE0)>>2));
            ret[j++] = (byte)((row[i+1] & 0x7C)<<1);
            // Console.WriteLine("green: " + ret[j-1]);
        }
        return ret;
    }

    /* alpha first? */
    private static byte[] bit32to24(byte[] row, int width) {
        byte[] ret = new byte[width * 3];
        int j = 0;
        for (int i = 0; i < width*4; i+=4) {
            ret[j++] = row[i+1];
            ret[j++] = row[i+2];
            ret[j++] = row[i+3]; 
        }
        return ret;
    }

    private static byte[] bit4to8(byte[] row, int width) {
        byte[] ret = new byte[width];
        for (int i = 0; i < width; i++) {
            if(i % 2 == 0) {
                ret[i] =(byte) ((row[i/2] & m11110000)>>4);
            } else {
                ret[i] =(byte) ((row[i/2] & m00001111));
            }
        }
        return ret;
    }

    private static byte[] bit1to8(byte[] row, int width) {
        byte[] ret = new byte[width];
        for (int i = 0; i < width; i++) {
            switch (i % 8) {
            case 0: ret[i] = (byte) ((row[i/8] & m10000000)>>7); break;
            case 1: ret[i] = (byte) ((row[i/8] & m01000000)>>6); break;
            case 2: ret[i] = (byte) ((row[i/8] & m00100000)>>5); break;
            case 3: ret[i] = (byte) ((row[i/8] & m00010000)>>4); break;
            case 4: ret[i] = (byte) ((row[i/8] & m00001000)>>3); break;
            case 5: ret[i] = (byte) ((row[i/8] & m00000100)>>2); break;
            case 6: ret[i] = (byte) ((row[i/8] & m00000010)>>1); break;
            case 7: ret[i] = (byte) ((row[i/8] & m00000001)); break;
            }
        }
        return ret;
    }

    private void parsePalette(System.IO.Stream stream, int size) {
        palette = new byte[size][];
        for (int i = 0; i < size; i++) {
            palette[i] = getBytes(stream, 4);
        }
    }

    private void skipNBytes(System.IO.Stream inputStream, int n) {
        getBytes(inputStream, n);
    }

    private byte[] getBytes(System.IO.Stream inputStream, int length) {
        byte[] buf = new byte[length];
        inputStream.Read(buf, 0, buf.Length);
        return buf;
    }
 
    private int read2BytesLE(System.IO.Stream inputStream) {
        byte[] buf = getBytes(inputStream, 2);
        int val = 0;
        val |= buf[ 1 ] & 0xff;
        val <<= 8;
        val |= buf[ 0 ] & 0xff;
        return val;
    }

    private int readSignedInt(System.IO.Stream inputStream) {
        byte[] buf = getBytes(inputStream, 4);
        long val = 0L;
        val |= (uint) buf[ 3 ] & 0xff;
        val <<= 8;
        val |= (uint) buf[ 2 ] & 0xff;
        val <<= 8;
        val |= (uint) buf[ 1 ] & 0xff;
        val <<= 8;
        val |= (uint) buf[ 0 ] & 0xff;
        return (int)val;
    }

    public int GetWidth() {
        return this.w;
    }

    public int GetHeight() {
        return this.h;
    }

    public byte[] GetData() {
        return this.deflated;
    }

}
}   // End of namespace PDFjet.NET
