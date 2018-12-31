/**
 *  PDF.cs
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
using System.Text;
using System.Collections.Generic;
using System.IO.Compression;
using System.Resources;
using System.Reflection;


namespace PDFjet.NET {
/**
 *
 *
 *
 */
public class PDF {

    internal int objNumber = 0;
    internal int metadataObjNumber = 0;
    internal int outputIntentObjNumber = 0;

    internal List<Font> fonts = new List<Font>();
    internal List<Image> images = new List<Image>();
    internal List<Page> pages = new List<Page>();
    internal Dictionary<String, Destination> destinations = new Dictionary<String, Destination>();

    private static int CR_LF = 0;
    private static int CR = 1;
    private static int LF = 2;

    private int compliance = 0;
	private Stream os = null;
    private List<Int32> objOffset = new List<Int32>();
    private List<Int32> contents = new List<Int32>();
    private String producer = "PDFjet v4.95 (http://pdfjet.com)";
    private String creationDate;
    private String createDate;
    private String title = "";
    private String subject = "";
    private String author = "";
    private int byte_count = 0;
    private int endOfLine = CR_LF;

    internal List<OptionalContentGroup> groups = new List<OptionalContentGroup>();


    /**
     * The default constructor - use when reading PDF files.
     * 
     *
     */
    public PDF() {}


    public PDF(Stream os) : this(os, 0) {}


    // Here is the layout of the PDF document:
    //
    // Metadata Object
    // Output Intent Object
    // Fonts
    // Images
    // Resources Object
    // Content1
    // Content2
    // ...
    // ContentN
    // Pages
    // Page1
    // Page2
    // ...
    // PageN
    // Info
    // Root
    // xref table
    // Trailer
    public PDF(Stream os, int compliance) {

        this.os = os;
        this.compliance = compliance;
        DateTime date = new DateTime(DateTime.Now.Ticks);
        SimpleDateFormat sdf1 = new SimpleDateFormat("yyyyMMddHHmmss'Z'");
        SimpleDateFormat sdf2 = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss");
        creationDate = sdf1.Format(date);
        createDate = sdf2.Format(date);

        Append("%PDF-1.4\n");
        Append('%');
        Append((byte) 0x00F2);
        Append((byte) 0x00F3);
        Append((byte) 0x00F4);
        Append((byte) 0x00F5);
        Append((byte) 0x00F6);
        Append('\n');

        if (compliance == Compliance.PDF_A_1B) {
            metadataObjNumber = AddMetadataObject("", true);
            outputIntentObjNumber = AddOutputIntentObject();
        }

    }


    internal void Newobj() {
        objOffset.Add(byte_count);
        Append(++objNumber);
        Append(" 0 obj\n");
    }


    internal void Endobj() {
        Append("endobj\n");
    }


    internal int AddMetadataObject(String notice, bool pad) {

        StringBuilder sb = new StringBuilder();
        sb.Append("<?xpacket begin='\uFEFF' id=\"W5M0MpCehiHzreSzNTczkc9d\"?>\n");
        sb.Append("<x:xmpmeta xmlns:x=\"adobe:ns:meta/\">\n");
        sb.Append("<rdf:RDF xmlns:rdf=\"http://www.w3.org/1999/02/22-rdf-syntax-ns#\">\n");

        sb.Append("<rdf:Description rdf:about=\"\" xmlns:pdf=\"http://ns.adobe.com/pdf/1.3/\" pdf:Producer=\"");
        sb.Append(producer);
        sb.Append("\"></rdf:Description>\n");

        sb.Append("<rdf:Description rdf:about=\"\" xmlns:dc=\"http://purl.org/dc/elements/1.1/\">\n");
        sb.Append("<dc:format>application/pdf</dc:format>\n");
        sb.Append("<dc:title><rdf:Alt><rdf:li xml:lang=\"x-default\">");
        sb.Append(title);
        sb.Append("</rdf:li></rdf:Alt></dc:title>\n");

        sb.Append("<dc:creator><rdf:Seq><rdf:li>");
        sb.Append(author);
        sb.Append("</rdf:li></rdf:Seq></dc:creator>\n");

        sb.Append("<dc:description><rdf:Alt><rdf:li xml:lang=\"en-US\">");
        sb.Append(notice);
        sb.Append("</rdf:li></rdf:Alt></dc:description>\n");

        sb.Append("</rdf:Description>\n");

        sb.Append("<rdf:Description rdf:about=\"\" xmlns:pdfaid=\"http://www.aiim.org/pdfa/ns/id/\">");
        sb.Append("<pdfaid:part>1</pdfaid:part>");
        sb.Append("<pdfaid:conformance>B</pdfaid:conformance>");
        sb.Append("</rdf:Description>");

        sb.Append("<rdf:Description rdf:about=\"\" xmlns:xmp=\"http://ns.adobe.com/xap/1.0/\">\n");
        sb.Append("<xmp:CreateDate>");
        sb.Append(createDate);
        sb.Append("</xmp:CreateDate>\n");
        sb.Append("</rdf:Description>\n");
        sb.Append("</rdf:RDF>\n");
        sb.Append("</x:xmpmeta>\n");

        if (pad) {
            // Add the recommended 2000 bytes padding
            for (int i = 0; i < 20; i++) {
                for (int j = 0; j < 10; j++) {
                    sb.Append("          ");
                }
                sb.Append("\n");
            }
        }

        sb.Append("<?xpacket end=\"w\"?>");

        byte[] xml = (new System.Text.UTF8Encoding()).GetBytes(sb.ToString());

        // This is the metadata object
        Newobj();
        Append("<<\n");
        Append("/Type /Metadata\n");
        Append("/Subtype /XML\n");
        Append("/Length ");
        Append(xml.Length);
        Append("\n");
        Append(">>\n");
        Append("stream\n");
        for (int i = 0; i < xml.Length; i++) {
            Append(xml[i]);
        }
        Append("\nendstream\n");
        Endobj();

        return objNumber;
    }


    private int AddOutputIntentObject() {
/*
        MemoryStream baos = new MemoryStream();
        Assembly assembly = Assembly.GetExecutingAssembly();
        Stream input = assembly.GetManifestResourceStream("sRGB_IEC61966-2-1_black_scaled.icc");
        int ch;
        while ((ch = input.ReadByte()) != -1) {
            baos.WriteByte((byte) ch);
        }
        input.Dispose();
        byte[] sRGB = baos.ToArray();

        MemoryStream baos2 = new MemoryStream();
        DeflaterOutputStream dos = new DeflaterOutputStream(baos2);
        dos.Write(sRGB, 0, sRGB.Length);
        dos.Finish();
*/

        Newobj();
        Append("<<\n");
        Append("/N 3\n");

        Append("/Length ");
        /* Append(baos2.Length); */
        Append(ICCBlackScaled.profile.Length);
        Append("\n");

        Append("/Filter /FlateDecode\n");
        Append(">>\n");
        Append("stream\n");
        /* Append(baos2); */
        Append(ICCBlackScaled.profile, 0, ICCBlackScaled.profile.Length);
        Append("\nendstream\n");
        Endobj();

        // OutputIntent object
        Newobj();
        Append("<<\n");
        Append("/Type /OutputIntent\n");
        Append("/S /GTS_PDFA1\n");
        Append("/OutputCondition (sRGB IEC61966-2.1)\n");
        Append("/OutputConditionIdentifier (sRGB IEC61966-2.1)\n");
        Append("/Info (sRGB IEC61966-2.1)\n");
        Append("/DestOutputProfile ");
        Append(objNumber - 1);
        Append(" 0 R\n");
        Append(">>\n");
        Endobj();

        return objNumber;
    }


    private int AddResourcesObject() {
        Newobj();
        Append("<<\n");

        if (fonts.Count > 0) {
            Append("/Font\n");
            Append("<<\n");
            for (int i = 0; i < fonts.Count; i++) {
                Font font = fonts[i];
                Append("/F");
                Append(font.objNumber);
                Append(' ');
                Append(font.objNumber);
                Append(" 0 R\n");
            }
            Append(">>\n");
        }

        if (images.Count > 0) {
            Append("/XObject\n");
            Append("<<\n");
            for (int i = 0; i < images.Count; i++) {
                Image image = images[i];
                Append("/Im");
                Append(image.objNumber);
                Append(' ');
                Append(image.objNumber);
                Append(" 0 R\n");
            }
            Append(">>\n");
        }

        if (groups.Count > 0) {
            Append("/Properties\n");
            Append("<<\n");
            for (int i = 0; i < groups.Count; i++) {
                OptionalContentGroup ocg = groups[i];
                Append("/OC");
                Append(i + 1);
                Append(' ');
                Append(ocg.objNumber);
                Append(" 0 R\n");
            }
            Append(">>\n");
        }

        Append(">>\n");
        Endobj();
        return objNumber;
    }


    private int AddPagesObject() {
        Newobj();
        Append("<<\n");
        Append("/Type /Pages\n");
        Append("/Kids [ ");
        int pageObjNumber = objNumber + 1;
        for (int i = 0; i < pages.Count; i++) {
            Page page = pages[i];
            page.SetDestinationsPageObjNumber(pageObjNumber);
            Append(pageObjNumber);
            Append(" 0 R ");
            pageObjNumber += (page.annots.Count + 1);
        }
        Append("]\n");
        Append("/Count ");
        Append(pages.Count);
        Append('\n');
        Append(">>\n");
        Endobj();
        return objNumber;
    }


    private int AddInfoObject() {
        // This is the info object
        Newobj();
        Append("<<\n");
        Append("/Title (");
        Append(title);
        Append(")\n");
        Append("/Subject (");
        Append(subject);
        Append(")\n");
        Append("/Author (");
        Append(author);
        Append(")\n");
        Append("/Producer (");
        Append(producer);
        Append(")\n");

        if (compliance != Compliance.PDF_A_1B) {
            Append("/CreationDate (D:");
            Append(creationDate);
            Append(")\n");
        }

        Append(">>\n");
        Endobj();
        return objNumber;
    }


    private void AddPageBox(String boxName, Page page, float[] rect) {
        Append("/");
        Append(boxName);
        Append(" [");
        Append(rect[0]);
        Append(' ');
        Append(page.height - rect[3]);
        Append(' ');
        Append(rect[2]);
        Append(' ');
        Append(page.height - rect[1]);
        Append("]\n");
    }


    private void AddAllPages(int pagesObjNumber, int resObjNumber) {
        for (int i = 0; i < pages.Count; i++) {
            Page page = pages[i];

            // Page object
            Newobj();
            Append("<<\n");
            Append("/Type /Page\n");
            Append("/Parent ");
            Append(pagesObjNumber);
            Append(" 0 R\n");
            Append("/MediaBox [0.0 0.0 ");
            Append(page.width);
            Append(' ');
            Append(page.height);
            Append("]\n");

            if (page.cropBox != null) {
                AddPageBox("CropBox", page, page.cropBox);
            }
            if (page.bleedBox != null) {
                AddPageBox("BleedBox", page, page.bleedBox);
            }
            if (page.trimBox != null) {
                AddPageBox("TrimBox", page, page.trimBox);
            }
            if (page.artBox != null) {
                AddPageBox("ArtBox", page, page.artBox);
            }

            Append("/Resources ");
            Append(resObjNumber);
            Append(" 0 R\n");
            Append("/Contents ");
            Append(contents[i]);
            Append(" 0 R\n");
            if (page.annots.Count > 0) {
                Append("/Annots [ ");
                for (int j = 0; j < page.annots.Count; j++) {
                    Append(objNumber + j + 1);
                    Append(" 0 R ");
                }
                Append("]\n");
            }
            Append(">>\n");
            Endobj();

            AddAnnotDictionaries(page);
        }
    }


	internal void AddPageContent(Page page) {
        MemoryStream baos = new MemoryStream();
		DeflaterOutputStream dos = new DeflaterOutputStream(baos);
		byte[] buf = page.buf.ToArray();
		dos.Write(buf, 0, buf.Length);
		dos.Finish();
        page.buf = null;    // Release the page content memory!

		Newobj();
		Append("<<\n");
		Append("/Filter /FlateDecode\n");
		Append("/Length ");
		Append(baos.Length);
		Append("\n");
		Append(">>\n");
		Append("stream\n");
		Append(baos);
		Append("\nendstream\n");
		Endobj();
        contents.Add(objNumber);
	}


    private void AddAnnotDictionaries(Page page) {
        for (int i = 0; i < page.annots.Count; i++) {
            Annotation annot = page.annots[i];
            Newobj();
            Append("<<\n");
            Append("/Type /Annot\n");
            Append("/Subtype /Link\n");
            Append("/Rect [");
            Append(annot.x1);
            Append(' ');
            Append(annot.y1);
            Append(' ');
            Append(annot.x2);
            Append(' ');
            Append(annot.y2);
            Append("]\n");
            Append("/Border[0 0 0]\n");
            if (annot.uri != null) {
                Append("/F 4\n");
                Append("/A <<\n");
                Append("/S /URI\n");
                Append("/URI (");
                Append(annot.uri);
                Append(")\n");
                Append(">>\n");
            }
            else if (annot.key != null) {
                Destination destination = (Destination) destinations[(String) annot.key];
                if (destination != null) {
                    Append("/Dest [");
                    Append(destination.pageObjNumber);
                    Append(" 0 R /XYZ 0 ");
                    Append(destination.yPosition);
                    Append(" 0]\n");
                }
            }
            Append(">>\n");
            Endobj();
        }
    }


    private void AddOCProperties() {
        if (groups.Count > 0) {
            Append("/OCProperties\n");
            Append("<<\n");
            Append("/OCGs [");
            foreach (OptionalContentGroup ocg in this.groups) {
                Append(' ');
                Append(ocg.objNumber);
                Append(" 0 R");
            }
            Append(" ]\n");
            Append("/D <<\n");
            Append("/BaseState /OFF\n");
            Append("/ON [");
            foreach (OptionalContentGroup ocg in this.groups) {
                if (ocg.visible) {
                    Append(' ');
                    Append(ocg.objNumber);
                    Append(" 0 R");
                }
            }
            Append(" ]\n");

            Append("/AS [\n");
            Append("<< /Event /Print /Category [/Print] /OCGs [");
            foreach (OptionalContentGroup ocg in this.groups) {
                if (ocg.printable) {
                    Append(' ');
                    Append(ocg.objNumber);
                    Append(" 0 R");
                }
            }
            Append(" ] >>\n");
            Append("<< /Event /Export /Category [/Export] /OCGs [");
            foreach (OptionalContentGroup ocg in this.groups) {
                if (ocg.exportable) {
                    Append(' ');
                    Append(ocg.objNumber);
                    Append(" 0 R");
                }
            }
            Append(" ] >>\n");
            Append("]\n");

            Append("/Order [[ ()");
            foreach (OptionalContentGroup ocg in this.groups) {
                Append(' ');
                Append(ocg.objNumber);
                Append(" 0 R");
            }
            Append(" ]]\n");
            Append(">>\n");
            Append(">>\n");
        }
    }


    public void AddPage(Page page) {
        int n = pages.Count;
        if (n > 0) {
            AddPageContent(pages[n - 1]);
        }
        pages.Add(page);
    }


    /**
     *  Writes the PDF object to the output stream.
     *  Does not close the underlying output stream.
     */
    public void Flush() {
        Flush(false);
    }


    /**
     *  Writes the PDF object to the output stream and closes it.
     */
    public void Close() {
        Flush(true);
    }


    private void Flush(bool close) {
        AddPageContent(pages[pages.Count - 1]);

        int resObjNumber = AddResourcesObject();
        int infoObjNumber = AddInfoObject();
        int pagesObjNumber = AddPagesObject();
        AddAllPages(pagesObjNumber, resObjNumber);

        // This is the root object
        Newobj();
        Append("<<\n");
        Append("/Type /Catalog\n");

        AddOCProperties();

        Append("/Pages ");
        Append(pagesObjNumber);
        Append(" 0 R\n");

        if (compliance == Compliance.PDF_A_1B) {
            Append("/Metadata ");
            Append(metadataObjNumber);
            Append(" 0 R\n");

            Append("/OutputIntents [");
            Append(outputIntentObjNumber);
            Append(" 0 R]\n");
        }

        Append(">>\n");
        Endobj();

        int startxref = byte_count;
        // Create the xref table
        Append("xref\n");
        Append("0 ");
        Append(objNumber + 1);
        Append('\n');
        Append("0000000000 65535 f \n");
        for (int i = 0; i < objOffset.Count; i++) {
            int offset = objOffset[i];
            String str = offset.ToString();
            for (int j = 0; j < 10 - str.Length; j++) {
                Append('0');
            }
            Append(str);
            Append(" 00000 n \n");
        }
        Append("trailer\n");
        Append("<<\n");
        Append("/Size ");
        Append(objNumber + 1);
        Append('\n');

        String id = (new Salsa20()).getID();
        Append("/ID[<");
        Append(id);
        Append("><");
        Append(id);
        Append(">]\n");

        Append("/Root ");
        Append(objNumber);
        Append(" 0 R\n");
        Append("/Info ");
        Append(infoObjNumber);
        Append(" 0 R\n");
        Append(">>\n");
        Append("startxref\n");
        Append(startxref);
        Append('\n');
        Append("%%EOF\n");

        os.Flush();
        if (close) {
            os.Dispose();
        }
    }


    /**
     *  Set the "Title" document property of the PDF file.
     *  @param title The title of this document.
     */
    public void SetTitle(String title) {
        this.title = title;
    }


    /**
     *  Set the "Subject" document property of the PDF file.
     *  @param subject The subject of this document.
     */
    public void SetSubject(String subject) {
        this.subject = subject;
    }


    /**
     *  Set the "Author" document property of the PDF file.
     *  @param author The author of this document.
     */
    public void SetAuthor(String author) {
        this.author = author;
    }


    internal void Append(int num) {
        Append(num.ToString());
    }


    internal void Append(double val) {
        Append(val.ToString().Replace(',', '.'));
    }


    internal void Append(String str) {
        int len = str.Length;
        for (int i = 0; i < len; i++) {
            os.WriteByte((byte) str[i]);
        }
        byte_count += len;
    }


    internal void Append(char ch) {
        Append((byte) ch);
    }


    internal void Append(byte b) {
        os.WriteByte(b);
        byte_count += 1;
    }


    internal void Append(byte[] buf, int off, int len) {
        os.Write(buf, off, len);
        byte_count += len;
    }


    internal void Append(MemoryStream baos) {
        baos.WriteTo(os);
        byte_count += (int) baos.Length;
    }


    public Dictionary<Int32, PDFobj> Read(Stream inputStream) {

        List<PDFobj> objects = new List<PDFobj>();

        MemoryStream baos = new MemoryStream();
        int ch;
        while ((ch = inputStream.ReadByte()) != -1) {
            baos.WriteByte((byte) ch);
        }
        byte[] pdf = baos.ToArray();

        int xref = GetStartXRef(pdf);

        PDFobj obj = GetObject(pdf, xref);
        if (obj.dict[0].Equals("xref")) {
            GetPdfObjects1(pdf, obj, objects);
        }
        else {
            GetPdfObjects2(pdf, obj, objects);
        }

        Dictionary<Int32, PDFobj> pdfObjects = new Dictionary<Int32, PDFobj>();

        for (int i = 0; i < objects.Count; i++) {
            obj = objects[i];
            obj.number = Int32.Parse(obj.dict[0]);
            if (obj.dict.Contains("stream")) {
                obj.SetStream(pdf, obj.GetLength(objects));
            }

            if (obj.GetValue("/Filter").Equals("/FlateDecode")) {
                Decompressor decompressor = new Decompressor(obj.stream);
                obj.data = decompressor.getDecompressedData();
            }

            if (obj.GetValue("/Type").Equals("/ObjStm")) {
                int first = Int32.Parse(obj.GetValue("/First"));
                int n = Int32.Parse(obj.GetValue("/N"));
                PDFobj o2 = GetObject(obj.data, 0, first);
                for (int j = 0; j < o2.dict.Count; j += 2) {
                    int num = Int32.Parse(o2.dict[j]);
                    int off = Int32.Parse(o2.dict[j + 1]);
                    int end = obj.data.Length;
                    if (j <= o2.dict.Count - 4) {
                        end = first + Int32.Parse(o2.dict[j + 3]);
                    }

                    PDFobj o3 = GetObject(obj.data, first + off, end);
                    o3.dict.Insert(0, "obj");
                    o3.dict.Insert(0, "0");
                    o3.dict.Insert(0, num.ToString());
                    pdfObjects[num] = o3;
                }
            }
            else {
                pdfObjects[obj.number] = obj;
            }
        }

        return pdfObjects;
    }


    private bool Process(PDFobj obj, StringBuilder buf, int off) {
        String token = buf.ToString().Trim();
        if (!token.Equals("")) {
            obj.dict.Add(token);
        }
        buf.Length = 0;
        if (token.Equals("stream") ||
                token.Equals("endobj") ||
                token.Equals("startxref")) {
            if (token.Equals("stream")) {
                if (endOfLine == CR_LF) {
                    obj.stream_offset = off + 1;
                }
                else if (endOfLine == CR || endOfLine == LF) {
                    obj.stream_offset = off;
                }
            }
            return true;
        }
        return false;
    }


    private PDFobj GetObject(byte[] buf, int off) {
        return GetObject(buf, off, buf.Length);
    }


    private PDFobj GetObject(byte[] buf, int off, int len) {

        PDFobj obj = new PDFobj(off);
        StringBuilder token = new StringBuilder();

        int p = 0;
        char c1 = ' ';
        bool done = false;
        while (!done && off < len) {
            char c2 = (char) buf[off++];
            if (c2 == '(') {
                if (p == 0) {
                    done = Process(obj, token, off);
                }
                if (!done) {
                    token.Append(c2);
                    ++p;
                }
            }
            else if (c2 == ')') {
                token.Append(c2);
                --p;
                if (p == 0) {
                    done = Process(obj, token, off);
                }
            }
            else if (c2 == 0x00         // Null
                    || c2 == 0x09       // Horizontal Tab
                    || c2 == 0x0A       // Line Feed (LF)
                    || c2 == 0x0C       // Form Feed
                    || c2 == 0x0D       // Carriage Return (CR)
                    || c2 == 0x20) {    // Space
                done = Process(obj, token, off);
                if (!done) {
                    c1 = ' ';
                }
            }
            else if (c2 == '/') {
                done = Process(obj, token, off);
                if (!done) {
                    token.Append(c2);
                    c1 = c2;
                }
            }
            else if (c2 == '<' || c2 == '>' || c2 == '%') {
                if (c2 != c1) {
                    done = Process(obj, token, off);
                    if (!done) {
                        token.Append(c2);
                        c1 = c2;
                    }
                }
                else {
                    token.Append(c2);
                    done = Process(obj, token, off);
                    if (!done) {
                        c1 = ' ';
                    }
                }
            }
            else if (c2 == '[' || c2 == ']' || c2 == '{' || c2 == '}') {
                done = Process(obj, token, off);
                if (!done) {
                    obj.dict.Add(c2.ToString());
                    c1 = c2;
                }
            }
            else {
                token.Append(c2);
                if (p == 0) {
                    c1 = c2;
                }
            }
        }

        return obj;
    }


    /**
     * Converts an array of bytes to an integer.
     * @param buf byte[]
     * @return int
     */
    private int ToInt(byte[] buf, int off, int len) {
        int i = 0;
        for (int j = 0; j < len; j++) {
            i |= buf[off + j] & 0xFF;
            if (j < len - 1) {
                i <<= 8;
            }
        }
        return i;
    }


    private void GetPdfObjects1(
            byte[] pdf, PDFobj obj, List<PDFobj> pdfObjects) {

        String xref = obj.GetValue("/Prev");
        if (!xref.Equals("")) {
            GetPdfObjects1(
                    pdf,
                    GetObject(pdf, Int32.Parse(xref)),
                    pdfObjects);
        }

        int i = 1;
        while (true) {
            String token = obj.dict[i++];
            if (token.Equals("trailer")) {
                break;
            }

            int n = Int32.Parse(obj.dict[i++]);     // Number of entries
            for (int j = 0; j < n; j++) {
                String offset = obj.dict[i++];      // Object offset
                String number = obj.dict[i++];      // Generation number
                String status = obj.dict[i++];      // Status keyword
                if (!status.Equals("f")) {
                    int off = Int32.Parse(offset);
                    if (off != 0) {
                        pdfObjects.Add(GetObject(pdf, off));
                    }
                }
            }
        }

    }


    private void GetPdfObjects2(
            byte[] pdf, PDFobj obj, List<PDFobj> pdfObjects) {

        String prev = obj.GetValue("/Prev");
        if (!prev.Equals("")) {
            GetPdfObjects2(
                    pdf,
                    GetObject(pdf, Int32.Parse(prev)),
                    pdfObjects);
        }

        obj.SetStream(pdf, Int32.Parse(obj.GetValue("/Length")));

        Decompressor decompressor = new Decompressor(obj.stream);
        byte[] data = decompressor.getDecompressedData();

        int p1 = 0; // Predictor byte
        int f1 = 0; // Field 1
        int f2 = 0; // Field 2
        int f3 = 0; // Field 3
        for (int i = 0; i < obj.dict.Count; i++) {
            String token = obj.dict[i];
            if (token.Equals("/Predictor")) {
                if (obj.dict[i + 1].Equals("12")) {
                    p1 = 1;
                }
                else {
                    // TODO:
                }
            }

            if (token.Equals("/W")) {
                // "/W [ 1 3 1 ]"
                f1 = Int32.Parse(obj.dict[i + 2]);
                f2 = Int32.Parse(obj.dict[i + 3]);
                f3 = Int32.Parse(obj.dict[i + 4]);
            }

        }

        int n = p1 + f1 + f2 + f3;   // Number of bytes per entry

        byte[] entry = new byte[n];

        for (int i = 0; i < data.Length; i += n) {

            // Apply the 'Up' filter.
            for (int j = 0; j < n; j++) {
                entry[j] += data[i + j];
            }

            if (entry[1] == 0x01) {
                int off = ToInt(entry, p1 + f1, f2);
                pdfObjects.Add(GetObject(pdf, off, pdf.Length));
            }

        }

    }


    private int GetStartXRef(byte[] buf) {

        StringBuilder sb = new StringBuilder();

        for (int i = (buf.Length - 10); i > 10; i--) {
            if (buf[i] == 's' &&
                    buf[i + 1] == 't' &&
                    buf[i + 2] == 'a' &&
                    buf[i + 3] == 'r' &&
                    buf[i + 4] == 't' &&
                    buf[i + 5] == 'x' &&
                    buf[i + 6] == 'r' &&
                    buf[i + 7] == 'e' &&
                    buf[i + 8] == 'f') {

                if (buf[i + 9] == 0x0D) {
                    if (buf[i + 10] == 0x0A) {
                        endOfLine = CR_LF;
                    }
                    else {
                        endOfLine = CR;
                    }
                }
                else if (buf[i + 9] == 0x0A) {
                    endOfLine = LF;
                }

                int j = (endOfLine == CR_LF) ? (i + 11) : (i + 10);

                char ch = (char) buf[j];
                while (ch == ' ' || Char.IsDigit(ch)) {
                    sb.Append(ch);
                    ch = (char) buf[++j];
                }
                break;
            }
        }

        return Int32.Parse(sb.ToString().Trim());
    }

}   // End of PDF.cs
}   // End of namespace PDFjet.NET
