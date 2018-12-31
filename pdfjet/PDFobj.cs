/**
 *  PDFobj.cs
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
 *  Used to create Java or .NET objects that represent the objects in PDF document. 
 *  See the PDF specification for more information.
 *
 */
public class PDFobj {

    public static String TYPE = "/Type";
    public static String SUBTYPE = "/Subtype";
    public static String FILTER = "/Filter";
    public static String WIDTH = "/Width";
    public static String HEIGHT = "/Height";
    public static String COLORSPACE = "/ColorSpace";
    public static String BITSPERCOMPONENT = "/BitsPerComponent";

    internal int offset;           // The object offset
    internal int number;           // The object number
    internal List<String> dict;
    internal int stream_offset;
    internal byte[] stream;        // The compressed stream
    internal byte[] data;          // The decompressed data


    /**
     *  Used to create Java or .NET objects that represent the objects in PDF document. 
     *  See the PDF specification for more information.
     *  Also see Example_19.
     *
     *  @param offset the object offset in the offsets table.
     */
    public PDFobj(int offset) {
        this.offset = offset;
        this.dict = new List<String>();
    }


    /**
     *  Sets this PDF object's stream.
     *
     */
    public void SetStream(byte[] pdf, int length) {
        stream = new byte[length];
        Array.Copy(pdf, this.stream_offset, stream, 0, length);
    }


    public byte[] GetData() {
        return this.data;
    }


    /**
     *  Returns the parameter value given the specified key.
     *
     *  @param key the specified key.
     *
     *  @return the value.
     */
    public String GetValue(String key) {
        for (int i = 0; i < dict.Count; i++) {
            String token = dict[i];
            if (token.Equals(key)) {
                return dict[i + 1];
            }
        }
        return "";
    }


    public float[] GetPageSize() {
        for (int i = 0; i < dict.Count; i++) {
            if (dict[i].Equals("/MediaBox")) {
                return new float[] {
                        Convert.ToSingle(dict[i + 4]),
                        Convert.ToSingle(dict[i + 5]) };
            }
        }
        return Letter.PORTRAIT;
    }


    /**
     *
     *
     *
     */
    internal int GetLength(List<PDFobj> objects) {
        for (int i = 0; i < dict.Count; i++) {
            String token = dict[i];
            if (token.Equals("/Length")) {
                int number = Int32.Parse(dict[i + 1]);
                if (dict[i + 2].Equals("0") &&
                        dict[i + 3].Equals("R")) {
                    return GetLength(objects, number);
                }
                else {
                    return number;
                }
            }
        }
        return 0;
    }


    /**
     *
     *
     *
     */
    internal int GetLength(List<PDFobj> objects, int number) {
        for (int i = 0; i < objects.Count; i++) {
            PDFobj obj = objects[i];
            int objNumber = Int32.Parse(obj.dict[0]);
            if (objNumber == number) {
                return Int32.Parse(obj.dict[3]);
            }
        }
        return 0;
    }

}
}   // End of namespace PDFjet.NET
