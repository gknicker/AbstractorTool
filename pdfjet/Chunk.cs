/**
 *  Chunk.cs
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
class Chunk {

    private long chunkLength;
    internal byte[] type;
    private byte[] data;
    private long crc;
    
    
    public Chunk() {
    }


    public long GetLength() {
        return this.chunkLength;
    }


    public void SetLength(long chunkLength) {
        this.chunkLength = chunkLength;
    }


    public void SetType(byte[] type) {
        this.type = type;
    }


    public byte[] GetData() {
        return this.data;
    }


    public void SetData(byte[] data) {
        this.data = data;
    }


    public long GetCrc() {
        return this.crc;
    }


    public void SetCrc(long crc) {
        this.crc = crc;
    }


    public bool hasGoodCRC() {
        CRC32 computedCRC = new CRC32();
        computedCRC.Update(type, 0, 4);
        computedCRC.Update(data, 0, (int) chunkLength);
        return (computedCRC.GetValue() == this.crc);
    }

}   // End of Chunk.cs
}   // End of namespace PDFjet.NET
